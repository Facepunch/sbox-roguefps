public class AddOneSkillTwo : ItemDef
{
	public override string Name => "Add One Skill Two";
	public override string Description => "Add +1 to your secoundary skill";
	public override string Icon => "ui/test/items/extendedmags.icon.png";
	public override ItemTier ItemTier => ItemTier.Common;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Common );
	public override Model Model => Model.Load( "models/citizen_props/coffeemug01.vmdl" );

	public override void ApplyUpgrade()
	{
		var startingSkill2 = Owner.StartingStats[Stats.PlayerStartingStats.SkillTwoUses];
		var skill2Increase = (int)(startingSkill2 + 1);  // +1 increase

		Owner.UpgradedStats[Stats.PlayerUpgradedStats.SkillTwoUses] += 1;
	}

	public override void RemoveUpgrade()
	{
		var startingSkill2 = Owner.StartingStats[Stats.PlayerStartingStats.SkillTwoUses];
		var skill2Increase = (int)(startingSkill2 + 1);  // -1 increase

		Owner.UpgradedStats[Stats.PlayerUpgradedStats.SkillTwoUses] -= 1;
	}
}
