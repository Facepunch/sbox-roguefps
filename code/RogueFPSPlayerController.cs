using Sandbox.Citizen;

namespace RogueFPS;

[Title( "Rogue FPS PlayerController" )]
[Category( "Player Controller" )]
[Icon( "accessibility", "yellow", "white" )]
public class RogueFPSPlayerController : Component
{
	[Property] public Vector3 Gravity { get; set; } = new Vector3( 0, 0, 800 );

	[Range( 0, 400 )]
	[Property] public float CameraDistance { get; set; } = 200.0f;

	public Vector3 WishVelocity { get; set; }

	[Property] GameObject Body { get; set; }
	[Property] public GameObject Eye { get; set; }
	[Property] bool FirstPerson { get; set; }
	[Property] CitizenAnimationHelper AnimationHelper { get; set; }

	public Angles EyeAngles;

	public float EyeDuckOffset { get; set; }

	[Range( 0, 10 )]
	[Property] public float BobbingAmount { get; set; } = 5.0f;

	[Range( 0, 10 )]
	[Property] public float BobbingSpeed { get; set; } = 5.0f;

	private float bobbingPhase = 0.0f;
	private float currentBobbingOffset = 0.0f;

	[Range( -10, 10 )]
	[Property] public float StrafeTiltAmount { get; set; } = 5.0f; // The maximum tilt angle when strafing

	private float currentTilt = 0.0f; // Store the current tilt angle

	private PlayerStats PlayerStats;

	public bool CantUseInputMove { get; set; } = false;

	protected override void OnStart()
	{
		base.OnStart();
		
		PlayerStats = GameObject.Components.Get<PlayerStats>();
	}

	protected override void OnUpdate()
	{
		// Eye input
		EyeAngles.pitch += Input.MouseDelta.y * 0.1f;
		EyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
		EyeAngles.roll = 0;

		EyeAngles.pitch = Math.Clamp( EyeAngles.pitch, -89.9f, 89.9f );

		// Determine the strafe direction and apply tilt
		float targetTilt = 0.0f;
		if ( Input.Down( "Left" ) ) targetTilt -= StrafeTiltAmount;
		if ( Input.Down( "Right" ) ) targetTilt += StrafeTiltAmount;

		// Smoothly interpolate the current tilt towards the target tilt
		currentTilt = Lerp( currentTilt, targetTilt, Time.Delta * 5.0f ); // Adjust the 5.0f value for faster/slower tilt response

		EyeAngles.roll = currentTilt;

		var cc = GameObject.Components.Get<RogueFPSCharacterController>();
		if ( cc is null ) return;

		// Update camera position
		var camera = GameObject.Components.Get<CameraComponent>( FindMode.EnabledInSelfAndChildren );
		if ( camera is not null )
		{
			var camPos = Eye.Transform.Position - EyeAngles.ToRotation().Forward * CameraDistance;

			if ( FirstPerson )
				camPos = Eye.Transform.Position + EyeAngles.ToRotation().Forward * 8 + Vector3.Up * EyeDuckOffset;

			if ( cc.Velocity.Length > 0.1f && cc.IsOnGround && !CantUseInputMove )
			{
				// Calculate the target bobbing offset using a sine wave
				float targetBobbingOffset = MathF.Sin( bobbingPhase * (cc.Velocity.Length / 100) ) * BobbingAmount;
				bobbingPhase += BobbingSpeed * Time.Delta;

				// Smoothly interpolate the current bobbing offset towards the target offset
				currentBobbingOffset = Lerp( currentBobbingOffset, targetBobbingOffset, Time.Delta * BobbingSpeed );

				// Apply the smoothed bobbing offset to the camera's position
				camPos.z += currentBobbingOffset;
			}
			else
			{
				// Reset the bobbing offset when the player is not moving
				currentBobbingOffset = 0;
			}
			
			camera.Transform.Position = camPos;
			camera.Transform.Rotation = EyeAngles.ToRotation();
		}

		float rotateDifference = 0;

		// rotate body to look angles
		if ( Body is not null )
		{
			var targetAngle = new Angles( 0, EyeAngles.yaw, 0 ).ToRotation();

			var v = cc.Velocity.WithZ( 0 );

			if ( v.Length > 10.0f )
			{
				targetAngle = Rotation.LookAt( v, Vector3.Up );
			}

			rotateDifference = Body.Transform.Rotation.Distance( targetAngle );

			if ( rotateDifference > 50.0f || cc.Velocity.Length > 10.0f )
			{
				Body.Transform.Rotation = Rotation.Lerp( Body.Transform.Rotation, targetAngle, Time.Delta * 2.0f );
			}
		}

		if ( AnimationHelper is not null )
		{
			AnimationHelper.WithVelocity( cc.Velocity );
			AnimationHelper.IsGrounded = cc.IsOnGround;
			AnimationHelper.FootShuffle = rotateDifference;
			AnimationHelper.WithLook( EyeAngles.Forward, 1, 1, 1.0f );
			AnimationHelper.MoveStyle = Input.Down( "Run" ) ? CitizenAnimationHelper.MoveStyles.Run : CitizenAnimationHelper.MoveStyles.Walk;
		}

		/*
			if ( Input.Pressed( "Jump" ) && jumpCount < PlayerStats.UpgradedStats[RogueFPSPlayerStats.PlayerUpgradedStats.AmountOfJumps] )
			{
				
				float flGroundFactor = 1.0f;
				float flMul = 268.3281572999747f * 1.2f;
				//if ( Duck.IsActive )
				//	flMul *= 0.8f;

				cc.Punch( Vector3.Up * flMul * flGroundFactor );
				//	cc.IsOnGround = false;
				

			DoJump();


			AnimationHelper?.TriggerJump();

			jumpCount++;
			}
		*/
	}
	/*
	private void DoJump()
	{
		var cc = GameObject.GetComponent<RogueFPSCharacterController>();
		if ( cc == null ) return;
		
		cc.IsOnGround = false;
		// Perform jump logic
		float jumpForce = CalculateJumpForce( PlayerStats.JumpHeight );
		//cc.Velocity = new Vector3( cc.Velocity.x, cc.Velocity.y, 0 ); // Reset vertical velocity
		cc.Velocity += Vector3.Up * jumpForce;

		AnimationHelper?.TriggerJump();
	}

	private float CalculateJumpForce( float height )
	{
		// Implement the calculation for jump force based on the desired jump height
		// This is a simple physics formula where jumpForce = sqrt(2 * height * gravity)
		return (float)Math.Sqrt( 2 * height * Gravity.z );
	}
	*/

	public static float Lerp( float a, float b, float t )
	{
		return a + t * (b - a);
	}

	protected override void OnFixedUpdate()
	{
		BuildWishVelocity();
		
		var cc = GameObject.Components.Get<RogueFPSCharacterController>();

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

	public bool boosting = false;

	public void SpeedBoost()
	{
		WishVelocity += 100000;	
	}

	private bool isSprtinging = false;

	public void BuildWishVelocity()
	{
		var rot = EyeAngles.ToRotation();

		WishVelocity = 0;

		if ( !CantUseInputMove )
		{
			if ( Input.Down( "Forward" ) ) WishVelocity += rot.Forward;
			if ( Input.Down( "Backward" ) ) WishVelocity += rot.Backward;
			if ( Input.Down( "Left" ) ) WishVelocity += rot.Left;
			if ( Input.Down( "Right" ) ) WishVelocity += rot.Right;
		}
		
		WishVelocity = WishVelocity.WithZ( 0 );

		if ( !WishVelocity.IsNearZeroLength ) WishVelocity = WishVelocity.Normal;

		if ( Input.Pressed( "duck" ) ) isSprtinging = !isSprtinging;


		//if ( Input.Down( "Run" ) ) WishVelocity *= boosting ?  840.0f : 420.0f;
		if ( isSprtinging ) WishVelocity *= PlayerStats.UpgradedStats[PlayerStats.PlayerUpgradedStats.SprintSpeed];
		else WishVelocity *= PlayerStats.UpgradedStats[PlayerStats.PlayerUpgradedStats.WalkSpeed];
		//else WishVelocity *= boosting ? 400.0f : 200.0f;
	}
}
