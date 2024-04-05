using Sandbox;
using System.Numerics;

public sealed class Npcbase : Component, Component.ITriggerListener
{
	public GameObject Target { get; set; }
	[Property] public NavMeshAgent Agent { get; set; }

	[Property] public PlayerStats Stats { get; set; }
	[Property] GameObject Xp { get; set; }
	[Property] GameObject Coin { get; set; }
	public float Health { get; set; } = 100f;
	public WaveSpawner WaveSpawner { get; set; }

	public TimeSince timeSinceLastDamaged { get; set; }

	public DamageTypes DamageType { get; set; }

	private const float DamageCooldownPeriod = 1.0f; // Time in seconds to consider the player has stopped being damaged
	public bool HasStoppedBeingDamaged => timeSinceLastDamaged > DamageCooldownPeriod;


	protected override void OnAwake()
	{
		Health = Stats.Health;
		Agent.MaxSpeed = Stats.UpgradedStats[PlayerStats.PlayerUpgradedStats.WalkSpeed];
		Agent.Acceleration = Stats.UpgradedStats[PlayerStats.PlayerUpgradedStats.WalkSpeed] * 2;
	}
	
	protected override void OnUpdate()
	{
		if ( HasStoppedBeingDamaged )
		{
			DamageType = DamageTypes.None;
		}

		if ( Target != null )
			if ( Target.Transform.Position.Distance( GameObject.Transform.Position ) >= 200f )
			{
				Agent.MoveTo( Target.Transform.Position );
			}
			else
			{
				Transform.Rotation = Rotation.LookAt( Target.Transform.Position - Transform.Position ).Angles().WithPitch(0);
			}

	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( other.GameObject.Tags.Has( "player" ) )
		{
			Target = other.GameObject.Parent;
		}
	}

	public void OnDamage(float damage, DamageTypes dmgType)
	{
		DamageNumbers.Add( (int)damage, Transform.Position + new Vector3( Random.Shared.Float( -20f, 20f ), Random.Shared.Float( -20f, 20f ), Random.Shared.Float( 10, 50f ) ) );

		timeSinceLastDamaged = 0;
		DamageType = dmgType;
		Health -= damage;
		if ( Health <= 0 )
		{
			if( Xp != null )
			{
				for( int i = 0; i < 2; i++ )
				{
					var xp = Xp.Clone();
					xp.Transform.Position = Transform.Position + new Vector3(0, 0, 50f) + Vector3.Random * 10;
					xp.Components.Get<XpItem>().TargetPlayer = Target;

					var coin = Coin.Clone();
					coin.Transform.Position = Transform.Position + new Vector3(0, 0, 50f) + Vector3.Random * 10;
					coin.Components.Get<CoinItem>().TargetPlayer = Target;
				}
			}
			WaveSpawner.spawnedEntities--;
			GameObject.Destroy();
		}

		//Log.Info( "Health: " + Health );
	}
}
