using RogueFPS;

public class TestThreeItem : BaseItem
{
	public override string ItemName { get; set; } = "Speed-Up";
	public override string UpgradeIcon { get; set; } = "ui/test/items/sink.png";
	public override string ItemDescription { get; set; } = "+25% Sprint Speed";
	public override PlayerStats.PlayerStartingStats UpgradeType { get; set; } = PlayerStats.PlayerStartingStats.SprintMultiplier;
	public override UpgradeRarity Rarity { get; set; } = UpgradeRarity.Legendary;
	public override float UpgradeAmount { get; set; } = 25f;
	public override bool AsAPercent { get; set; } = true;
	public override bool IsStatUpgrade { get; set; } = true;
}
