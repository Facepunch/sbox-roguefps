using Sandbox;

public class TestWeapon : BaseWeaponItem
{
	[Property]
	public GameObject BulletTrace { get; set; }

	public override bool RandomSpread { get; set; } = true;
	public override float Spread { get; set; } = 15.0f;

	/// <summary>
	/// The object that we're tracing from.
	/// </summary>
	/// <returns></returns>
	public GameObject GetTraceObject()
	{
		if ( UseGameObjectForTrace ) return GameObject;
		return Scene.Camera.GameObject;
	}

	[Property] public bool UseGameObjectForTrace { get; set; } = false;

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

		var cameraObject = GetTraceObject();

		var tr = TraceBullet( cameraObject.Transform.Position, cameraObject.Transform.Position + cameraObject.Transform.Rotation.Forward * 1000f );

		ViewModel?.Set("b_attack", true);

		var tracer = BulletTrace.Clone();
		var tracerParticle = tracer.Components.Get<TracerBulletParticle>();
		tracerParticle.Start.Transform.Position = tr.StartPosition + cameraObject.Transform.Rotation.Forward * 10f;
		tracerParticle.End.Transform.Position = tr.EndPosition;

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
}
