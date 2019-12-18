using UnityEngine;

namespace EoE.Information
{
	public class PlayerSettings : EntitieSettings
	{
		//Camera Settings
		public float CameraToPlayerDistance = 7;
		public Vector3 CameraAnchorOffset = new Vector3(0, 1.6f, 0);
		public Vector3 CameraAnchorOffsetWhenTargeting = new Vector3(0, 1.6f, 0);
		public Vector2 CameraRotationPower = new Vector2(200, 75);
		public float CameraRotationSpeed = 10;
		public Vector2 CameraVerticalAngleClamps = new Vector2(-50, 50);
		public float CameraExtraZoomOnVertical = 0.5f;
		public float MaxEnemyTargetingDistance = 100;
		public Vector2 CameraClampsWhenTargeting = new Vector2(-30, 30);
		public float CameraBaseFOV = 60;

		//Endurance Settings
		public float Endurance = 100;
		public bool DoEnduranceRegen = true;
		public float EnduranceRegen = 6;
		public float EnduranceRegenInCombatMultiplier = 0.5f;
		public float EnduranceAfterUseCooldown = 3;
		public float EnduranceRegenAfterUseMultiplier = 0.5f;

		//Endurance Costs
		public float JumpEnduranceCost = 4;
		public float RunEnduranceCost = 3;

		//Jumping
		public Vector3 JumpPower = new Vector3(0, 10, 0);
		public float JumpImpulsePower = 2.5f;
		public float JumpBackwardMultiplier = 0.65f;

		//Dodging
		public float DodgePower = 1;
		public float DodgeDuration = 0.2f;
		public float DodgeModelExistTime = 0.4f;
		public float DodgeCooldown = 0.5f;
		public float DodgeEnduranceCost = 20;
		public Material DodgeModelMaterial = null;

		//Blocking
		public float StartBlockingInertia = 0.3f;
		public Buff BlockingBuff = default;
		public float StopBlockingInertia = 0.3f;

		//IFrames
		public bool InvincibleAfterHit = true;
		public float InvincibleAfterHitTime = 0.3f;
		public Color InvincibleModelFlashColor = Color.white;
		public float InvincibleModelFlashDelay = 0.4f;
		public float InvincibleModelFlashTime = 0.4f;

		//Inventory
		public StartItem[] StartItems = new StartItem[0];
		public int InventorySize = 24;

		//Animation
		public float MaxModelTilt = 10;
		public float SideTurnLerpSpeed = 10;
		public float SpringLerpStiffness = 1;
		public float WalkAnimationLerpSpeed = 4;

		///FX
		//On Player do attack
		public FXObject[] EffectsOnCauseDamage = default;
		public FXObject[] EffectsOnCauseCrit = default;
		public FXObject[] EffectsOnEnemyKilled = default;
		public FXObject[] EffectsOnLevelup = default;

		//Player Movement
		public FXObject[] EffectsWhileWalk = default;
		public FXObject[] EffectsWhileRun = default;
		public FXObject[] EffectsOnJump = default;
		public float PlayerLandingVelocityThreshold = 5f;
		public FXObject[] EffectsOnPlayerLanding = default;
		public FXObject[] EffectsOnPlayerDodge = default;

		//On Player receive attack
		public FXObject[] EffectsOnReceiveDamage = default;
		public FXObject[] EffectsOnReceiveKnockback = default;
		public float EffectsHealthThreshold = 0.3f;
		public FXObject[] EffectsWhileHealthBelowThreshold = default;
		public FXObject[] EffectsOnPlayerDeath = default;

		[System.Serializable]
		public class StartItem
		{
			public Item Item;
			public int ItemCount = 1;
			public bool ForceEquip;
		}
	}
}