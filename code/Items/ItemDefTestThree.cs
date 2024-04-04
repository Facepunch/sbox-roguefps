public class ItemDefTestThree : ItemDef
{
	public override string Name => "Test Item Three";
	public override string Description => "A test item again again.";
	public override string Icon => "ui/test/items/icecream.png";
	public override ItemTier ItemTier => ItemTier.Epic;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Epic );
	public override Model Model => Model.Load("models/citizen_props/balloonregular01.vmdl_c");
	public override int StatUpgradeAmount => 1;
	public override GameObject PickUpPrefab => GetPickUpPrefab("prefab/weapon/fx/bullettracer.prefab");

}
