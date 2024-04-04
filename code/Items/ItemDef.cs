public class ItemDef
{
	public string Icon { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public GameObject PickUpPrefab { get; set; }
	public ItemTier ItemTier { get; set; }
	public Model Model { get; set; } = Model.Load( "models/citizen_props/balloonears01.vmdl_c" );
	public string ItemColor { get; set; }
	public PlayerStats Owner { get; set; }
	public int StatUpgradeAmount { get; set; } = 1;
	int ItemAmount { get; set; }

	public PrefabScene GetPickUpPrefab(string prefabPath)
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

	public void ApplyUpgrade()
	{
		// Get the current amount from the inventory
		int itemAmount = GetAmountFromInventory();

		// Adjust the stat based on the amount
		Owner.UpgradedStats[PlayerStats.PlayerUpgradedStats.AttackSpeed] += StatUpgradeAmount * itemAmount;
	}


	public void RemoveUpgrade()
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
