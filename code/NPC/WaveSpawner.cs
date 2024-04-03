using Sandbox;
using System.Collections.Generic;
using Sandbox.Navigation;
using System;

[EditorHandle( "materials/editor/lootspawn.png" )]
public sealed class WaveSpawner : Component
{
	[Property] public GameObject EntityPrefab { get; set; }
	[Property] public int SpawnPerWave { get; set; } = 10; // Entities to spawn per wave
	[Property] public int MaxEntities { get; set; } = 50; // Maximum entities allowed at once
	[Property] public int WaveInterval { get; set; } = 5000; // Interval between waves in milliseconds

	[Property] public int spawnedEntities;
	private TimeSince timeSinceLastWave = 0;

	protected override void OnStart()
	{
		base.OnStart();
		SpawnWave(); // Start by spawning the first wave
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		// Check if it's time for a new wave and if we haven't exceeded the maximum entities
		if ( timeSinceLastWave > WaveInterval / 1000f && spawnedEntities < MaxEntities )
		{
			SpawnWave();
		}

		// Optionally, clean up any null entries in our list if entities are destroyed elsewhere
		//spawnedEntities.RemoveAll( entity => entity == null );
	}

	private void SpawnWave()
	{
		Log.Info( "Spawning wave" );
		int entitiesToSpawn = (int)MathF.Min( SpawnPerWave, MaxEntities - spawnedEntities );
		for ( int i = 0; i < entitiesToSpawn; i++ )
		{
			SpawnEntity();
		}
		timeSinceLastWave = 0; // Reset the timer for the next wave
	}

	private void SpawnEntity()
	{
		if ( EntityPrefab == null ) return;
		var entity = EntityPrefab.Clone();
		var spawnPos = Scene.NavMesh.GetRandomPoint( Transform.Position, 200 );
		entity.Components.Get<Npcbase>().WaveSpawner = this;

		if ( spawnPos.HasValue )
		{
			entity.Transform.Position = spawnPos.Value;
			entity.Transform.Rotation = Rotation.FromYaw( Random.Shared.Float( -360, 360 ) );
		}

		spawnedEntities++;
	}
}
