using Sandbox;
using System.Collections.Generic;
using System.Text;
using static RogueFPS.PlayerStats;

namespace RogueFPS;

[Title( "Base Item" )]
[Category( "Item" )]
[Icon( "upgrade", "red", "white" )]
[EditorHandle( "materials/editor/upgrade.png" )]
public class BaseItem : Component, Component.ITriggerListener
{
	public enum UpgradeRarity
	{ Common, Uncommon, Rare, Epic, Legendary }

	[Property, ImageAssetPath] public virtual string UpgradeIcon { get; set; } = "materials/editor/upgrade.png";
	[Property] public virtual bool IsStatUpgrade { get; set; } = true;
	[Property, ShowIf( nameof( IsStatUpgrade ), true )] public virtual PlayerStats.PlayerStartingStats UpgradeType { get; set; } = PlayerStats.PlayerStartingStats.Health;
	[Property, ShowIf( nameof( IsStatUpgrade ), true )] public virtual float UpgradeAmount { get; set; } = 100f;
	[Property, ShowIf( nameof( IsStatUpgrade ), true )] public virtual bool AsAPercent { get; set; } = false;
	[Property] public virtual UpgradeRarity Rarity { get; set; } = UpgradeRarity.Common;
	[Property] public virtual string ItemName { get; set; } = "Item Name";
	[Property, TextArea] public virtual string ItemDescription { get; set; } = "Item Description";
	[Property] public virtual Model Model { get; set; } = Model.Load( "models/editor/iv_helper.vmdl_c" );
	[Property] public int Amount { get; set; } = 1;
	private const float BaseChance = 10.0f; // 10%
	private const float MaxChance = 80.0f; // 80%
	public virtual float ChanceIncrementPerUpgrade { get; set; } = 5.0f; // 5%
																		 //
	private GameObject Player { get; set; }
	//


	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( other.GameObject.Tags.Has( "player" ) )
		{
			Player = other.GameObject.Parent;
			var plyStatComp = Player.Components.Get<PlayerStats>();
			if ( plyStatComp != null )
			{
				// Create an UpgradeHas object
				//var pickedUpgrade = new UpgradeHas( UpgradeIcon, ItemName, 0, Rarity ); // Initial amount set to 0
				//plyStatComp.AddUpgrade( pickedUpgrade );

				var typeDesc = TypeLibrary.GetType( GetType() );
				//if player does not have the component, create it
				if ( !plyStatComp.HasItem( this.ItemName ) )
				{
					plyStatComp.Components.Create( typeDesc );
					plyStatComp.PickedUpItems.Add( this );
				}
				else
				{
					CalculateUpdgrade();
					//if player already has the component, add the upgrade to the existing component
					var upgradeComp = plyStatComp.GetItem( this.ItemName );
					if ( upgradeComp != null )
						upgradeComp.Amount++;
				}

				var pickupui = Player.Components.Get<PickedUpItemUI>( FindMode.EnabledInSelfAndDescendants );
				pickupui.NewItem( ItemName, ItemDescription, UpgradeIcon, Rarity );

				//Player.AddComponent<RogueFPSSlideUpgrade>( true );
				Log.Info( "Player picked up an upgrade" );
				GameObject.Destroy();
			}

			else
			{
				Log.Warning( "RogueFPSPlayerStats component not found on the player." );
			}
		}
	}

	public virtual void DoUpgradeUpdate()
	{
		//Log.Info( "Upgrade on Update" );
	}

	public virtual void DoAttackUpgrade( SceneTraceResult trace )
	{
		Log.Info( "Upgrade on Attack" );
	}
	public virtual void CalculateUpdgrade()
	{
		Log.Info( "Upgrade on Calculate" );
		Log.Info( IsStatUpgrade );
		if ( IsStatUpgrade )
		{
			var plyStatComp = Player.Components.Get<PlayerStats>();

			if ( plyStatComp != null )
			{
				//Get base stat value

				var baseStat = plyStatComp.GetStartingStat( UpgradeType );

				//Apply upgrade
				if ( AsAPercent )
				{
					plyStatComp.ApplyUpgrade( GetUpgradeStatFromBase(UpgradeType), baseStat + (baseStat * (UpgradeAmount / 100f)) );
				}
				else
				{
					plyStatComp.ApplyUpgrade( GetUpgradeStatFromBase(UpgradeType), baseStat + UpgradeAmount );
					Log.Info( GetUpgradeStatFromBase( UpgradeType ) );
				}
			}

		}
	}

	public PlayerStats.PlayerUpgradedStats GetUpgradeStatFromBase( PlayerStats.PlayerStartingStats stat)
	{

		switch ( stat )
		{
			case PlayerStats.PlayerStartingStats.Health:
				return PlayerStats.PlayerUpgradedStats.Health;
			case PlayerStats.PlayerStartingStats.WalkSpeed:
				return PlayerStats.PlayerUpgradedStats.WalkSpeed;
			case PlayerStats.PlayerStartingStats.JumpHeight:
				return PlayerStats.PlayerUpgradedStats.JumpHeight;
			case PlayerStats.PlayerStartingStats.AmountOfJumps:
				return PlayerStats.PlayerUpgradedStats.AmountOfJumps;
			case PlayerStats.PlayerStartingStats.AttackSpeed:
				return PlayerStats.PlayerUpgradedStats.AttackSpeed;
			case PlayerStats.PlayerStartingStats.SprintMultiplier:
				return PlayerStats.PlayerUpgradedStats.SprintMultiplier;
			default:
				return PlayerStats.PlayerUpgradedStats.Health;
		}

	}

	public bool ShouldEffectTrigger( int upgradeAmount )
	{
		float currentChance = BaseChance + (ChanceIncrementPerUpgrade * (upgradeAmount - 1));
		currentChance = Math.Min( currentChance, MaxChance ); // Clamp to MaxChance

		float randomChance = Random.Shared.Float( 0.0f, 100.0f );

		Log.Info( $"Current chance: {currentChance}, Random chance: {randomChance}" );

		return randomChance <= currentChance;
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		//Log.Info( "OnTriggerExit" );
	}
}
