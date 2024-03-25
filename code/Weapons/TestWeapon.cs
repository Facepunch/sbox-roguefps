using Sandbox;

public class TestWeapon : BaseWeaponItem
{
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
			}
		}

	}

	public override void OnPrimaryFire()
	{
		base.OnPrimaryFire();

		Sound.Play( "ui.popup.message.close", GameObject.Parent.Transform.Position );
		DoBulletTrace(Scene.Camera.Transform.Position, Scene.Camera.Transform.Rotation.Forward * 1000f );
		ViewModel.Set("b_attack", true);
	}
}
