using Sandbox;

public sealed class ItemChest : Component, Component.ITriggerListener
{
	List<PrefabFile> Items;

	[Property]
	[Group( "Item Chest" )]
	GameObject ItemSpawnLocation { get; set; }

	[Property]
	[Group( "Item Chest" )]
	ItemRarity Rarity { get; set; } = ItemRarity.Common;

	[Property]
	bool UseRandomItem { get; set; } = true;

	[Property]
	[Group( "Item Chest" )]
	int Cost { get; set; } = 100;
	PrefabScene RandomItem { get; set; }

	GameObject PlayerInside { get; set; }

	protected override void OnStart()
	{
		base.OnStart();

		//Load the items from a folder.
		Items = ResourceLibrary.GetAll<PrefabFile>().Where( x => x.ResourcePath.StartsWith( GetPath( Rarity ) ) ).ToList();

		Log.Info( $"Items: {Items.Count}" );

		if(UseRandomItem)
		{
			RandomItem = SceneUtility.GetPrefabScene( Items[Random.Shared.Int( 0, Items.Count - 1 )] );
		}
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( PlayerInside != null && Input.Pressed( "use" ) )
		{
			//Check if the player has enough coins
			var stats = PlayerInside.Parent.Components.Get<PlayerStats>();

			if ( stats != null && stats.PlayerCoinsAndXp[PlayerStats.CoinsAndXp.Coins] >= Cost )
			{
				stats.AddCoin( -Cost );
				SpawnItem();
			}else
			{
				Log.Info( stats.PlayerCoinsAndXp[PlayerStats.CoinsAndXp.Coins] );
			}

			//SpawnItem();
		}
	}

	void SpawnItem()
	{
		//var randomItem = SceneUtility.GetPrefabScene( Items[Random.Shared.Int( 0, Items.Count - 1 )] );

		var item = RandomItem.Clone();
		if ( ItemSpawnLocation != null )
			item.Transform.Position = ItemSpawnLocation.Transform.Position;

		var rb = item.Components.Get<Rigidbody>( FindMode.EnabledInSelfAndChildren );
		if ( rb != null )
		{
			rb.ApplyForce( Vector3.Up * 5000000000f );
			rb.ApplyForce( Vector3.Left * Random.Shared.Float( -500000000f, 500000000f ) );
			rb.ApplyForce( Vector3.Forward * Random.Shared.Float( -500000000f, 500000000f ) );
		}
	}
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		Log.Info( "OnTriggerEnter" );

		if ( other.GameObject.Tags.Has( "player" ) )
		{
			PlayerInside = other.GameObject;

			var parent = other.GameObject.Parent;
			var ui = parent.Components.Get<ScreenPanel>( FindMode.EnabledInSelfAndDescendants );
			var itemUI = ui.Components.Get<ItemsUI>( FindMode.EnabledInSelfAndDescendants );

			var pickupui = new ItemPickUp( RandomItem.Components.Get<BaseItem>(FindMode.EnabledInSelfAndChildren), Cost );

			itemUI.Panel.AddChild( pickupui );
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		Log.Info( "OnTriggerExit" );

		if ( other.GameObject.Tags.Has( "player" ) )
		{
			PlayerInside = null;

			var parent = other.GameObject.Parent;
			var ui = parent.Components.Get<ScreenPanel>( FindMode.EnabledInSelfAndDescendants );
			var itemUI = ui.Components.Get<ItemsUI>( FindMode.EnabledInSelfAndDescendants );

			itemUI.Panel.DeleteChildren();
		}
	}

	string GetPath(ItemRarity rarity)
	{
		switch ( rarity )
		{
			case ItemRarity.Common:
				return "prefab/items/common/";
			case ItemRarity.Uncommon:
				return "prefab/items/uncommon/";
			case ItemRarity.Rare:
				return "prefab/items/rare/";
			case ItemRarity.Epic:
				return "prefab/items/epic/";
			case ItemRarity.Legendary:
				return "prefab/items/legendary/";
			default:
				return "prefab/items/common/";
		}
	}

	enum ItemRarity
	{
		Common,
		Uncommon,
		Rare,
		Epic,
		Legendary
	}
}


