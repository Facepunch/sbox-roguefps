using Sandbox;

public sealed class EquipmentChest : Interactable, Component.ITriggerListener
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

	public override string Name { get; set; } = "Open Chest";
	protected override void OnStart()
	{
		base.OnStart();

		if ( UseRandomItem )
		{
			RandomItem = SceneUtility.GetPrefabScene( Items[Random.Shared.Int( 0, Items.Count - 1 )] );
		}

		Cost = (int)(25 * Scene.GetAllComponents<MasterGameManager>().FirstOrDefault().Current.TotalFactor * 1.25f);
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

		if ( stats != null && stats.PlayerCoinsAndXp[Stats.CoinsAndXp.Coins] >= Cost )
		{
			stats.AddCoin( -Cost );
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
		var itemGet = new BaseEquipmentBase();
		go.BreakFromPrefab();
		go.Name = itemGet.Name;
		go.Components.Get<ModelRenderer>(FindMode.InChildren).Model = itemGet.Model;
		go.Components.Get<ModelRenderer>( FindMode.InChildren ).Tint = itemGet.ItemColor;
		if ( itemGet.IsEquipment )
		{
			go.Components.Get<ItemHelper>( FindMode.InChildren ).Equipment = itemGet;
		}
		else
		{
			go.Components.Get<ItemHelper>( FindMode.InChildren ).Item = itemGet;
		}
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

		if ( IsOpen ) return;
	
		if ( other.GameObject.Tags.Has( "player" ) )
		{
			_UI = WorldUI.Clone();
			_UI.Transform.Position = Transform.Position + Vector3.Up * 55;
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
}


