public class LegendaryScrap : ItemDef
{
	public override string Name => "Legendary Scrap";
	public override string Description => "Does nothing.";
	public override string Icon => "ui/test/items/scrap.png";
	public override ItemTier ItemTier => ItemTier.Legendary;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Legendary );
	public override Model Model => Model.Load( "models/citizen_props/trashbag02.vmdl_c" );
	public override int StatUpgradeAmount => 25;
	public override bool IsScrap => true;

	public override void ApplyUpgrade()
	{

	}

	public override void RemoveUpgrade()
	{

	}
}
