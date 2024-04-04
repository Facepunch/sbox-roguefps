public class ItemDefTestThree : ItemDef
{
	public ItemDefTestThree()
	{
		Name = "Test Item Three";
		Description = "A test item again again.";
		Icon = "ui/test/items/icecream.png";
		ItemTier = ItemTier.Epic;
		ItemColor = ColorSelection.GetRarityColor( ItemTier );
		Model = Model.Load( "models/citizen_props/balloonregular01.vmdl_c" );
		StatUpgradeAmount = 1;
		PickUpPrefab = GetPickUpPrefab( "prefab/weapon/fx/bullettracer.prefab" );
	}
}
