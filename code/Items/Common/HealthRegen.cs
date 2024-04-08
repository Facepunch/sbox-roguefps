public class HealthRegen : ItemDef
{
	public override string Name => "Health Regen";
	public override string Description => "Increases health Regen by +3% (+3% per Stack).";
	public override string Icon => "ui/test/items/regen.png";
	public override ItemTier ItemTier => ItemTier.Common;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Common );
	public override Model Model => Model.Load( "models/citizen_props/hotdog01.vmdl_c" );
	public override int StatUpgradeAmount => 25;

	public override void ApplyUpgrade()
	{
		Log.Info( $"!!!!!!{Owner.Inventory.GetItemOwner( this )}!!!!!!" );
		Owner.Inventory.GetItemOwner( this ).UpgradedStats[Stats.PlayerUpgradedStats.HealthRegen] = Owner.Inventory.GetItemOwner( this ).UpgradedStats[Stats.PlayerUpgradedStats.HealthRegen] + 0.05f;
	}

	public override void RemoveUpgrade()
	{
		Owner.Inventory.GetItemOwner( this ).UpgradedStats[Stats.PlayerUpgradedStats.HealthRegen] = Owner.Inventory.GetItemOwner( this ).UpgradedStats[Stats.PlayerUpgradedStats.HealthRegen] - 0.03f;
	}
}
