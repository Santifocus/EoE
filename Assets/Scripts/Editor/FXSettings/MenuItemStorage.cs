using EoE.Combatery;
using UnityEditor;
using static EoE.EoEEditor;

namespace EoE.Information
{
	public static class MenuItemStorage
	{
		//Items
		[MenuItem("EoE/Item/ConsumableItem")] public static void CreateBuffItem() => AssetCreator<ConsumableItem>("Settings", "Items");
		[MenuItem("EoE/Item/WeaponItem")] public static void CreateWeaponItem() => AssetCreator<WeaponItem>("Settings", "Items");
		[MenuItem("EoE/Item/SpellItem")] public static void CreateSpellItem() => AssetCreator<SpellItem>("Settings", "Items");
		[MenuItem("EoE/Item/ArmorItem")] public static void CreateArmorItem() => AssetCreator<ArmorItem>("Settings", "Items");

		//VFX
		[MenuItem("EoE/FX/ScreenShake")] public static void CreateScreenShake() => AssetCreator<ScreenShake>("Settings", "FX");
		[MenuItem("EoE/FX/ScreenBlur")] public static void CreateScreenBlur() => AssetCreator<ScreenBlur>("Settings", "FX");
		[MenuItem("EoE/FX/ScreenBorderColor")] public static void CreateScreenBorderColor() => AssetCreator<ScreenBorderColor>("Settings", "FX");
		[MenuItem("EoE/FX/ScreenTint")] public static void CreateScreenTint() => AssetCreator<ScreenTint>("Settings", "FX");
		[MenuItem("EoE/FX/ControllerRumble")] public static void CreateControllerRumble() => AssetCreator<ControllerRumble>("Settings", "FX");
		[MenuItem("EoE/FX/TimeDilation")] public static void CreateTimeDilation() => AssetCreator<TimeDilation>("Settings", "FX");
		[MenuItem("EoE/FX/ParticleEffect")] public static void CreateParticleEffect() => AssetCreator<ParticleEffect>("Settings", "FX");
		[MenuItem("EoE/FX/CameraFOVWarp")] public static void CreateCameraFOVWarp() => AssetCreator<CameraFOVWarp>("Settings", "FX");
		[MenuItem("EoE/FX/DialogueInput")] public static void CreateDialogueInput() => AssetCreator<DialogueInput>("Settings", "FX");
		[MenuItem("EoE/FX/Notification")] public static void CreateNotification() => AssetCreator<Notification>("Settings", "FX");
		[MenuItem("EoE/FX/CustomUI")] public static void CreateCustomUI() => AssetCreator<CustomUI>("Settings", "FX");

		//SFX
		[MenuItem("EoE/FX/Sound/Base")] public static void CreateSound() => AssetCreator<Sounds.Sound>("SFX", "SoundBases");
		[MenuItem("EoE/FX/Sound/Effect")] public static void CreateSoundEffect() => AssetCreator<SoundEffect>("Settings", "FX");

		//Combat
		[MenuItem("EoE/Combat/ProjectileData")] public static void CreateProjectileData() => AssetCreator<ProjectileData>("Settings", "Combat");
		[MenuItem("EoE/Combat/Effect/EffectSingle")] public static void CreateEffectSingle() => AssetCreator<EffectSingle>("Settings", "Combat");
		[MenuItem("EoE/Combat/Effect/EffectAOE")] public static void CreateEffectAOE() => AssetCreator<EffectAOE>("Settings", "Combat");
		[MenuItem("EoE/Combat/Spell")] public static void CreateSpell() => AssetCreator<Spell>("Settings", "Combat", "Spell");
		[MenuItem("EoE/Combat/Physical/Weapon")] public static void CreateWeapon() => AssetCreator<Weapon>("Settings", "Combat", "Weapon");
		[MenuItem("EoE/Combat/Physical/ComboSet")] public static void CreateComboSet() => AssetCreator<ComboSet>("Settings", "Combat", "Weapon", "ComboSets");

		//Other
		[MenuItem("EoE/Buff")] public static void CreateBuff() => AssetCreator<Buff>("Settings", "Buffs");
		[MenuItem("EoE/DropTable")] public static void CreateDropTable() => AssetCreator<DropTable>("Settings", "EntitieSettings", "DropTables");
		[MenuItem("EoE/LevelingSettings")] public static void CreateLevelingSettings() => AssetCreator<LevelingSettings>("Settings", "EntitieSettings", "LevelingSettings");

		//Data Collectors
		[MenuItem("EoE/DataCollection/Collect Items")]
		public static void CollectItemData()
		{
			string[] itemCollectorGUID = AssetDatabase.FindAssets("t:ItemCollector");
			if (itemCollectorGUID.Length == 0)
			{
				ItemCollector newCollector = AssetCreator<ItemCollector>("Settings", "Data Collection");
				newCollector.CollectData();
			}
			else if (itemCollectorGUID.Length > 1)
			{
				UnityEngine.Debug.Log("Found more then 1 Item collector... Deleting all and creating new one.");
				for (int i = 0; i < itemCollectorGUID.Length; i++)
				{
					AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(itemCollectorGUID[i]));
				}
				ItemCollector newCollector = AssetCreator<ItemCollector>("Settings", "Data Collection");
				newCollector.CollectData();
			}
			else
			{
				ItemCollector itemCollector = (ItemCollector)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(itemCollectorGUID[0]), typeof(ItemCollector));
				itemCollector.CollectData();
				EditorUtility.SetDirty(itemCollector);
			}
		}
	}
}