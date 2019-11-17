using UnityEditor;
using UnityEngine;

namespace EoE.Information
{
	public class ItemEditor : Editor
	{
		[MenuItem("EoE/Item/BuffItem")] public static void CreateItem() => EoEEditor.AssetCreator<BuffItem>("Settings", "Items");
		[MenuItem("EoE/Item/HealItem")] public static void CreateHealItem() => EoEEditor.AssetCreator<HealItem>("Settings", "Items");
		[MenuItem("EoE/Item/Weapon")] public static void CreateWeapon() => EoEEditor.AssetCreator<WeaponItem>("Settings", "Items");
		[MenuItem("EoE/Item/Spell")] public static void CreateSpell() => EoEEditor.AssetCreator<SpellItem>("Settings", "Items");
		[MenuItem("EoE/Item/Armor")] public static void CreateArmor() => EoEEditor.AssetCreator<ArmorItem>("Settings", "Items");
	}
}