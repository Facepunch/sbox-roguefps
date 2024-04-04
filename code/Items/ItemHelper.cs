public sealed class ItemHelper : Component, Component.ITriggerListener
{
	[Property]
	public ItemDef Item { get; set; }
	protected override void OnPreRender()
	{
		base.OnPreRender();
	
		//Cba to do this properly
		var glow = GameObject.Components.Get<HighlightOutline>(FindMode.EverythingInSelf);
		if(Item != null)
		{
			glow.Color = Item.ItemColor;
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
			var player = other.GameObject.Components.Get<PlayerStats>(FindMode.InParent).GameObject;
			if ( player != null )
			{
				OnPickedUp( player );
			}
			else
			{
				Log.Warning( "PlayerStats component not found on the player." );
			}
		}
	}

	public void OnPickedUp(GameObject player)
	{
		// This is called when the item is picked up by the player.
		// You can add any logic here that should be executed when the item is picked up.

		var stats = player.Components.Get<PlayerStats>();
		if ( stats != null )
		{
			var inventory = stats.Inventory;
			inventory.AddItem( Item );

			var pickupui = player.Components.Get<PickedUpItemUI>( FindMode.EnabledInSelfAndDescendants );
			pickupui.NewItem( Item.Name, Item.Description, Item.Icon, Item.ItemColor );

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
}
