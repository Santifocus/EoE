using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(PlayerSettings), true), CanEditMultipleObjects]
	public class PlayerSettingsEditor : EntitieSettingsEditor
	{
		private static bool CameraSettingsOpen;
		private static bool EnduranceSettingsOpen;
		private static bool DodgeSettingsOpen;
		private static bool BlockingSettingsOpen;
		private static bool IFramesSettingsOpen;
		private static bool HUDSettingsOpen;
		private static bool InventorySettingsOpen;
		private static bool InventoryStartItemsOpen;

		//VFXEffectArrays
		private static bool ReceiveDamageOpen;
		private static bool ReceiveKnockbackOpen;
		private static bool CauseDamageOpen;
		private static bool CauseCritOpen;
		private static bool LevelUpOpen;
		private static bool OnPlayerLandingOpen;
		private static bool OnPlayerDodgeOpen;
		private static bool OnEnemyKilledOpen;
		private static bool OnPlayerDeathOpen;
		private static bool LowHealthThresholdOpen;

		protected override void CustomInspector()
		{
			base.CustomInspector();
			CameraSettingsArea();
			DodgeSettingsArea();
			BlockingSettingsArea();
			IFramesSettingsArea();
			HUDSettingsArea();
			InventorySettingsArea();
			FXSettingsArea();
		}

		private void CameraSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FoldoutHeader("Camera Settings", ref CameraSettingsOpen);
			if (CameraSettingsOpen)
			{
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
			EndFoldoutHeader();
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
			GUILayout.Space(2);
			Vector3Field(new GUIContent("Jump Power", "With which velocity does this Entitie jump? (X == Sideways, Y == Upward, Z == Foreward)"), ref settings.JumpPower);
			FloatField(new GUIContent("Jump Impulse Power"), ref settings.JumpImpulsePower);
		}
		private void EnduranceSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;
			LineBreak();

			FloatField(new GUIContent("Endurance", "What base Endurance does the Player have?"), ref settings.Endurance);
			BoolField(new GUIContent("Do Endurance Regen", "Should the Player Regen Endurance over time?"), ref settings.DoEnduranceRegen);

			if (settings.DoEnduranceRegen)
			{
				FloatField(new GUIContent("Endurance Regen", "How much Endurance should the Player regenerate? (Per Second)"), ref settings.EnduranceRegen, 1);
				FloatField(new GUIContent("Endurance Regen In Combat Multiplier", "In combat regeneration will be multiplied by this amount."), ref settings.EnduranceRegenInCombatMultiplier, 1);
				GUILayout.Space(3);

				FloatField(new GUIContent("Endurance After Use Cooldown", "After the Player uses Endurance how long will there be a Regen muliplier active (See below)? (Per Second)"), ref settings.EnduranceAfterUseCooldown, 1);
				FloatField(new GUIContent("Endurance Regen After Use Multiplier", "If the player recently used Endurance how will the Endurance regen multiplied? (Per Second)"), ref settings.EnduranceRegenAfterUseMultiplier, 1);
			}
		}
		private void DodgeSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FoldoutHeader("Dodge Settings", ref DodgeSettingsOpen);
			if (DodgeSettingsOpen)
			{
				FloatField(new GUIContent("Dodge Power", "How strong does the player dodge into the intended direction? (Value * Movespeed)"), ref settings.DodgePower, 1);
				FloatField(new GUIContent("Dodge Duration"), ref settings.DodgeDuration, 1);
				FloatField(new GUIContent("Dodge Model Exist Time", "The Silhouette that appears after dodging will exist for this amount of time."), ref settings.DodgeModelExistTime, 1);
				FloatField(new GUIContent("Dodge Cooldown", "After finishing a Dodge, how long should it be on cooldown? (In Seconds)"), ref settings.DodgeCooldown, 1);
				FloatField(new GUIContent("Dodge Endurance Cost", "How much Endurance does dodging drain?"), ref settings.DodgeEnduranceCost, 1);
				ObjectField(new GUIContent("Dodge Model Material", "What material should be used for the Dodge Silhouette?"), ref settings.DodgeModelMaterial, 1);
			}
			EndFoldoutHeader();
		}
		private void BlockingSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FoldoutHeader("Blocking Settings", ref BlockingSettingsOpen);
			if (BlockingSettingsOpen)
			{
				FloatField(new GUIContent("Start Blocking Inertia"), ref settings.StartBlockingInertia, 1);
				ObjectField<Buff>(new GUIContent("Blocking Buff"), ref settings.BlockingBuff, 1);
				FloatField(new GUIContent("Stop Blocking Inertia"), ref settings.StopBlockingInertia, 1);
			}
			EndFoldoutHeader();
		}
		private void IFramesSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FoldoutHeader("IFrames Settings", ref IFramesSettingsOpen);
			if (IFramesSettingsOpen)
			{
				BoolField(new GUIContent("Invincible After Hit", "Should the player be invincible for a set time after getting damaged?"), ref settings.InvincibleAfterHit, 1);
				if (settings.InvincibleAfterHit)
				{
					FloatField(new GUIContent("Invincible After Hit Time", "How long should this invincibility last?."), ref settings.InvincibleAfterHitTime, 2);
					ColorField(new GUIContent("Invincible Model Flash Color", "In which color should the player model flash while he is invincible?"), ref settings.InvincibleModelFlashColor, 2);
					FloatField(new GUIContent("Invincible Model Flash Delay", "How long of a delay between each colored model flash?"), ref settings.InvincibleModelFlashDelay, 2);
					FloatField(new GUIContent("Invincible Model Flash Time", "How long does a model color flash last?"), ref settings.InvincibleModelFlashTime, 2);
				}
			}
			EndFoldoutHeader();
		}
		private void HUDSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FoldoutHeader("HUD Settings", ref HUDSettingsOpen);
			if (HUDSettingsOpen)
			{
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
			EndFoldoutHeader();
		}
		private void InventorySettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FoldoutHeader("Inventory Settings", ref InventorySettingsOpen);
			if (InventorySettingsOpen)
			{
				DrawArray(new GUIContent("Start Items"), DrawStartItem, ref settings.StartItems, ref InventoryStartItemsOpen, 1);
				IntField("Use Inventory Size", ref settings.InventorySize, 1);
			}
			EndFoldoutHeader();
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
		private void FXSettingsArea()
		{
			PlayerSettings settings = target as PlayerSettings;

			FoldoutHeader("FX Settings", ref VFXSettingsOpen);
			if (VFXSettingsOpen)
			{
				ObjectArrayField(new GUIContent("Effects On Receive Damage"), ref settings.EffectsOnReceiveDamage, ref ReceiveDamageOpen, new GUIContent("Effect "), 1);
				ObjectArrayField(new GUIContent("Effects On Receive Knockback"), ref settings.EffectsOnReceiveKnockback, ref ReceiveKnockbackOpen, new GUIContent("Effect "), 1);
				ObjectArrayField(new GUIContent("Effects On Cause Damage"), ref settings.EffectsOnCauseDamage, ref CauseDamageOpen, new GUIContent("Effect "), 1);
				ObjectArrayField(new GUIContent("Effects On Cause Crit"), ref settings.EffectsOnCauseCrit, ref CauseCritOpen, new GUIContent("Effect "), 1);
				ObjectArrayField(new GUIContent("Effects On Levelup"), ref settings.EffectsOnLevelup, ref LevelUpOpen, new GUIContent("Effect "), 1);

				GUILayout.Space(4);
				bool changedFloat = FloatField(new GUIContent("Effects Health Threshold", "The effects in the 'Effects On Damage When Below Threshold' will only be played when the player is below this (Health / MaxHealth) Threshold. (0 - 1)"), ref settings.EffectsHealthThreshold, 1);
				if (changedFloat)
				{
					settings.EffectsHealthThreshold = Mathf.Clamp01(settings.EffectsHealthThreshold);
				}
				ObjectArrayField(new GUIContent("Effects On Damage When Below Threshold"), ref settings.EffectsOnDamageWhenBelowThreshold, ref LowHealthThresholdOpen, new GUIContent("Effect "), 1);

				GUILayout.Space(4);
				changedFloat = FloatField(new GUIContent("Player Landing Velocity Threshold", "The player must land with this or a higher velocity to trigger the on land effects."), ref settings.PlayerLandingVelocityThreshold, 1);
				if (changedFloat)
				{
					settings.EffectsHealthThreshold = Mathf.Clamp01(settings.EffectsHealthThreshold);
				}
				ObjectArrayField(new GUIContent("Effects On Player Landing"), ref settings.EffectsOnPlayerLanding, ref OnPlayerLandingOpen, new GUIContent("Effect "), 1);

				GUILayout.Space(4);
				ObjectArrayField(new GUIContent("Effects On Player Dodge"), ref settings.EffectsOnPlayerDodge, ref OnPlayerDodgeOpen, new GUIContent("Effect "), 1);
				ObjectArrayField(new GUIContent("Effects On Enemy Killed"), ref settings.EffectsOnEnemyKilled, ref OnEnemyKilledOpen, new GUIContent("Effect "), 1);
				ObjectArrayField(new GUIContent("Effects On Player Death"), ref settings.EffectsOnPlayerDeath, ref OnPlayerDeathOpen, new GUIContent("Effect "), 1);
			}
			EndFoldoutHeader();
		}
	}
}