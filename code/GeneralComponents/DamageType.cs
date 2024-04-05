public enum DamageTypes
{
	None,
	Fire,
	Ice,
	Poison,
	Electric
}
//Get damage type icon from damage type
public class DamageType
{
	public static string GetDamageTypeIcon(DamageTypes dmgType)
	{
		return dmgType switch
		{
			DamageTypes.Fire => "ui/test/damagetypes/fire_damage_type.png",
			DamageTypes.Ice => "ui/test/ability/ab2.png",
			DamageTypes.Poison => "ui/test/ability/ab3.png",
			DamageTypes.Electric => "ui/test/damagetypes/electric_damage_type.png",
			_ => "",
		};
	}
}
