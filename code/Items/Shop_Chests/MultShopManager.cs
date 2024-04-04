using Sandbox;
using Sandbox.UI;

public sealed class MultShopManager : Component
{
	[Property] public MultiShopItem Slot1 { get; set; }
	[Property] public MultiShopItem Slot2 { get; set; }
	[Property] public MultiShopItem Slot3 { get; set; }
	[Property] PrefabScene WorldUI { get; set; }
	[Property] GameObject PriceLocation { get; set; }
	[Property] public int Cost { get; set; } = 25;

	public bool itemBeenPicked = false;

	GameObject costPanel;

	protected override void OnStart()
	{
		base.OnStart();

		costPanel = WorldUI.Clone();
		if(PriceLocation != null)
		costPanel.Transform.Position = PriceLocation.Transform.Position;
		costPanel.Components.Get<WorldCostPanel>().Value = Cost;
	}

	public void ItemPicked()
	{
		itemBeenPicked = true;

		costPanel.Destroy();

		if (Slot1 != null)
		{
			Slot1.CloseItem();
		}
		if (Slot2 != null)
		{
			Slot2.CloseItem();
		}
		if (Slot3 != null)
		{
			Slot3.CloseItem();
		}
	}

}
