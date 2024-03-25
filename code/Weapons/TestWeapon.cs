using Sandbox;

public class TestWeapon : BaseWeaponItem
{
	[Property]
	public GameObject BulletTrace { get; set; }
	protected override void OnUpdate()
	{
		base.OnUpdate();

	}

	public override void Reload()
	{
		base.Reload();

		if (!IsReloading)
		{
			ReloadTime = PlayerStats.UpgradedStats[PlayerStats.PlayerUpgradedStats.ReloadTime];
			IsReloading = true;
			ViewModel.Set("b_reload", true);
		}

		if (IsReloading)
		{
			if (ReloadTime <= 0)
			{
				CurrentAmmoCount = MaxAmmoCount;
				IsReloading = false;
				ViewModel.Set( "b_reload", false );
			}
		}

	}

	public override void OnPrimaryFire()
	{
		base.OnPrimaryFire();

		Sound.Play( "ui.popup.message.close", GameObject.Parent.Transform.Position );
		DoBulletTrace(Scene.Camera.Transform.Position, Scene.Camera.Transform.Rotation.Forward * 1000f );
		
		var tr = TraceBullet( Scene.Camera.Transform.Position, Scene.Camera.Transform.Position + Scene.Camera.Transform.Rotation.Forward * 1000f );

		ViewModel.Set("b_attack", true);

		var tracer = BulletTrace.Clone();
		var tracerParticle = tracer.Components.Get<TracerBulletParticle>();
		tracerParticle.Start.Transform.Position = tr.StartPosition + Scene.Camera.Transform.Rotation.Forward * 10f;
		tracerParticle.End.Transform.Position = tr.EndPosition;
	}
}
