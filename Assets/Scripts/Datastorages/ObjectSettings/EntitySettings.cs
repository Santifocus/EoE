using EoE.Combatery;
using UnityEngine;

namespace EoE.Information
{
	public class EntitySettings : ObjectSettings
	{
		//Constants
		public const float MIN_ENTITIE_MASS = 0.001f;

		//Base Settings
		public float EntitieMass = 1;
		public Vector3 MassCenter = Vector3.zero;
		public bool ShowEntitieLevel = true;

		public ElementType EntitieElement = ElementType.None;
		public DropTable PossibleDropsTable = null;
		public LevelingSettings LevelSettings = null;
		public int ExperienceWorth = 10;

		public ActivationEffect[] DeathEffects = new ActivationEffect[0];

		//Basic Combat Data
		public float BasePhysicalDamage = 10;
		public float BaseMagicalDamage = 10;
		public float BaseDefense = 2;

		//Health
		public float Health = 50;
		public bool DoHealthRegen = false;
		public float HealthRegen = 0;
		public float HealthRegenCooldownAfterTakingDamage = 4;
		public float HealthRegenInCombatMultiplier = 0.5f;

		public GameObject HealthRegenParticles = null;
		public Vector3 HealthRegenParticlesOffset = Vector3.zero;

		//Mana
		public float Mana = 50;
		public bool DoManaRegen = false;
		public float ManaRegen = 0;
		public float ManaRegenInCombatMultiplier = 0.5f;

		//Movement
		public float WalkSpeed = 8;
		public float RunSpeedMultiplicator = 1.25f;
		public float TurnSpeed = 90;
		public float InAirTurnSpeedMultiplier = 0.25f;

		//Movement acceleration
		public float MoveAcceleration = 0.5f;
		public float NoMoveDeceleration = 0.25f;
		public float InAirAccelerationMultiplier = 0.3f;
	}
}