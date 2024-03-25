using RogueFPS;

public class TestItem : PlayerUpgrade
{
	private GameObject BulletTrace { get; set; } = SceneUtility.GetPrefabScene( ResourceLibrary.Get<PrefabFile>( "prefab/weapon/fx/bullettracertest.prefab" ) );
	public override bool IsStatUpgrade { get; set; } = false;

	public override float ChanceIncrementPerUpgrade { get; set; } = 15.0f;

	public override void DoAttackUpgrade( SceneTraceResult trace )
	{
		if ( ShouldEffectTrigger(Amount) )
		{
			var tracer = BulletTrace.Clone();
			var tracerParticle = tracer.Components.Get<TracerBulletParticle>();
			tracerParticle.Start.Transform.Position = trace.StartPosition + Scene.Camera.Transform.Rotation.Forward * 10f;
			tracerParticle.End.Transform.Position = trace.EndPosition;
		}
	}
}
