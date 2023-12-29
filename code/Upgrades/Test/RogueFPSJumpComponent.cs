
using Sandbox.Citizen;

namespace RogueFPS;

[Title( "Rogue FPS Jump Component" )]
[Category( "Player Upgrade" )]
[Icon( "upgrade", "red", "white" )]
public partial class RogueFPSJumpComponent : PlayerControlBaseComponent
{	
	[Property] private RogueFPSPlayerController PlayerControllerComponent { get; set; }
	[Property] private RogueFPSCharacterController PlayerCharacterControllerComponent { get; set; }
	[Property] private PlayerStats PlayerStatsComponent { get; set; }

	//Use my own one later.
	[Property] CitizenAnimationHelper AnimationHelper { get; set; }

	//Keep Track of the amount of jumps performed.
	private int jumpCount = 0;

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( Input.Pressed( "Jump" ) && jumpCount < PlayerStatsComponent.UpgradedStats[PlayerStats.PlayerUpgradedStats.AmountOfJumps] )
		{
			DoJump();

			AnimationHelper?.TriggerJump();

			jumpCount++;
		}
	}

	private void DoJump()
	{
		var cc = GameObject.Components.Get<RogueFPSCharacterController>();
		if ( cc == null ) return;

		cc.IsOnGround = false;
		// Reset vertical velocity to ensure consistent jump height
		cc.Velocity = cc.Velocity.WithZ( 0 );
		// Calculate and apply jump force
		float jumpForce = CalculateJumpForce( PlayerStatsComponent.UpgradedStats[PlayerStats.PlayerUpgradedStats.JumpHeight] );
		cc.Velocity += Vector3.Up * jumpForce;

		AnimationHelper?.TriggerJump();
	}

	private float CalculateJumpForce( float height )
	{
		// Implement the calculation for jump force based on the desired jump height
		// This is a simple physics formula where jumpForce = sqrt(2 * height * gravity)
		return (float)Math.Sqrt( 2 * height * PlayerControllerComponent.Gravity.z );
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		if ( PlayerCharacterControllerComponent.IsOnGround )
		{
			jumpCount = 0;

			PlayerCharacterControllerComponent.Velocity = PlayerCharacterControllerComponent.Velocity.WithZ( 0 );
			PlayerCharacterControllerComponent.Accelerate( PlayerControllerComponent.WishVelocity );
			if ( !PlayerControllerComponent.CantUseInputMove )
			{
				PlayerCharacterControllerComponent.ApplyFriction( 4.0f );
			}
			else
			{
				PlayerCharacterControllerComponent.ApplyFriction( 0.15f );
			}
		}
		else
		{
			PlayerCharacterControllerComponent.Velocity -= PlayerControllerComponent.Gravity * Time.Delta * 0.5f;
			PlayerCharacterControllerComponent.Accelerate( PlayerControllerComponent.WishVelocity.ClampLength( 50 ) );
			PlayerCharacterControllerComponent.ApplyFriction( 0.1f );
		}
	}
}
