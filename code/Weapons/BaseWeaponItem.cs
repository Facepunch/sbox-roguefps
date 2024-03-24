using RogueFPS;
using Sandbox;

public class BaseWeaponItem : Component
{
	[Property] public virtual string WeaponName { get; set; } = "Weapon";
	[Property] public virtual string WeaponDescription { get; set; } = "Weapon";
	[Property, ImageAssetPath] public virtual string Icon { get; set; } = "ui/test/ability/ab1.png";
	[Property] public virtual bool UsesAmmo { get; set; } = true;
	[Property,ShowIf( "UsesAmmo",true)] public virtual int MaxAmmoCount { get; set; } = 30;
	public int CurrentAmmoCount { get; set; }
	public TimeSince LastFired { get; set; }
	public TimeUntil ReloadTime { get; set; } = 1f;
	public bool IsReloading { get; set; }
	[Property] public virtual InputType WeaponInputType { get; set; } = InputType.Primary;

	public PlayerStats PlayerStats { get; set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		PlayerStats = GameObject.Components.Get<PlayerStats>( FindMode.InParent );
		CurrentAmmoCount = MaxAmmoCount;
	}

	protected override void OnUpdate()
	{

		if ( Input.Down( "attack1" ) && WeaponInputType == InputType.Primary )
		{
			if ( UsesAmmo && CurrentAmmoCount <= 0 )
			{
				Reload();
				return;
			}
			else
			{
				if(LastFired >= PlayerStats.UpgradedStats[PlayerStats.PlayerUpgradedStats.AttackSpeed] )
				{
					LastFired = 0;
					OnPrimaryFire();
					Log.Info( $"Primary Speed: {PlayerStats.UpgradedStats[PlayerStats.PlayerUpgradedStats.AttackSpeed]}" );
				}
			}
		}

		if ( Input.Down( "attack2" ) && WeaponInputType == InputType.Secondary )
		{
			OnSecondaryFire();
		}

	}
	public virtual void Reload()
	{
		Log.Info( $"Reloading: {GameObject.Name}" );
	}

	public virtual void OnPrimaryFire()
	{
		Log.Info( $"Primary Fire: {GameObject.Name}" );

		CurrentAmmoCount--;
	}

	public virtual void OnSecondaryFire()
	{
		Log.Info( $"Secondary Fire: {GameObject.Name}" );
	}

	public void DoBulletTrace( Vector3 start, Vector3 end )
	{
		var tr = Scene.Trace.Ray( start, end )
			.WithoutTags( "player" )
			.Run();

		if ( tr.Hit )
		{
			Log.Info( $"Hit: {tr.GameObject.Name}" );
		}

		Gizmo.Draw.Line( start, tr.EndPosition );
	}
}
public enum InputType
{
	Primary,
	Secondary,
	Utility,
	Ultimate,
	Passive
}
