using Sandbox;

namespace RogueFPS;

[Title( "Rogue FPS Coin" )]
[Category( "Rogue FPS" )]
[Icon( "savings", "red", "white" )]
public sealed class RogueFPSCoin : Component, Component.ITriggerListener
{
	[Property] public int CoinAmount { get; set; } = 10;
	[Property] public GameObject TargetPlayer { get; set; }

	private float moveSpeed = 5000f; // Adjust the speed as needed

	private float timeSinceSpawned = 0f;

	protected override void OnUpdate()
	{
		base.OnUpdate();

		timeSinceSpawned += Time.Delta;

		if ( timeSinceSpawned > 0.40f && timeSinceSpawned < 0.45f )
		{
			/*var physbody = GameObject.Components.Get<Rigidbody>();
			physbody.Velocity = 0;
			physbody.AngularVelocity = 0;
			*/
			Log.Info( "Coin spawned." );
		}

		// Move the coin towards the target player
		if ( TargetPlayer != null && timeSinceSpawned > 0.50f )
		{
			Log.Info( "Coin moving towards player." );

			Vector3 direction = (TargetPlayer.Transform.Position - GameObject.Transform.Position).Normal;
			GameObject.Transform.Position += direction * moveSpeed * Time.Delta;
			GameObject.Transform.Rotation = Rotation.LookAt( direction );
		}
	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( other.GameObject.Tags.Has( "player" ) )
		{
			var Player = other.GameObject.Parent;
			var plyStatComp = Player.Components.Get<PlayerStats>();
			if ( plyStatComp != null )
			{
				plyStatComp.AddCoin( CoinAmount );
				GameObject.Destroy();
			}
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		//Log.Info( "OnTriggerExit" );

	}
}
