public class AddJump : ItemDef
{
	public override string Name => "Jumpy";
	public override string Description => "+1 Jump.";
	public override string Icon => "ui/test/items/hyper.png";
	public override ItemTier ItemTier => ItemTier.Uncommon;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Uncommon );
	public override Model Model => Model.Load("models/citizen_props/balloonears01.vmdl_c");
	public override int StatUpgradeAmount => 1;

	public override void ApplyUpgrade()
	{
		Log.Info( $"!!!!!!{Owner.Inventory.GetItemOwner( this )}!!!!!!" );
		Owner.Inventory.GetItemOwner( this ).UpgradedStats[PlayerStats.PlayerUpgradedStats.AmountOfJumps] = GetAmountFromInventory() + 1;
	}

	public override void RemoveUpgrade()
	{
		if(GetAmountFromInventory() == 0)
		{
			Owner.Inventory.GetItemOwner( this ).UpgradedStats[PlayerStats.PlayerUpgradedStats.AmountOfJumps] = Owner.StartingStats[PlayerStats.PlayerStartingStats.AmountOfJumps];
		}
		else
		{
			Owner.Inventory.GetItemOwner( this ).UpgradedStats[PlayerStats.PlayerUpgradedStats.AmountOfJumps] = GetAmountFromInventory();
		}

		//Owner.Inventory.GetItemOwner( this ).UpgradedStats[PlayerStats.PlayerUpgradedStats.AmountOfJumps] = Owner.StartingStats[PlayerStats.PlayerStartingStats.AmountOfJumps] + GetAmountFromInventory();
	}
}
