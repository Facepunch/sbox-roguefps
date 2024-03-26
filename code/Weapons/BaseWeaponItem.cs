using RogueFPS;
using Sandbox;

public class BaseWeaponItem : BaseAbilityItem
{
	[Property] public override string AbilityName { get; set; } = "Weapon";
	[Property] public override string AbilityDescription { get; set; } = "Weapon";
	[Property, ImageAssetPath] public override string AbilityIcon { get; set; } = "ui/test/ability/ab1.png";
	[Property] public override bool HasUses { get; set; } = true;
	[Property,ShowIf( "UsesAmmo",true)] public override int MaxUseCount { get; set; } = 30;
	public TimeSince LastFired { get; set; }
	[Property] public override InputType WeaponInputType { get; set; } = InputType.Primary;
	[Property] public GameObject ViewModelObject { get; set; }
	public SkinnedModelRenderer ViewModel { get; set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		PlayerStats = GameObject.Components.Get<PlayerStats>( FindMode.InParent );
		CurrentUseCount = MaxUseCount;

		ViewModel = ViewModelObject.Components.Get<SkinnedModelRenderer>();
		PlayerController = GameObject.Components.Get<PlayerController>( FindMode.InParent );
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
		base.DoFire();

		if ( LastFired >= PlayerStats.UpgradedStats[PlayerStats.PlayerUpgradedStats.AttackSpeed] )
		{
			LastFired = 0;
			OnPrimaryFire();
			var sprintcomp = PlayerController.Components.Get<SprintMechanic>( FindMode.EverythingInSelfAndChildren );
			if ( sprintcomp != null )
			{
				sprintcomp.IsSprinting = false;
			}
		}

	}

	public virtual void OnPrimaryFire()
	{
		CurrentUseCount--;

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
