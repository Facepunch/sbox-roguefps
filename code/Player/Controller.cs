using Sandbox.Citizen;
using Sandbox.ModelEditor.Nodes;

namespace RogueFPS;

public partial class Controller : Component
{

	public static Controller Local;

	[Property]
	public CameraComponent Camera { get; set; }
	[Property]
	public float MoveSpeed { get; set; } = 260f;
	[Property]
	public CitizenAnimationHelper AnimationHelper { get; set; }
	[Property]
	public SkinnedModelRenderer Citizen { get; set; }
	[Property]
	public float Acceleration { get; set; } = 4.0f;
	[Property] 
	public PlayerStats PlayerStatsComponent { get; set; }


	[Property] public CameraController CameraController { get; set; }

	public GameObject CameraGameObject => CameraController.Camera.GameObject;

	public Ray AimRay => CameraController.AimRay;

	public bool IsSprinting;
	public TimeUntil TimeUntilRespawn { get; private set; }
	public bool IsDead { get; private set; }
	public bool IsAlive { get; private set; }

	public Rigidbody Rigidbody;
	public Vector3 InputVector;
	Vector3 StartPosition;
	bool GibNotRagdoll => true;
	bool Grounded;
	bool Momentum;
	Vector3 MomentumVelocity;

	public Angles EyeAngles;
	Vector3 VelocityLastFrame;
	Vector3 MoveVelocity;
	Vector3 BaseVelocity;
	GameObject Ragdoll;

	float? CurrentEyeHeightOverride = 64;

	public void AddVelocity( Vector3 velocity )
	{
		BaseVelocity = velocity;
		Momentum = true;
	}

	protected override void OnStart()
	{
		base.OnStart();

		Local = this;
		Rigidbody = Components.Get<Rigidbody>();
		Rigidbody.PhysicsBody.SpeculativeContactEnabled = true;
		StartPosition = Transform.Position;

		if ( Citizen == null ) return;

		Citizen.OnFootstepEvent = HandleFootstep;
	}

	void CheckForCrush()
	{
		if ( Rigidbody?.PhysicsBody == null )
		{
			return;
		}

		var traces = Scene.Trace.Box( Rigidbody.PhysicsBody.GetBounds().Grow( -5f ), Vector3.Zero, Vector3.Zero )
			.WithoutTags( "player", "ragdoll" )
			.RunAll();
		var crushCount = 0;

		foreach ( var tr in traces )
		{
			if ( tr.StartedSolid )
			{
				crushCount++;
			}
		}

		if ( crushCount > 1 )
		{
			Kill();
		}
	}

	protected override void OnFixedUpdate()
	{
		if ( IsDead ) return;

		CheckForCrush();
		CheckForGround();

		if ( !Momentum && MomentumVelocity.Length > 0 )
		{
			Rigidbody.Velocity += MomentumVelocity;
			MomentumVelocity = 0;
		}

		if ( Input.Pressed( "Run" ) )
		{
			IsSprinting = !IsSprinting;
		}

		InputVector = Vector3.Zero;

		InputVector = Input.AnalogMove.Normal;

		var moveVec = Vector3.Zero;

		if ( InputVector != 0 )
		{
			var cameraFwdish = Camera.Transform.Rotation.Angles().WithPitch( 0 ).WithRoll( 0 ).ToRotation();
			var dir = cameraFwdish * InputVector;
			var speed = GetSpeed();
			Log.Info( $"Speed: {speed}" );
			var inputVelocity = dir.Normal * speed;

			if ( Grounded )
			{
				Transform.Rotation = Rotation.Slerp( Transform.Rotation, Rotation.LookAt( dir ), 8f * Time.Delta );
			}

			moveVec = inputVelocity;
		}

		MoveVelocity = MoveVelocity.LerpTo( moveVec, Acceleration * Time.Delta );

		if ( Grounded && Input.Pressed( "Jump" ) )
		{
			if ( Input.Pressed( "Jump" ) )
			{
				Rigidbody.Velocity += Vector3.Up * 360;
				AnimationHelper?.TriggerJump();
			}
		}

		var finalMoveVec = MoveVelocity + BaseVelocity;

		if ( BaseVelocity.Length > 0 )
		{
			Momentum = false;
			MomentumVelocity = BaseVelocity;
			BaseVelocity = Vector3.Zero;
		}

		if ( finalMoveVec != 0 )
		{
			var mh = new CharacterControllerHelper( Scene.Trace.Box( Bounds, 0, 0 ).WithoutTags( "player", "ragdoll" ), 0, 0 );
			mh.Position = Transform.Position;
			mh.Velocity = finalMoveVec;
			var fraction = mh.TryMoveWithStep( Time.Delta, 16.0f );

			if ( fraction > 0 )
			{
				finalMoveVec = mh.Velocity;
			}

			var baseMove = Rigidbody.Transform.World.WithPosition( Rigidbody.Transform.Position + finalMoveVec * Time.Delta );
			Rigidbody.PhysicsBody.Move( baseMove, Time.Delta );
		}
	}

	float SmoothEyeHeight = 64f;
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if(!IsProxy && CameraController != null)
		{
			var cameraGameObject = CameraController.Camera.GameObject;

			var eyeHeightOffset = GetEyeHeightOffset();

			SmoothEyeHeight = SmoothEyeHeight.LerpTo( eyeHeightOffset, Time.Delta * 10f );

			cameraGameObject.Transform.Position = Transform.Position + Vector3.Zero.WithZ( SmoothEyeHeight );



			EyeAngles.pitch += Input.MouseDelta.y * 0.1f;
			EyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
			EyeAngles.roll = 0;

			// we're a shooter game!
			EyeAngles.pitch = EyeAngles.pitch.Clamp( -90, 90 );

			var cam = CameraController.Camera;
			var lookDir = EyeAngles.ToRotation();

			cam.Transform.Rotation = lookDir;
		}

		if ( IsDead && TimeUntilRespawn < 0 )
		{
			Respawn();
		}

		UpdateAnimation();
	}

	void DoGibs()
	{
		var gibModel = Model.Load( $"models/citizen_gibs/citizen_gib.vmdl" );
		var breaklist = gibModel.GetData<ModelBreakPiece[]>();

		if ( breaklist == null || breaklist.Length <= 0 ) return;

		foreach ( var model in breaklist )
		{
			var gib = new GameObject( true, $"{GameObject.Name} (gib)" );

			gib.Transform.Position = Transform.World.PointToWorld( model.Offset );
			gib.Transform.Rotation = Transform.Rotation;
			gib.Tags.Add( "ragdoll" );

			var c = gib.Components.Create<Gib>( false );
			c.FadeTime = model.FadeTime;
			c.Model = Model.Load( model.Model );
			c.Enabled = true;

			var phys = gib.Components.Get<Rigidbody>( true );

			if ( phys is not null && Rigidbody is not null )
			{
				//phys.Velocity = Rigidbody.Velocity + MoveVelocity + BaseVelocity;

				var randDir = Random.Shared.VectorInSphere();
				var randStr = Random.Shared.NextInt64( 200, 550 );
				phys.Velocity += randDir * randStr;
				phys.AngularVelocity = randDir * randStr * .1f;
			}
		}
	}

	void DoRagdoll()
	{
		Ragdoll = new GameObject( true, "Ragdoll" );

		var mrs = GameObject.Components.GetAll<ModelRenderer>( FindMode.EverythingInSelfAndDescendants );
		foreach ( var mr in mrs )
		{
			var go = new GameObject( true );
			go.SetParent( Ragdoll );

			go.Transform.Position = mr.Transform.Position;
			go.Transform.Rotation = mr.Transform.Rotation;
			go.Transform.Scale = mr.Transform.Scale;

			var smr = go.Components.Create<SkinnedModelRenderer>();
			smr.Model = mr.Model;

			var mphys = go.Components.Create<ModelPhysics>();
			mphys.Model = mr.Model;
			mphys.Renderer = smr;

			mphys.Enabled = false;
			mphys.Enabled = true;
		}
	}

	public void Kill()
	{
		if ( IsDead && !IsAlive ) return;

		if ( Ragdoll != null )
		{
			Ragdoll.Destroy();
			Ragdoll = null;
		}

		if ( GibNotRagdoll )
		{
			DoGibs();
		}
		else
		{
			DoRagdoll();
		}

		IsDead = true;
		TimeUntilRespawn = 3;
		Citizen.GameObject.Enabled = false;
		IsAlive = false;
	}

	void Respawn()
	{
		if ( !IsDead && IsAlive ) return;

		if ( Ragdoll != null )
		{
			Ragdoll.Destroy();
			Ragdoll = null;
		}

		IsDead = false;
		Citizen.GameObject.Enabled = true;
		Rigidbody.Velocity = Vector3.Zero;
		BaseVelocity = 0;
		MoveVelocity = 0;
		MomentumVelocity = 0;
		Momentum = false;

		
		Transform.Position = StartPosition;
		

		IsAlive = true;
	}

	void CheckForGround()
	{
		Grounded = false;

		if ( Rigidbody.Velocity.z > 260f ) return;

		var startTx = Transform.World.WithPosition( Transform.Position + Vector3.Up );
		var endTx = Transform.World.WithPosition( Transform.Position + Vector3.Down * 3 );

		var traces = Scene.Trace
			.Sweep( Rigidbody.PhysicsBody, startTx, endTx )
			.WithoutTags( "player", "ragdoll" )
			.RunAll();

		foreach ( var trace in traces )
		{
			if ( trace.Normal.z > 0.55f )
			{
				Grounded = true;
				break;
			}
		}
	}

	protected float GetSpeed()
	{
		if(IsSprinting)
		{
			return PlayerStatsComponent.UpgradedStats[PlayerStats.PlayerUpgradedStats.WalkSpeed] * PlayerStatsComponent.UpgradedStats[PlayerStats.PlayerUpgradedStats.SprintMultiplier];
		}
		else
		{
			return PlayerStatsComponent.UpgradedStats[PlayerStats.PlayerUpgradedStats.WalkSpeed];
		}
	}

	protected float GetEyeHeightOffset()
	{
		if ( CurrentEyeHeightOverride is not null ) return CurrentEyeHeightOverride.Value;
		return 64f;
	}

	BBox Bounds => new( new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );

	void UpdateAnimation()
	{
		if ( AnimationHelper == null ) return;

		var cameraFwdish = Rotation.FromYaw( Camera.Transform.Rotation.Yaw() );
		var dir = cameraFwdish * InputVector;
		var desiredVelocity = dir.Normal * MoveSpeed;
		var rotationDifference = Transform.Rotation.Distance( Rotation.LookAt( dir ) );

		AnimationHelper.WithVelocity( MoveVelocity );
		AnimationHelper.WithWishVelocity( desiredVelocity );
		AnimationHelper.FootShuffle = rotationDifference;
		AnimationHelper.WithLook( Transform.Rotation.Forward, 1, 1, 1.0f );
		AnimationHelper.IsGrounded = Grounded;
		//AnimationHelper.MoveStyle = IsRunning ? CitizenAnimationHelper.MoveStyles.Run : CitizenAnimationHelper.MoveStyles.Walk;
	}

	TimeSince timeSinceFootstep;
	void HandleFootstep( SceneModel.FootstepEvent e )
	{
		if ( timeSinceFootstep < 0.2f ) return;

		var volumeAlpha = Rigidbody.Velocity.Length.LerpInverse( 0, MoveSpeed );
		if ( volumeAlpha <= 0.08f ) return;

		var tr = Scene.Trace.Box( this.Bounds, Transform.Position, Transform.Position + Vector3.Down * 5 )
			.WithoutTags( "Player", "ragdoll" )
			.Run();

		if ( !tr.Hit ) return;
		if ( tr.Surface is null ) return;

		var sound = e.FootId == 0 ? tr.Surface.Sounds.FootLeft : tr.Surface.Sounds.FootRight;
		if ( sound is null ) return;

		timeSinceFootstep = 0;

		var handle = Sound.Play( sound, tr.HitPosition + tr.Normal * 5 );
		handle.Volume *= e.Volume * volumeAlpha;
		handle.Update();
	}

}
