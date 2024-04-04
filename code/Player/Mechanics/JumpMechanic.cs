namespace RogueFPS;

/// <summary>
/// A sprinting mechanic.
/// </summary>
public partial class JumpMechanic : BasePlayerControllerMechanic
{
	//Keep Track of the amount of jumps performed.
	private int jumpCount = 0;
	
	public override bool ShouldBecomeActive()
	{	
		return Input.Pressed( "Jump" ) && !HasAnyTag( "slide" ) && jumpCount < PlayerStatsComponent.UpgradedStats[PlayerStats.PlayerUpgradedStats.AmountOfJumps];
	}

	public override void OnActiveUpdate()
	{
		base.OnActiveUpdate();

		var inventory = PlayerController.PlayerStatsComponent.Inventory;
		foreach ( var item in inventory.itemPickUps )
		{
			item.Item.OnJump();
		}


		PlayerController.CharacterController.IsOnGround = false;
		PlayerController.CharacterController.Velocity = PlayerController.CharacterController.Velocity.WithZ( 0 );
		float jumpForce = CalculateJumpForce( PlayerStatsComponent.UpgradedStats[PlayerStats.PlayerUpgradedStats.JumpHeight] );
		PlayerController.CharacterController.Velocity += Vector3.Up * jumpForce;

		jumpCount++;
	}


	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( PlayerController.IsGrounded )
			jumpCount = 0;
	}

	private float CalculateJumpForce( float height )
	{
		// Implement the calculation for jump force based on the desired jump height
		// This is a simple physics formula where jumpForce = sqrt(2 * height * gravity)
		return (float)Math.Sqrt( 2 * height * PlayerController.Gravity.z );
	}

}
