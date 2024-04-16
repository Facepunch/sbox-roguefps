public sealed class Npcbase : Agent, Component.ITriggerListener
{
	public GameObject Target { get; set; }

	[Property] GameObject Xp { get; set; }
	[Property] GameObject Coin { get; set; }
	public WaveSpawner WaveSpawner { get; set; }

	public TimeSince timeSinceLastDamaged { get; set; }

	public DamageTypes DamageType { get; set; }

	private const float DamageCooldownPeriod = 1.0f; // Time in seconds to consider the player has stopped being damaged
	public bool HasStoppedBeingDamaged => timeSinceLastDamaged > DamageCooldownPeriod;

	protected override void OnAwake()
	{
		Health = Stats.Health;
		BaseMovementSpeed = Stats.WalkSpeed;
	}
	
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( HasStoppedBeingDamaged )
		{
			DamageType = DamageTypes.None;
		}
	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( other.GameObject.Tags.Has( "player" ) )
		{
			Target = other.GameObject.Parent;
		}
	}

	public void OnDamage(float damage, DamageTypes dmgType, GameObject dmgDealer)
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
					xp.Components.Get<XpItem>().TargetPlayer = dmgDealer;

					var coin = Coin.Clone();
					coin.Transform.Position = Transform.Position + new Vector3(0, 0, 50f) + Vector3.Random * 10;
					coin.Components.Get<CoinItem>().TargetPlayer = dmgDealer;
				}
			}
			if ( WaveSpawner is not null ) WaveSpawner.spawnedEntities--;
			GameObject.Destroy();
		}

		//Log.Info( "Health: " + Health );
	}
}
