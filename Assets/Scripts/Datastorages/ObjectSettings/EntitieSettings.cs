using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class EntitieSettings : ObjectSettings
	{
		public const float MIN_ENTITIE_MASS = 0.001f;

		public float EntitieMass = 1;
		public Vector3 MassCenter = Vector3.zero;

		public ElementType EntitieElement = ElementType.None;

		//Health
		public float Health = 10;
		public bool DoHealthRegen = false;
		public float HealthRegen = 0;
		public float HealthRegenInCombatFactor = 0.5f;
		//Endurance
		public float Endurance;
		public bool DoEnduranceRegen = false;
		public float EnduranceRegen = 0;
		public float EnduranceRegenInCombatFactor = 0.5f;
		//Mana
		public float Mana;
		public bool DoManaRegen = false;
		public float ManaRegen = 0;
		public float ManaRegenInCombatFactor = 0.5f;

		//Movement
		public float WalkSpeed = 1;
		public float RunSpeedMultiplicator = 1.5f;
		public Vector3 JumpPower = new Vector3(0, 10, 0);
		public float TurnSpeed = 0.35f;
		//Movement acceleration
		public float MoveAcceleration = 1;
		public float NoMoveDeceleration = 1;
		//Costs
		public float JumpEnduranceCost = 0;
		public float RunEnduranceCost = 0;
	}
}