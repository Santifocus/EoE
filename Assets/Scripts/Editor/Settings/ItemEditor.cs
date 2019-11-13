using UnityEditor;
using UnityEngine;

namespace EoE.Information
{
	public class ItemEditor : Editor
	{
		[MenuItem("EoE/Item/BuffItem")]
		public static void CreateItem()
		{
			BuffItem asset = CreateInstance<BuffItem>();

			AssetDatabase.CreateAsset(asset, "Assets/Settings/Items/New BuffItem.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
			Debug.Log("Created: 'New BuffItem' at: Assets/Settings/Items/...");
		}
		[MenuItem("EoE/Item/HealItem")]
		public static void CreateHealItem()
		{
			HealItem asset = CreateInstance<HealItem>();

			AssetDatabase.CreateAsset(asset, "Assets/Settings/Items/New HealItem.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
			Debug.Log("Created: 'New HealItem' at: Assets/Settings/Items/...");
		}
		[MenuItem("EoE/Item/Weapon")]
		public static void CreateWeapon()
		{
			WeaponItem asset = CreateInstance<WeaponItem>();

			AssetDatabase.CreateAsset(asset, "Assets/Settings/Items/New Weapon.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
			Debug.Log("Created: 'New Weapon' at: Assets/Settings/Items/...");
		}
		[MenuItem("EoE/Item/Spell")]
		public static void CreateSpell()
		{
			SpellItem asset = CreateInstance<SpellItem>();

			AssetDatabase.CreateAsset(asset, "Assets/Settings/Items/New Spell.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
			Debug.Log("Created: 'New Spell' at: Assets/Settings/Items/...");
		}
		[MenuItem("EoE/Item/Armor")]
		public static void CreateArmor()
		{
			ArmorItem asset = CreateInstance<ArmorItem>();

			AssetDatabase.CreateAsset(asset, "Assets/Settings/Items/New Armor.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
			Debug.Log("Created: 'New Armor' at: Assets/Settings/Items/...");
		}
	}
}