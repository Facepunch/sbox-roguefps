using Sandbox;

public sealed class KeyCardDoorLock : Interactable
{
	[Property] DoorMove DoorToOpen { get; set; }
	public override string Name => "Use Keycard";
	public KeyCardColor KeyCardNeeded { get; set; } = KeyCardColor.Blue;
	public override string PingIcon => "ui/ping/keycard_lock.png";

	protected override void OnStart()
	{
		base.OnStart();

		Name = KeyCardNeeded.ToString() + " Keycard";
		PingString = KeyCardNeeded.ToString() + " Keycard Door Lock";

		Components.Get<ModelRenderer>().Tint = ColorSelection.GetRarityColor( ItemTier.KeyCard );
	}

	public override void OnInteract( GameObject player )
	{
		Player = player;


		var inventory = player.Components.Get<Stats>().Inventory;

		//Get all keycards and check if they match the lock required keycard
		foreach ( var keycard in inventory.itemPickUps )
		{
			if ( keycard.Item is BaseKeyCard keyCard )
			{
				if ( keyCard.KeyColor == KeyCardNeeded )
				{
					//Unlock the door
					IsOpen = true;
					CreateGlow();
					OpenDoor();
					return;
				}
				else
				{
					Log.Info( "Wrong Card" );
				}
			}
			else
			{
				Log.Info( "No keycards" );
			}
		}
	}

	public void OpenDoor()
	{
		if( DoorToOpen != null )
		{
			DoorToOpen.Open();
		}
		else
		{
			Log.Info( "No door to open" );
		}
	}
}
