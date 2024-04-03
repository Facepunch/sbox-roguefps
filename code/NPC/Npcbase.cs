using Sandbox;
using System.Numerics;

public sealed class Npcbase : Component, Component.ITriggerListener
{
	public GameObject Target { get; set; }
	[Property] public NavMeshAgent Agent { get; set; }

	[Property] public PlayerStats Stats { get; set; }
	[Property] GameObject Xp { get; set; }
	[Property] GameObject Coin { get; set; }
	float Health { get; set; } = 100f;
	public WaveSpawner WaveSpawner { get; set; }
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
		var txt = new TextObject(Scene.SceneWorld);
		txt.Transform = Transform.World.WithScale(Transform.Scale * 0.2f * Target.Transform.Position.Distance( Transform.Position ) / 200f);
		txt.Position = Transform.Position + new Vector3( Random.Shared.Float( -20f, 20f ), Random.Shared.Float( -20f, 20f ), Random.Shared.Float(10,50f));
		txt.Rotation = Rotation.LookAt( Target.Transform.Position - Transform.Position ).Angles().WithPitch(0);
		txt.Text = damage.ToString();
		txt.ColorTint = Color.Random;

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

		Log.Info( "Health: " + Health );
	}
}

file class TextObject : SceneCustomObject
{
	public float FontSize { get; set; } = 64;
	public string FontFamily { get; set; } = "Quantico";
	public string Text { get; set; } = "ELLO!";
	public TextObject(SceneWorld world ) : base(world)
	{
		RenderLayer = SceneRenderLayer.Default;
	}
	public override void RenderSceneObject()
	{
		if ( string.IsNullOrWhiteSpace( Text ) )
			return;

		var textFlags = TextFlag.DontClip | TextFlag.Center;

		Graphics.Attributes.SetCombo( "D_WORLDPANEL", 1 );

		// Set a dummy WorldMat matrix so that ScenePanelObject doesn't break the transforms.
		Matrix mat = Matrix.CreateRotation( Rotation.From( 0, 90, 90 ) );
		Graphics.Attributes.Set( "WorldMat", mat );

		Graphics.DrawText( new Rect( 0 ), Text, ColorTint, FontFamily, FontSize, 800.0f, textFlags );
		//Make the text smaller over time
		Transform = Transform.WithScale(MathF.Sign( Transform.Scale.x ) * (Transform.Scale.x - RealTime.Delta * 0.75f));
		if( Transform.Scale.x <= 0 )
		{
			Delete();
		}
	}
}
