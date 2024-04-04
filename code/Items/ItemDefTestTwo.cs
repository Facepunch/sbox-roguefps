public class ItemDefTestTwo : ItemDef
{
	public override string Name => "Test Item Two";
	public override string Description => "A test item again.";
	public override string Icon => "ui/test/items/hyper.png";
	public override ItemTier ItemTier => ItemTier.Uncommon;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Uncommon );
	public override Model Model => Model.Load("models/citizen_props/balloonears01.vmdl_c");
	public override int StatUpgradeAmount => 1;
	public override GameObject PickUpPrefab => GetPickUpPrefab("prefab/weapon/fx/bullettracer.prefab");
}
