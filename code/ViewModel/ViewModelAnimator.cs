using Sandbox;

public sealed class ViewModelAnimator : Component
{
	[Property]
	[Group( "Handling" )]
	bool IsTwoHanded { get; set; } = false;
	[Property]
	[Group( "Handling" )]
	bool IsEmpty { get; set; } = false;
	[Property]
	[Group( "Handling" )]
	bool LoweredWeapon { get; set; } = false;

	[Property]
	[Group( "Handling" )]
	bool HolsterWeapon { get; set; } = false;

	/*
	[Property]
	[Group( "Handling" )]
	bool DeployWeapon { get; set; } = false;
	*/

	[Range( 0, 10 )]
	[Property]
	[Group( "Firing" )]
	int FiringMode { get; set; } = 0;

	[Range( 0, 8 )]
	[Property]
	[Group( "Handling" )]
	int HoldType { get; set; } = 0;

	[Range( 0, 1 )]
	[Property]
	[Group( "IronSight" )]
	float IronSightFireScale { get; set; } = 1;

	[Range( 0, 50 )]
	[Property]
	[Group( "Movement" )]
	float InertiaDamping { get; set; } = 20.0f;

	[Property]
	Vector3 PosOffset { get; set; } = Vector3.Zero;

	[Property]
	bool Visible { get; set; } = true;

	//
	[Property]
	[Group( "Don't worry about these" )]
	private Controller Player { get; set; }

	[Property]
	[Group( "Don't worry about these" )]
	CameraComponent CameraComp { get; set; }
	//

	//Use these to keep track of the guns movement
	public float YawInertia { get; private set; }
	public float PitchInertia { get; private set; }
	private float lastYaw;
	private float lastPitch;
	//

	protected override void OnStart()
	{
		base.OnStart();

		GameObject.Parent = CameraComp.GameObject;
	}

	protected override void OnUpdate()
	{

		Transform.LocalPosition = PosOffset;

		Visible = Input.Pressed( "slot1" ) ? !Visible : Visible;



		var newYaw = CameraComp.Transform.Rotation.Yaw();
		var newPitch = CameraComp.Transform.Rotation.Pitch();

		//var yawDelta = CameraComp.Transform.Rotation.Angles().yaw - lastYaw;
		var yawDelta = Angles.NormalizeAngle( lastYaw - newYaw );
		var pitchDelta = Angles.NormalizeAngle( lastPitch - newPitch );

		YawInertia += yawDelta;
		PitchInertia += pitchDelta;

		var dir = Player.InputVector;
		var forward = Player.Transform.Rotation.Forward.Dot( dir );
		var sideward = Player.Transform.Rotation.Right.Dot( dir );

		var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

		var child = Components.Get<SkinnedModelRenderer>( FindMode.InSelf );
		var anim = child;

		if ( Visible )
		{
			child.Enabled = true;
			anim.Enabled = true;
		}
		else
		{
			child.Enabled = false;
			anim.Enabled = false;
		}

		//Movement
		anim.Set( "b_jump", !Player.AnimationHelper.IsGrounded );
		anim.Set( "move_direction", angle );
		anim.Set( "move_speed", Player.InputVector.Length );
		anim.Set( "move_groundspeed", Player.InputVector.WithZ( 0 ).Length );
		anim.Set( "move_y", sideward );
		anim.Set( "move_x", forward );
		anim.Set( "move_z", Player.InputVector.z );
		anim.Set( "b_grounded", Player.AnimationHelper.IsGrounded );
		//anim.Set( "move_sprint", Player.HasTag( "sprint" ) );
		//anim.Set( "b_sprint", Player.HasTag("sprint"));
		anim.Set( "b_crouch", Input.Down( "duck" ) );

		//Fire
		//anim.Set( "b_attack", Input.Pressed( "Attack1" ) );
		//anim.Set( "b_attack_dry", Input.Pressed( "Attack1" ) && IsEmpty );

		//Reload
		//anim.Set( "b_reload", Input.Pressed( "Reload" ) );

		//Empty
		anim.Set( "b_empty", IsEmpty );

		//TwoHanded
		anim.Set( "b_twohanded", IsTwoHanded );

		//Iron sight
		anim.Set( "ironsights", Input.Down( "attack2" ) ? 2 : 0 );
		anim.Set( "ironsights_fire_scale", IronSightFireScale );

		//Fire Mode
		anim.Set( "firing_mode", FiringMode );

		//HoldType
		anim.Set( "holdtype_pose", HoldType );

		//Lowered
		anim.Set( "b_lower_weapon", LoweredWeapon );

		//Holster
		anim.Set( "b_holster", HolsterWeapon );

		//Deploy
		//anim.Set( "b_deploy", !HolsterWeapon );

		//Inertia sway
		anim.Set( "aim_yaw_inertia", YawInertia );
		anim.Set( "aim_pitch_inertia", PitchInertia );

		lastYaw = newYaw;
		lastPitch = newPitch;

		YawInertia = YawInertia.LerpTo( 0, Time.Delta * InertiaDamping );
		PitchInertia = PitchInertia.LerpTo( 0, Time.Delta * InertiaDamping );
	}
}
