using Sandbox;
using System.Collections.Generic;
using static RogueFPS.PlayerStats;

namespace RogueFPS;

[Title( "Rogue FPS Player Upgrade" )]
[Category( "Player Upgrade" )]
[Icon( "upgrade", "red", "white" )]
[EditorHandle( "materials/gizmo/charactercontroller.png" )]
public sealed class PlayerUpgrade : Component, Component.ITriggerListener
{

	public struct UpgradeHas
	{
		public string Icon;
		public string Name;
		public int Amount;

		public UpgradeHas( string icon, string name, int amount )
		{
			Icon = icon;
			Name = name;
			Amount = amount;
		}
	}

	[Property] public string UpgradeIcon { get; set; } = "roguefps/ui/test/ability/ab1.png";
	[Property] public PlayerStats.PlayerUpgradedStats UpgradeType { get; set; } = PlayerStats.PlayerUpgradedStats.Health;
	//[Property] public string UpgradeName { get; set; } = "Health";	
	[Property] public float UpgradeAmount { get; set; } = 100f;
	[Property] public bool AsAPercent { get; set; } = false;

	[Property] public string ItemName { get; set; } = "Item Name";

	[Property] public string ItemDescription { get; set; } = "Item Description";

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
				// Convert upgraded stat to starting stat
				PlayerStartingStats startingStatType = plyStatComp.ConvertToStartingStat( UpgradeType );

				// Get the starting stat value
				float startingStatValue = plyStatComp.GetStartingStat( startingStatType );
				float upgradeValue = AsAPercent ? startingStatValue * (UpgradeAmount / 100f) : UpgradeAmount;

				// Apply the upgrade
				plyStatComp.Modify( UpgradeType, upgradeValue );

				// Create an UpgradeHas object
				var pickedUpgrade = new UpgradeHas( UpgradeIcon, ItemName, 0 ); // Initial amount set to 0
				plyStatComp.AddUpgrade( pickedUpgrade );

				//Player.AddComponent<RogueFPSSlideUpgrade>( true );

				GameObject.Parent.Destroy();
			}

			else
			{
				Log.Warning( "RogueFPSPlayerStats component not found on the player." );
			}
		}
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
