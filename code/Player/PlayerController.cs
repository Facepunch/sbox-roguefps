namespace RogueFPS;

public partial class PlayerController : Actor
{
	/// <summary>
	/// A reference to the player's body (the GameObject)
	/// </summary>
	[Property] public GameObject Body { get; set; }

	/// <summary>
	/// A reference to the player's head (the GameObject)
	/// </summary>
	[Property] public GameObject Head { get; set; }

	/// <summary>
	/// A reference to the animation helper (normally on the Body GameObject)
	/// </summary>
	[Property] public AnimationHelper AnimationHelper { get; set; }

	/// <summary>
	/// The current gravity. Make this a gamerule thing later?
	/// </summary>
	[Property] public Vector3 Gravity { get; set; } = new Vector3( 0, 0, 800 );

	/// <summary>
	/// The current camera controller for this player.
	/// </summary>
	[Property] public CameraController CameraController { get; set; }

	// Actor
	public override GameObject CameraObject => CameraController.Camera.GameObject;

	/// <summary>
	/// A reference to the View Model's camera. This will be disabled by the View Model.
	/// </summary>
	[Property] public CameraComponent ViewModelCamera { get; set; }

	/// <summary>
	/// Get a quick reference to the real Camera GameObject.
	/// </summary>
	public GameObject CameraGameObject => CameraController.Camera.GameObject;

	/// <summary>
	/// Finds the first enabled weapon on our player.
	/// Make this quicker and not fetching components every time.
	/// </summary>
	//public Weapon Weapon => Components.Get<Weapon>( FindMode.EnabledInSelfAndDescendants );

	/// <summary>
	/// Finds the first <see cref="SkinnedModelRenderer"/> on <see cref="Body"/>
	/// </summary>
	public SkinnedModelRenderer BodyRenderer => Body.Components.Get<SkinnedModelRenderer>();

	/// <summary>
	/// An accessor to get the camera controller's aim ray.
	/// </summary>
	public Ray AimRay => CameraController.AimRay;

	/// <summary>
	/// The current holdtype for the player.
	/// </summary>
	[Property] AnimationHelper.HoldTypes CurrentHoldType { get; set; } = AnimationHelper.HoldTypes.None;

	[Property, System.ComponentModel.ReadOnly( true )] public bool IsAiming { get; private set; }

	/// <summary>
	/// Called when the player jumps.
	/// </summary>
	[Property] public Action OnJump { get; set; }

	// Properties used only in this component.
	public Angles EyeAngles;

	public bool IsGrounded { get; set; }

	protected float GetEyeHeightOffset()
	{
		if ( CurrentEyeHeightOverride is not null ) return CurrentEyeHeightOverride.Value;
		return 0f;
	}

	float SmoothEyeHeight = 0f;
	protected override void OnAwake()
	{
		baseAcceleration = CharacterController.Acceleration;

		Stats = Components.Get<Stats>( FindMode.EverythingInSelfAndAncestors );
	}

	protected override void OnUpdate()
	{
		var cc = CharacterController;
		/*
		if ( Weapon.IsValid() )
		{
			CurrentHoldType = Weapon.GetHoldType();
		}
		*/
		// Eye input

		if ( !IsProxy && cc != null )
		{
			var cameraGameObject = CameraController.Camera.GameObject;

			var eyeHeightOffset = GetEyeHeightOffset();

			SmoothEyeHeight = SmoothEyeHeight.LerpTo( eyeHeightOffset, Time.Delta * 10f );

			cameraGameObject.Transform.LocalPosition = Vector3.Zero.WithZ( SmoothEyeHeight );

			EyeAngles.pitch += Input.MouseDelta.y * 0.1f;
			EyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
			EyeAngles.roll = 0;

			// we're a shooter game!
			EyeAngles.pitch = EyeAngles.pitch.Clamp( -90, 90 );

			var cam = CameraController.Camera;
			var lookDir = EyeAngles.ToRotation();

			cam.Transform.Rotation = lookDir;

			IsAiming = Input.Down( "Attack2" );
		}

		float rotateDifference = 0;

		// rotate body to look angles
		if ( Body is not null )
		{
			var targetAngle = new Angles( 0, EyeAngles.yaw, 0 ).ToRotation();

			rotateDifference = Body.Transform.Rotation.Distance( targetAngle );

			if ( rotateDifference > 50.0f || ( cc != null && cc.Velocity.Length > 10.0f ) )
			{
				Body.Transform.Rotation = Rotation.Lerp( Body.Transform.Rotation, targetAngle, Time.Delta * 10.0f );
			}
		}

		var wasGrounded = IsGrounded;
		IsGrounded = cc.IsOnGround;

		if ( wasGrounded != IsGrounded )
		{
			OnGroundedChanged();
		}

		if ( AnimationHelper is not null && cc is not null )
		{
			AnimationHelper.WithVelocity( cc.Velocity );
			AnimationHelper.WithWishVelocity( WishVelocity );
			AnimationHelper.IsGrounded = IsGrounded;
			AnimationHelper.FootShuffle = rotateDifference;
			AnimationHelper.WithLook( EyeAngles.Forward, 1, 1, 1.0f );
			AnimationHelper.MoveStyle = MechanicTags.Has( "sprint" ) ? AnimationHelper.MoveStyles.Run : AnimationHelper.MoveStyles.Walk;
			AnimationHelper.DuckLevel = MechanicTags.Has( "crouch" ) ? 100 : 0;
			AnimationHelper.HoldType = CurrentHoldType;
			AnimationHelper.SkidAmount = MechanicTags.Has( "slide" ) ? 1 : 0;
		}
	}

	private void OnGroundedChanged()
	{
		var nowOffGround = IsGrounded == false;
	}


	/// <summary>
	/// Get the current friction.
	/// </summary>
	/// <returns></returns>
	protected override float GetFriction()
	{
		if ( !CharacterController.IsOnGround ) return 0.1f;
		if ( CurrentFrictionOverride is not null ) return CurrentFrictionOverride.Value;

		return 4.0f;
	}

	private void ApplyAccceleration()
	{
		if ( CurrentAccelerationOverride is not null )
		{
			CharacterController.Acceleration = CurrentAccelerationOverride.Value;
		}
		else
		{
			CharacterController.Acceleration = baseAcceleration;
		}
	}

	public override Angles GetEyeAngles()
	{
		return EyeAngles;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy )
			return;

		var cc = CharacterController;
		if ( cc == null )
			return;

		BuildWishInput();

		// Mechanics
		DoMechanicsUpdate();

		BuildWishVelocity();

		ApplyAccceleration();

		if ( cc.IsOnGround )
		{
			cc.Velocity = cc.Velocity.WithZ( 0 );
			cc.Accelerate( WishVelocity );
		}
		else
		{
			cc.Velocity -= Gravity * Time.Delta * 0.5f;
			cc.Accelerate( WishVelocity / 2 );
		}

		cc.ApplyFriction( GetFriction() );
		cc.Move();

		if ( !cc.IsOnGround )
		{
			cc.Velocity -= Gravity * Time.Delta * 0.5f;
		}
		else
		{
			cc.Velocity = cc.Velocity.WithZ( 0 );
		}
	}


	protected override float GetWishSpeed()
	{
		if ( CurrentSpeedOverride is not null ) return CurrentSpeedOverride.Value;

		// Default speed
		return Stats.UpgradedStats[Stats.PlayerUpgradedStats.WalkSpeed];
	}

	protected override void BuildWishInput()
	{
		WishMove = Input.AnalogMove.Normal;
	}

	public void Write( ref ByteStream stream )
	{
		stream.Write( EyeAngles );
	}

	public void Read( ByteStream stream )
	{
		EyeAngles = stream.Read<Angles>();
	}
}
