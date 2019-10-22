using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(PlayerSettings), true), CanEditMultipleObjects]
	public class PlayerSettingsEditor : EntitieSettingsEditor
	{
		protected override void CustomInspector()
		{
			base.CustomInspector();
			CameraSettingsArea();
			HUDSettingsArea();
		}

		private void CameraSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			EoEEditor.Header("Camera Settings");
			EoEEditor.FloatField(new GUIContent("Camera to Player Distance", "How far from the Player should the camera be?"), ref settings.CameraToPlayerDistance);
			EoEEditor.Vector3Field(new GUIContent("Camera Anchor offset", "The camera will always look at the Camera Anchor."), ref settings.CameraAnchorOffset);
			EoEEditor.Vector2Field(new GUIContent("Camera Rotation Power", "The amount of rotation that will be added when the player tries to rotate the camera."), ref settings.CameraRotationPower);
			EoEEditor.FloatField(new GUIContent("Camera Rotation Speed", "How fast should the added rotation be interpolated. Higher value means less smooth, lower means that even after multiple seconds the camera might still slightly move."), ref settings.CameraRotationSpeed);
			EoEEditor.Vector2Field(new GUIContent("Camera Vertical Angle Clamps", "How far around the X Axis can the player rotate the camera. (Up / Down)"), ref settings.CameraVerticalAngleClamps);
		}
		protected override void StatSettingsArea()
		{
			base.StatSettingsArea();
			EnduranceSettingsArea();
		}
		private void EnduranceSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			EoEEditor.Header("Endurance Settings");
			EoEEditor.IntField(new GUIContent("Endurance Bars", "How many Endurance reserve bars does the player have?"), ref settings.EnduranceBars);
			EoEEditor.FloatField(new GUIContent("Endurance per Bar", "How much Endurance points are stored per small bar. The player Endurance can be calculated by multiplying this value times 'EnduranceBars'"), ref settings.EndurancePerBar);
			EoEEditor.FloatField(new GUIContent("Endurance Regen", "The base Endurance regen value."), ref settings.EnduranceRegen);
			EoEEditor.FloatField(new GUIContent("Locked Endurance Regen Mutliplier", "How fast should the locked Endurance bar regenerate? (Multipies the base regen, Combat multiplier will also be added to the calculation (Base x LE x CE))"), ref settings.LockedEnduranceRegenMutliplier);
			EoEEditor.FloatField(new GUIContent("Endurance Regen In Combat", "If the player is in combat how will the Endurance regen multiplied?"), ref settings.EnduranceRegenInCombat);
			EoEEditor.FloatField(new GUIContent("Endurance Regen Delay After Use", "After the player used Endurance how long will there be a cooldown for the main Endurance bar to regenerate. (In Seconds), (Small bars always regenerate)"), ref settings.EnduranceRegenDelayAfterUse);

			GUILayout.Space(5);
			EoEEditor.FloatField(new GUIContent("Jump Endurance Cost", "How much Endurance will be used from a jump?"), ref settings.JumpEnduranceCost);
			EoEEditor.FloatField(new GUIContent("Run Endurance Cost", "How much Endurance does running use? (Per Second)"), ref settings.RunEnduranceCost);
		}
		private void HUDSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			EoEEditor.Header("HUD Settings");
			EoEEditor.FloatField(new GUIContent("Stat Text Update Speed", "How fast should the Stat text display update its number? Depending on the current difference between actuall health and displayed health the update speed increases."), ref settings.StatTextUpdateSpeed);
			EoEEditor.FloatField(new GUIContent("Endurance Bar Lerp Speed", "How fast should the Endurance bar(s) lerp to the actuall Endurance amount. (In 1/X Seconds)"), ref settings.EnduranceBarLerpSpeed);

			GUILayout.Space(5);
			EoEEditor.GradientField(new GUIContent("Health Text Colors", "Gradient Start == No Health, Gradient End == Full Health"), ref settings.HealthTextColors);
			EoEEditor.GradientField(new GUIContent("Mana Text Colors", "Gradient Start == No Mana, Gradient End == Full Mana"), ref settings.ManaTextColors);

			GUILayout.Space(5);
			EoEEditor.ColorField("Reserved Endurance Bar Color", ref settings.ReserveEnduranceBarColor);
			EoEEditor.ColorField("Active Endurance Bar Color", ref settings.ActiveEnduranceBarColor);
			EoEEditor.ColorField("Reloading Endurance Bar Color", ref settings.ReloadingEnduranceBarColor);
		}
	}
}