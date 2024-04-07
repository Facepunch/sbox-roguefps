public class RareScrap : ItemDef
{
	public override string Name => "Rare Scrap";
	public override string Description => "Does nothing.";
	public override string Icon => "ui/test/items/scrap.png";
	public override ItemTier ItemTier => ItemTier.Rare;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Rare );
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
