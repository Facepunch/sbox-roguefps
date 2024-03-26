using RogueFPS;

public class TestTwoItem : BaseItem
{
	[Property] public override string ItemName { get; set; } = "Test Item 2";
	public override bool IsStatUpgrade { get; set; } = false;
}
