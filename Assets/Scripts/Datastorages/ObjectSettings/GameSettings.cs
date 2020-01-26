using EoE.Sounds;
using EoE.UI;
using UnityEngine;

namespace EoE.Information
{
	public class GameSettings : ScriptableObject
	{
		public bool IsDebugEnabled
		{
			get
			{
#if UNITY_EDITOR
				return debugEnabledInternal;
#else
				return false;
#endif
			}
		}
		public bool debugEnabledInternal = false;
		//Entitie Velocity
		public float WhenFallingExtraGravity = 0.5f;
		public AnimationCurve FallDamageCurve = new AnimationCurve();
		public float GroundHitVelocityLoss = 0.5f;
		public float GroundHitVelocityLossMinThreshold = 10;
		public float GroundHitVelocityLossMaxThreshold = 50;

		//Behaiviors
		public float CombatCooldown = 3;
		public float IdleMovementUrgency = 0.5f;
		public float EnemyMinimumInvestigationArea = 10;

		//Visuals
		public EntityStatDisplay EntitieStatDisplayPrefab = default;
		public float EntitieHealthBarLerpSpeed = 2;
		public float EntitieHealthBarLerpDelay = 0.5f;

		//Damage Numbers
		public float DamageNumberLifeTime = 1;
		public bool ShowRegenNumbers = true;
		public float DamageNumberFlySpeed = 1;
		public float DamageNumberRandomMovementPower = 1;
		public Gradient PhysicalDamageColors = new Gradient();
		public Gradient MagicalDamageColors = new Gradient();
		public Gradient HealColors = new Gradient();
		public Gradient StandardTextColor = new Gradient();

		//Dialogue Settings
		public DialogueBox DialogueBoxPrefab = default;
		public float DialogueDelayPerLetter = 0.1f;
		public bool SkipDelayOnSpace = true;
		public float DelayToNextDialogue = 1.5f;

		//Other
		public float MinDamage = 1;
		public float CritDamageMultiplier = 2;
		public float SecondsPerEntitieHealthRegen = 0.5f;
		public float ItemDropRandomVelocityStrenght = 2;

		//CurrencySettings
		public float ItemSellMultiplier = 0.70f;
		public int BaseCurrencyPerSoul = 10;
		public int ExtraRandomCurrencyPerSoul = 5;

		//Damage Calculation
		public float PhysicalDamageLevelAdd = 12;
		public float PhysicalDamageDivider = 10;

		public float PhysicalDefenseLevelAdd = 10;
		public float PhysicalDefenseLevelDivider = 11;

		public float MagicDamageLevelAdd = 3;
		public float MagicDamageDivider = 6;

		public ElementEffectivenessRow[] EffectivenessMatrix;

		public float GetEffectiveness(ElementType inflicter, ElementType receiver)
		{
			int inflicterIndex = 0;
			int receiverIndex = 0;

			int typeIndex = (int)inflicter;
			while (typeIndex >= 2)
			{
				typeIndex /= 2;
				inflicterIndex++;
			}

			typeIndex = (int)receiver;
			while (typeIndex >= 2)
			{
				typeIndex /= 2;
				receiverIndex++;
			}
			return EffectivenessMatrix[inflicterIndex][receiverIndex];
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
				for (int i = 0; i < elementCount; i++)
				{
					Row[i] = 1;
				}
			}
		}
	}
}