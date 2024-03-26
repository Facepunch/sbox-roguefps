using RogueFPS;

public class TestTwoItem : BaseItem
{
	public override string ItemName { get; set; } = "Test Item 2";
	public override string UpgradeIcon { get; set; } = "ui/test/items/hyper.png";
	public override string ItemDescription { get; set; } = "This is a test item 2";
	public override PlayerStats.PlayerStartingStats UpgradeType { get; set; } = PlayerStats.PlayerStartingStats.AmountOfJumps;
	public override float UpgradeAmount { get; set; } = 1;
	public override bool IsStatUpgrade { get; set; } = true;

	public override void CalculateUpdgrade()
	{
		base.CalculateUpdgrade();
	}
}
