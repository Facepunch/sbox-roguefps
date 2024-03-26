using Sandbox;

public sealed class PlayerAbilities : Component
{
	[Property, Group( "Primary" )] public string AbilityPrimaryName { get; set; } = "Primary";
	[Property, Group( "Primary" )] public string AbilityPrimaryDescription { get; set; } = "Primary";
	[Property, Group( "Primary" )] public string AbilityPrimaryIcons { get; set; } = "ui/test/ability/ab1.png";
	[Property, Group( "Primary" )] public BaseWeaponItem PrimaryWeaponItem { get; set; }


	[Property, Group( "Secondary" )] public string AbilitySecondaryName { get; set; } = "Secondary";
	[Property, Group( "Secondary" )] public string AbilitySecondaryDescription { get; set; } = "Secondary";
	[Property, Group( "Secondary" )] public string AbilitySecondaryIcons { get; set; } = "ui/test/ability/ab2.png";
	[Property, Group( "Secondary" )] public BaseWeaponItem SecondaryWeaponItem { get; set; }

	[Property, Group( "Utility" )] public string AbilityUtilityName { get; set; } = "Utility";
	[Property, Group( "Utility" )] public string AbilityUtilityDescription { get; set; } = "Utility";
	[Property, Group( "Utility" )] public string AbilityUtilityIcons { get; set; } = "ui/test/ability/ab3.png";
	[Property, Group( "Utility" )] public BaseAbilityItem UtilityAbilityItem { get; set; }

	[Property, Group( "Ultimate" )] public string AbilityUltimateName { get; set; } = "Ultimate";
	[Property, Group( "Ultimate" )] public string AbilityUltimateDescription { get; set; } = "Ultimate";
	[Property, Group( "Ultimate" )] public string AbilityUltimateIcons { get; set; } = "ui/test/ability/ab4.png";
	[Property, Group( "Ultimate" )] public BaseAbilityItem UltimateAbilityItem { get; set; }

	[Property, Group( "Passive" )] public string AbilityPassiveName { get; set; } = "Passive";
	[Property, Group( "Passive" )] public string AbilityPassiveDescription { get; set; } = "Passive";
	[Property, Group( "Passive" )] public string AbilityPassiveIcons { get; set; } = "ui/test/ability/ab1.png";

	protected override void OnStart()
	{
		base.OnStart();
	}


	protected override void OnUpdate()
	{

	}
}
