using Sandbox;

public partial class BaseBulletProjectile : Component, Component.ITriggerListener
{
	[Property] public virtual float Speed { get; set; } = 1000f;

	TimeSince timeSinceSpawned = 0f;
	protected override void OnUpdate()
	{
		Transform.Position += Transform.Rotation.Forward * Speed * Time.Delta;

		if ( timeSinceSpawned > 10f )
		{
			GameObject.Destroy();
		}
	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( other.GameObject.Tags.Has( "player" ) )
		{
			var Player = other.GameObject.Parent;
			var plyStatComp = Player.Components.Get<Actor>();
			if ( plyStatComp != null )
			{
				var dmgInfo = new DamageInfo( 5, null, GameObject );
				plyStatComp.OnDamage( dmgInfo );
				GameObject.Destroy();
			}
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		//Log.Info( "OnTriggerExit" );

	}
}
