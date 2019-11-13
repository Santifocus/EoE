using UnityEngine;

namespace EoE.Information
{
	public class PlayerSettings : EntitieSettings
	{
		//Leveling settings
		public LevelingSettings LevelSettings = null;

		//Camera Settings
		public float CameraToPlayerDistance = 7;
		public Vector3 CameraAnchorOffset = new Vector3(0, 1.6f, 0);
		public Vector2 CameraRotationPower = new Vector2(200, 75);
		public float CameraRotationSpeed = 10;
		public Vector2 CameraVerticalAngleClamps = new Vector2(-50, 50);
		public float CameraExtraZoomOnVertical = 0.5f;
		public float MaxEnemyTargetingDistance = 100;
		public Vector2 CameraClampsWhenTargeting = new Vector2(-30, 30);

		//Endurance Settings
		public int EnduranceBars = 4;
		public float EndurancePerBar = 25;
		public float EnduranceRegen = 5;
		public float LockedEnduranceRegenMutliplier = 0.5f;
		public float EnduranceRegenInCombat = 0.5f;
		public float EnduranceRegenDelayAfterUse = 3;

		//Endurance Costs
		public float JumpEnduranceCost = 4;
		public float RunEnduranceCost = 3;

		//Dodging
		public float DodgePower = 1;
		public float DodgeDuration = 0.2f;
		public float DodgeModelExistTime = 0.4f;
		public float DodgeCooldown = 0.5f;
		public float DodgeEnduranceCost = 20;
		public Material DodgeModelMaterial = null;

		//IFrames
		public bool InvincibleAfterHit = true;
		public float InvincibleAfterHitTime = 0.3f;
		public Color InvincibleModelFlashColor = Color.white;
		public float InvincibleModelFlashDelay = 0.4f;
		public float InvincibleModelFlashTime = 0.4f;

		//UI
		public float StatTextUpdateSpeed = 1;
		public float EnduranceBarLerpSpeed = 5;
		public Gradient HealthTextColors = new Gradient();
		public Gradient ManaTextColors = new Gradient();

		public Color ReserveEnduranceBarColor = new Color(0.65f, 0.325f, 0, 1);
		public Color ActiveEnduranceBarColor = new Color(1, 0.5f, 0, 1);
		public Color ReloadingEnduranceBarColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);

		///VFX
		//ScreenColor
		public bool ColorScreenOnDamage = true;
		public Color ColorScreenColor = Color.red;
		public float ColorScreenDepth = 0.15f;
		public float ColorScreenDuration = 0.75f;

		//ShakeOnknockback
		public bool ShakeScreenOnKnockback = true;
		public float ShakeTimeOnKnockback = 0.5f;
		public float ShakeScreenAxisIntensity = 0.25f;
		public float ShakeScreenAngleIntensity = 0.25f;

		//Blur on lowlife
		public bool BlurScreenOnDamage = true;
		public float BlurScreenHealthThreshold = 0.35f;
		public float BlurScreenBaseIntensity = 0.05f;
		public float BlurScreenDuration = 0.5f;

		//Rumble on damage receive
		public bool RumbleOnReceiveDamage = true;
		public float RumbleOnReceiveDamageTime = 0.1f;
		public float RumbleOnReceiveDamageLeftIntensityStart = 0.6f;
		public float RumbleOnReceiveDamageRightIntensityStart = 0.9f;
		public float RumbleOnReceiveDamageLeftIntensityEnd = 0.5f;
		public float RumbleOnReceiveDamageRightIntensityEnd = 0.8f;

		//Slow on Crit
		public bool SlowOnCriticalHit = true;
		public float SlowOnCritTimeIn = 0.2f;
		public float SlowOnCritTimeStay = 0.5f;
		public float SlowOnCritTimeOut = 0.2f;
		public float SlowOnCritScale = 0.3f;

		//Shake on Crit
		public bool ScreenShakeOnCrit = true;
		public float ShakeTimeOnCrit = 0.15f;
		public float OnCritShakeAxisIntensity = 0.25f;
		public float OnCritShakeAngleIntensity = 0.25f;

		//Rumble on Crit
		public bool RumbleOnCrit = true;
		public float RumbleOnCritTime = 0.1f;
		public float RumbleOnCritLeftIntensityStart = 0.6f;
		public float RumbleOnCritRightIntensityStart = 0.9f;
		public float RumbleOnCritLeftIntensityEnd = 0.5f;
		public float RumbleOnCritRightIntensityEnd = 0.8f;
	}
}