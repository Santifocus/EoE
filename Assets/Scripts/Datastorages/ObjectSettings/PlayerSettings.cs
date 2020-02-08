using EoE.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class PlayerSettings : EntitySettings
	{
		//Camera Settings
		public float CameraToPlayerDistance = 7;
		public float CameraYLerpSpeed = 5;
		public float CameraYLerpSringStiffness = 0.2f;
		public Vector3 CameraAnchorOffset = new Vector3(0, 1.6f, 0);
		public Vector3 CameraAnchorOffsetWhenTargeting = new Vector3(0, 1.6f, 0);
		public Vector2 CameraRotationPower = new Vector2(200, 75);
		public float CameraRotationSpeed = 10;
		public Vector2 CameraVerticalAngleClamps = new Vector2(-50, 50);
		public float CameraExtraZoomOnVertical = 0.5f;
		public float MaxEnemyTargetingDistance = 100;
		public Vector2 CameraClampsWhenTargeting = new Vector2(-30, 30);
		public float CameraBaseFOV = 60;

		//Stamina Settings
		[UnityEngine.Serialization.FormerlySerializedAs("Endurance")] public float Stamina = 100;
		[UnityEngine.Serialization.FormerlySerializedAs("DoEnduranceRegen")] public bool DoStaminaRegen = true;
		[UnityEngine.Serialization.FormerlySerializedAs("EnduranceRegen")] public float StaminaRegen = 6;
		[UnityEngine.Serialization.FormerlySerializedAs("EnduranceRegenInCombatMultiplier")] public float StaminaRegenInCombatMultiplier = 0.5f;
		[UnityEngine.Serialization.FormerlySerializedAs("EnduranceAfterUseCooldown")] public float StaminaAfterUseCooldown = 3;
		[UnityEngine.Serialization.FormerlySerializedAs("EnduranceRegenAfterUseMultiplier")] public float StaminaRegenAfterUseMultiplier = 0.5f;

		//Stamina Costs
		[UnityEngine.Serialization.FormerlySerializedAs("JumpEnduranceCost")] public float JumpStaminaCost = 4;
		[UnityEngine.Serialization.FormerlySerializedAs("RunEnduranceCost")] public float RunStaminaCost = 3;
		[UnityEngine.Serialization.FormerlySerializedAs("DashEnduranceCost")] public float DashStaminaCost = 20;

		//Jumping
		public Vector3 JumpPower = new Vector3(0, 10, 0);
		public float JumpImpulsePower = 2.5f;
		public float JumpBackwardMultiplier = 0.65f;

		//Dashing
		public float DashPower = 1;
		public float DashDuration = 0.2f;
		public float DashModelExistTime = 0.4f;
		public float DashCooldown = 0.5f;
		public Material DashModelMaterial = null;

		//Shielding
		public ShieldData ShieldSettings = default;

		//IFrames
		public bool InvincibleAfterHit = true;
		public float InvincibleAfterHitTime = 0.3f;
		public Color InvincibleModelFlashColor = Color.white;
		public float InvincibleModelFlashDelay = 0.4f;
		public float InvincibleModelFlashTime = 0.4f;

		//Inventory
		public int InventorySize = 24;

		//Animation
		public float MaxModelTilt = 10;
		public float SideTurnSpringLerpSpeed = 13;
		public float SideTurnLerpSpringStiffness = 0.1f;
		public float WalkAnimationLerpSpeed = 4;
		public float AnimationWalkSpeedDivider = 10;

		public float BodyTurnHorizontalClamp = 30;
		public float BodyTurnWeight = 0.5f;
		public Vector4 HeadLookAngleClamps = new Vector4(60, 40, 60, 30);
		public float LookLerpSpeed = 4;
		public float LookLerpSpringStiffness = 0.05f;

		///FX
		public FXObject[] EffectsOnPlayerSpawn = default;

		public FXObject[] EffectsOnCombatStart = default;
		public ChanceBasedFXGroup[] EffectsOnCombatStartChanceBased = default;
		public FXObject[] EffectsOnUltimateCharged = default;
		public FXObject[] EffectsWhileUltimateCharged = default;
		public FXObject[] EffectsOnLevelup = default;

		//On Player do attack
		public FXObject[] EffectsOnCauseDamage = default;
		public FXObject[] EffectsOnCauseCrit = default;
		public FXObject[] EffectsOnEnemyKilled = default;

		//Player Movement
		public FXObject[] EffectsWhileWalk = default;
		public FXObject[] EffectsWhileRun = default;
		public FXObject[] EffectsWhileDecelerating = default;
		public FXObject[] EffectsOnJump = default;
		public float PlayerLandingVelocityThreshold = 5f;
		public FXObject[] EffectsOnPlayerLanding = default;
		public FXObject[] EffectsOnPlayerDash = default;

		//On Player receive attack
		public FXObject[] EffectsOnReceiveDamage = default;
		public FXObject[] EffectsOnReceiveKnockback = default;
		public float EffectsHealthThreshold = 0.3f;
		public FXObject[] EffectsWhileHealthBelowThreshold = default;
	}
	
	[System.Serializable]
	public class ChanceBasedFXGroup
	{
		public ChanceBasedFX[] Group = new ChanceBasedFX[0];
	}
	[System.Serializable]
	public class ChanceBasedFX
	{
		public FXObject[] Effects = new FXObject[0];
		public int GroupRelativeChance = 1;
	}
}