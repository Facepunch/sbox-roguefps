public class BaseKeyCard : ItemDef
{
	public virtual KeyCardColor KeyColor => KeyCardColor.Red;
	public override string ItemColor => ColorSelection.GetRarityColor(ItemTier.KeyCard);

}

public enum KeyCardColor
{
	Red,
	Green,
	Blue,
	Yellow,
	Purple,
	Orange,
	White,
	Black,
}
