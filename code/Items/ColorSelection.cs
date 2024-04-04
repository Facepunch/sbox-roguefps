public class ColorSelection
{	public static string GetRarityColor( ItemTier upgrade )
	{
		switch ( upgrade )
		{
			case ItemTier.Common:
				return "#3A3A3C";
			case ItemTier.Uncommon:
				return "#1DB954";
			case ItemTier.Rare:
				return "#0077B5";
			case ItemTier.Epic:
				return "#9B30FF";
			case ItemTier.Legendary:
				return "#E00707";
			case ItemTier.None:
				return "#FFFFFF";
			default:
				return "#FFFFFF";
		}
	}
}
