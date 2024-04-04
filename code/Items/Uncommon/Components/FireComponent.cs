using Sandbox;

public sealed class FireComponent : Component
{
	public int Length { get; set; } = 1;
	public TimeSince timeSinceSpawned { get; set; } = 0;
	public PrefabScene fireParticle { get; set; } = SceneUtility.GetPrefabScene( ResourceLibrary.Get<PrefabFile>( "prefab/particles/items/uncommon/fireeffect.prefab" ) );
	GameObject fire;
	bool spawned = false;
	protected override void OnStart()
	{
		base.OnStart();
		timeSinceSpawned = 0;
	}

	public void Hit()
	{
		timeSinceSpawned = 0;
	}

	protected override void OnUpdate()
	{
		var npc = GameObject.Components.Get<Npcbase>();

		if( npc.Health == 0 && fire != null)
		{
			fire.Destroy();
		}

		if ( timeSinceSpawned < Length )
		{
			if ( npc.Health > 0 )
			{
				npc.OnDamage( 0.1f );
				if(!spawned)
				{
					spawned = true;
					fire = fireParticle.Clone();
					fire.Transform.Position = GameObject.Transform.Position + Vector3.Up * 50;
					fire.Transform.Rotation = Rotation.From( new Angles( 0, 0, 0 ) );
				}
			}
		}
		else
		{
			Destroy();
			fire.Destroy();
		}

		if(fire != null )
		{
			fire.Transform.Position = GameObject.Transform.Position + Vector3.Up * 50;
		}
	}
}
