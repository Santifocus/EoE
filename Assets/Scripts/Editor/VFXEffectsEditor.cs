using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EoE.Information
{
	public class VFXEffectsEditor : Editor
	{
		[MenuItem("EoE/VFX/ScreenShake")] public static void CreateScreenShake() => EoEEditor.AssetCreator<ScreenShake>("Settings", "VFX");
		[MenuItem("EoE/VFX/ScreenBlur")] public static void CreateScreenBlur() => EoEEditor.AssetCreator<ScreenBlur>("Settings", "VFX");
		[MenuItem("EoE/VFX/ScreenBorderColor")] public static void CreateScreenBorderColor() => EoEEditor.AssetCreator<ScreenBorderColor>("Settings", "VFX");
		[MenuItem("EoE/VFX/ScreenTint")] public static void CreateScreenTint() => EoEEditor.AssetCreator<ScreenTint>("Settings", "VFX");
		[MenuItem("EoE/VFX/ControllerRumble")] public static void CreateControllerRumble() => EoEEditor.AssetCreator<ControllerRumble>("Settings", "VFX");
		[MenuItem("EoE/VFX/TimeDilation")] public static void CreateTimeDilation() => EoEEditor.AssetCreator<TimeDilation>("Settings", "VFX");
	}
}