using Sandbox;

public sealed class CoinChest : Interactable, Component.ITriggerListener
{
	List<PrefabFile> Items;
	[Property] [Group( "Item Chest" )] GameObject ItemSpawnLocation { get; set; }
	[Property] [Group( "Item Chest" )] ItemRarity Rarity { get; set; } = ItemRarity.Common;
	[Property] bool UseRandomItem { get; set; } = true;
	[Property] GameObject Top { get; set; }
	[Property] bool IsFree { get; set; } = false;
	[Property] GameObject Coin { get; set; }
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

		PingString = $"Coin Chest";
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( IsOpen )
		{
			Top.Transform.Rotation = Rotation.Slerp( Top.Transform.Rotation, Rotation.From( new Angles( -120, 0, 0 ) + Transform.Rotation.Angles() ), Time.Delta * 5f );
			return;
		}
	}

	public override void OnInteract(GameObject player)
	{
		Player = player;

		OpenChest(player);
	}

	public void OpenChest(GameObject player )
	{
		if ( IsFree)
		{
			SpawnItem();
			IsOpen = true;
		}
	}

	void SpawnItem()
	{
		var go = Coin.Clone();
		go.BreakFromPrefab();
		var interactable = go.Components.Get<CoinItem>();
		interactable.TargetPlayer = Player;
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

	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{

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


