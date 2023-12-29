using Sandbox;
namespace RogueFPS;
public sealed class RogueFPSOpenLootBox : Component, Component.ITriggerListener
{
	private GameObject Player;

	[Property] public int LootBoxCost { get; set; } = 10;

	[Property] public string LootBoxName { get; set; } = "Small Crate";

	[Property] public GameObject LootBoxPriceLabel { get; set; }

	private GameObject priceLabel { get; set; }

	private TestHudPanel UIInteraction { get; set; }

	private bool BeenOpened = false;

	protected override void OnUpdate()
	{
		
		if( Player != null)
		{
			var plyStatComp = Player.Components.Get<PlayerStats>();
			if ( plyStatComp != null )
			{
				if ( Input.Pressed( "use" ) && plyStatComp.PlayerCoinsAndXp[PlayerStats.CoinsAndXp.Coins] >= LootBoxCost && !BeenOpened )
				{
					Log.Info( "Open Loot Box" );

					var boxInsides = GameObject.Parent.Components.Get<RogueFPSLootBox>();
					
					boxInsides.CreateItem();

					plyStatComp.AddCoin( -LootBoxCost );

					BeenOpened = true;

					plyStatComp.LootBoxInteract = false;
				}
				else
				{
					Log.Info( "Can't afford it" );
				}
			}
		}

	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( other.GameObject.Tags.Has( "player" ) )
		{
			Player = other.GameObject.Parent;
			var plyStatComp = Player.Components.Get<PlayerStats>();
			
			priceLabel = SceneUtility.Instantiate( LootBoxPriceLabel, Vector3.Up * 50 );
			priceLabel.Parent = GameObject.Parent;
			var price = priceLabel.Components.Get<LootBoxInteract>();
			price.LootBoxPrice = LootBoxCost;
			
			if ( plyStatComp.PlayerCoinsAndXp[PlayerStats.CoinsAndXp.Coins] >= LootBoxCost )
			{
				//var plyStatComp = Player.GetComponent<RogueFPSPlayerStats>();
				plyStatComp.LootBoxName = LootBoxName;
				plyStatComp.LootBoxCost = LootBoxCost;
				plyStatComp.LootBoxInteract = true;
			}
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		var plyStatComp = Player.Components.Get<PlayerStats>();
		plyStatComp.LootBoxInteract = false;

		priceLabel.Destroy();
		priceLabel = null;
		Player = null;

	}
}
