public class HealthIncrease : ItemDef
{
	public override string Name => "Health Item";
	public override string Description => "Increases maximum health by 25 (+25 per stack).";
	public override string Icon => "ui/test/items/healthitem.png";
	public override ItemTier ItemTier => ItemTier.Uncommon;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Uncommon );
	public override Model Model => Model.Load( "models/citizen_props/roadcone01.vmdl_c" );
	public override int StatUpgradeAmount => 25;

	public override void ApplyUpgrade()
	{
		Log.Info( $"!!!!!!{Owner.Inventory.GetItemOwner( this )}!!!!!!" );
		Owner.Inventory.GetItemOwner( this ).UpgradedStats[Stats.PlayerUpgradedStats.Health] = Owner.Inventory.GetItemOwner( this ).UpgradedStats[Stats.PlayerUpgradedStats.Health] + StatUpgradeAmount;
		Owner.Components.Get<PlayerController>().Health += StatUpgradeAmount;
	}

	public override void RemoveUpgrade()
	{
		if(GetAmountFromInventory() == 0)
		{
			Owner.Inventory.GetItemOwner( this ).UpgradedStats[Stats.PlayerUpgradedStats.Health] = Owner.StartingStats[Stats.PlayerStartingStats.Health];
		}
		else
		{
			Owner.Inventory.GetItemOwner( this ).UpgradedStats[Stats.PlayerUpgradedStats.Health] = Owner.Inventory.GetItemOwner( this ).UpgradedStats[Stats.PlayerUpgradedStats.Health] - StatUpgradeAmount * GetAmountFromInventory();
		}

		//Owner.Inventory.GetItemOwner( this ).UpgradedStats[PlayerStats.PlayerUpgradedStats.AmountOfJumps] = Owner.StartingStats[PlayerStats.PlayerStartingStats.AmountOfJumps] + GetAmountFromInventory();
	}
}
