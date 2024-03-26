using RogueFPS;

public class TestItem : BaseItem
{
	public override string ItemName {get; set; } = "Speed Boost";
	public override bool AsAPercent { get; set; } = true;
	public override PlayerStats.PlayerStartingStats UpgradeType { get; set; } = PlayerStats.PlayerStartingStats.AttackSpeed;
	public override float UpgradeAmount { get; set; } = -0.1f;
	public override string UpgradeIcon { get; set; } = "ui/test/items/hyper.png";
	public override string ItemDescription { get; set; } = "Increases attack speed by 10%";
	public override UpgradeRarity Rarity { get; set; } = UpgradeRarity.Common;
	private GameObject BulletTrace { get; set; } = SceneUtility.GetPrefabScene( ResourceLibrary.Get<PrefabFile>( "prefab/weapon/fx/bullettracertest.prefab" ) );
	//public override bool IsStatUpgrade { get; set; } = false;

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
