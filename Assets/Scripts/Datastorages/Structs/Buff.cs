using EoE.Entities;
using UnityEngine;

namespace EoE.Information
{
	public enum BuffType : byte { Negative = 0, Neutral = 1, Positive = 2 }
	public enum TargetBaseStat { Health = 0, Mana = 1, Endurance = 2, PhysicalDamage = 3, MagicalDamage = 4, Defense = 5, MoveSpeed = 6, JumpHeightMultiplier = 7, TrueDamageMultiplier = 8 }
	public enum TargetStat { Health = 0, Mana = 1, Endurance = 2 }
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

		public static void ApplyBuff(Buff buff, Entity target, Entity applier, BuffStackingStyle stackingStyle = BuffStackingStyle.Reapply)
		{
			if (stackingStyle == BuffStackingStyle.Stack)
			{
				target.AddBuff(buff, applier);
			}
			else if (stackingStyle == BuffStackingStyle.Reapply)
			{
				if (!(target.TryReapplyBuff(buff, applier).Item1))
				{
					target.AddBuff(buff, applier);
				}
			}
			else //stackingStyle == BuffStackingStyle.DoNothing
			{
				if (!(target.HasBuffActive(buff, applier).HasValue))
				{
					target.AddBuff(buff, applier);
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
		private float RemainingTime;

		public BuffInstance(Buff Base, Entity Applier, Entity Target)
		{
			this.Base = Base;
			this.Applier = Applier;
			this.FlatChanges = new float[Base.Effects.Length];
			this.DOTCooldowns = new float[Base.DOTs.Length];

			Reset();

			BoundEffects = new FXInstance[Base.FXEffects != null ? Base.FXEffects.Length : 0];
			for (int i = 0; i < BoundEffects.Length; i++)
			{
				BoundEffects[i] = FXManager.PlayFX(Base.FXEffects[i], Target.transform, Target is Player);
			}
		}
		public void Reset()
		{
			this.RemainingTime = Base.FinishConditions.OnTimeout ? Base.FinishConditions.TimeStay : 1;
		}
		public bool Update()
		{
			if(Base.FinishConditions.OnTimeout)
				RemainingTime -= Time.deltaTime;

			for(int i = 0; i < DOTCooldowns.Length; i++)
			{
				DOTCooldowns[i] -= Time.deltaTime;
			}

			return (RemainingTime <= 0) || (Base.FinishConditions.OnConditionMet && Base.FinishConditions.Condition.ConditionMet());
		}
		public void OnRemove()
		{
			for (int i = 0; i < BoundEffects.Length; i++)
			{
				if (BoundEffects[i] != null)
					BoundEffects[i].FinishFX();
			}
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