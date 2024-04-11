public sealed class ItemHelper : Component, Component.ITriggerListener
{
	[Property]
	public ItemDef Item { get; set; } = null;
	[Property]
	public BaseEquipmentItem Equipment { get; set; } = null;
	protected override void OnUpdate()
	{
		base.OnUpdate();

		var glow = GameObject.Components.Get<HighlightOutline>(FindMode.InSelf);
		if(Item != null)
		{
			glow.Color = Item.ItemColor;
		}
		else if ( Equipment != null)
		{
			glow.Color = Equipment.ItemColor;
		}
		else
		{		
			glow.Color = Color.White;
		}
		
	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( other.GameObject.Tags.Has( "player" ) )
		{
		
			var player = other.GameObject.Components.Get<Stats>(FindMode.InParent).GameObject;
			if ( player != null )
			{
				if( Equipment == null )
				{
					OnPickedUp( player );
				}
			}
			else
			{
				Log.Warning( "PlayerStats component not found on the player." );
			}
		}
	}

	ItemDef item;

	public void OnPickedUp(GameObject player)
	{
		var stats = player.Components.Get<Stats>();
		if ( stats != null )
		{
			var inventory = stats.Inventory;
			if( Equipment != null && inventory.equippedItem != null && Equipment.IsEquipment)
			{
				inventory.ReplaceEquipment( Equipment );
				RecreateEquipment( inventory.equippedItem );

				return;
			}

			if( Equipment != null )
			{
				Log.Info( "Equipping item" );
				inventory.AddEquipment( Equipment );
			}
			else
			{
				inventory.AddItem( Item );
			}

			if( Equipment != null )
			{
				item = Equipment;
			}
			else
			{
				item = Item;
			}

			item.OnPickUp( stats );
			item.Owner = stats;

			var pickupui = player.Components.Get<PickedUpItemUI>( FindMode.EnabledInSelfAndDescendants );
			pickupui.NewItem( item.Name, item.Description, item.Icon, item.ItemColor );

			var interact = player.Components.Get<InteractorUse>( FindMode.EnabledInSelfAndDescendants );
			interact.DestroyUI();

			//Player.AddComponent<RogueFPSSlideUpgrade>( true );
			GameObject.Destroy();
			if ( GameObject.Parent != Scene )
			{
				GameObject.Parent.Destroy();
			}
		}
	}

	public void RecreateEquipment(BaseEquipmentItem item)
	{
		Components.Get<ModelRenderer>( FindMode.InSelf ).Model = item.Model;
		Components.Get<ModelRenderer>( FindMode.InSelf ).Tint = item.ItemColor;
		Equipment = item;
	}
}
