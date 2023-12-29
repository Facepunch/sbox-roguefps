using Sandbox;

namespace RogueFPS;
public sealed class RogueFPSTestUltimate : Component
{
	[Property] public string Name { get; set; } = "Slide";
	[Property] public string Description { get; set; } = "Slide to get under small gaps";
	[Property] public string Icons { get; set; } = "roguefps/ui/test/ability/ab3.png";

	[Property] private RogueFPSPlayerController PlayerControllerComponent { get; set; }
	[Property] private RogueFPSCharacterController PlayerCharacterControllerComponent { get; set; }
	[Property] private PlayerStats PlayerStatsComponent { get; set; }


	[Property] private GameObject UltiBullet { get; set; }

	protected override void OnStart()
	{
		base.OnStart();

		PlayerControllerComponent = GameObject.Components.Get<RogueFPSPlayerController>();
		PlayerCharacterControllerComponent = GameObject.Components.Get<RogueFPSCharacterController>();
		PlayerStatsComponent = GameObject.Components.Get<PlayerStats>();

		ultiCoolDown = 0;
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		TryUlti();
	}

	public TimeUntil ultiCoolDown = 0;
	//public bool isSliding = false;
	//private float slideDuration = 1.0f; // Duration of slide in seconds
	private TimeUntil ultiEndTime;

	public void TryUlti()
	{
		if(Input.Pressed("reload") && ultiCoolDown <= 0 )
		{
			SceneUtility.Instantiate( UltiBullet, PlayerControllerComponent.Eye.Transform.Position, PlayerControllerComponent.EyeAngles.ToRotation() );

			ultiCoolDown = PlayerStatsComponent.UpgradedStats[PlayerStats.PlayerUpgradedStats.UltimateCoolDown];
		}
	}

	protected override void OnUpdate()
	{
		if ( PlayerStatsComponent != null )
		{
			PlayerStatsComponent.AddUltimate( "railblast", Icons, Name );
			PlayerStatsComponent.HandleUltimateCoolDown( ultiCoolDown );
			//PlayerStatsComponent.PickedUpAbilities.Add( "Slide", new RogueFPSPlayerStats.AbiliyHas( Icons, Name ) );
		}
	}
}
