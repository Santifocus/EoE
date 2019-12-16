using UnityEditor;
using EoE.Combatery;
using static EoE.EoEEditor;
using UnityEngine.UI;

namespace EoE.Information
{
	public static class MenuItemStorage
	{
		//Items
		[MenuItem("EoE/Item/BuffItem")] public static void CreateBuffItem() => AssetCreator<BuffItem>("Settings", "Items");
		[MenuItem("EoE/Item/HealItem")] public static void CreateHealItem() => AssetCreator<HealItem>("Settings", "Items");
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
		[MenuItem("EoE/Combat/Physical/Weapon Controller")] public static void CreateWeaponController()
		{
			UnityEngine.Debug.LogError("Not implemented!");
		}

		//Other
		[MenuItem("EoE/Buff")] public static void CreateBuff() => AssetCreator<Buff>("Settings", "Buffs");
		[MenuItem("EoE/DropTable")] public static void CreateDropTable() => AssetCreator<DropTable>("Settings", "EntitieSettings", "DropTables");
		[MenuItem("EoE/LevelingSettings")] public static void CreateLevelingSettings() => AssetCreator<LevelingSettings>("Settings", "EntitieSettings", "LevelingSettings");

		//Data Collectors
		[MenuItem("EoE/DataCollection/Collect Items")] 
		public static void CollectItemData() 
		{
			string[] itemCollectorGUID = AssetDatabase.FindAssets("t:ItemCollector");
			if(itemCollectorGUID.Length == 0)
			{
				ItemCollector newCollector = AssetCreator<ItemCollector>("Settings", "Data Collection");
				newCollector.CollectData();
			}
			else if(itemCollectorGUID.Length > 1)
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
			}
		}
		[MenuItem("EoE/RepairConsumables")]
		public static void RepairConsumables()
		{
			string[] buffItemsGUIDS = AssetDatabase.FindAssets("t:BuffItem");
			string[] healItemsGUIDS = AssetDatabase.FindAssets("t:HealItem");

			for (int i = 0; i < buffItemsGUIDS.Length; i++)
			{
				BuffItem buffItem = (BuffItem)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(buffItemsGUIDS[i]), typeof(BuffItem));
				ConsumableItem newConsumable = AssetCreator<ConsumableItem>("Settings", "Items");

				newConsumable.ItemName = buffItem.ItemName;
				newConsumable.ItemDescription = buffItem.ItemDescription;
				newConsumable.SortingID = buffItem.SortingID;
				newConsumable.ItemModel = buffItem.ItemModel;
				newConsumable.MaxStack = buffItem.MaxStack;
				newConsumable.ItemIcon = buffItem.ItemIcon;
				newConsumable.RemoveOnUse = buffItem.RemoveOnUse;
				newConsumable.FXEffectsOnUse = buffItem.FXEffectsOnUse;
				newConsumable.FXEffectsWhenEquipped = buffItem.FXEffectsWhenEquipped;
				newConsumable.UseCooldown = buffItem.UseCooldown;

				newConsumable.BuffsToApply = buffItem.buffsToApply;
				newConsumable.StackingStyle = buffItem.stackable ? BuffStackingStyle.Stack : BuffStackingStyle.Reapply;
			}

			for (int i = 0; i < healItemsGUIDS.Length; i++)
			{
				HealItem healItem = (HealItem)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(healItemsGUIDS[i]), typeof(HealItem));
				ConsumableItem newConsumable = AssetCreator<ConsumableItem>("Settings", "Items");

				newConsumable.ItemName = healItem.ItemName;
				newConsumable.ItemDescription = healItem.ItemDescription;
				newConsumable.SortingID = healItem.SortingID;
				newConsumable.ItemModel = healItem.ItemModel;
				newConsumable.MaxStack = healItem.MaxStack;
				newConsumable.ItemIcon = healItem.ItemIcon;
				newConsumable.RemoveOnUse = healItem.RemoveOnUse;
				newConsumable.FXEffectsOnUse = healItem.FXEffectsOnUse;
				newConsumable.FXEffectsWhenEquipped = healItem.FXEffectsWhenEquipped;
				newConsumable.UseCooldown = healItem.UseCooldown;

				newConsumable.HealEffects = healItem.healEffects;
			}
		}
	}
}