using static RogueFPS.BaseItem;

namespace RogueFPS;

[Title( "Player Stats" )]
[Category( "Stats" )]
[Icon( "analytics", "yellow", "white" )]
public sealed class PlayerStats : Component
{
	//Default Stats Property
	[Property] public float Health { get; set; } = 100f;
	[Property] public float Armor { get; set; } = 0f;
	[Property, Group( "Movement" )] public float WalkSpeed { get; set; } = 280f;
	[Property, Group( "Movement" )] public float SprintMultiplier { get; set; } = 1.45f;
	[Property, Group( "Jump" )] public float AmountOfJumps { get; set; } = 1;
	[Property, Group( "Jump" )] public float JumpHeight { get; set; } = 1f;
	[Property, Group( "Attack" )] public float AttackSpeed { get; set; } = 1f;
	[Property, Group( "Attack" )] public float AttackDamage { get; set; } = 1f;
	[Property, Group( "Attack" )] public float ReloadTime { get; set; } = 1f;
	[Property, Group( "Attack" )] public float SecondaryAttackCoolDown { get; set; } = 5f;
	[Property, Group( "Skill" )] public float SkillOneCoolDown { get; set; } = 5f;
	[Property, Group( "Skill" )] public float SkillOneUses { get; set; } = 1f;
	[Property, Group( "Skill" )] public float UltimateCoolDown { get; set; } = 50f;
	[Property, Group( "Skill" )] public float UltimateUses { get; set; } = 1f;
	//
	public List<BaseItem> PickedUpItems { get; set; } = new List<BaseItem>();
	//

	//Coin and XP
	public enum CoinsAndXp { Coins, Xp }
	
	public IDictionary<CoinsAndXp, int> PlayerCoinsAndXp { get; private set; }
	//

	//Create Starting Stats
	public IDictionary<PlayerStartingStats, float> StartingStats { get; private set; }
	public bool HasStat( PlayerStartingStats stat ) => StartingStats[stat] > 0f;
	public enum PlayerStartingStats
	{
		Health, Armor, WalkSpeed, SprintMultiplier, AmountOfJumps, JumpHeight, AttackSpeed, AttackDamage, ReloadTime, SecondaryAttackCoolDown, SkillOneCoolDown, SkillOneUses, UltimateCoolDown, UltimateUses
	}
	//

	//Create Upgraded Stats
	public IDictionary<PlayerUpgradedStats, float> UpgradedStats { get; private set; }
	public enum PlayerUpgradedStats
	{
		Health, Armor, WalkSpeed, SprintMultiplier, AmountOfJumps, JumpHeight, AttackSpeed, AttackDamage, ReloadTime, SecondaryAttackCoolDown, SkillOneCoolDown, SkillOneUses, UltimateCoolDown, UltimateUses
	}
	//

	// Can't work out another way
	public bool LootBoxInteract = false;
	public int LootBoxCost = 10;
	public string LootBoxName = "Loot Box";
	//

	// Keep track of what ability the player has
	public struct AbiliyHas
	{
		public string Icon;
		public string Name;

		public AbiliyHas( string icon, string name )
		{
			Icon = icon;
			Name = name;
		}
	}
	public Dictionary<string, AbiliyHas> PickedUpAbilities { get; private set; }
	//

	// Keep track of what Ultimate the player has
	public struct UltimateHas
	{
		public string Icon;
		public string Name;

		public UltimateHas( string icon, string name )
		{
			Icon = icon;
			Name = name;
		}
	}
	public Dictionary<string, UltimateHas> PickedUpUltimate { get; private set; }
	//

	//
	public float AbilityCooldown { get; set; }
	public float UltimateCooldown { get; set; }
	//

	//
	public float AbilityUsed { get; set; }
	public float UltimateUsed { get; set; }
	//

	protected override void OnAwake()
	{
		base.OnAwake();
		StartingStats = new Dictionary<PlayerStartingStats, float>();
		UpgradedStats = new Dictionary<PlayerUpgradedStats, float>();
		PlayerCoinsAndXp = new Dictionary<CoinsAndXp, int>();

		PickedUpAbilities = new Dictionary<string, AbiliyHas>();
		PickedUpUltimate = new Dictionary<string, UltimateHas>();

		GetStatingStats();
	}

	protected override void OnStart()
	{
		base.OnStart();
		//PickedUpUpgrades = new Dictionary<string, UpgradeHas>();

		//	StartingStats = new Dictionary<PlayerStartingStats, float>();
		//	UpgradedStats = new Dictionary<PlayerUpgradedStats, float>();
		//	GetStatingStats();
	}

	public void AddItemComponent( BaseItem comp )
	{
		var typeDesc = TypeLibrary.GetType( GetType() );
		Components.Create( typeDesc );
	}

	public void ApplyUpgrade( string upgradeName, float upgradeAmount )
	{
		TypeLibrary.SetProperty( this, upgradeName, upgradeAmount );
		//Log.Info( $"Applied {upgradeName} upgrade to {upgradeAmount}." );
	}

	//public void AddUpgrade( UpgradeHas upgrade )
	//{
	//	if ( PickedUpUpgrades.TryGetValue( upgrade.Name, out UpgradeHas existingUpgrade ) )
	//	{
	//		existingUpgrade.Amount++;
	//		PickedUpUpgrades[upgrade.Name] = existingUpgrade;
	//	}
	//	else
	//	{
	//		upgrade.Amount = 1;
	//		PickedUpUpgrades.Add( upgrade.Name, upgrade );
	//	}

	//	Log.Info( $"Picked up upgrade: {upgrade.Name}, Total: {PickedUpUpgrades[upgrade.Name].Amount}" );
	//}

	public void AddCoin( int amount )
	{
		if ( PlayerCoinsAndXp.ContainsKey( CoinsAndXp.Coins ) )
		{
			// If the player already has some coins, add to the existing amount
			PlayerCoinsAndXp[CoinsAndXp.Coins] += amount;
		}
		else
		{
			// If this is the first time the player is getting coins
			PlayerCoinsAndXp[CoinsAndXp.Coins] = amount;
		}

		Log.Info( $"Added {amount} coins. Total coins: {PlayerCoinsAndXp[CoinsAndXp.Coins]}" );
	}
	public void AddItem( BaseItem comp )
	{
		//var comp = Components.Create( type );
		//PickedUpItems.Add( comp );
	}
	public bool HasItem( string item )
	{
		return PickedUpItems.Contains( PickedUpItems.FirstOrDefault( x => x.ItemName == item ) );
	}
	public BaseItem GetItem( string item )
	{
		return Components.GetAll<BaseItem>().FirstOrDefault( x => x.ItemName == item );
	}

	public List<BaseItem> GetAllItems()
	{
		return Components.GetAll<BaseItem>().ToList();
	}
	public PlayerStartingStats ConvertToStartingStat( PlayerUpgradedStats upgradedStat )
	{
		return Enum.TryParse( upgradedStat.ToString(), out PlayerStartingStats startingStat ) ? startingStat : default;
	}

	public float GetStartingStat( PlayerStartingStats stat )
	{
		return StartingStats.ContainsKey( stat ) ? StartingStats[stat] : 0f;
	}

	public void Modify( PlayerUpgradedStats stat, float amount )
	{
		UpgradedStats[stat] += amount;
	}

	protected override void OnUpdate()
	{
		if ( Input.Pressed( "slot1" ) )
		{
			Log.Info( PickedUpAbilities.FirstOrDefault() );

		}

		var items = Components.GetAll<BaseItem>();
		foreach ( var item in items )
		{
			item.DoUpgradeUpdate();
		}
		
	}
	
	//Handle Ability Stuff
	public void HandleAbilityCoolDown(float cooldownTime)
	{
		AbilityCooldown = cooldownTime;
	}

	//Handle Ability Uses
	public void HandleAbilityUses( float abilityused )
	{
		AbilityUsed = abilityused;
	}

	public void AddAbility( string indx, string abilityName, string abilityIcon )
	{
		if( !PickedUpAbilities.ContainsKey(indx))
		{
			PickedUpAbilities.Add( indx, new AbiliyHas( abilityName, abilityIcon ) );
		}
	}
	//

	//Handle Ultimate Stuff
	public void HandleUltimateCoolDown( float cooldownTime )
	{
		UltimateCooldown = cooldownTime;
	}

	//Handle Ultimate Uses
	public void HandleUltimateUses( float ultimateused )
	{
		UltimateUsed = ultimateused;
	}

	public void AddUltimate( string indx, string abilityName, string abilityIcon )
	{
		if ( !PickedUpUltimate.ContainsKey( indx ) )
		{
			PickedUpUltimate.Add( indx, new UltimateHas( abilityName, abilityIcon ) );
		}
	}
	//

	public void GetStatingStats()
	{
		//foreach ( int i in Enum.GetValues( typeof( PlayerStartingStats ) ) )
		//	StartingStats[(PlayerStartingStats)i] = 0f;

		PlayerCoinsAndXp.Add( CoinsAndXp.Coins, 0 );
		PlayerCoinsAndXp.Add( CoinsAndXp.Xp, 0 );

		//Set Starting Stats
		StartingStats[PlayerStartingStats.Health] = Health;
		StartingStats[PlayerStartingStats.Armor] = Armor;
		StartingStats[PlayerStartingStats.WalkSpeed] = WalkSpeed;
		StartingStats[PlayerStartingStats.SprintMultiplier] = SprintMultiplier;
		StartingStats[PlayerStartingStats.AmountOfJumps] = AmountOfJumps;
		StartingStats[PlayerStartingStats.JumpHeight] = JumpHeight;
		StartingStats[PlayerStartingStats.AttackSpeed] = AttackSpeed;
		StartingStats[PlayerStartingStats.AttackDamage] = AttackDamage;
		StartingStats[PlayerStartingStats.ReloadTime] = ReloadTime;
		StartingStats[PlayerStartingStats.SecondaryAttackCoolDown] = SecondaryAttackCoolDown;
		StartingStats[PlayerStartingStats.SkillOneCoolDown] = SkillOneCoolDown;
		StartingStats[PlayerStartingStats.SkillOneUses] = SkillOneUses;
		StartingStats[PlayerStartingStats.UltimateCoolDown] = UltimateCoolDown;
		StartingStats[PlayerStartingStats.UltimateUses] = UltimateUses;


		//Set Upgraded Stats
		UpgradedStats[PlayerUpgradedStats.Health] = Health;
		UpgradedStats[PlayerUpgradedStats.Armor] = Armor;
		UpgradedStats[PlayerUpgradedStats.WalkSpeed] = WalkSpeed;
		UpgradedStats[PlayerUpgradedStats.SprintMultiplier] = SprintMultiplier;
		UpgradedStats[PlayerUpgradedStats.AmountOfJumps] = AmountOfJumps;
		UpgradedStats[PlayerUpgradedStats.JumpHeight] = JumpHeight;
		UpgradedStats[PlayerUpgradedStats.AttackSpeed] = AttackSpeed;
		UpgradedStats[PlayerUpgradedStats.AttackDamage] = AttackDamage;
		UpgradedStats[PlayerUpgradedStats.ReloadTime] = ReloadTime;
		UpgradedStats[PlayerUpgradedStats.SecondaryAttackCoolDown] = SecondaryAttackCoolDown;
		UpgradedStats[PlayerUpgradedStats.SkillOneCoolDown] = SkillOneCoolDown;
		UpgradedStats[PlayerUpgradedStats.SkillOneUses] = SkillOneUses;
		UpgradedStats[PlayerUpgradedStats.UltimateCoolDown] = UltimateCoolDown;
		UpgradedStats[PlayerUpgradedStats.UltimateUses] = UltimateUses;

	}
}
