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
			EnduranceSettingsArea();
		}
		protected override void MovementSettingsArea()
		{
			base.MovementSettingsArea();

			PlayerSettings settings = target as PlayerSettings;

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			Vector3Field(new GUIContent("Jump Power", "With which velocity does this Entitie jump? (X == Sideways, Y == Upward, Z == Foreward)"), ref settings.JumpPower, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.JumpImpulsePower))), ref settings.JumpImpulsePower, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.JumpBackwardMultiplier))), ref settings.JumpBackwardMultiplier, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.JumpEnduranceCost))), ref settings.JumpEnduranceCost, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.RunEnduranceCost))), ref settings.RunEnduranceCost, 1);
		}
		private void EnduranceSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			FloatField(new GUIContent("Endurance", "What base Endurance does the Player have?"), ref settings.Endurance, 1);
			BoolField(new GUIContent("Do Endurance Regen", "Should the Player Regen Endurance over time?"), ref settings.DoEnduranceRegen, 1);

			if (settings.DoEnduranceRegen)
			{
				FloatField(new GUIContent("Endurance Regen", "How much Endurance should the Player regenerate? (Per Second)"), ref settings.EnduranceRegen, 2);
				FloatField(new GUIContent("Endurance Regen In Combat Multiplier", "In combat regeneration will be multiplied by this amount."), ref settings.EnduranceRegenInCombatMultiplier, 2);
				GUILayout.Space(3);

				FloatField(new GUIContent("Endurance After Use Cooldown", "After the Player uses Endurance how long will there be a Regen muliplier active (See below)? (Per Second)"), ref settings.EnduranceAfterUseCooldown, 2);
				FloatField(new GUIContent("Endurance Regen After Use Multiplier", "If the player recently used Endurance how will the Endurance regen multiplied? (Per Second)"), ref settings.EnduranceRegenAfterUseMultiplier, 2);
			}
		}
		private void DashSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DashPower))), ref settings.DashPower, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DashDuration))), ref settings.DashDuration, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DashModelExistTime))), ref settings.DashModelExistTime, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DashCooldown))), ref settings.DashCooldown, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DashEnduranceCost))), ref settings.DashEnduranceCost, 1);
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

			SerializedProperty startItemsArrayProperty = serializedObject.FindProperty(nameof(settings.StartItems));
			DrawArray<PlayerSettings.StartItem>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartItems))), ref settings.StartItems, startItemsArrayProperty, DrawStartItem, new GUIContent(". Start Item"), 1);
			IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.InventorySize))), ref settings.InventorySize, 1);
		}
		private void DrawStartItem(GUIContent content, PlayerSettings.StartItem settings, SerializedProperty property, int offSet)
		{
			if(settings == null)
			{
				isDirty = true;
				settings = new PlayerSettings.StartItem();
			}
			Foldout(content, property, offSet);

			if (property.isExpanded)
			{
				ObjectField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Item))), ref settings.Item, offSet + 1);
				IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ItemCount))), ref settings.ItemCount, offSet + 1);
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ForceEquip))), ref settings.ForceEquip, offSet + 1);
			}
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

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			Header("Player Look Turning");
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BodyTurnHorizontalClamp))), ref settings.BodyTurnHorizontalClamp, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BodyTurnWeight))), ref settings.BodyTurnWeight, 1);

			GUILayout.Space(4);
			Vector4Field(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.HeadLookAngleClamps))), ref settings.HeadLookAngleClamps, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.LookLerpSpeed))), ref settings.LookLerpSpeed, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.LookLerpSpringStiffness))), ref settings.LookLerpSpringStiffness, 1);
		}
		private void FXSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			Header("Generall");
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnCombatStart))), ref settings.EffectsOnCombatStart, serializedObject.FindProperty(nameof(settings.EffectsOnCombatStart)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnUltimateCharged))), ref settings.EffectsOnUltimateCharged, serializedObject.FindProperty(nameof(settings.EffectsOnUltimateCharged)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsWhileUltimateCharged))), ref settings.EffectsWhileUltimateCharged, serializedObject.FindProperty(nameof(settings.EffectsWhileUltimateCharged)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnLevelup))), ref settings.EffectsOnLevelup, serializedObject.FindProperty(nameof(settings.EffectsOnLevelup)), new GUIContent(". Effect"), 1);

			Header("On Player Attacking");
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnCauseDamage))), ref settings.EffectsOnCauseDamage, serializedObject.FindProperty(nameof(settings.EffectsOnCauseDamage)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnCauseCrit))), ref settings.EffectsOnCauseCrit, serializedObject.FindProperty(nameof(settings.EffectsOnCauseCrit)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnEnemyKilled))), ref settings.EffectsOnEnemyKilled, serializedObject.FindProperty(nameof(settings.EffectsOnEnemyKilled)), new GUIContent(". Effect"), 1);

			Header("On Player Movement");
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsWhileWalk))), ref settings.EffectsWhileWalk, serializedObject.FindProperty(nameof(settings.EffectsWhileWalk)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsWhileRun))), ref settings.EffectsWhileRun, serializedObject.FindProperty(nameof(settings.EffectsWhileRun)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnJump))), ref settings.EffectsOnJump, serializedObject.FindProperty(nameof(settings.EffectsOnJump)), new GUIContent(". Effect"), 1);
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
			Foldout(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsWhileHealthBelowThreshold))), ref OnHealthCriticalEffectsOpen, 1);
			if (OnHealthCriticalEffectsOpen)
			{
				FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsHealthThreshold))), ref settings.EffectsHealthThreshold, 0, 1, 2);
				ObjectArrayField<FXObject>(new GUIContent("Effects"), ref settings.EffectsWhileHealthBelowThreshold, serializedObject.FindProperty(nameof(settings.EffectsWhileHealthBelowThreshold)), new GUIContent(". Effect"), 2);
			}
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnPlayerDeath))), ref settings.EffectsOnPlayerDeath, serializedObject.FindProperty(nameof(settings.EffectsOnPlayerDeath)), new GUIContent(". Effect"), 1);
		}
	}
}