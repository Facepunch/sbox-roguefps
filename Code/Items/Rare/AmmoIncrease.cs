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
	}

	public override void RemoveUpgrade()
	{
		var startingAmmo = Owner.StartingStats[Stats.PlayerStartingStats.AmmoCount];
		var ammoIncrease = (int)(startingAmmo * 0.1);  // 10% increase

		Owner.UpgradedStats[Stats.PlayerUpgradedStats.AmmoCount] -= ammoIncrease;
	}
	public override List<StyledTextPart> GetStyledDescriptionParts()
	{
		return new List<StyledTextPart>
		{
			new("Increase ammo capacity by ", "default"),
			new("10%", "utility"),
			new("(+10% per stack)", "stack")
		};
	}
}
