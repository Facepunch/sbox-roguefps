using Sandbox;

public sealed class ItemTerminal : Interactable, Component.ITriggerListener
{
    List<PrefabFile> Items;
    [Property] public GameObject ItemSpawnLocation { get; set; }
    [Property] ItemRarity Rarity { get; set; } = ItemRarity.Common;
    [Property] public bool UseRandomItem { get; set; } = true;
    [Property] public GameObject Door { get; set; }
    [Property] public PrefabScene WorldUI { get; set; }
    [Property] public GameObject WorldUILocation { get; set; }
    [Property] public GameObject Screen { get; set; }
	[Property] public GameObject Light { get; set; }

	RealTimeSince refreshTimer;
	float refreshDuration = 2.0f;

    GameObject _UI;
    WorldCostPanel _Panel;
    PrefabScene RandomItem { get; set; }
    int ItemSpawned { get; set; } = 0;
    float ItemChance { get; set; } = 0.4f; // 50% chance to get an item
    public override string Name { get; set; } = "Open Terminal";
	public override string PingString { get; set; } = "Terminal";
	protected override void OnStart()
	{
		base.OnStart();

		//Load the items from a folder.
		Items = ResourceLibrary.GetAll<PrefabFile>().Where( x => x.ResourcePath.StartsWith( GetPath( Rarity ) ) ).ToList();

		if ( UseRandomItem )
		{
			RandomItem = SceneUtility.GetPrefabScene( Items[Random.Shared.Int( 0, Items.Count - 1 )] );
		}

		Cost = (int)(15 * Scene.GetAllComponents<MasterGameManager>().FirstOrDefault().Current.TotalFactor * 1.25f);

		PingString = $"Terminal ({Cost} Coins)";
	}

	TimeSince timeSinceItem = 0;
	float doorScale = 0.0f;
	Color ScreenColor = Color.White;
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( Door != null )
		{
			doorScale = doorScale.LerpTo( IsOpen ? 1.0f : 0.0f, Time.Delta * 5.0f );
			Door.Transform.Scale = Door.Transform.Scale.WithZ( doorScale );
		}

		var screenTint = Screen.Components.Get<ModelRenderer>();

		if ( refreshTimer <= refreshDuration || IsOpen )
		{
			screenTint.Tint = Color.Black;
		}
		else
		{
			screenTint.Tint = Color.Lerp( ScreenColor, Color.White, timeSinceItem );
		}

		if(Light != null)
		{
			var light = Light.Components.Get<ModelRenderer>();
			light.Tint = Color.Lerp( ScreenColor, Color.White, timeSinceItem / 5 );
		}

		if ( IsOpen )
		{
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
		
		if( refreshTimer < refreshDuration ) return;

		refreshTimer = 0;

		if ( stats != null && stats.PlayerCoinsAndXp[Stats.CoinsAndXp.Coins] >= Cost )
		{
			stats.AddCoin( -Cost );
			Cost = (int)(Cost * 1.25f);
			PingString = $"Terminal ({Cost} Coins)";
			if ( _Panel != null )
			{
				_Panel.Value = Cost;
			}
			if ( Random.Shared.Float() < ItemChance ) // Check if an item should be given
			{
				SpawnItem();
				ItemSpawned++;

				if ( ItemSpawned == 2 )
				{
					IsOpen = true;
				}
				timeSinceItem = 0;
				ScreenColor = Color.Green;
			}
			else
			{
				timeSinceItem = 0;
				ScreenColor = Color.Red;
			}
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
	
		if ( other.GameObject.Tags.Has( "player" ) )
		{
			_UI = WorldUI.Clone();
			_UI.Transform.Position = WorldUILocation.Transform.Position;
			_UI.Transform.Rotation = WorldUILocation.Transform.Rotation;
			_UI.Transform.Scale = 0.5f;
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


