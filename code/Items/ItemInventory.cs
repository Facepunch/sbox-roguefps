public class ItemInventory
{
	public List<InvetoryItem> itemPickUps = new List<InvetoryItem>();

	public void AddItem( ItemDef item )
	{
		int index = itemPickUps.FindIndex( x => x.Item.Name == item.Name );
		if ( index != -1 )
		{
			// Found item, increment amount
			var existingItem = itemPickUps[index];
			existingItem.Amount++; // Increment amount on the copy
			itemPickUps[index] = existingItem; // Put the modified copy back in the list
			existingItem.Item.ApplyUpgrade(); // Apply the upgrade to the player
			//Log.Info( $"Added {item.Name} to inventory. Amount: {existingItem.Amount}" );
		}
		else
		{
			// Item not found, add new
			itemPickUps.Add( new InvetoryItem { Item = item, Amount = 1 } );
			item.ApplyUpgrade(); // Apply the upgrade to the player
			//Log.Info( $"Added New {item.Name} to inventory. Amount: 1" );
		}
	}

	//Remove an item from the inventory, but only 1 at a time
	public void RemoveItem( ItemDef item )
	{
		for ( int i = 0; i < itemPickUps.Count; i++ )
		{
			if ( itemPickUps[i].Item.Name == item.Name )
			{
				var itemPickUp = itemPickUps[i];

				itemPickUp.Amount--;
				if ( itemPickUp.Amount <= 0 )
				{
					itemPickUps.RemoveAt( i );
					itemPickUp.Item.RemoveUpgrade();
				}
				else
				{
					itemPickUps[i] = itemPickUp;
					itemPickUp.Item.RemoveUpgrade();
				}
				break;
			}
		}
	}

	//Get the amount of a specific item in the inventory I don't think this is needed but it's here for now
	public int GetItemCount(ItemDef item)
	{
		var itemPickUp = itemPickUps.Find(x => x.Item.Name == item.Name);

		if (itemPickUp.Item != null)
		{
			return itemPickUp.Amount;
		}
		else
		{
			return 0;
		}
	}
}
public struct InvetoryItem
{
	public ItemDef Item;
	public PlayerStats OwnerStat;
	public int Amount;
}
