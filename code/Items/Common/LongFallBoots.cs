public class LongFallBoots : ItemDef
{
	public override string Name => "Long Fall Boots";
	public override string Description => "Reduce Falldamage by 10% (+10% Stack) anymore that 10 will do no effect.";
	public override string Icon => "ui/test/items/fallboots.png";
	public override ItemTier ItemTier => ItemTier.Common;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Common );
	public override Model Model => Model.Load( "models/citizen_props/wheel02.vmdl_c" );
	public override int StatUpgradeAmount => 25;

	public override void ApplyUpgrade()
	{
		Log.Info( $"!!!!!!{Owner.Inventory.GetItemOwner( this )}!!!!!!" );
	}

	public override void RemoveUpgrade()
	{
	}

	public override float OnFallDamage( float damage )
	{
		float damageReduction = damage * 0.1f * GetAmountFromInventory();
		return -damageReduction;
	}
}
