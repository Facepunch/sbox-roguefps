using Sandbox;
using System;

public partial class FallDamageMechanic : PlayerMechanic
{
	//This isn't working how I want to but it's a start.

	private bool wasAirborne = false;
	private float landingVelocity = 0.0f;

	public override bool ShouldBecomeActive()
	{
		return true;
	}

	public override void OnActiveUpdate()
	{
		if ( !wasAirborne && !Player.CharacterController.IsOnGround )
		{
			wasAirborne = true;
			landingVelocity = 0.0f;
		}
		else if ( wasAirborne && Player.CharacterController.IsOnGround )
		{
			wasAirborne = false;
			if ( MathF.Abs( landingVelocity ) > 750.0f )
			{
				Player.OnDamage( new DamageInfo
				{
					Damage = CalculateFallDamage( MathF.Abs( landingVelocity) )
				} );
			}
		}

		if ( !Player.CharacterController.IsOnGround )
		{
			landingVelocity = Player.CharacterController.Velocity.z;
		}

	}

	float CalculateFallDamage( float fallSpeed )
	{

		return MathF.Max( 0.0f, MathF.Abs( fallSpeed ) - 50.0f ) * 0.02f;
	}
}
