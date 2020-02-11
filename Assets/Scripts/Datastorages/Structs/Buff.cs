using EoE.Behaviour.Entities;
using UnityEngine;

namespace EoE.Information
{
	public enum BuffType : byte { Negative = 0, Neutral = 1, Positive = 2 }
	public enum TargetBaseStat { Health = 0, Mana = 1, Stamina = 2, PhysicalDamage = 3, MagicalDamage = 4, Defense = 5, MoveSpeed = 6, JumpHeightMultiplier = 7, TrueDamageMultiplier = 8 }
	public enum TargetStat { Health = 0, Mana = 1, Stamina = 2, UltimateCharge = 3 }
	public enum BuffStackingStyle { Stack = 1, Reapply = 2, DoNothing = 4 }
	public class Buff : ScriptableObject
	{
		public string Name;
		public BuffType Quality = BuffType.Neutral;
		public FinishConditions FinishConditions = new FinishConditions();
		public Sprite Icon;

		public Effect[] Effects;
		public CustomEffect CustomEffects;
		public DOT[] DOTs;
		public FXObject[] FXEffects;

		public static BuffInstance ApplyBuff(Buff buff, Entity target, Entity applier, float multiplier = 1, BuffStackingStyle stackingStyle = BuffStackingStyle.Reapply)
		{
			if (stackingStyle == BuffStackingStyle.Stack)
			{
				return target.AddBuff(buff, applier, multiplier);
			}
			else if (stackingStyle == BuffStackingStyle.Reapply)
			{
				(bool reApplied, BuffInstance reAppliedBuff) = target.TryReapplyBuff(buff, applier);
				if (!reApplied)
				{
					return target.AddBuff(buff, applier, multiplier);
				}
				else
				{
					return reAppliedBuff;
				}
			}
			else //stackingStyle == BuffStackingStyle.DoNothing
			{
				if (!(target.HasBuffActive(buff, applier).HasValue))
				{
					return target.AddBuff(buff, applier, multiplier);
				}
				else
				{
					return null;
				}
			}
		}
	}

	public class BuffInstance
	{
		public Buff Base;
		public Entity Applier;
		public Entity Target;
		public float[] FlatChanges;
		public float[] DOTCooldowns;
		public FXInstance[] BoundEffects;
		private float TimeMultiplier;
		private float RemainingTime;

		public BuffInstance(Buff Base, Entity Applier, Entity Target, float TimeMultiplier)
		{
			this.Base = Base;
			this.Applier = Applier;
			this.FlatChanges = new float[Base.Effects.Length];
			this.DOTCooldowns = new float[Base.DOTs.Length];

			this.TimeMultiplier = TimeMultiplier;
			Reset();
			FXManager.ExecuteFX(Base.FXEffects, Target.transform, Target is Player, out BoundEffects);
		}
		public void Reset()
		{
			this.RemainingTime = Base.FinishConditions.OnTimeout ? (Base.FinishConditions.TimeStay * TimeMultiplier) : 1;
		}
		public bool Update()
		{
			if(Base.FinishConditions.OnTimeout)
				RemainingTime -= Time.deltaTime;

			for(int i = 0; i < DOTCooldowns.Length; i++)
			{
				DOTCooldowns[i] -= Time.deltaTime;
			}

			return (RemainingTime <= 0) || (Base.FinishConditions.OnConditionMet && Base.FinishConditions.ConditionMet());
		}
		public void OnRemove()
		{
			FXManager.FinishFX(ref BoundEffects);
		}
	}

	[System.Serializable]
	public struct Effect
	{
		public TargetBaseStat TargetBaseStat;
		public bool Percent;
		public float Amount;
	}
	[System.Serializable]
	public struct CustomEffect
	{
		public bool ApplyMoveStun;
		public bool Invincible;
	}
	[System.Serializable]
	public struct DOT
	{
		public ElementType Element;
		public TargetStat TargetStat;
		public float DelayPerActivation;
		public float BaseDamage;
	}
}