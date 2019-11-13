using UnityEngine;

namespace EoE.Information
{
	public class EntitieSettings : ObjectSettings
	{
		//Constants
		public const float MIN_ENTITIE_MASS = 0.001f;

		//Base Settings
		public float EntitieMass = 1;
		public Vector3 MassCenter = Vector3.zero;

		public ElementType EntitieElement = ElementType.None;
		public DropTable PossibleDropsTable = null;
		public int SoulWorth = 10;

		//Basic Combat Data
		public float BaseAttackDamage = 4;
		public float BaseMagicDamage = 3;
		public float BaseDefense = 2;

		//Health
		public float Health = 50;
		public bool DoHealthRegen = false;
		public float HealthRegen = 0;
		public float HealthRegenInCombatMultiplier = 0.5f;

		//Mana
		public float Mana = 50;
		public bool DoManaRegen = false;
		public float ManaRegen = 0;
		public float ManaRegenInCombatMultiplier = 0.5f;

		//Movement
		public float WalkSpeed = 8;
		public float RunSpeedMultiplicator = 1.25f;
		public Vector3 JumpPower = new Vector3(0, 10, 0);
		public float TurnSpeed = 90;
		public float InAirTurnSpeedMultiplier = 0.25f;

		//Movement acceleration
		public float MoveAcceleration = 0.5f;
		public float NoMoveDeceleration = 0.25f;
		public float InAirAccelerationMultiplier = 0.3f;
	}
}