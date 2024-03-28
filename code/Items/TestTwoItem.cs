using RogueFPS;

public class TestTwoItem : BaseItem
{
	public override string ItemName { get; set; } = "Monkey Time";
	public override string UpgradeIcon { get; set; } = "ui/test/items/icecream.png";
	public override string ItemDescription { get; set; } = "+1 Jumps";
	public override PlayerStats.PlayerStartingStats UpgradeType { get; set; } = PlayerStats.PlayerStartingStats.AmountOfJumps;
	public override UpgradeRarity Rarity { get; set; } = UpgradeRarity.Rare;
	public override float UpgradeAmount { get; set; } = 1;
	public override bool IsStatUpgrade { get; set; } = true;

	public override void CalculateUpdgrade()
	{
		var stats = Player.Components.Get<PlayerStats>();
		var item = Player.Components.Get<TestTwoItem>();
		Log.Info( Amount );

		if ( stats != null && item != null )
		{
			stats.UpgradedStats[stats.ConvertToUpgradedStat( UpgradeType )] = stats.GetStartingStat( UpgradeType ) + item.Amount;

		}
	}
}
