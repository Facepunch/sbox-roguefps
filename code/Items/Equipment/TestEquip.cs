public class TestEquip : BaseEquipmentItem
{
	public override string Name => "Test Item";
	public override string Icon => "ui/test/items/kelly.png";
	public override Model Model => Model.Load( "models/citizen_props/bathroomsink01.vmdl_c" );
	public override ItemTier ItemTier => ItemTier.Legendary;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Legendary );
	public override bool IsEquipment => true;
	public override float Cooldown => 10f;
	public override int MaxUseCount { get; } = 10;
	public override int CurrentUseCount { get; set; } = 10;

	public override void OnUpdate()
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

	public override void DoCooldown()
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

	public override void DoAction()
	{
		DoFire();

		Log.Info( "Firing" );
	}

	public override void DoFire()
	{
		CurrentUseCount--;
	}
}
