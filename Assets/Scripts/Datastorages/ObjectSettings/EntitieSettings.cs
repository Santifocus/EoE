using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class EntitieSettings : ObjectSettings
	{
		//Constants
		public const float MIN_ENTITIE_MASS = 0.001f;

		//Base Settings
		public float EntitieMass					= 1;
		public Vector3 MassCenter					= Vector3.zero;

		public ElementType EntitieElement			= ElementType.None;

		//Health
		public float Health							= 50;
		public bool DoHealthRegen					= false;
		public float HealthRegen					= 0;
		public float HealthRegenInCombatMultiplier	= 0.5f;

		//Mana
		public float Mana							= 50;
		public bool DoManaRegen						= false;
		public float ManaRegen						= 0;
		public float ManaRegenInCombatMultiplier	= 0.5f;

		//Movement
		public float WalkSpeed						= 8;
		public float RunSpeedMultiplicator			= 1.25f;
		public Vector3 JumpPower					= new Vector3(0, 10, 0);
		public float TurnSpeed						= 90;

		//Movement acceleration
		public float MoveAcceleration				= 0.5f;
		public float NoMoveDeceleration				= 0.25f;
	}
}