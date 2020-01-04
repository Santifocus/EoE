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
		public bool Permanent = false;
		public float BuffTime = 5;
		public Sprite Icon;

		public Effect[] Effects;
		public CustomEffect CustomEffects;
		public DOT[] DOTs;
		public FXObject[] FXEffects;

		public static void ApplyBuff(Buff buff, Entitie target, Entitie applier, BuffStackingStyle stackingStyle = BuffStackingStyle.Reapply)
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
		public Entitie Applier;
		public Entitie Target;
		public float RemainingTime;
		public float[] FlatChanges;
		public float[] DOTCooldowns;
		public FXInstance[] BoundEffects;

		public BuffInstance(Buff Base, Entitie Applier, Entitie Target)
		{
			this.Base = Base;
			this.Applier = Applier;
			this.RemainingTime = Base.BuffTime;
			this.FlatChanges = new float[Base.Effects.Length];
			this.DOTCooldowns = new float[Base.DOTs.Length];

			BoundEffects = new FXInstance[Base.FXEffects != null ? Base.FXEffects.Length : 0];
			for (int i = 0; i < BoundEffects.Length; i++)
			{
				BoundEffects[i] = FXManager.PlayFX(Base.FXEffects[i], Target.transform, Target is Player);
			}
		}
		public void OnRemove()
		{
			for (int i = 0; i < BoundEffects.Length; i++)
			{
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