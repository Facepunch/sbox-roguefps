using Sandbox;

public class TestSlide : BaseAbilityItem
{
	[Property] public SlideMechanic SlideMechanic { get; set; }
	[Property] public override Stats.PlayerUpgradedStats StatToUse { get; set; } = Stats.PlayerUpgradedStats.SkillOneCoolDown;

	protected override void OnAwake()
	{
		base.OnAwake();

		MaxUseCount = (int)Stats.UpgradedStats[Stats.PlayerUpgradedStats.SkillOneUses];
		CurrentUseCount = MaxUseCount;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

	}
}
