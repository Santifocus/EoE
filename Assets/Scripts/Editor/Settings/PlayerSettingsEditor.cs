using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(PlayerSettings), true), CanEditMultipleObjects]
	public class PlayerSettingsEditor : EntitieSettingsEditor
	{
		private static bool CameraSettingsOpen;
		private static bool DodgeSettingsOpen;
		private static bool BlockingSettingsOpen;
		private static bool IFramesSettingsOpen;
		private static bool HUDSettingsOpen;
		private static bool InventorySettingsOpen;
		private static bool AnimationSettingsOpen;

		private static bool InventoryStartItemsOpen;
		//VFXEffectArrays
		private static bool OnLandingEffectsOpen;
		private static bool OnHealthCriticalEffectsOpen;

		protected override void CustomInspector()
		{
			base.CustomInspector();
			DrawInFoldoutHeader(new GUIContent("Camera Settings"), ref CameraSettingsOpen, CameraSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Dodge Settings"), ref DodgeSettingsOpen, DodgeSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Blocking Settings"), ref BlockingSettingsOpen, BlockingSettingsArea);
			DrawInFoldoutHeader(new GUIContent("IFrames Settings"), ref IFramesSettingsOpen, IFramesSettingsArea);
			DrawInFoldoutHeader(new GUIContent("HUD Settings"), ref HUDSettingsOpen, HUDSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Inventory Settings"), ref InventorySettingsOpen, InventorySettingsArea);
			DrawInFoldoutHeader(new GUIContent("Animation Settings"), ref AnimationSettingsOpen, AnimationSettingsArea);
			DrawInFoldoutHeader(new GUIContent("FX Settings"), ref VFXSettingsOpen, FXSettingsArea);
		}

		private void CameraSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FloatField(new GUIContent("Camera to Player Distance", "How far from the Player should the camera be?"), ref settings.CameraToPlayerDistance, 1);
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
			FloatField(new GUIContent("Jump Impulse Power"), ref settings.JumpImpulsePower, 1);
			FloatField(new GUIContent("Jump Backward Multiplier"), ref settings.JumpBackwardMultiplier, 1);
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
		private void DodgeSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FloatField(new GUIContent("Dodge Power", "How strong does the player dodge into the intended direction? (Value * Movespeed)"), ref settings.DodgePower, 1);
			FloatField(new GUIContent("Dodge Duration"), ref settings.DodgeDuration, 1);
			FloatField(new GUIContent("Dodge Model Exist Time", "The Silhouette that appears after dodging will exist for this amount of time."), ref settings.DodgeModelExistTime, 1);
			FloatField(new GUIContent("Dodge Cooldown", "After finishing a Dodge, how long should it be on cooldown? (In Seconds)"), ref settings.DodgeCooldown, 1);
			FloatField(new GUIContent("Dodge Endurance Cost", "How much Endurance does dodging drain?"), ref settings.DodgeEnduranceCost, 1);
			ObjectField(new GUIContent("Dodge Model Material", "What material should be used for the Dodge Silhouette?"), ref settings.DodgeModelMaterial, 1);
		}
		private void BlockingSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FloatField(new GUIContent("Start Blocking Inertia"), ref settings.StartBlockingInertia, 1);
			ObjectField<Buff>(new GUIContent("Blocking Buff"), ref settings.BlockingBuff, 1);
			FloatField(new GUIContent("Stop Blocking Inertia"), ref settings.StopBlockingInertia, 1);
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
		private void HUDSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FloatField(new GUIContent("Stat Text Update Speed", "How fast should the Stat text display update its number? Depending on the current difference between actuall health and displayed health the update speed increases."), ref settings.StatTextUpdateSpeed, 1);
			FloatField(new GUIContent("Endurance Bar Lerp Speed", "How fast should the Endurance bar(s) lerp to the actuall Endurance amount. (In 1/X Seconds)"), ref settings.EnduranceBarLerpSpeed, 1);

			GUILayout.Space(5);
			GradientField(new GUIContent("Health Text Colors", "Gradient Start == No Health, Gradient End == Full Health"), ref settings.HealthTextColors, 1);
			GradientField(new GUIContent("Mana Text Colors", "Gradient Start == No Mana, Gradient End == Full Mana"), ref settings.ManaTextColors, 1);

			GUILayout.Space(5);
			ColorField("Reserved Endurance Bar Color", ref settings.ReserveEnduranceBarColor, 1);
			ColorField("Active Endurance Bar Color", ref settings.ActiveEnduranceBarColor, 1);
			ColorField("Reloading Endurance Bar Color", ref settings.ReloadingEnduranceBarColor, 1);
		}
		private void InventorySettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			DrawArray(new GUIContent("Start Items"), DrawStartItem, ref settings.StartItems, ref InventoryStartItemsOpen, 1);
			IntField("Use Inventory Size", ref settings.InventorySize, 1);
		}
		private bool DrawStartItem(int index, int offset, PlayerSettings.StartItem[] array)
		{
			bool changed = false;
			if (array[index] == null)
				array[index] = new PlayerSettings.StartItem();

			changed |= ObjectField(new GUIContent("Start Item " + (index + 1)), ref array[index].Item, offset);
			changed |= IntField(new GUIContent("Item Count"), ref array[index].ItemCount, offset);
			changed |= BoolField(new GUIContent("Force Equip"), ref array[index].ForceEquip, offset);
			return changed;
		}
		private void AnimationSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FloatField(new GUIContent("Max Side Turn"), ref settings.MaxModelTilt, 1);
			FloatField(new GUIContent("Side Turn Lerp Speed"), ref settings.SideTurnLerpSpeed, 1);
			FloatField(new GUIContent("Spring Lerp Stiffness"), ref settings.SpringLerpStiffness, 1);
		}
		private void FXSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			Header("On Player Attacking");
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnCauseDamage))), ref settings.EffectsOnCauseDamage, serializedObject.FindProperty(nameof(settings.EffectsOnCauseDamage)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnCauseCrit))), ref settings.EffectsOnCauseCrit, serializedObject.FindProperty(nameof(settings.EffectsOnCauseCrit)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnEnemyKilled))), ref settings.EffectsOnEnemyKilled, serializedObject.FindProperty(nameof(settings.EffectsOnEnemyKilled)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnLevelup))), ref settings.EffectsOnLevelup, serializedObject.FindProperty(nameof(settings.EffectsOnLevelup)), new GUIContent(". Effect"), 1);

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
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnPlayerDodge))), ref settings.EffectsOnPlayerDodge, serializedObject.FindProperty(nameof(settings.EffectsOnPlayerDodge)), new GUIContent(". Effect"), 1);

			Header("On Player Receiving Damage");
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnReceiveDamage))), ref settings.EffectsOnReceiveDamage, serializedObject.FindProperty(nameof(settings.EffectsOnReceiveDamage)), new GUIContent(". Effect"), 1);
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnReceiveKnockback))), ref settings.EffectsOnReceiveKnockback, serializedObject.FindProperty(nameof(settings.EffectsOnReceiveKnockback)), new GUIContent(". Effect"), 1);
			Foldout(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsWhileHealthBelowThreshold))), ref OnHealthCriticalEffectsOpen, 1);
			if (OnHealthCriticalEffectsOpen)
			{
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsHealthThreshold))), ref settings.EffectsHealthThreshold, 2);
				ObjectArrayField<FXObject>(new GUIContent("Effects"), ref settings.EffectsWhileHealthBelowThreshold, serializedObject.FindProperty(nameof(settings.EffectsWhileHealthBelowThreshold)), new GUIContent(". Effect"), 2);
			}
			ObjectArrayField<FXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectsOnPlayerDeath))), ref settings.EffectsOnPlayerDeath, serializedObject.FindProperty(nameof(settings.EffectsOnPlayerDeath)), new GUIContent(". Effect"), 1);
		}
	}
}