public class ItemDefTest : ItemDef
{
	public override string Name => "Test Item";
	public override string Description => "A test item.";
	public override string Icon => "ui/test/items/chips.png";
	public override ItemTier ItemTier => ItemTier.Legendary;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Legendary );
	public override Model Model => Model.Load("models/citizen_props/balloonheart01.vmdl_c");
	public override int StatUpgradeAmount => 1;
	public override GameObject PickUpPrefab => GetPickUpPrefab("prefab/weapon/fx/bullettracer.prefab");
}
