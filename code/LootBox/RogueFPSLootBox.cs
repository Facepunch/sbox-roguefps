using Sandbox;
using System;

public sealed class RogueFPSLootBox : Component
{
	[Property] public GameObject LootBoxObject { get; set; }
	[Property] public GameObject LootBoxObject1 { get; set; }
	[Property] public GameObject LootBoxObject2 { get; set; }
	[Property] public GameObject LootBoxObject3 { get; set; }

	[Property] public GameObject Coin { get; set; }
	[Property] public int amountOfCoins { get; set; } = 10;

	[Property] public GameObject Player { get; set; }

	private RogueFPSHealthComp healthComp;

	private bool itemCreated = false;

	protected override void OnStart()
	{
		base.OnStart();

		healthComp = GameObject.Components.Get<RogueFPSHealthComp>();
	}

	protected override void OnUpdate()
	{
		var body = GameObject.Components.Get<Rigidbody>();
		body.Velocity = 0;
		body.AngularVelocity = 0;
		/*
		if ( healthComp.Health <= 0f )
		{
			Log.Info( "Loot box destroyed." );
			if ( !itemCreated )
			{
				CreateItem();
				itemCreated = true;
			}
		}
		*/
	}


	public void CreateItem()
	{
		Random rand = new Random();
		GameObject[] lootObjects = new GameObject[] { LootBoxObject, LootBoxObject1, LootBoxObject2, LootBoxObject3 };
	
		// Pick a random loot object
		GameObject randomLootObject = lootObjects[rand.Next( lootObjects.Length )];

		// Instantiate the selected loot object
		var item = SceneUtility.Instantiate( randomLootObject, GameObject.Transform.Position, GameObject.Transform.Rotation );
		var body = item.Components.Get<Rigidbody>();

		// Generate a random direction
		//Random rand = new Random();
		float x = (float)(rand.NextDouble() * 2 - 1); // Random value between -1 and 1
		float y = (float)(rand.NextDouble() * 2 - 1); // Random value between -1 and 1
		float z = (float)(rand.NextDouble());         // Random value between 0 and 1, to ensure upward movement

		Vector3 randomDirection = new Vector3( x, y, z * 10 ).Normal;

		// Apply the random velocity
		float launchSpeed = 700; // You can adjust this value as needed
		body.Velocity = randomDirection * launchSpeed;

		item.Transform.Position = GameObject.Transform.Position;

		healthComp.Health = 0;
		/*
		for ( int i = 0; i < amountOfCoins; i++ )
		{
			// CreateCoins
			var coin = SceneUtility.Instantiate( Coin, GameObject.Transform.Position );
			var coinTarget = coin.GetComponent<RogueFPSCoin>();
			coinTarget.TargetPlayer = Player;
			var coinbody = coin.GetComponent<PhysicsComponent>();

			// Generate a random direction
			//Random rand = new Random();
			float x1 = (float)(rand.NextDouble() * 2 - 1); // Random value between -1 and 1
			float y1 = (float)(rand.NextDouble() * 2 - 1); // Random value between -1 and 1
			float z1 = (float)(rand.NextDouble());         // Random value between 0 and 1, to ensure upward movement

			Vector3 randomDirection1 = new Vector3( 0, 0, z1 * 10 ).Normal;

			// Apply the random velocity
			float launchSpeed1 = 10; // You can adjust this value as needed
			coinbody.Velocity = randomDirection1 * launchSpeed1;

			coin.Transform.Position = GameObject.Transform.Position;
		}
		*/
	}
}
