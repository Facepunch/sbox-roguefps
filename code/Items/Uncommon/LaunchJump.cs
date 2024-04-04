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
		Log.Info( $"!!!!!!{Owner.Inventory.GetItemOwner( this )}!!!!!!" );
		Owner.Inventory.GetItemOwner( this ).UpgradedStats[PlayerStats.PlayerUpgradedStats.AmountOfJumps] = GetAmountFromInventory() + 1;
	}

	public override void RemoveUpgrade()
	{
		if(GetAmountFromInventory() == 0)
		{
			Owner.Inventory.GetItemOwner( this ).UpgradedStats[PlayerStats.PlayerUpgradedStats.AmountOfJumps] = Owner.StartingStats[PlayerStats.PlayerStartingStats.AmountOfJumps];
		}
		else
		{
			Owner.Inventory.GetItemOwner( this ).UpgradedStats[PlayerStats.PlayerUpgradedStats.AmountOfJumps] = GetAmountFromInventory();
		}

		//Owner.Inventory.GetItemOwner( this ).UpgradedStats[PlayerStats.PlayerUpgradedStats.AmountOfJumps] = Owner.StartingStats[PlayerStats.PlayerStartingStats.AmountOfJumps] + GetAmountFromInventory();
	}

	public override void OnJump()
	{
		Log.Info( "Jump Launch" );
		if ( Owner.Components.Get<PlayerController>().CharacterController.IsOnGround && Owner.Components.Get<PlayerController>().HasAnyTag( "sprint" ) )
			Owner.Components.Get<PlayerController>().CharacterController.Punch(Owner.Components.Get<PlayerController>().EyeAngles.Forward * 100f * GetAmountFromInventory());
	}
}
