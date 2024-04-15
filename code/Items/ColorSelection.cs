public class ColorSelection
{	
	public static string GetRarityColor( ItemTier upgrade )
	{
		return upgrade switch
		{
			ItemTier.Common => "#3A3A3C",
			ItemTier.Uncommon => "#1DB954",
			ItemTier.Rare => "#0077B5",
			ItemTier.Epic => "#9B30FF",
			ItemTier.Legendary => "#E00707",
			ItemTier.KeyCard => "#d93dc1",
			ItemTier.None => "#FFFFFF",
			_ => "#FFFFFF",
		};
	}
}
