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

	protected override void OnStart()
	{
		base.OnStart();

		//Load the items from a folder.
		Items = ResourceLibrary.GetAll<PrefabFile>().Where( x => x.ResourcePath.StartsWith( GetPath( Rarity ) ) ).ToList();

		Log.Info( $"Items: {Items.Count}" );
	}
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		Log.Info( "OnTriggerEnter" );

		if ( other.GameObject.Tags.Has( "player" ) )
		{
			var randomItem = SceneUtility.GetPrefabScene( Items[Random.Shared.Int( 0, Items.Count - 1 )] );

			var item = randomItem.Clone();
			if( ItemSpawnLocation != null)
				item.Transform.Position = ItemSpawnLocation.Transform.Position;

			var rb = item.Components.Get<Rigidbody>( FindMode.EnabledInSelfAndChildren );
			if ( rb != null )
			{
				rb.ApplyForce( Vector3.Up * 5000000000f );
				rb.ApplyForce( Vector3.Left * Random.Shared.Float( -500000000f, 500000000f ) );
				rb.ApplyForce( Vector3.Forward * Random.Shared.Float( -500000000f, 500000000f ) );
			}

		}
	}
	void ITriggerListener.OnTriggerExit( Collider other )
	{
		Log.Info( "OnTriggerExit" );
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


