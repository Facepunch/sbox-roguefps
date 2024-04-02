using Sandbox;
using System.Numerics;

public sealed class Npcbase : Component, Component.ITriggerListener
{
	GameObject Target { get; set; }
	[Property] public NavMeshAgent Agent { get; set; }

	[Property] public PlayerStats Stats { get; set; }
	[Property] GameObject Xp { get; set; }
	[Property] GameObject Coin { get; set; }
	float Health { get; set; } = 100f;

	protected override void OnAwake()
	{
		Health = Stats.Health;
		Agent.MaxSpeed = Stats.UpgradedStats[PlayerStats.PlayerUpgradedStats.WalkSpeed];
		Agent.Acceleration = Stats.UpgradedStats[PlayerStats.PlayerUpgradedStats.WalkSpeed] * 2;
	}
	
	protected override void OnUpdate()
	{
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

	public void OnDamage(float damage)
	{
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

			GameObject.Destroy();
		}

		Log.Info( "Health: " + Health );
	}
}
