
public class FireHit : ItemDef
{
	public override string Name => "Fire Hit";
	public override string Description => "Set hit target on fire.";
	public override string Icon => "ui/test/items/fireplace.png";
	public override ItemTier ItemTier => ItemTier.Uncommon;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Uncommon );
	public override Model Model => Model.Load( "models/citizen_props/balloontall01.vmdl_c" );
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

	public override void OnHit( GameObject target )
	{
		if ( target != null )
		{
			if ( target.Components.Get<FireComponent>() == null )
			{
				target.Components.Create<FireComponent>().Length += 5;
			}
			else
			{
				target.Components.Get<FireComponent>().Length += 5;
				target.Components.Get<FireComponent>().Hit();
			}
		}
	}
}
