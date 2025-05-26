public class LaunchJump : ItemDef
{
	public override string Name => "Launch Jump";
	public override string Description => "Jumping while running forward will launch you forward for 100units (+100units per stack).";
	public override string Icon => "ui/test/items/icecream.png";
	public override ItemTier ItemTier => ItemTier.Rare;
	public override string ItemColor => ColorSelection.GetRarityColor( ItemTier.Rare );
	public override Model Model => Model.Load("models/citizen_props/balloonears01.vmdl_c");
	public override int StatUpgradeAmount => 1;

	public override void ApplyUpgrade()
	{

	}

	public override void RemoveUpgrade()
	{

	}

	public override void OnJump()
	{
		Log.Info( "Jump Launch" );
		if ( Owner.Components.Get<RogueFPS.PlayerController>().CharacterController.IsOnGround && Owner.Components.Get<RogueFPS.PlayerController>().MechanicTags.Has( "sprint" ) )
			Owner.Components.Get<RogueFPS.PlayerController>().CharacterController.Punch(Owner.Components.Get<RogueFPS.PlayerController>().EyeAngles.Forward * 100f * GetAmountFromInventory());
	}
}
