﻿public class ItemsAndContent
{
	//Lets make a list of all the items in the game
	public static List<ItemDef> Items = new List<ItemDef>
	{
		new AddJump(),
		new LaunchJump(),
		new FireHit(),
		new HealthRegen(),
		new HealthIncrease(),
		new LongFallBoots(),
		new AmmoIncrease(),
		new AddOneSkillTwo(),
	};

	public static List<BaseEquipmentItem> Equipments = new List<BaseEquipmentItem>
	{
		new BaseEquipmentItem(),
		new TestEquip()
	};

	public static List<BaseKeyCard> KeyCards = new List<BaseKeyCard>
	{
		new BlueKeyCard()
	};

}

