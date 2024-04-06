namespace RogueFPS;

[Title( "Stats" )]
[Category( "Stats" )]
[Icon( "analytics", "yellow", "white" )]
public sealed class Stats : Component
{
	public static Stats Local { get; private set; }

	//Default Stats Property
	[Property] public float Health { get; set; } = 100f;
	[Property] public float Armor { get; set; } = 0f;
	[Property, Group( "Movement" )] public float WalkSpeed { get; set; } = 280f;
	[Property, Group( "Movement" )] public float SprintMultiplier { get; set; } = 1.45f;
	[Property, Group( "Jump" )] public float AmountOfJumps { get; set; } = 1;
	[Property, Group( "Jump" )] public float JumpHeight { get; set; } = 1f;
	[Property, Group( "Attack" )] public float AttackSpeed { get; set; } = 1f;
	[Property, Group( "Attack" )] public float AttackDamage { get; set; } = 10f;
	[Property, Group( "Attack" )] public float ReloadTime { get; set; } = 1f;
	[Property, Group( "Attack" )] public float SecondaryAttackCoolDown { get; set; } = 5f;
	[Property, Group( "Skill" )] public float SkillOneCoolDown { get; set; } = 5f;
	[Property, Group( "Skill" )] public float SkillOneUses { get; set; } = 1f;
	[Property, Group( "Skill" )] public float UltimateCoolDown { get; set; } = 50f;
	[Property, Group( "Skill" )] public float UltimateUses { get; set; } = 1f;
	//

	//Coin and XP
	public enum CoinsAndXp { Coins, Xp }
	public int CurrentLevel { get; set; } = 1;
	
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

	public ItemInventory Inventory;

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

		Inventory = new ItemInventory(this);

		//PickedUpUpgrades = new Dictionary<string, UpgradeHas>();

		//	StartingStats = new Dictionary<PlayerStartingStats, float>();
		//	UpgradedStats = new Dictionary<PlayerUpgradedStats, float>();
		//	GetStatingStats();
		if ( IsProxy ) return;
		Local = this;
	}

	//public void ApplyUpgrade( string upgradeName, float upgradeAmount )
	//{
	//	TypeLibrary.SetProperty( this, upgradeName, upgradeAmount );
	//	Log.Info( $"Applied {upgradeName} upgrade to {upgradeAmount}." );
	//}

	public void ApplyUpgrade( PlayerUpgradedStats stat, float amount )
	{
		UpgradedStats[stat] += amount;
		Log.Info( $"Applied {stat} upgrade to {amount}." );
	}

	public void RemoveUpgrade( PlayerUpgradedStats stat, float amount )
	{
		UpgradedStats[stat] -= amount;
		Log.Info( $"Removed {stat} upgrade to {amount}." );
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
		//Don't go below 0
		else if ( PlayerCoinsAndXp[CoinsAndXp.Coins] < 0 )
		{
			PlayerCoinsAndXp[CoinsAndXp.Coins] = 0;
		}
		else
		{
			// If this is the first time the player is getting coins
			PlayerCoinsAndXp[CoinsAndXp.Coins] = amount;
		}

		//Log.Info( $"Added {amount} coins. Total coins: {PlayerCoinsAndXp[CoinsAndXp.Coins]}" );
	}

	public static int MaxExperienceForLevel( int level )
	{
		return (int)(10f + MathF.Pow( level + 1, 2.7f ));
	}

	public void AddXP( int amount )
	{
		// If the player has 10 xp add a level, then double the amount needed for the next level
		if ( PlayerCoinsAndXp[CoinsAndXp.Xp] >= MaxExperienceForLevel(CurrentLevel) )
		{
			CurrentLevel++;
			PlayerCoinsAndXp[CoinsAndXp.Xp] = 0;

			float healthRatio = Components.Get<Actor>().Health / UpgradedStats[PlayerUpgradedStats.Health];

			UpgradedStats[PlayerUpgradedStats.Health] = GetStartingStat( PlayerStartingStats.Health ) * CurrentLevel * 1.15f;
			UpgradedStats[PlayerUpgradedStats.AttackDamage] = GetStartingStat( PlayerStartingStats.AttackDamage ) * CurrentLevel * 0.5f;

			Components.Get<Actor>().Health = UpgradedStats[PlayerUpgradedStats.Health] * healthRatio;
		}
		else
		{
			PlayerCoinsAndXp[CoinsAndXp.Xp] += amount;
		}
	}

	public PlayerStartingStats ConvertToStartingStat( PlayerUpgradedStats upgradedStat )
	{
		return Enum.TryParse( upgradedStat.ToString(), out PlayerStartingStats startingStat ) ? startingStat : default;
	}

	public PlayerUpgradedStats ConvertToUpgradedStat( PlayerStartingStats startingStat )
	{
		return Enum.TryParse( startingStat.ToString(), out PlayerUpgradedStats upgradedStat ) ? upgradedStat : default;
	}

	public float GetStartingStat( PlayerStartingStats stat )
	{
		return StartingStats.ContainsKey( stat ) ? StartingStats[stat] : 0f;
	}

	public void Modify( PlayerUpgradedStats stat, float amount )
	{
		UpgradedStats[stat] += amount;
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


	[ConCmd( "Rogue_GiveCoins" )]
	public static void GiveCoins( int amount )
	{
		var stat = Stats.Local;
		stat.AddCoin( amount );
	}
}
