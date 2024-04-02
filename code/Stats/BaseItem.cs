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
	[Property] public int Amount { get; set; } = 1;
	private const float BaseChance = 10.0f; // 10%
	private const float MaxChance = 80.0f; // 80%
	public virtual float ChanceIncrementPerUpgrade { get; set; } = 5.0f; // 5%
	[Property] public float SpawnChance { get; set; } = 100.0f;
	public GameObject Player { get; set; }
	//

	protected override void OnAwake()
	{
		base.OnAwake();
		var highlight = Components.Get<HighlightOutline>();
		if ( highlight != null )
		highlight.Color = PlayerStats.GetRarityColor( Rarity );
	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( other.GameObject.Tags.Has( "player" ) )
		{
			Player = other.GameObject.Parent;
			var plyStatComp = Player.Components.Get<PlayerStats>();
			if ( plyStatComp != null )
			{
				PickUpItem();
			}

			else
			{
				Log.Warning( "RogueFPSPlayerStats component not found on the player." );
			}
		}
	}

	public void PickUpItem()
	{
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

				CalculateUpdgrade();
			}
			else
			{
				//if player already has the component, add the upgrade to the existing component
				var upgradeComp = plyStatComp.GetItem( this.ItemName );
				if ( upgradeComp != null )
					upgradeComp.Amount++;

				CalculateUpdgrade();
			}

			var pickupui = Player.Components.Get<PickedUpItemUI>( FindMode.EnabledInSelfAndDescendants );
			pickupui.NewItem( ItemName, ItemDescription, UpgradeIcon, Rarity );

			var interact = Player.Components.Get<InteractorUse>( FindMode.EnabledInSelfAndDescendants );
			interact.DestroyUI();

			//Player.AddComponent<RogueFPSSlideUpgrade>( true );
			GameObject.Destroy();
			if ( GameObject.Parent != Scene )
			{
				GameObject.Parent.Destroy();
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

		if ( IsStatUpgrade )
		{
			var plyStatComp = Player.Components.Get<PlayerStats>();

			var item = plyStatComp.GetItem(ItemName);

			if ( plyStatComp != null )
			{
				if ( AsAPercent )
				{
					if ( item != null )
					{
						plyStatComp.UpgradedStats[plyStatComp.ConvertToUpgradedStat( UpgradeType )] = plyStatComp.GetStartingStat( UpgradeType ) + (UpgradeAmount / 100) * item.Amount;
					}
					else
					{
						plyStatComp.UpgradedStats[plyStatComp.ConvertToUpgradedStat( UpgradeType )] = plyStatComp.GetStartingStat( UpgradeType ) + (plyStatComp.GetStartingStat( UpgradeType ) * (UpgradeAmount / 100));
					}
				}
				else
				{
					if ( item != null )
					{
						plyStatComp.UpgradedStats[plyStatComp.ConvertToUpgradedStat( UpgradeType )] = plyStatComp.GetStartingStat( UpgradeType ) + UpgradeAmount * item.Amount;
					}
					else
					{
						plyStatComp.UpgradedStats[plyStatComp.ConvertToUpgradedStat( UpgradeType )] = plyStatComp.GetStartingStat( UpgradeType ) + UpgradeAmount;
					}
				}
			}

		}
	}

	public bool ShouldEffectTrigger( int upgradeAmount )
	{
		float currentChance = BaseChance + (ChanceIncrementPerUpgrade * (upgradeAmount - 1));
		currentChance = Math.Min( currentChance, MaxChance ); // Clamp to MaxChance

		float randomChance = Random.Shared.Float( 0.0f, 100.0f );

		//Log.Info( $"Current chance: {currentChance}, Random chance: {randomChance}" );

		return randomChance <= currentChance;
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		//Log.Info( "OnTriggerExit" );
	}
}
