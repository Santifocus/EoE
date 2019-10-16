using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class GameSettings : ScriptableObject
	{
		public float SecondsPerEntititeRegen;
		public AnimationCurve TurnSpeedCurve = new AnimationCurve();
		public float EntitieVelocityLerpSpeed = 4;

		public float WhenFallingExtraVelocity = 0.5f;
		public AnimationCurve FallDamageCurve = new AnimationCurve();
		public float GroundHitVelocityLoss = 0.5f;

		//Damage Numbers
		public float DamageNumberLifeTime = 1;
		public bool ShowRegenNumbers = true;
		public float DamageNumberFlySpeed = 1;
		public float DamageNumberRandomMovementPower = 1;
		public Gradient PhysicalDamageColors = new Gradient();
		public Gradient MagicalDamageColors = new Gradient();
		public Gradient HealColors = new Gradient();

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