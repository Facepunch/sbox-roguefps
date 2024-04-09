using static Sandbox.Services.Stats;

public class BaseEquipmentBase : ItemDef
{
	public override string Name => "Base Equipment";
	public override string Icon => "ui/test/items/equipment.png";
	public override Model Model => Model.Load( "models/citizen_props/trashbag02.vmdl_c" );
	public override ItemTier ItemTier => ItemTier.Legendary;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Legendary );
	public override bool IsEquipment => true;
	public virtual float Cooldown => 10f;
	public virtual int MaxUseCount { get; } = 2;
	public int CurrentUseCount { get; set; } = 2;
	public TimeSince LastUsed { get; set; }
	public TimeUntil ReloadTime { get; set; } = 1f;
	public float ReloadTimeAmount { get; set; } = 2f;
	public bool IsReloading { get; set; }
	public InputType InputType { get; set; } = InputType.Equipment;
	public string InputName { get; set; } = "equipment";

	public virtual void OnUpdate()
	{
		if ( Input.Pressed( InputName ) && CurrentUseCount != 0 )
		{
			DoAction();
		}

		if (CurrentUseCount != MaxUseCount )
		{
			DoCooldown();
			return;
		}
	}

	public virtual void DoCooldown()
	{
		if ( !IsReloading )
		{
			ReloadTime = ReloadTimeAmount;
			IsReloading = true;
		}

		if ( CurrentUseCount != MaxUseCount && ReloadTime <= 0 )
		{
			CurrentUseCount++;
			IsReloading = false;
		}
	}

	public virtual void DoAction()
	{
		DoFire();

		Log.Info( "Firing" );
	}

	public virtual void DoFire()
	{
		CurrentUseCount--;
	}
}
