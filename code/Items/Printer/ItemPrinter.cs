using Sandbox;
using Sandbox.UI;

public sealed class ItemPrinter : Interactable
{
	public override string Name { get; set; } = "Item Printer";
	[Property] GameObject ItemSpawnLocation { get; set; }
	[Property] GameObject ItemLocation { get; set; }
	[Property] PrefabScene WorldUI { get; set; }
	[Property] GameObject PriceLocation { get; set; }
	GameObject costPanel;
	ItemDef ItemDef { get; set; }

	ItemTier ItemTier { get; set; } = ItemTier.Uncommon;

	protected override void OnStart()
	{
		base.OnStart();

		//Get a random item from the list that are uncommon.
		ItemDef = ItemsAndContent.Items.Where( x => x.ItemTier == ItemTier ).OrderBy( x => Random.Shared.Next() ).FirstOrDefault();

		SetUpItemPrinter( ItemDef, ItemLocation );

		costPanel = WorldUI.Clone();
		if ( PriceLocation != null )
		costPanel.Transform.Position = PriceLocation.Transform.Position;
		costPanel.Transform.Rotation = PriceLocation.Transform.Rotation;
		costPanel.Components.Get<WorldCostPanel>().IsText = true;
		costPanel.Components.Get<WorldCostPanel>().Text = "1 Item";
	}

	public void SetUpItemPrinter( ItemDef item, GameObject itemLocation )
	{
		ItemDef = item;

		var itemRenderer = itemLocation.Components.Get<ModelRenderer>();
		itemRenderer.Model = item.Model;
		itemRenderer.Tint = item.ItemColor;
	}

	public override void OnInteract( GameObject player )
	{
		base.OnInteract( player );

		var playerStats = player.Components.Get<Stats>();
		if ( playerStats == null ) return;

		var inventory = playerStats.Inventory;
		// Prioritize scrap items, else take a random item of matching tier
		var itemToUse = inventory.itemPickUps
			.Where( x => x.Item.ItemTier == ItemTier && x.Amount > 0 )
			.OrderByDescending( x => x.Item.IsScrap )
			.ThenBy( x => Random.Shared.Next() )
			.FirstOrDefault();

		if ( itemToUse.Item != null )
		{
			inventory.RemoveItem( itemToUse.Item );
			SpawnItem();
		}
		else
		{
			// Notify the player they don't have a required item (this might be UI feedback or log message)
			Log.Info( "You don't have a required item." );
		}
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
		interactable.PingString = itemGet.Name;
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
