using Sandbox;

public class TestSlide : BaseAbilityItem
{
	[Property] public SlideMechanic SlideMechanic { get; set; }
	[Property] public override PlayerStats.PlayerUpgradedStats StatToUse { get; set; } = PlayerStats.PlayerUpgradedStats.SkillOneCoolDown;

	protected override void OnAwake()
	{
		base.OnAwake();

		MaxUseCount = (int)PlayerStats.UpgradedStats[PlayerStats.PlayerUpgradedStats.SkillOneUses];
		CurrentUseCount = MaxUseCount;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

	}
}
