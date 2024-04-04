public class ItemDefTestTwo : ItemDef
{
	public ItemDefTestTwo()
	{
		Name = "Test Item Two";
		Description = "A test item again.";
		Icon = "ui/test/items/hyper.png";
		ItemTier = ItemTier.Legendary;
		ItemColor = ColorSelection.GetRarityColor( ItemTier );
		Model = Model.Load( "models/citizen_props/balloonears01.vmdl_c" );
		StatUpgradeAmount = 1;
		PickUpPrefab = GetPickUpPrefab( "prefab/weapon/fx/bullettracer.prefab" );
	}
}
