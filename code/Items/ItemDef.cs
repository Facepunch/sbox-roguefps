public class ItemDef
{
	public virtual string Name { get; }
	public virtual string Description { get; }
	public virtual string Icon { get; }
	public virtual ItemTier ItemTier { get; }
	public virtual Model Model { get; }
	public virtual string ItemColor {get;}
	public virtual int StatUpgradeAmount { get; }
	public virtual GameObject PickUpPrefab { get; }
	public PlayerStats Owner { get; set; }
	int ItemAmount { get; set; }

	public static PrefabScene GetPickUpPrefab(string prefabPath)
	{
		if (string.IsNullOrEmpty(prefabPath))
		{
			return null;
		}
		return SceneUtility.GetPrefabScene( ResourceLibrary.Get<PrefabFile>( prefabPath ) );
	}
	public void OnPickUp(PlayerStats player)
	{
		Owner = player;
	}

	public int GetAmountFromInventory()
	{
		return Owner.Inventory.GetItemCount(this);
	}

	public virtual void ApplyUpgrade()
	{
		// Get the current amount from the inventory
		int itemAmount = GetAmountFromInventory();

		// Adjust the stat based on the amount
		Owner.UpgradedStats[PlayerStats.PlayerUpgradedStats.AttackSpeed] += StatUpgradeAmount * itemAmount;
	}


	public virtual void RemoveUpgrade()
	{
		// Get the current amount from the inventory
		int itemAmount = GetAmountFromInventory();

		// Decrease the stat. If the itemAmount is 0, it means this was the last item, and we should remove all related upgrades.
		if ( itemAmount == 0 )
		{
			Owner.UpgradedStats[PlayerStats.PlayerUpgradedStats.AttackSpeed] -= StatUpgradeAmount;
		}
		else
		{
			// If there are still items left, adjust the stat accordingly. This might not be necessary
			// depending on your logic for when RemoveUpgrade is called.
			Owner.UpgradedStats[PlayerStats.PlayerUpgradedStats.AttackSpeed] = Owner.GetStartingStat( PlayerStats.PlayerStartingStats.AttackSpeed ) + StatUpgradeAmount * itemAmount;
		}
	}
}
