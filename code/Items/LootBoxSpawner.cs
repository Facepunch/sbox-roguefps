using Sandbox;
using Sandbox.Navigation;
[EditorHandle( "materials/editor/lootspawn.png" )]
public sealed class LootBoxSpawner : Component
{
	[Property] public GameObject LootBoxPrefab { get; set; }
	[Property] public GameObject MultiShopPrefab { get; set; }
	[Property] public int SpawnAmount { get; set; } = 10;

	[Property] int currentSpawned = 0;

	protected override void DrawGizmos()
	{
		base.DrawGizmos();
/*
		for ( int i = 0; i < SpawnAmount; i++ )
		{
			var pos = Scene.NavMesh.GetClosestPoint( Vector3.Random * 10000, 800000 );
			if (pos.HasValue)
			{
				Gizmo.Draw.SolidSphere( pos.Value, 10f);
			}
		}
*/
	}

	protected override void OnStart()
	{
		base.OnStart();

		_=SpawnBoxes();
	}

	protected override void OnUpdate()
	{
		if( currentSpawned == SpawnAmount ) return;


	}

	async Task SpawnBoxes()
	{
		for ( int i = 0; i < SpawnAmount; i++ )
		{
			await Task.Delay( 100 );
			if ( LootBoxPrefab is null ) return;
			var lootBox = LootBoxPrefab.Clone();
			var pos = Scene.NavMesh.GetRandomPoint( Transform.Position, 2000 );

			if ( pos.HasValue )
			{
				lootBox.Transform.Position = pos.Value;

				lootBox.Transform.Rotation = Rotation.FromPitch(Random.Shared.Float( -10, 10 ));
				lootBox.Transform.Rotation = Rotation.FromRoll(Random.Shared.Float( -10, 10 ));
				lootBox.Transform.Rotation = Rotation.FromYaw( Random.Shared.Float( -360, 360 ) );
				//lootBox.BreakFromPrefab();
			}
		}
		for ( int i = 0; i < SpawnAmount / 2; i++ )
		{
			await Task.Delay( 100 );
			if ( MultiShopPrefab is null ) return;
			var multiShop = MultiShopPrefab.Clone();
			var pos = Scene.NavMesh.GetRandomPoint( Transform.Position, 2000 );

			if ( pos.HasValue )
			{
				multiShop.Transform.Position = pos.Value;

				multiShop.Transform.Rotation = Rotation.FromPitch(Random.Shared.Float( -10, 10 ));
				multiShop.Transform.Rotation = Rotation.FromRoll(Random.Shared.Float( -10, 10 ));
				multiShop.Transform.Rotation = Rotation.FromYaw( Random.Shared.Float( -360, 360 ) );
				//multiShop.BreakFromPrefab();
			}
		}
	}
}
