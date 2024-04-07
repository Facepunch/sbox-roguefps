using Sandbox;

public sealed class ItemScrapper : Interactable, Component.ITriggerListener
{
	List<PrefabFile> Items;

	[Property]
	[Group( "Item Chest" )]
	GameObject ItemSpawnLocation { get; set; }


	[Property]
	bool UseRandomItem { get; set; } = true;

	[Property]
	GameObject Top { get; set; }

	[Property] PrefabScene WorldUI { get; set; }

	GameObject _UI;
	WorldCostPanel _Panel;

	PrefabScene RandomItem { get; set; }

	public override string Name { get; set; } = "Scrap Items";
	protected override void OnStart()
	{
		base.OnStart();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

	}

	public override void OnInteract(GameObject player)
	{
		var screen = player.Components.Get<ScreenPanel>(FindMode.InChildren);
		var scrapui = screen.Components.Create<ScrapItemUI>();
		var playerInventory = player.Components.Get<Stats>();
		scrapui.SetItem( playerInventory.Inventory, this, playerInventory );
	}

	public void ScrapItem(InvetoryItem item,Stats plyStats)
	{
		// Assume that player is correctly set to the player's GameObject
		var playerStats = plyStats;
		if (playerStats != null)
		{
			if(item.Item.ItemTier == ItemTier.Uncommon)
			{
				//Only scrap up to 10 items at a time
				var amount = item.Amount > 10 ? 10 : item.Amount;
				for(int i = 0; i < amount; i++)
				{
					SpawnItem(new UncommonScrap());
					playerStats.Inventory.RemoveItem( item.Item );
				}
			}

			// Remove the item from the player's inventory


			// Optional: Apply any effects or consequences of scrapping the item
			Log.Info($"{item.Item.Name} has been scrapped.");
		}
	}

	public void OpenChest(GameObject player )
	{

	}

	void SpawnItem(ItemDef item)
	{
		//var randomItem = SceneUtility.GetPrefabScene( Items[Random.Shared.Int( 0, Items.Count - 1 )] );
		var prefab = SceneUtility.GetPrefabScene( ResourceLibrary.Get<PrefabFile>( "prefab/items/baseitem.prefab" ) );
		var go = prefab.Clone();
		var itemGet = item;
		go.BreakFromPrefab();
		go.Name = itemGet.Name;
		go.Components.Get<ModelRenderer>(FindMode.InChildren).Model = itemGet.Model;
		go.Components.Get<ModelRenderer>( FindMode.InChildren ).Tint = itemGet.ItemColor;
		go.Components.Get<ItemHelper>( FindMode.InChildren ).Item = itemGet;
		var interactable = go.Components.Get<Interactable>( );
		interactable.Name = itemGet.Name;
		//var item = RandomItem.Clone();
		if ( ItemSpawnLocation != null )
			go.Transform.Position = ItemSpawnLocation.Transform.Position;

		var rb = go.Components.Get<Rigidbody>(FindMode.EnabledInSelf);

		if ( rb != null )
		{
			//This seems dumb
			rb.ApplyForce( Vector3.Up * 1000f * 20f );
			rb.ApplyForce( Transform.Rotation * Vector3.Forward * 1000f * 10f);
		}
	}
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		//Log.Info( "OnTriggerEnter" );

	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		//Log.Info( "OnTriggerExit" );
		
		if ( other.GameObject.Tags.Has( "player" ) )
		{
			if ( _UI != null )
				_UI.Destroy();
		}	
	}
}


