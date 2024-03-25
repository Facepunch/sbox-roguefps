using Sandbox;
using System.Collections.Generic;
using System.Text;
using static RogueFPS.PlayerStats;

namespace RogueFPS;

[Title( "Upgrade" )]
[Category( "Player Upgrade" )]
[Icon( "upgrade", "red", "white" )]
[EditorHandle( "materials/editor/upgrade.png" )]
public class PlayerUpgrade : Component, Component.ITriggerListener
{
	public struct UpgradeHas
	{
		public string Icon;
		public string Name;
		public int Amount;
		public UpgradeRarity Rarity;

		public UpgradeHas( string icon, string name, int amount, UpgradeRarity rarity )
		{
			Icon = icon;
			Name = name;
			Amount = amount;
			Rarity = rarity;
		}
	}
	public enum UpgradeRarity
	{ Common, Uncommon, Rare, Epic, Legendary }

	[Property, ImageAssetPath] public virtual string UpgradeIcon { get; set; } = "materials/editor/upgrade.png";
	[Property] public virtual bool IsStatUpgrade { get; set; } = true;
	[Property, ShowIf( nameof( IsStatUpgrade ), true )] public virtual PlayerStats.PlayerUpgradedStats UpgradeType { get; set; } = PlayerStats.PlayerUpgradedStats.Health;
	[Property, ShowIf( nameof( IsStatUpgrade ), true )] public virtual float UpgradeAmount { get; set; } = 100f;
	[Property, ShowIf( nameof( IsStatUpgrade ), true )] public virtual bool AsAPercent { get; set; } = false;
	[Property] public virtual UpgradeRarity Rarity { get; set; } = UpgradeRarity.Common;
	[Property] public virtual string ItemName { get; set; } = "Item Name";
	[Property, TextArea] public virtual string ItemDescription { get; set; } = "Item Description";
	[Property] public virtual Model Model { get; set; } = Model.Load( "models/editor/iv_helper.vmdl_c" );
	public int Amount { get; set; } = 1;
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
				if ( IsStatUpgrade )
				{
					// Convert upgraded stat to starting stat
					PlayerStartingStats startingStatType = plyStatComp.ConvertToStartingStat( UpgradeType );

					// Get the starting stat value
					float startingStatValue = plyStatComp.GetStartingStat( startingStatType );
					float upgradeValue = AsAPercent ? startingStatValue * (UpgradeAmount / 100f) : UpgradeAmount;

					// Apply the upgrade
					plyStatComp.Modify( UpgradeType, upgradeValue );

					// Create an UpgradeHas object
					var pickedUpgrade = new UpgradeHas( UpgradeIcon, ItemName, 0, Rarity ); // Initial amount set to 0
					plyStatComp.AddUpgrade( pickedUpgrade );
				}
				else
				{
					// Create an UpgradeHas object
					var pickedUpgrade = new UpgradeHas( UpgradeIcon, ItemName, 0, Rarity ); // Initial amount set to 0
					plyStatComp.AddUpgrade( pickedUpgrade );

					var typeDesc = TypeLibrary.GetType( GetType() );
					//if player does not have the component, create it
					if ( plyStatComp.Components.Get<PlayerUpgrade>() == null )
					{
						Player.Components.Create( typeDesc );
					}
					else
					{
						//if player already has the component, add the upgrade to the existing component
						var upgradeComp = plyStatComp.Components.Get<PlayerUpgrade>();
						upgradeComp.Amount++;
					}
				}

				var pickupui = Player.Components.Get<PickedUpItemUI>(FindMode.EnabledInSelfAndDescendants);
				pickupui.NewItem(ItemName, ItemDescription, UpgradeIcon, Rarity );
				
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

	public virtual void DoAttackUpgrade(SceneTraceResult trace )
	{
		Log.Info( "Upgrade on Attack" );
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
//keeping this around might be useful later.
/*
if ( other.GameObject.Tags.Has( "player" ) )
{
	Player = other.GameObject.Parent;
	Log.Info( "apply" );
	var stats = Player.GetComponent<RogueFPSPlayerStats>();
	if ( stats != null )
	{
		var current = TypeLibrary.GetPropertyValue( stats, UpgradeType.ToString() );

		if ( AsAPercent )
		{
			stats.ApplyUpgrade( UpgradeType.ToString(), (float)current + (float)current * (UpgradeAmount / 100f) );
		}
		else
		{
			stats.ApplyUpgrade( UpgradeType.ToString(), (float)current + UpgradeAmount );
		}

		GameObject.Parent.Destroy();
	}
	else
	{
		Log.Warning( "RogueFPSPlayerStats component not found on the player." );
	}
}
*/
