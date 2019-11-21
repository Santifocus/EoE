using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EoE.Information
{
	public class FXEffectsEditor : Editor
	{
		[MenuItem("EoE/FX/ScreenShake")] public static void CreateScreenShake() => EoEEditor.AssetCreator<ScreenShake>("Settings", "FX");
		[MenuItem("EoE/FX/ScreenBlur")] public static void CreateScreenBlur() => EoEEditor.AssetCreator<ScreenBlur>("Settings", "FX");
		[MenuItem("EoE/FX/ScreenBorderColor")] public static void CreateScreenBorderColor() => EoEEditor.AssetCreator<ScreenBorderColor>("Settings", "FX");
		[MenuItem("EoE/FX/ScreenTint")] public static void CreateScreenTint() => EoEEditor.AssetCreator<ScreenTint>("Settings", "FX");
		[MenuItem("EoE/FX/ControllerRumble")] public static void CreateControllerRumble() => EoEEditor.AssetCreator<ControllerRumble>("Settings", "FX");
		[MenuItem("EoE/FX/TimeDilation")] public static void CreateTimeDilation() => EoEEditor.AssetCreator<TimeDilation>("Settings", "FX");
		[MenuItem("EoE/FX/ParticleEffect")] public static void CreateParticleEffect() => EoEEditor.AssetCreator<ParticleEffect>("Settings", "FX");
		[MenuItem("EoE/FX/DialogueInput")] public static void CreateDialogueInput() => EoEEditor.AssetCreator<DialogueInput>("Settings", "FX");
		[MenuItem("EoE/FX/CameraFOVWarp")] public static void CreateCameraFOVWarp() => EoEEditor.AssetCreator<CameraFOVWarp>("Settings", "FX");

		[MenuItem("EoE/FX/Sound/Base")] public static void CreateSound() => EoEEditor.AssetCreator<Sounds.Sound>("SFX", "SoundBases");
		[MenuItem("EoE/FX/Sound/Effect")] public static void CreateSoundEffect() => EoEEditor.AssetCreator<SoundEffect>("Settings", "FX");
	}
}