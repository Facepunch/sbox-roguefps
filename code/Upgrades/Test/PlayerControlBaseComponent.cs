using Sandbox.Citizen;

namespace RogueFPS;
public class PlayerControlBaseComponent : Component
{	
	[Property] private RogueFPSPlayerController PlayerControllerComponent { get; set; }
	[Property] private RogueFPSCharacterController PlayerCharacterControllerComponent { get; set; }
	[Property] private PlayerStats PlayerStatsComponent { get; set; }

	//Use my own one later.
	[Property] CitizenAnimationHelper AnimationHelper { get; set; }


	protected override void OnStart()
	{
		base.OnStart();

		PlayerControllerComponent = GameObject.Components.Get<RogueFPSPlayerController>();
		PlayerCharacterControllerComponent = GameObject.Components.Get<RogueFPSCharacterController>();
		PlayerStatsComponent = GameObject.Components.Get<PlayerStats>();
		
	}
}
