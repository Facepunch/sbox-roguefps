using Sandbox;

public class TestWeapon : BaseWeaponItem
{
	[Property]
	public GameObject BulletTrace { get; set; }

	public override bool RandomSpread { get; set; } = true;
	public override float Spread { get; set; } = 15.0f;

	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	public override void DoReloadAnimation( bool should )
	{
		base.DoReloadAnimation( should );

		ViewModel.Set( "b_reload", should );
	}

	public override void OnPrimaryFire()
	{
		base.OnPrimaryFire();

		Sound.Play( "ui.popup.message.close", GameObject.Parent.Transform.Position );
		
		var tr = TraceBullet( Scene.Camera.Transform.Position, Scene.Camera.Transform.Position + Scene.Camera.Transform.Rotation.Forward * 1000f );

		ViewModel.Set("b_attack", true);

		var tracer = BulletTrace.Clone();
		var tracerParticle = tracer.Components.Get<TracerBulletParticle>();
		tracerParticle.Start.Transform.Position = tr.StartPosition + Scene.Camera.Transform.Rotation.Forward * 10f;
		tracerParticle.End.Transform.Position = tr.EndPosition;

		var inventory = PlayerStats.Inventory;

		foreach ( var item in inventory.itemPickUps )
		{
			item.Item.OnShoot();
		}

		if ( tr.Hit )
		{
			if ( tr.GameObject.Components.Get<Npcbase>() != null )
			{
				OnHit( tr.GameObject );
			}
		}
	}
}
