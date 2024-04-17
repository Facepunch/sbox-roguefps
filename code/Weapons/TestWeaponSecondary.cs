public class TestWeaponSecondary : BaseWeaponItem
{
	[Property] public GameObject BulletTrace { get; set; }
	public override int MaxUseCount { get; set; } = 2;
	public override bool ReloadAfterUse { get; set; } = true;
	public override bool TapFire { get; set; } = true;
	[Property] public bool UseGameObjectForTrace { get; set; } = false;
	[Property] public float BurstInterval { get; set; } = 0.1f; // Time in seconds between shots in a burst
	private float timeSinceLastShot = 0;
	private int shotsFired = 0;
	private bool isBursting = false;

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( isBursting )
		{
			timeSinceLastShot += Time.Delta;

			if ( timeSinceLastShot >= BurstInterval )
			{
				timeSinceLastShot = 0;
				ShootBullet();

				shotsFired++;
				if ( shotsFired >= 10 )
				{
					isBursting = false;
					shotsFired = 0;
				}
			}
		}
	}

	public override void DoAction()
	{
		base.DoAction();
		if ( !isBursting )
		{
			isBursting = true;
			shotsFired = 0;
			timeSinceLastShot = BurstInterval; // Start firing immediately
		}
	}

	private void ShootBullet()
	{
		Sound.Play( "ui.popup.message.close", GameObject.Parent.Transform.Position );
		var cameraObject = GetTraceObject();

		var tr = TraceBullet( cameraObject.Transform.Position, cameraObject.Transform.Position + cameraObject.Transform.Rotation.Forward * 1000f );
		ViewModel?.Set( "b_attack", true );

		var cameraController = cameraObject.Components.Get<CameraController>(FindMode.EverythingInAncestors);
		cameraController.ApplyRecoil();

		var tracer = BulletTrace.Clone();
		var tracerParticle = tracer.Components.Get<TracerBulletParticle>();
		tracerParticle.Start.Transform.Position = tr.StartPosition + cameraObject.Transform.Rotation.Forward * 10f;
		tracerParticle.End.Transform.Position = tr.EndPosition;

		CameraShake?.Shake();
		CrosshairUI?.OnShoot();

		var inventory = Stats.Inventory;

		foreach ( var item in inventory.itemPickUps )
		{
			item.Item.OnShoot();
		}

		if ( tr.Hit )
		{
			if ( tr.GameObject.Components.Get<Npcbase>() != null )
			{
				OnHit( tr.GameObject );

				foreach ( var item in inventory.itemPickUps )
				{
					item.Item.OnHit( tr.GameObject );
				}
			}
		}
	}

	public GameObject GetTraceObject()
	{
		return UseGameObjectForTrace ? GameObject : Scene.Camera.GameObject;
	}
}
