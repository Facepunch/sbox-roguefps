using Sandbox;

namespace RogueFPS;

[Title( "Rogue FPS Slide Component" )]
[Category( "Player Upgrade" )]
[Icon( "upgrade", "red", "white" )]
public sealed class RogueFPSSlideUpgrade : Component
{
	[Property] public string Name { get; set; } = "Slide";
	[Property] public string Description { get; set; } = "Slide to get under small gaps";
	[Property] public string Icons { get; set; } = "roguefps/ui/test/ability/ab3.png";

	[Property] private RogueFPSPlayerController PlayerControllerComponent {get; set; }
	[Property] private RogueFPSCharacterController PlayerCharacterControllerComponent { get; set; }
	[Property] private PlayerStats PlayerStatsComponent { get; set; }

	private int slideCharges = 1; // Number of slide charges
	private float rechargeRate = 5f; // Time in seconds to recharge one slide charge
	private float timeSinceLastSlide = 0f; // Time since last slide

	protected override void OnStart()
	{
		base.OnStart();

		PlayerControllerComponent = GameObject.Components.Get<RogueFPSPlayerController>();
		PlayerCharacterControllerComponent = GameObject.Components.Get<RogueFPSCharacterController>();
		PlayerStatsComponent = GameObject.Components.Get<PlayerStats>();

		slideCoolDown = 0;
		slideCharges = (int)PlayerStatsComponent.StartingStats[PlayerStats.PlayerStartingStats.SkillOneUses];
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		RechargeSlideCharges();
		TryDuck();
	}


	private void RechargeSlideCharges()
	{
		if ( slideCharges < PlayerStatsComponent.UpgradedStats[PlayerStats.PlayerUpgradedStats.SkillOneUses] )
		{
			timeSinceLastSlide += Time.Delta;

			if ( slideCoolDown <= 0 )
			{
				slideCharges++;
				timeSinceLastSlide = 0f;

				if( slideCharges < PlayerStatsComponent.UpgradedStats[PlayerStats.PlayerUpgradedStats.SkillOneUses] )
				{
					slideCoolDown = PlayerStatsComponent.UpgradedStats[PlayerStats.PlayerUpgradedStats.SkillOneCoolDown];
				}


				Log.Info( "Slide charge replenished. Current charges: " + slideCharges );
			}
		}
		else
		{
			Log.Info( "Slide charges full." );
		}
	}

	public TimeUntil slideCoolDown = 0;
	public bool isSliding = false;
	private float slideDuration = 1.0f; // Duration of slide in seconds
	private TimeUntil slideEndTime;

	public void TryDuck()
	{
		// Start sliding
		if ( Input.Down( "run" ) && slideCharges >= 1 && !isSliding )
		{
			slideCharges--;
			PlayerControllerComponent.CantUseInputMove = true;
			isSliding = true;
			slideEndTime = slideDuration; // Set the time when the slide should end

			// Check if the player is moving
			if ( PlayerControllerComponent.WishVelocity.IsNearlyZero() )
			{
				// If the player is not moving, set a default forward direction for sliding
				var forwardDirection = PlayerControllerComponent.EyeAngles.ToRotation().Forward;
				PlayerCharacterControllerComponent.Velocity = forwardDirection + 500; // Adjust the multiplier for sliding speed
			}
			else
			{
				// If the player is moving, use the existing WishVelocity direction for sliding
				var slideVelocity = PlayerControllerComponent.WishVelocity.Normal * 500;
				PlayerCharacterControllerComponent.Velocity += slideVelocity;
			}

			PlayerCharacterControllerComponent.ApplyFriction( 0.01f ); // Reduced friction during slide
			if ( slideCoolDown <= 0 )
			{
				slideCoolDown = PlayerStatsComponent.UpgradedStats[PlayerStats.PlayerUpgradedStats.SkillOneCoolDown];
			}
		}

		// Check if sliding duration has elapsed
		if ( isSliding && slideEndTime <= 0 )
		{
			isSliding = false;
			PlayerControllerComponent.CantUseInputMove = false;
		}
		PlayerControllerComponent.EyeDuckOffset = PlayerControllerComponent.EyeDuckOffset.LerpTo( isSliding ? -32 : 0, Time.Delta * 10.0f );
	}

	protected override void OnUpdate()
	{
		if ( PlayerStatsComponent != null )
		{
			PlayerStatsComponent.AddAbility( "slide", Icons, Name );
			PlayerStatsComponent.HandleAbilityCoolDown( slideCoolDown );
			PlayerStatsComponent.HandleAbilityUses( slideCharges );
			//PlayerStatsComponent.PickedUpAbilities.Add( "Slide", new RogueFPSPlayerStats.AbiliyHas( Icons, Name ) );
		}
	}
}
