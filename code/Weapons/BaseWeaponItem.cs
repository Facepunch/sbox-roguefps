using RogueFPS;
using Sandbox;

public class BaseWeaponItem : BaseAbilityItem
{
	[Property, Group("Info")] public override string AbilityName { get; set; } = "Weapon";
	[Property, Group( "Info" )] public override string AbilityDescription { get; set; } = "Weapon";
	[Property, ImageAssetPath, Group( "Info" )] public override string AbilityIcon { get; set; } = "ui/test/ability/ab1.png";
	[Property, Group( "Stats" )] public override int MaxUseCount { get; set; } = 30;
	[Property, Group( "Stats" )] public virtual bool RandomSpread { get; set; }
	[Property, Group( "Stats" )] public virtual float Spread { get; set; } = 0.1f;
	[Property, Group( "Stats" )] public bool UseMuzzle { get; set; } = true;
	public TimeSince LastFired { get; set; }
	[Property, Group( "Input" )] public override InputType WeaponInputType { get; set; } = InputType.Primary;
	[Property, Group( "Visual" )] public GameObject ViewModelObject { get; set; }
	public Vector3 Muzzle { get; set; }
	public SkinnedModelRenderer ViewModel { get; set; }
	[Property, Group( "Visual" )] public CameraShake CameraShake { get; set; }
	[Property, ImageAssetPath, Group( "Visual" )] public string Crosshair { get; set; } = "ui/crosshair/crosshair001.png";

	BasicCrosshairUI CrosshairUI { get; set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		PlayerStats = GameObject.Components.Get<PlayerStats>( FindMode.InParent );
		CurrentUseCount = MaxUseCount;

		ViewModel = ViewModelObject.Components.Get<SkinnedModelRenderer>();
		PlayerController = GameObject.Components.Get<PlayerController>( FindMode.InParent );

		CrosshairUI = GameObject.Parent.Components.Get<BasicCrosshairUI>(FindMode.EverythingInChildren);

		if(ViewModelObject != null)
		{

			Log.Info( $"Muzzle: {Muzzle}" );
		}
	}

	protected override void DrawGizmos()
	{
		base.DrawGizmos();

	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( Input.Down( "attack2" ) && WeaponInputType == InputType.Secondary )
		{
			OnSecondaryFire();
		}
	}

	public override void DoFire()
	{
		if ( LastFired >= PlayerStats.UpgradedStats[PlayerStats.PlayerUpgradedStats.AttackSpeed] )
		{
			LastFired = 0;
			OnPrimaryFire();
			base.DoFire();
			CameraShake.Shake();
			CrosshairUI.OnShoot();
			var sprintcomp = PlayerController.Components.Get<SprintMechanic>( FindMode.EverythingInSelfAndChildren );
			if ( sprintcomp != null )
			{
				sprintcomp.IsSprinting = false;
			}
		}

	}

	public override void DoReleaseAction()
	{
		base.DoReleaseAction();

		//CrosshairUI.StopShoot();
	}

	public virtual void OnPrimaryFire()
	{

		var items = PlayerStats.Components.GetAll<BaseItem>();
		foreach ( var item in items )
		{
			item.DoAttackUpgrade( TraceBullet( Scene.Camera.Transform.Position, Scene.Camera.Transform.Position + Scene.Camera.Transform.Rotation.Forward * 1000f ) );
		}
	}

	public virtual void OnSecondaryFire()
	{
		Log.Info( $"Secondary Fire: {GameObject.Name}" );
	}

	public SceneTraceResult TraceBullet( Vector3 start, Vector3 end )
	{
		if(RandomSpread)
		{
			end += Vector3.Random.Normal * Spread;
		}

		if(UseMuzzle)
		{
			Muzzle = ViewModelObject.Components.Get<SkinnedModelRenderer>().GetAttachment( "muzzle" ).Value.Position;
			start = Muzzle;
		}

		var tr = Scene.Trace.Ray( start, end )
		.WithoutTags( "player" )
		.Run();

		return tr;
	}

	public void DoBulletTrace( Vector3 start, Vector3 end )
	{
		//DoNothing
	}
}
