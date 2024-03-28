using RogueFPS;
using Sandbox;

public class BaseAbilityItem : Component
{
	[Property, Group( "Info" )] public virtual string AbilityName { get; set; } = "Weapon";
	[Property, Group( "Info" )] public virtual string AbilityDescription { get; set; } = "Weapon";
	[Property, ImageAssetPath, Group( "Info" )] public virtual string AbilityIcon { get; set; } = "ui/test/ability/ab1.png";
	[Property, Group( "Stats" )] public virtual int MaxUseCount { get; set; } = 1;
	public int CurrentUseCount { get; set; }
	public TimeSince LastUsed { get; set; }
	public TimeUntil ReloadTime { get; set; } = 1f;
	public bool IsReloading { get; set; }
	[Property, Group( "Input" )] public virtual InputType WeaponInputType { get; set; } = InputType.Primary;
	[Property, Group( "Stats" )] public virtual PlayerStats.PlayerUpgradedStats StatToUse { get; set; } = PlayerStats.PlayerUpgradedStats.AttackSpeed;
	public PlayerStats PlayerStats { get; set; }
	public PlayerController PlayerController { get; set; }
	public string InputName { get; set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		PlayerStats = GameObject.Components.Get<PlayerStats>( FindMode.InParent );
		CurrentUseCount = MaxUseCount;

		PlayerController = GameObject.Components.Get<PlayerController>( FindMode.InParent );

		InputName = GetInputName( WeaponInputType );
	}

	protected override void OnUpdate()
	{
		if ( CurrentUseCount <= 0 )
		{
			DoCooldown();
			return;
		}

		if ( Input.Down( InputName ) && !IsReloading )
		{
			DoAction();
		}
	}

	public virtual void DoCooldown()
	{
		if ( !IsReloading )
		{
			ReloadTime = PlayerStats.UpgradedStats[StatToUse];
			IsReloading = true;
			DoReloadAnimation( true );
		}
		
		if(CurrentUseCount != MaxUseCount && ReloadTime <= 0)
		{
			CurrentUseCount = MaxUseCount;
			IsReloading = false;
			DoReloadAnimation( false );
		}
	}
	public virtual void DoReloadAnimation(bool should)
	{
		Log.Info( "Reloading" );

	}

	public virtual void DoAction()
	{
		DoFire();
	}

	public virtual void DoFire()
	{
		CurrentUseCount--;
	}

	private static string GetInputName( InputType type)
	{
		switch ( type )
		{
			case InputType.Primary:
				return "attack1";
			case InputType.Secondary:
				return "attack2";
			case InputType.Utility:
				return "slide";
			case InputType.Ultimate:
				return "reload";
			default:
				return "attack1";
		}
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

