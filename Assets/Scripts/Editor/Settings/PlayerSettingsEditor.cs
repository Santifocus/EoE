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
		private static bool VFXSettingsOpen;
		protected override void CustomInspector()
		{
			base.CustomInspector();
			CameraSettingsArea();
			DodgeSettingsArea();
			HUDSettingsArea();
			VFXSettingsArea();
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
		private void DodgeSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FoldoutHeader("Dodge Settings", ref DodgeSettingsOpen);
			if (DodgeSettingsOpen)
			{
				FloatField(new GUIContent("Dodge Power", "How strong does the player dodge into the intended direction? (Value * Movespeed)"), ref settings.DodgePower);
				FloatField(new GUIContent("Dodge Duration"), ref settings.DodgeDuration);
				FloatField(new GUIContent("Dodge Model Exist Time", "The Silhouette that appears after dodging will exist for this amount of time."), ref settings.DodgeModelExistTime);
				FloatField(new GUIContent("Dodge Cooldown", "After finishing a Dodge, how long should it be on cooldown? (In Seconds)"), ref settings.DodgeCooldown);
				FloatField(new GUIContent("Dodge Endurance Cost", "How much Endurance does dodging drain?"), ref settings.DodgeEnduranceCost);
				ObjectField(new GUIContent("Dodge Model Material", "What material should be used for the Dodge Silhouette?"), ref settings.DodgeModelMaterial);
			}
			EndFoldoutHeader();
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
		private void VFXSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FoldoutHeader("VFX Settings", ref VFXSettingsOpen);
			if (VFXSettingsOpen)
			{
				BoolField(new GUIContent("Color Screen On Damage", "When the player get hit, should there be a color screen feedback?"), ref settings.ColorScreenOnDamage);
				if (settings.ColorScreenOnDamage)
				{
					ColorField(new GUIContent("Color Screen Color", "The Color of the Screen coloring"), ref settings.ColorScreenColor, 1);
					FloatField(new GUIContent("Color Screen Depth", "How far should the color go into the screen?"), ref settings.ColorScreenDepth, 1);
					FloatField(new GUIContent("Color Screen Duration", "How long should the screen coloring stay?"), ref settings.ColorScreenDuration, 1);
				}

				GUILayout.Space(6);
				BoolField(new GUIContent("Shake Screen On Knockback", "When the player experiences an impact should the screen shake?"), ref settings.ShakeScreenOnKnockback);
				if (settings.ShakeScreenOnKnockback)
				{
					FloatField(new GUIContent("Shake Time On Knockback", "How long should the screen shake?"), ref settings.ShakeTimeOnKnockback, 1);
					FloatField(new GUIContent("Shake Screen Axis Intensity", "XYZ Position shake multiplier"), ref settings.ShakeScreenAxisIntensity, 1);
					FloatField(new GUIContent("Shake Screen Angle Intensity", "XYZ Rotation shake multiplier"), ref settings.ShakeScreenAngleIntensity, 1);
				}

				GUILayout.Space(6);
				BoolField(new GUIContent("Blur Screen On Damage", "When the player is below a health threshold, should there be a Blur on the sight?"), ref settings.BlurScreenOnDamage);
				if (settings.BlurScreenOnDamage)
				{
					FloatField(new GUIContent("Blur Screen Health Threshold", "If the player is below this threshold (0 - 1) the blur effect will be caused."), ref settings.BlurScreenHealthThreshold, 1);
					FloatField(new GUIContent("Blur Screen Base Intensity", "How strong should the intensity be? (0 - 1), Will be multiplied by the caused damage and how much health is currently left."), ref settings.BlurScreenBaseIntensity, 1);
					FloatField(new GUIContent("Blur Screen Duration", "How long should the blur stay? Will be multiplied the same as the intensity"), ref settings.BlurScreenDuration, 1);
				}

				GUILayout.Space(6);
				BoolField(new GUIContent("Slow On Critical Hit", "When the player hits a critical strike should there be a time dilution? (Slowdown or Speedup)"), ref settings.SlowOnCriticalHit);
				if (settings.SlowOnCriticalHit)
				{
					FloatField(new GUIContent("Slow On Crit Time In", "How long should the slowdown take until it is at its maximum?"), ref settings.SlowOnCritTimeIn, 1);
					FloatField(new GUIContent("Slow On Crit Time Stay", "How long should the slowdown stay?"), ref settings.SlowOnCritTimeStay, 1);
					FloatField(new GUIContent("Slow On Crit Time Out", "How long should the slowdown take until it has faded away?"), ref settings.SlowOnCritTimeOut, 1);
					FloatField(new GUIContent("Slow On Crit Scale", "What is the maximum slow (This is the time scale while Stay time is active)"), ref settings.SlowOnCritScale, 1);
				}

				GUILayout.Space(6);
				BoolField(new GUIContent("Screen Shake On Crit"), ref settings.ScreenShakeOnCrit);
				if (settings.ScreenShakeOnCrit)
				{
					FloatField(new GUIContent("Shake Time On Crit", "How long should the screen shake?"), ref settings.ShakeTimeOnCrit, 1);
					FloatField(new GUIContent("On Crit Shake Axis Intensity", "XYZ Position shake multiplier"), ref settings.OnCritShakeAxisIntensity, 1);
					FloatField(new GUIContent("On Crit Shake Angle Intensity", "XYZ Rotation shake multiplier"), ref settings.OnCritShakeAngleIntensity, 1);
				}

				GUILayout.Space(6);
				BoolField(new GUIContent("Rumble On Crit"), ref settings.RumbleOnCrit);
				if (settings.ScreenShakeOnCrit)
				{
					FloatField(new GUIContent("Rumble On Crit Time", "How long should the controller rumble? (The intensity will be Interpolated between Star->End intensity)"), ref settings.RumbleOnCritTime, 1);
					FloatField(new GUIContent("Rumble On Crit Left Intensity Start", "The start rumble intensity of the left motor. (Left motor is Low Frequency therefore it is recommended to use about 1/3 of the right motor as intensity)"), ref settings.RumbleOnCritLeftIntensityStart, 1);
					FloatField(new GUIContent("Rumble On Crit Right Intensity Start", "The start rumble intensity of the right motor. (Left motor is High Frequency therefore it is recommended to use about 3 time the intensity of the left motor)"), ref settings.RumbleOnCritRightIntensityStart, 1);
					FloatField(new GUIContent("Rumble On Crit Left Intensity End", "The end rumble intensity of the left motor."), ref settings.RumbleOnCritLeftIntensityEnd, 1);
					FloatField(new GUIContent("Rumble On Crit Right Intensity End", "The end rumble intensity of the right motor."), ref settings.RumbleOnCritRightIntensityEnd, 1);
				}
			}
			EndFoldoutHeader();
		}
	}
}