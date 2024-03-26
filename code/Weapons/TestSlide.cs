using Sandbox;

public class TestSlide : BaseAbilityItem
{
	[Property] public SlideMechanic SlideMechanic { get; set; }

	public override int MaxUseCount { get; set; }
	public override PlayerStats.PlayerUpgradedStats StatToUse { get; set; } = PlayerStats.PlayerUpgradedStats.SkillOneCoolDown;

	protected override void OnAwake()
	{
		base.OnAwake();

		MaxUseCount = (int)PlayerStats.UpgradedStats[PlayerStats.PlayerUpgradedStats.SkillOneUses];
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();


	}

	public override void DoAction()
	{
		base.DoAction();

	}
}
