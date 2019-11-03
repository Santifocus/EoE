using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class PlayerSettings : EntitieSettings
	{
		//Leveling settings
		public LevelingSettings LevelSettings		= null;

		//Camera Settings
		public float CameraToPlayerDistance			= 7;
		public Vector3 CameraAnchorOffset			= new Vector3(0, 1.6f, 0);
		public Vector2 CameraRotationPower			= new Vector2(200, 75);
		public float CameraRotationSpeed			= 10;
		public Vector2 CameraVerticalAngleClamps	= new Vector2(-50, 50);
		public float CameraExtraZoomOnVertical		= 0.5f;
		public float MaxEnemyTargetingDistance		= 100;
		public Vector2 CameraClampsWhenTargeting	= new Vector2(-30, 30);

		//Endurance Settings
		public int EnduranceBars					= 4;
		public float EndurancePerBar				= 25;
		public float EnduranceRegen					= 5;
		public float LockedEnduranceRegenMutliplier = 0.5f;
		public float EnduranceRegenInCombat			= 0.5f;
		public float EnduranceRegenDelayAfterUse	= 3;

		//Endurance Costs
		public float JumpEnduranceCost				= 4;
		public float RunEnduranceCost				= 3;

		//UI
		public float StatTextUpdateSpeed			= 1;
		public float EnduranceBarLerpSpeed			= 5;
		public Gradient HealthTextColors			= new Gradient();
		public Gradient ManaTextColors				= new Gradient();

		public Color ReserveEnduranceBarColor		= new Color(0.65f, 0.325f, 0, 1);
		public Color ActiveEnduranceBarColor		= new Color(1, 0.5f, 0, 1);
		public Color ReloadingEnduranceBarColor		= new Color(0.4f, 0.4f, 0.4f, 0.8f);

		//Dodging
		public float DodgePower						= 1;
		public float DodgeDuration					= 0.2f;
		public float DodgeCooldown					= 0.5f;
		public float DodgeUpForce					= 1;
		public float DodgeEnduranceCost				= 20;
		public Material DodgeModelMaterial			= null;
	}
}