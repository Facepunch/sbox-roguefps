using Sandbox;

public sealed class MultiShopItem : Interactable
{

	[Property] GameObject ItemSpawnLocation { get; set; }
	[Property] GameObject Door { get; set; }
	[Property] public GameObject ItemLocationOne { get; set; }

	ItemDef ItemDef { get; set; }

	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		if ( ItemLocationOne != null )
		{
			Gizmo.Draw.SolidSphere( ItemLocationOne.Transform.LocalPosition, 5.0f );
		}
	}

	protected override void OnStart()
	{
		base.OnStart();

		// Ensure there are at least 3 items to choose from
		if ( ItemsAndContent.Items.Count >= 3 )
		{
			// Randomly select 3 distinct items
			var randomItems = ItemsAndContent.Items.OrderBy( x => Random.Shared.Next() ).Take( 3 ).ToList();

			SetUpItemShop( randomItems[0], ItemLocationOne );
		}
	}

	public void SetUpItemShop( ItemDef item, GameObject itemLocation )
	{
		ItemDef = item;

		var itemRenderer = itemLocation.Components.Get<ModelRenderer>();
		itemRenderer.Model = item.Model;
		itemRenderer.Tint = item.ItemColor;

		var interactable = GameObject.Components.Get<Interactable>();
		if ( interactable != null )
		{
			interactable.Cost = Components.Get<MultShopManager>(FindMode.EverythingInSelfAndParent).Cost;
		}
	}

	public override void OnInteract( GameObject player )
	{
		OpenItem(player);
	}
	float doorScale = 0.0f;
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( Door != null )
		{
			doorScale = doorScale.LerpTo( IsOpen ? 1.0f : 0.0f, Time.Delta * 5.0f );
			Door.Transform.Scale = Door.Transform.Scale.WithZ( doorScale );
		}
	}

	public void OpenItem( GameObject player )
	{
		//Check if the player has enough coins
		var stats = player.Components.Get<Stats>();

		if ( stats != null && stats.PlayerCoinsAndXp[Stats.CoinsAndXp.Coins] >= Cost )
		{
			stats.AddCoin( -Cost );
			OnPurchase();

			IsOpen = true;
			ItemLocationOne.Enabled = false;

			var manager = Components.Get<MultShopManager>( FindMode.EverythingInSelfAndParent );
			manager.ItemPicked();
		}
		else
		{
			Log.Info( stats.PlayerCoinsAndXp[Stats.CoinsAndXp.Coins] );
		}
	}
	public void OnPurchase()
	{
		SpawnItem();
	}

	public void CloseItem()
	{
		IsOpen = true;
		ItemLocationOne.Enabled = false;
	}

	void SpawnItem()
	{
		//var randomItem = SceneUtility.GetPrefabScene( Items[Random.Shared.Int( 0, Items.Count - 1 )] );
		var prefab = SceneUtility.GetPrefabScene( ResourceLibrary.Get<PrefabFile>( "prefab/items/baseitem.prefab" ) );
		var go = prefab.Clone();
		var itemGet = ItemDef;
		go.BreakFromPrefab();
		go.Name = itemGet.Name;
		go.Components.Get<ModelRenderer>( FindMode.InChildren ).Model = itemGet.Model;
		go.Components.Get<ModelRenderer>( FindMode.InChildren ).Tint = itemGet.ItemColor;
		go.Components.Get<ItemHelper>( FindMode.InChildren ).Item = itemGet;
		var interactable = go.Components.Get<Interactable>();
		interactable.Name = itemGet.Name;
		//var item = RandomItem.Clone();
		if ( ItemSpawnLocation != null )
			go.Transform.Position = ItemSpawnLocation.Transform.Position;

		var rb = go.Components.Get<Rigidbody>( FindMode.EnabledInSelf );

		if ( rb != null )
		{
			//This seems dumb
			rb.ApplyForce( Vector3.Up * 1000f * 20f );
			rb.ApplyForce( Transform.Rotation * Vector3.Forward * 1000f * 10f );
		}
	}
}
