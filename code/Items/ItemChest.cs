using Sandbox;

public sealed class ItemChest : Interactable, Component.ITriggerListener
{
	List<PrefabFile> Items;
	[Property] [Group( "Item Chest" )] GameObject ItemSpawnLocation { get; set; }
	[Property] [Group( "Item Chest" )] ItemRarity Rarity { get; set; } = ItemRarity.Common;
	[Property] bool UseRandomItem { get; set; } = true;
	[Property] GameObject Top { get; set; }
	[Property] PrefabScene WorldUI { get; set; }
	[Property] bool IsFree { get; set; } = false;
	GameObject _UI;
	WorldCostPanel _Panel;
	PrefabScene RandomItem { get; set; }
	public override string Name { get; set; } = "Open Chest";

	protected override void OnStart()
	{
		base.OnStart();

		//Load the items from a folder.
		Items = ResourceLibrary.GetAll<PrefabFile>().Where( x => x.ResourcePath.StartsWith( GetPath( Rarity ) ) ).ToList();

		if ( UseRandomItem )
		{
			RandomItem = SceneUtility.GetPrefabScene( Items[Random.Shared.Int( 0, Items.Count - 1 )] );
		}

		Cost = (int)(25 * Scene.GetAllComponents<MasterGameManager>().FirstOrDefault().Current.TotalFactor * 1.25f);

		PingString = $"Chest ({Cost} Coins)";
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( IsOpen )
		{
			Top.Transform.Rotation = Rotation.Slerp( Top.Transform.Rotation, Rotation.From( new Angles( -120, 0, 0 ) + Transform.Rotation.Angles() ), Time.Delta * 5f );
			if ( _UI != null )
				_UI.Destroy();
			return;
		}
	}

	public override void OnInteract(GameObject player)
	{
		OpenChest(player);
	}

	public void OpenChest(GameObject player )
	{
		//Check if the player has enough coins
		var stats = player.Components.Get<Stats>();

		if ( stats != null && stats.PlayerCoinsAndXp[Stats.CoinsAndXp.Coins] >= Cost || IsFree)
		{
			if ( !IsFree )
			{
				stats.AddCoin( -Cost );
			}
			SpawnItem();

			IsOpen = true;
			//DestroyUI();
		}
		else
		{
			Log.Info( stats.PlayerCoinsAndXp[Stats.CoinsAndXp.Coins] );
		}
	}

	void SpawnItem()
	{
		//var randomItem = SceneUtility.GetPrefabScene( Items[Random.Shared.Int( 0, Items.Count - 1 )] );
		var prefab = SceneUtility.GetPrefabScene( ResourceLibrary.Get<PrefabFile>( "prefab/items/baseitem.prefab" ) );
		var go = prefab.Clone();
		var itemGet = ItemsAndContent.Items[Random.Shared.Int( 0, ItemsAndContent.Items.Count - 1 )];
		go.BreakFromPrefab();
		go.Name = itemGet.Name;
		go.Components.Get<ModelRenderer>(FindMode.InChildren).Model = itemGet.Model;
		go.Components.Get<ModelRenderer>( FindMode.InChildren ).Tint = itemGet.ItemColor;
		go.Components.Get<ItemHelper>( FindMode.InChildren ).Item = itemGet;
		var interactable = go.Components.Get<Interactable>( );
		interactable.PingString = itemGet.Name;
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

		if ( IsOpen ) return;

		if ( IsFree ) return;
	
		if ( other.GameObject.Tags.Has( "player" ) )
		{
			_UI = WorldUI.Clone();
			_UI.Transform.Position = Transform.Position + Vector3.Up * 35;
			_Panel = _UI.Components.Get<WorldCostPanel>();
			_Panel.Value = Cost;
		}
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

	string GetPath( ItemRarity rarity )
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


