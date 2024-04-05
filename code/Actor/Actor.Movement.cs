namespace RogueFPS;

public partial class Actor
{
	[Property, Group( "Movement" )] public CharacterController CharacterController { get; set; }
	[Property, Group( "Movement" )] public float BaseMovementSpeed { get; set; } = 100f;
	[Property, Group( "Data" ), ReadOnly] public Vector3 WishVelocity { get; set; }

	public Vector3 WishMove;

	public virtual Angles GetEyeAngles()
	{
		return new Angles( 0, Transform.Rotation.Yaw(), 0 );
	}

	/// <summary>
	/// Get the current friction for this actor
	/// </summary>
	/// <returns></returns>
	protected virtual float GetFriction()
	{
		if ( !CharacterController.IsOnGround ) return 4.0f;
		if ( CurrentFrictionOverride is not null ) return CurrentFrictionOverride.Value;

		return 4.0f;
	}

	protected virtual void BuildWishInput()
	{
	//	WishMove = 0;
	}


	[ConVar( "op_dev_speed" )]
	private static float DevSpeed { get; set; } = 0f;

	protected virtual float GetWishSpeed()
	{
		if ( CurrentSpeedOverride is not null ) return CurrentSpeedOverride.Value;

		if ( this is PlayerController player && DevSpeed > 0f )
		{
			return DevSpeed;
		}


		// Default speed
		return BaseMovementSpeed;
	}

	protected virtual void BuildWishVelocity()
	{
		WishVelocity = 0;

		if ( WishMove.x < 0f ) WishMove.x *= 0.7f;

		var rot = GetEyeAngles().WithPitch(0).WithRoll(0).ToRotation();
		var wishDirection = WishMove * rot;
		wishDirection = wishDirection.WithZ( 0 );

		WishVelocity = wishDirection * GetWishSpeed();

		if ( !CharacterController.IsOnGround ) WishVelocity *= 0.1f;
	}

	protected float baseAcceleration = 10f;

	protected Vector3 HalfGravity => Scene.PhysicsWorld.Gravity * Time.Delta * 0.5f;

	protected virtual void UpdateMovement()
	{
		BuildWishInput();
		DoMechanicsUpdate();
		BuildWishVelocity();
		Accelerate();
	}

	protected virtual void Move()
	{
		CharacterController.Move();
	}

	private void ApplyGravity()
	{
		if ( !CharacterController.IsOnGround )
		{
			CharacterController.Velocity += HalfGravity;
		}
		else
		{
			CharacterController.Velocity = CharacterController.Velocity.WithZ( 0 );
		}
	}

	protected virtual void Accelerate()
	{
		CharacterController.ApplyFriction( GetFriction() );

		if ( CurrentAccelerationOverride is not null )
		{
			CharacterController.Acceleration = CurrentAccelerationOverride.Value;
		}
		else
		{
			CharacterController.Acceleration = baseAcceleration;
		}

		if ( CharacterController.IsOnGround )
		{
			CharacterController.Accelerate( WishVelocity );
			CharacterController.Velocity = CharacterController.Velocity.WithZ( 0 );
		}
		else
		{
			CharacterController.Velocity += HalfGravity;
			CharacterController.Accelerate( WishVelocity );
		}

		Move();
		ApplyGravity();
	}
}
