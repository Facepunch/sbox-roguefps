public class ItemDefTest : ItemDef
{
	public ItemDefTest()
	{
		Name = "Test Item";
		Description = "A test item.";
		Icon = "ui/test/items/chips.png";
		ItemTier = ItemTier.Rare;
		ItemColor = ColorSelection.GetRarityColor( ItemTier );
		Model = Model.Load( "models/citizen_props/balloonheart01.vmdl_c" );
		StatUpgradeAmount = 1;
		PickUpPrefab = GetPickUpPrefab( "prefab/weapon/fx/bullettracer.prefab" );
	}
}
