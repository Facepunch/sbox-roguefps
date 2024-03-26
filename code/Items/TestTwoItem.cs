using RogueFPS;

public class TestTwoItem : BaseItem
{
	[Property] public override string ItemName { get; set; } = "Test Item 2";
	[Property] public override string UpgradeIcon { get; set; } = "ui/test/items/hyper.png";
	public override bool IsStatUpgrade { get; set; } = false;
}
