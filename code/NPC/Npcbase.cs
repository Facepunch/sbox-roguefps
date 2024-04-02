using Sandbox;
using System.Numerics;

public sealed class Npcbase : Component, Component.ITriggerListener
{
	GameObject Target { get; set; }
	[Property] public NavMeshAgent Agent { get; set; }

	[Property] public PlayerStats Stats { get; set; }
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
			GameObject.Destroy();
		}

		Log.Info( "Health: " + Health );
	}
}
