﻿using UnityEditor;
using EoE.Weapons;
using static EoE.EoEEditor;

namespace EoE.Information
{
	public static class MenuItemStorage
	{
		//Items
		[MenuItem("EoE/Item/BuffItem")] public static void CreateBuffItem() => AssetCreator<BuffItem>("Settings", "Items");
		[MenuItem("EoE/Item/HealItem")] public static void CreateHealItem() => AssetCreator<HealItem>("Settings", "Items");
		[MenuItem("EoE/Item/Weapon")] public static void CreateWeaponItem() => AssetCreator<WeaponItem>("Settings", "Items");
		[MenuItem("EoE/Item/Spell")] public static void CreateSpellItem() => AssetCreator<SpellItem>("Settings", "Items");
		[MenuItem("EoE/Item/Armor")] public static void CreateArmorItem() => AssetCreator<ArmorItem>("Settings", "Items");

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
		[MenuItem("EoE/Combat/Spell/Spell")] public static void CreateSpell() => AssetCreator<Spell>("Settings", "Spell");
		[MenuItem("EoE/Combat/Spell/SpellEffect")] public static void CreateSpellEffect() => AssetCreator<SpellEffect>("Settings", "Spell", "SpellEffects");

		//Other
		[MenuItem("EoE/Buff")] public static void CreateBuff() => AssetCreator<Buff>("Settings", "Buffs");
		[MenuItem("EoE/DropTable")] public static void CreateDropTable() => AssetCreator<DropTable>("Settings", "DropTables");
		[MenuItem("EoE/LevelingSettings")] public static void CreateLevelingSettings() => AssetCreator<LevelingSettings>("Settings", "EntitieSettings", "LevelingSettings");
	}
}