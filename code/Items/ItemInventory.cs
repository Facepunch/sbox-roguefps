public class ItemInventory
{
	public List<InvetoryItem> itemPickUps = new List<InvetoryItem>();

	public Stats playerStats;

	public ItemInventory( Stats stats )
	{
		playerStats = stats;

		Log.Info( "Inventory Created" );
	}

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
			Log.Info( $"Added {item.Name} to inventory. Amount: {existingItem.Amount}" );
		}
		else
		{
			// Item not found, add new
			itemPickUps.Add( new InvetoryItem { Item = item, Amount = 1, OwnerStat = playerStats } );
			item.OnPickUp( playerStats ); // Set the owner of the item
			item.ApplyUpgrade(); // Apply the upgrade to the player
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

	//Get the owner of the item
	public Stats GetItemOwner(ItemDef item)
	{
		var itemPickUp = itemPickUps.Find(x => x.Item.Name == item.Name);

		if (itemPickUp.Item != null)
		{
			return itemPickUp.OwnerStat;
		}
		else
		{
			return null;
		}
	}
}
public struct InvetoryItem
{
	public ItemDef Item;
	public Stats OwnerStat;
	public int Amount;
}
