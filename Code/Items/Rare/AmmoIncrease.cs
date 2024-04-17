using static Sandbox.Services.Stats;

public class AmmoIncrease : ItemDef
{
	public override string Name => "Ammo Box";
	public override string Description => "Increase Ammo Count by 10% of the base ammo count.";
	public override string Icon => "ui/test/items/ammo.png";
	public override ItemTier ItemTier => ItemTier.Rare;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Rare );
	public override Model Model => Model.Load( "models/citizen_props/recyclingbin01.vmdl" );

	public override void ApplyUpgrade()
	{
		var startingAmmo = Owner.StartingStats[Stats.PlayerStartingStats.AmmoCount];
		var ammoIncrease = (int)(startingAmmo * 0.1);  // 10% increase

		Owner.UpgradedStats[Stats.PlayerUpgradedStats.AmmoCount] += ammoIncrease;

		Log.Info( $"Applied ammo upgrade: +{ammoIncrease}. Total ammo now: {Owner.UpgradedStats[Stats.PlayerUpgradedStats.AmmoCount]}" );
	}

	public override void RemoveUpgrade()
	{
		var startingAmmo = Owner.StartingStats[Stats.PlayerStartingStats.AmmoCount];
		var ammoIncrease = (int)(startingAmmo * 0.1);  // 10% increase

		Owner.UpgradedStats[Stats.PlayerUpgradedStats.AmmoCount] -= ammoIncrease;

		Log.Info( $"Removed ammo upgrade: -{ammoIncrease}. Total ammo now: {Owner.UpgradedStats[Stats.PlayerUpgradedStats.AmmoCount]}" );
	}
}
