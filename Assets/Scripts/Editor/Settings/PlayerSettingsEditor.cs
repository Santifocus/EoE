using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(PlayerSettings), true), CanEditMultipleObjects]
	public class PlayerSettingsEditor : EntitieSettingsEditor
	{
		private static bool CameraSettingsOpen;
		private static bool EnduranceSettingsOpen;
		private static bool HUDSettingsOpen;
		private static bool DodgeSettingsOpen;
		protected override void CustomInspector()
		{
			base.CustomInspector();
			CameraSettingsArea();
			HUDSettingsArea();
			DodgeSettingsArea();
		}

		private void CameraSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FoldoutHeader("Camera Settings", ref CameraSettingsOpen);
			if (CameraSettingsOpen)
			{
				FloatField(new GUIContent("Camera to Player Distance", "How far from the Player should the camera be?"), ref settings.CameraToPlayerDistance);
				Vector3Field(new GUIContent("Camera Anchor offset", "The camera will always look at the Camera Anchor."), ref settings.CameraAnchorOffset);
				Vector2Field(new GUIContent("Camera Rotation Power", "The amount of rotation that will be added when the player tries to rotate the camera."), ref settings.CameraRotationPower);
				FloatField(new GUIContent("Camera Rotation Speed", "How fast should the added rotation be interpolated. Higher value means less smooth, lower means that even after multiple seconds the camera might still slightly move."), ref settings.CameraRotationSpeed);
				Vector2Field(new GUIContent("Camera Vertical Angle Clamps", "How far around the X Axis can the player rotate the camera. (Up / Down)"), ref settings.CameraVerticalAngleClamps);
				FloatField(new GUIContent("Camera Extra Zoom On Vertical", "When the camera look down on the player the zoom will be multiplied by '1 + value'."), ref settings.CameraExtraZoomOnVertical);
				FloatField(new GUIContent("Max Enemy Targeting Distance", "When the player tries to target an Entitie, how far from the Player can that Entitie be at max?"), ref settings.MaxEnemyTargetingDistance);
				Vector2Field(new GUIContent("Camera Clamps When Targeting", "How far around the X Axis can the player rotate the camera when the player is targeting a Entitie. (Up / Down)"), ref settings.CameraClampsWhenTargeting);
			}
			EndFoldoutHeader();
		}
		protected override void StatSettingsArea()
		{
			ObjectField(new GUIContent("Level Settings", "The Level settings that should be applied to the Player."), ref (target as PlayerSettings).LevelSettings);
			base.StatSettingsArea();
			EnduranceSettingsArea();
		}
		private void EnduranceSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			Foldout("Endurance Settings", ref EnduranceSettingsOpen);
			if (EnduranceSettingsOpen)
			{
				IntField(new GUIContent("Endurance Bars", "How many Endurance reserve bars does the player have?"), ref settings.EnduranceBars, 1);
				FloatField(new GUIContent("Endurance per Bar", "How much Endurance points are stored per small bar. The player Endurance can be calculated by multiplying this value times 'EnduranceBars'"), ref settings.EndurancePerBar, 1);
				FloatField(new GUIContent("Endurance Regen", "The base Endurance regen value."), ref settings.EnduranceRegen, 1);
				FloatField(new GUIContent("Locked Endurance Regen Mutliplier", "How fast should the locked Endurance bar regenerate? (Multipies the base regen, Combat multiplier will also be added to the calculation (Base x LE x CE))"), ref settings.LockedEnduranceRegenMutliplier, 1);
				FloatField(new GUIContent("Endurance Regen In Combat", "If the player is in combat how will the Endurance regen multiplied?"), ref settings.EnduranceRegenInCombat, 1);
				FloatField(new GUIContent("Endurance Regen Delay After Use", "After the player used Endurance how long will there be a cooldown for the main Endurance bar to regenerate. (In Seconds), (Small bars always regenerate)"), ref settings.EnduranceRegenDelayAfterUse, 1);

				GUILayout.Space(5);
				FloatField(new GUIContent("Jump Endurance Cost", "How much Endurance will be used from a jump?"), ref settings.JumpEnduranceCost, 1);
				FloatField(new GUIContent("Run Endurance Cost", "How much Endurance does running use? (Per Second)"), ref settings.RunEnduranceCost, 1);
			}
		}
		private void HUDSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FoldoutHeader("HUD Settings", ref HUDSettingsOpen);
			if (HUDSettingsOpen)
			{
				FloatField(new GUIContent("Stat Text Update Speed", "How fast should the Stat text display update its number? Depending on the current difference between actuall health and displayed health the update speed increases."), ref settings.StatTextUpdateSpeed);
				FloatField(new GUIContent("Endurance Bar Lerp Speed", "How fast should the Endurance bar(s) lerp to the actuall Endurance amount. (In 1/X Seconds)"), ref settings.EnduranceBarLerpSpeed);

				GUILayout.Space(5);
				GradientField(new GUIContent("Health Text Colors", "Gradient Start == No Health, Gradient End == Full Health"), ref settings.HealthTextColors);
				GradientField(new GUIContent("Mana Text Colors", "Gradient Start == No Mana, Gradient End == Full Mana"), ref settings.ManaTextColors);

				GUILayout.Space(5);
				ColorField("Reserved Endurance Bar Color", ref settings.ReserveEnduranceBarColor);
				ColorField("Active Endurance Bar Color", ref settings.ActiveEnduranceBarColor);
				ColorField("Reloading Endurance Bar Color", ref settings.ReloadingEnduranceBarColor);
			}
			EndFoldoutHeader();
		}
		private void DodgeSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FoldoutHeader("Dodge Settings", ref DodgeSettingsOpen);
			if (DodgeSettingsOpen)
			{
				FloatField(new GUIContent("Dodge Power", "How strong does the player dodge into the intended direction? (Value * Movespeed)"), ref settings.DodgePower);
				FloatField(new GUIContent("Dodge Up Force", "This will be added as the Vertical force. 0 Would mean high friction with the ground the player is standing on, therefore it is recommended to use a higher value."), ref settings.DodgeUpForce);
				FloatField(new GUIContent("Dodge Duration"), ref settings.DodgeDuration);
				FloatField(new GUIContent("Dodge Cooldown", "After finishing a Dodge, how long should it be on cooldown? (In Seconds)"), ref settings.DodgeCooldown);
				FloatField(new GUIContent("Dodge Endurance Cost", "How much Endurance does dodging drain?"), ref settings.DodgeEnduranceCost);
				ObjectField(new GUIContent("Dodge Model Material", "What material should be used for the Dodge Silhouette?"), ref settings.DodgeModelMaterial);
			}
			EndFoldoutHeader();
		}
	}
}