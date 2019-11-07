using EoE.Entities;
using EoE.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class GameSettings : ScriptableObject
	{
		//Entitie Velocity
		public float WhenFallingExtraVelocity				= 0.5f;
		public AnimationCurve FallDamageCurve				= new AnimationCurve();
		public float GroundHitVelocityLoss					= 0.5f;

		//Behaiviors
		public float CombatCooldown							= 3;
		public float EnemyWanderingUrgency					= 0.5f;
		public float EnemyMinimumInvestigationArea			= 10;

		//Visuals
		public ParticleSystem HitEntitieParticles			= default;
		public ParticleSystem HitTerrainParticles			= default;

		public EntitieStatDisplay EntitieStatDisplayPrefab	= default;
		public float EnemeyHealthBarLerpSpeed				= 4;

		//Damage Numbers
		public float DamageNumberLifeTime					= 1;
		public bool ShowRegenNumbers						= true;
		public float DamageNumberFlySpeed					= 1;
		public float DamageNumberRandomMovementPower		= 1;
		public Gradient PhysicalDamageColors				= new Gradient();
		public Gradient MagicalDamageColors					= new Gradient();
		public Gradient HealColors							= new Gradient();
		public Gradient StandardTextColor					= new Gradient();

		//Dialogue Settings
		public DialogueBox DialogueBoxPrefab				= default;
		public float ShowDialogueBaseDelay					= 0.75f;
		public float DialogueDelayPerLetter					= 0.1f;
		public bool SkipDelayOnSpace						= true;
		public float DelayToNextDialogue					= 1.5f;

		//Other
		public float CritDamageMultiplier					= 2;
		public float SecondsPerEntititeRegen				= 0.5f;
		public SoulDrop SoulDropPrefab						= default;
		public float ItemDropRandomVelocityStrenght			= 2;

		//Damage Calculation
		public float DamageLevelAdd							= 12;
		public float DamageDivider							= 10;

		public float DefenseLevelAdd						= 10;
		public float DefenseLevelDivider					= 11;

		public ElementEffectivenessRow[] EffectivenessMatrix;

		public float GetEffectiveness(ElementType inflicter, ElementType receiver)
		{
			return EffectivenessMatrix[(int)inflicter][(int)receiver];
		}

		[System.Serializable]
		public class ElementEffectivenessRow
		{
			public float this[int index] { get => Row[index]; set => Row[index] = value; }
			public float[] Row;
			public ElementEffectivenessRow()
			{
				int elementCount = System.Enum.GetNames(typeof(ElementType)).Length;
				Row = new float[elementCount];
				for(int i = 0; i < elementCount; i++)
				{
					Row[i] = 1;
				}
			}
		}
	}
}