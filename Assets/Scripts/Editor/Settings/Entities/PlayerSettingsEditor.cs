﻿using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(PlayerSettings), true), CanEditMultipleObjects]
	public class PlayerSettingsEditor : EntitySettingsEditor
	{
		private static bool CameraSettingsOpen;
		private static bool DashSettingsOpen;
		private static bool ShieldingSettingsOpen;
		private static bool IFramesSettingsOpen;
		private static bool InventorySettingsOpen;
		private static bool AnimationSettingsOpen;

		//VFXEffectArrays
		private static bool OnStaminaEmptyEffectsOpen;
		private static bool OnDecelerateEffectsOpen;
		private static bool OnLandingEffectsOpen;
		private static bool OnHealthCriticalEffectsOpen;

		protected override void CustomInspector()
		{
			base.CustomInspector();
			DrawInFoldoutHeader(new GUIContent("Camera Settings"), ref CameraSettingsOpen, CameraSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Dash Settings"), ref DashSettingsOpen, DashSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Shielding Settings"), ref ShieldingSettingsOpen, ShieldingSettingsArea);
			DrawInFoldoutHeader(new GUIContent("IFrames Settings"), ref IFramesSettingsOpen, IFramesSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Inventory Settings"), ref InventorySettingsOpen, InventorySettingsArea);
			DrawInFoldoutHeader(new GUIContent("Animation Settings"), ref AnimationSettingsOpen, AnimationSettingsArea);
			DrawInFoldoutHeader(new GUIContent("FX Settings"), ref EffectSettingsOpen, FXSettingsArea);
		}
		protected override void CombatSettings()
		{
			base.CombatSettings();
			PlayerSettings settings = target as PlayerSettings;
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.FirstStrikeDamageMultiplier))), ref settings.FirstStrikeDamageMultiplier, 1);
		}
		private void CameraSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FloatField(new GUIContent("Camera to Player Distance", "How far from the Player should the camera be?"), ref settings.CameraToPlayerDistance, 1);
			FloatField(new GUIContent("Camera Y Lerp Speed"), ref settings.CameraYLerpSpeed, 1);
			FloatField(new GUIContent("Camera Y Lerp Sring Stiffness"), ref settings.CameraYLerpSringStiffness, 1);
			Vector3Field(new GUIContent("Camera Anchor Offset", "The anchor off the camera will be offset by this amount."), ref settings.CameraAnchorOffset, 1);
			Vector3Field(new GUIContent("Camera Anchor Offset When Targeting", "The anchor off the camera will be offset by this amount when the player is targeting something."), ref settings.CameraAnchorOffsetWhenTargeting, 1);
			Vector2Field(new GUIContent("Camera Rotation Power", "The amount of rotation that will be added when the player tries to rotate the camera."), ref settings.CameraRotationPower, 1);
			FloatField(new GUIContent("Camera Rotation Speed", "How fast should the added rotation be interpolated. Higher value means less smooth, lower means that even after multiple seconds the camera might still slightly move."), ref settings.CameraRotationSpeed, 1);
			Vector2Field(new GUIContent("Camera Vertical Angle Clamps", "How far around the X Axis can the player rotate the camera. (Up / Down)"), ref settings.CameraVerticalAngleClamps, 1);
			FloatField(new GUIContent("Camera Extra Zoom On Vertical", "When the camera look down on the player the zoom will be multiplied by '1 + value'."), ref settings.CameraExtraZoomOnVertical, 1);
			FloatField(new GUIContent("Max Enemy Targeting Distance", "When the player tries to target an Entitie, how far from the Player can that Entitie be at max?"), ref settings.MaxEnemyTargetingDistance, 1);
			Vector2Field(new GUIContent("Camera Clamps When Targeting", "How far around the X Axis can the player rotate the camera when the player is targeting a Entitie. (Up / Down)"), ref settings.CameraClampsWhenTargeting, 1);
		}
		protected override void StatSettingsArea()
		{
			base.StatSettingsArea();
			StaminaSettingsArea();
		}
		protected override void MovementSettingsArea()
		{
			base.MovementSettingsArea();

			PlayerSettings settings = target as PlayerSettings;

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			Vector3Field(new GUIContent("Jump Power", "With which velocity does this Entitie jump? (X == Sideways, Y == Upward, Z == Foreward)"), ref settings.JumpPower, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.JumpImpulsePower))), ref settings.JumpImpulsePower, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.JumpBackwardMultiplier))), ref settings.JumpBackwardMultiplier, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.JumpStaminaCost))), ref settings.JumpStaminaCost, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.RunStaminaCost))), ref settings.RunStaminaCost, 1);
		}
		private void StaminaSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Stamina))), ref settings.Stamina, 1);
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DoStaminaRegen))), ref settings.DoStaminaRegen, 1);

			if (settings.DoStaminaRegen)
			{
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StaminaRegen))), ref settings.StaminaRegen, 2);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StaminaRegenInCombatMultiplier))), ref settings.StaminaRegenInCombatMultiplier, 2);
				GUILayout.Space(3);

				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StaminaAfterUseCooldown))), ref settings.StaminaAfterUseCooldown, 2);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StaminaRegenAfterUseMultiplier))), ref settings.StaminaRegenAfterUseMultiplier, 2);
			}
		}
		private void DashSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DashPower))), ref settings.DashPower, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DashDuration))), ref settings.DashDuration, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DashModelExistTime))), ref settings.DashModelExistTime, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DashCooldown))), ref settings.DashCooldown, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DashStaminaCost))), ref settings.DashStaminaCost, 1);
			ObjectField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DashModelMaterial))), ref settings.DashModelMaterial, 1);
		}
		private void ShieldingSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;
			ObjectField<ShieldData>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ShieldSettings))), ref settings.ShieldSettings, 1);
		}
		private void IFramesSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			BoolField(new GUIContent("Invincible After Hit", "Should the player be invincible for a set time after getting damaged?"), ref settings.InvincibleAfterHit, 1);
			if (settings.InvincibleAfterHit)
			{
				FloatField(new GUIContent("Invincible After Hit Time", "How long should this invincibility last?."), ref settings.InvincibleAfterHitTime, 2);
				ColorField(new GUIContent("Invincible Model Flash Color", "In which color should the player model flash while he is invincible?"), ref settings.InvincibleModelFlashColor, 2);
				FloatField(new GUIContent("Invincible Model Flash Delay", "How long of a delay between each colored model flash?"), ref settings.InvincibleModelFlashDelay, 2);
				FloatField(new GUIContent("Invincible Model Flash Time", "How long does a model color flash last?"), ref settings.InvincibleModelFlashTime, 2);
			}
		}
		private void InventorySettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;
			IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.InventorySize))), ref settings.InventorySize, 1);
		}
		private void AnimationSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			Header("Player Velocity Turning");
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MaxModelTilt))), ref settings.MaxModelTilt, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.SideTurnSpringLerpSpeed))), ref settings.SideTurnSpringLerpSpeed, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.SideTurnLerpSpringStiffness))), ref settings.SideTurnLerpSpringStiffness, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AnimationWalkSpeedDivider))), ref settings.AnimationWalkSpeedDivider, 1);
		}
		private void FXSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			Header("Generall");
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnPlayerSpawn))), ref settings.EffectsOnPlayerSpawn, serializedObject.FindProperty(nameof(settings.EffectsOnPlayerSpawn)), new GUIContent(". Effect"), 1);

			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnCombatStart))), ref settings.EffectsOnCombatStart, serializedObject.FindProperty(nameof(settings.EffectsOnCombatStart)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnUltimateCharged))), ref settings.EffectsOnUltimateCharged, serializedObject.FindProperty(nameof(settings.EffectsOnUltimateCharged)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsWhileUltimateCharged))), ref settings.EffectsWhileUltimateCharged, serializedObject.FindProperty(nameof(settings.EffectsWhileUltimateCharged)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnLevelup))), ref settings.EffectsOnLevelup, serializedObject.FindProperty(nameof(settings.EffectsOnLevelup)), new GUIContent(". Effect"), 1);

			DrawArray<ChanceBasedFXGroup>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnCombatStartChanceBased))), ref settings.EffectsOnCombatStartChanceBased, serializedObject.FindProperty(nameof(settings.EffectsOnCombatStartChanceBased)), DrawChanceBasedFXGroup, new GUIContent(". Group"), 1);

			Header("Action Based");
			//DecelerationEffects
			Foldout(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnStaminaMissing))), ref OnStaminaEmptyEffectsOpen, 1);
			if (OnStaminaEmptyEffectsOpen)
			{
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnStaminaMissingCooldown))), ref settings.EffectsOnStaminaMissingCooldown, 2);
				ObjectArrayField<FXObject>(new GUIContent("Effects"), ref settings.EffectsOnStaminaMissing, serializedObject.FindProperty(nameof(settings.EffectsOnStaminaMissing)), new GUIContent(". Effect"), 2);
			}

			Header("On Player Attacking");
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnCauseDamage))), ref settings.EffectsOnCauseDamage, serializedObject.FindProperty(nameof(settings.EffectsOnCauseDamage)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnFirstStrike))), ref settings.EffectsOnFirstStrike, serializedObject.FindProperty(nameof(settings.EffectsOnFirstStrike)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnCauseCrit))), ref settings.EffectsOnCauseCrit, serializedObject.FindProperty(nameof(settings.EffectsOnCauseCrit)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnEnemyKilled))), ref settings.EffectsOnEnemyKilled, serializedObject.FindProperty(nameof(settings.EffectsOnEnemyKilled)), new GUIContent(". Effect"), 1);

			Header("On Player Movement");
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsWhileWalk))), ref settings.EffectsWhileWalk, serializedObject.FindProperty(nameof(settings.EffectsWhileWalk)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsWhileRun))), ref settings.EffectsWhileRun, serializedObject.FindProperty(nameof(settings.EffectsWhileRun)), new GUIContent(". Effect"), 1);

			//DecelerationEffects
			Foldout(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsWhileDecelerating))), ref OnDecelerateEffectsOpen, 1);
			if (OnDecelerateEffectsOpen)
			{
				FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AccelerationThreshold))), ref settings.AccelerationThreshold, 0, 1, 2);
				ObjectArrayField<FXObject>(new GUIContent("Effects"), ref settings.EffectsWhileDecelerating, serializedObject.FindProperty(nameof(settings.EffectsWhileDecelerating)), new GUIContent(". Effect"), 2);
			}

			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnJump))), ref settings.EffectsOnJump, serializedObject.FindProperty(nameof(settings.EffectsOnJump)), new GUIContent(". Effect"), 1);

			//Land Effects
			Foldout(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnPlayerLanding))), ref OnLandingEffectsOpen, 1);
			if (OnLandingEffectsOpen)
			{
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.PlayerLandingVelocityThreshold))), ref settings.PlayerLandingVelocityThreshold, 2);
				ObjectArrayField<FXObject>(new GUIContent("Effects"), ref settings.EffectsOnPlayerLanding, serializedObject.FindProperty(nameof(settings.EffectsOnPlayerLanding)), new GUIContent(". Effect"), 2);
			}
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnPlayerDash))), ref settings.EffectsOnPlayerDash, serializedObject.FindProperty(nameof(settings.EffectsOnPlayerDash)), new GUIContent(". Effect"), 1);

			Header("On Player Receiving Damage");
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnReceiveDamage))), ref settings.EffectsOnReceiveDamage, serializedObject.FindProperty(nameof(settings.EffectsOnReceiveDamage)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnReceiveKnockback))), ref settings.EffectsOnReceiveKnockback, serializedObject.FindProperty(nameof(settings.EffectsOnReceiveKnockback)), new GUIContent(". Effect"), 1);

			//Health below threshold Effects
			Foldout(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsWhileHealthBelowThreshold))), ref OnHealthCriticalEffectsOpen, 1);
			if (OnHealthCriticalEffectsOpen)
			{
				FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsHealthThreshold))), ref settings.EffectsHealthThreshold, 0, 1, 2);
				ObjectArrayField<FXObject>(new GUIContent("Effects"), ref settings.EffectsWhileHealthBelowThreshold, serializedObject.FindProperty(nameof(settings.EffectsWhileHealthBelowThreshold)), new GUIContent(". Effect"), 2);
			}

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DeathEffects))), ref settings.DeathEffects, serializedObject.FindProperty(nameof(settings.DeathEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
		}
		private void DrawChanceBasedFXGroup(GUIContent content, ChanceBasedFXGroup settings, SerializedProperty property, int offSet)
		{
			if (settings == null)
				return;

			DrawArray<ChanceBasedFX>(content, ref settings.Group, property.FindPropertyRelative(nameof(settings.Group)), DrawChanceBasedFX, new GUIContent(". Chance Based FX"), offSet + 1);
		}
		private void DrawChanceBasedFX(GUIContent content, ChanceBasedFX settings, SerializedProperty property, int offSet)
		{
			if (settings == null)
				return;

			Foldout(content, property, offSet);

			if (property.isExpanded)
			{
				ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effects))), ref settings.Effects, property.FindPropertyRelative(nameof(settings.Effects)), new GUIContent(". Effect"), offSet + 1);
				IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.GroupRelativeChance))), ref settings.GroupRelativeChance, offSet + 1);
			}
		}
	}
}