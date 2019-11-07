using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{ 
	public enum BuffType : byte { Negative = 0, Neutral = 1, Positive = 2 }
	public enum TargetStat { Health = 0, Mana = 1, Endurance = 2, PhysicalDamage = 3, MagicalDamage = 4, Defense = 5, MoveSpeed = 6, JumpHeight = 7 }
	[System.Serializable]
	public class Buff
	{
		public string Name;
		public BuffType Quality = BuffType.Neutral;
		public bool Permanent = false;
		public float BuffTime = 5;
		public Sprite Icon;

		public Effect[] Effects;
		public DOT[] DOTs;
	}

	public class BuffInstance
	{
		public Buff Base;
		public Entitie Applier;
		public float RemainingTime;
		public float[] FlatChanges;
		public float[] DOTCooldowns;

		public BuffInstance(Buff Base, Entitie Applier)
		{
			this.Base = Base;
			this.Applier = Applier;
			this.RemainingTime = Base.BuffTime;
			this.FlatChanges = new float[Base.Effects.Length];
			this.DOTCooldowns = new float[Base.DOTs.Length];
		}
	}

	[System.Serializable]
	public struct Effect
	{
		public TargetStat targetStat;
		public bool Percent;
		public float Amount;
	}
	[System.Serializable]
	public struct DOT
	{
		public ElementType Element;
		public float DelayPerActivation;
		public float BaseDamage;
	}
}