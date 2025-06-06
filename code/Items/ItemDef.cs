﻿public class ItemDef
{
	public virtual string Name { get; }
	public virtual string Description { get; }
	public virtual string Icon { get; }
	public virtual ItemTier ItemTier { get; }
	public virtual Model Model { get; }
	public virtual string ItemColor {get;}
	public virtual int StatUpgradeAmount { get; }
	public virtual GameObject PickUpPrefab { get; }
	public virtual bool IsScrap { get; } = false;
	public virtual bool IsEquipment { get; } = false;
	public Stats Owner { get; set; }
	int ItemAmount { get; set; }

	public static PrefabScene GetPickUpPrefab(string prefabPath)
	{
		if (string.IsNullOrEmpty(prefabPath))
		{
			return null;
		}
		return SceneUtility.GetPrefabScene( ResourceLibrary.Get<PrefabFile>( prefabPath ) );
	}
	public void OnPickUp(Stats player)
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
		Owner.UpgradedStats[Stats.PlayerUpgradedStats.AttackSpeed] += StatUpgradeAmount * itemAmount;
	}


	public virtual void RemoveUpgrade()
	{
		// Get the current amount from the inventory
		int itemAmount = GetAmountFromInventory();

		// Decrease the stat. If the itemAmount is 0, it means this was the last item, and we should remove all related upgrades.
		if ( itemAmount == 0 )
		{
			Owner.UpgradedStats[Stats.PlayerUpgradedStats.AttackSpeed] -= StatUpgradeAmount;
		}
		else
		{
			// If there are still items left, adjust the stat accordingly. This might not be necessary
			// depending on your logic for when RemoveUpgrade is called.
			Owner.UpgradedStats[Stats.PlayerUpgradedStats.AttackSpeed] = Owner.GetStartingStat( Stats.PlayerStartingStats.AttackSpeed ) + StatUpgradeAmount * itemAmount;
		}
	}
	public virtual List<StyledTextPart> GetStyledDescriptionParts()
	{
		return new List<StyledTextPart> { new( Description, "default" ) };
	}

	public virtual void OnFall() { }
	public virtual float OnFallDamage(float damage) { return damage; }
	public virtual void OnUse() { }
	public virtual void OnJump() { }
	public virtual void OnShoot() { }
	public virtual void OnHit(GameObject target) { }
}

public class StyledTextPart
{
	public string Text { get; set; }
	public string Style { get; set; }

	public StyledTextPart( string text, string style )
	{
		Text = text;
		Style = style;
	}
}
