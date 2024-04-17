using RogueFPS;
using Sandbox;

public class BaseAbilityItem : Component
{
	[Property, Group( "Info" )] public virtual string AbilityName { get; set; } = "Weapon";
	[Property, Group( "Info" )] public virtual string AbilityDescription { get; set; } = "Weapon";
	[Property, ImageAssetPath, Group( "Info" )] public virtual string AbilityIcon { get; set; } = "ui/test/ability/ab1.png";
	[Property, Group( "Stats" )] public virtual int MaxUseCount { get; set; } = 1;
	[Property, Group( "Stats" )] public virtual bool ReloadAfterUse { get; set; } = false;
	[Property, Group( "Stats" )] public virtual bool TapFire { get; set; } = false;
	public int CurrentUseCount { get; set; }
	public TimeSince LastUsed { get; set; }
	public TimeUntil ReloadTime { get; set; } = 1f;
	public bool IsReloading { get; set; }
	[Property, Group( "Input" )] public virtual InputType WeaponInputType { get; set; } = InputType.Primary;
	[Property, Group( "Stats" )] public virtual Stats.PlayerUpgradedStats StatToUse { get; set; } = Stats.PlayerUpgradedStats.AttackSpeed;
	public Stats Stats { get; set; }
	public PlayerController PlayerController { get; set; }
	public string InputName { get; set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		Stats = GameObject.Components.Get<Stats>( FindMode.InParent );
		CurrentUseCount = MaxUseCount;

		PlayerController = GameObject.Components.Get<PlayerController>( FindMode.InParent );

		InputName = GetInputName( WeaponInputType );
	}
	public virtual void GetUpdatedStats()
	{
		
	}
	protected override void OnUpdate()
	{
		if( ReloadAfterUse )
		{
			if ( TapFire ? Input.Pressed( InputName ) : Input.Down( InputName ) )
			{
				if( CurrentUseCount <= 0) return;
				DoAction();
			}
			else
			{
				DoReleaseAction();
			}

			if ( CurrentUseCount != MaxUseCount )
			{
				DoCooldown();
				return;
			}

		}
		else
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
			else
			{
				DoReleaseAction();
			}
		}
	}

	public virtual void DoCooldown()
	{
		if ( !IsReloading )
		{
			ReloadTime = Stats.UpgradedStats[StatToUse];
			IsReloading = true;
			DoReloadAnimation( true );
		}
		
		if(CurrentUseCount != MaxUseCount && ReloadTime <= 0)
		{
			if( ReloadAfterUse )
			{
				CurrentUseCount++;
			}
			else
			{
				CurrentUseCount = MaxUseCount;
			}
			IsReloading = false;
			DoReloadAnimation( false );
		}
	}
	public virtual void DoReloadAnimation(bool should)
	{
		//Log.Info( "Reloading" );

	}

	public virtual void DoAction()
	{
		DoFire();
	}

	public virtual void DoReleaseAction()
	{
		DoReleaseFire();
	}

	public virtual void DoFire()
	{
		CurrentUseCount--;
	}

	public virtual void DoReleaseFire()
	{
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
				return "ability";
			case InputType.Ultimate:
				return "reload";
			case InputType.Equipment:
				return "equipment";
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
	Equipment,
	Passive
}

