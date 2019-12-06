using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	public class EffectSingle : ScriptableObject
	{
		public TargetMask AffectedTargets = TargetMask.Enemy;
		public ElementType DamageElement = ElementType.None;
		public CauseType CauseType = CauseType.Magic;

		public float DamageMultiplier = 1;
		public float CritChanceMultiplier = 0;

		public float KnockbackMultiplier = 1;
		public EffectiveDirection KnockbackOrigin = EffectiveDirection.Center;
		public DirectionBase KnockbackDirection = DirectionBase.Forward;
		public Vector3 KnockbackAxisMultiplier = Vector3.one;

		public BuffStackingStyle BuffStackStyle = BuffStackingStyle.Reapply;
		public Buff[] BuffsToApply = new Buff[0];

		public FXObject[] Effects = new FXObject[0];
		#region Activation
		public void ActivateEffectSingle(Entitie effectCauser, Entitie target, CombatObject infoBase, Vector3 forceDirection, Vector3 hitPoint)
		{
			//Damage / Knockback
			float casterBaseDamage = CauseType == CauseType.Physical ? (effectCauser.curPhysicalDamage) : (CauseType == CauseType.Magic ? effectCauser.curMagicalDamage : 0);
			float knockback = KnockbackMultiplier * infoBase.BaseKnockback;
			float damage = (infoBase.BaseDamage + casterBaseDamage) * DamageMultiplier;

			target.ChangeHealth(new ChangeInfo(
				effectCauser,
				CauseType,
				DamageElement,
				TargetStat.Health,
				hitPoint,
				new Vector3(forceDirection.x * KnockbackAxisMultiplier.x, forceDirection.y * KnockbackAxisMultiplier.y, forceDirection.z * KnockbackAxisMultiplier.z),
				damage,
				(Random.value < (infoBase.BaseCritChance * CritChanceMultiplier)),
				(knockback > 0) ? (float?)knockback : (null)
				));

			//Buffs
			for (int j = 0; j < BuffsToApply.Length; j++)
			{
				if (BuffStackStyle == BuffStackingStyle.Stack)
				{
					target.AddBuff(BuffsToApply[j], effectCauser);
				}
				else if (BuffStackStyle == BuffStackingStyle.Reapply)
				{
					if (!(target.TryReapplyBuff(BuffsToApply[j], effectCauser).Item1))
					{
						target.AddBuff(BuffsToApply[j], effectCauser);
					}
				}
				else //effect.BuffStackStyle == BuffStackingStyle.DoNothing
				{
					if (!(target.HasBuffActive(BuffsToApply[j], effectCauser)))
					{
						target.AddBuff(BuffsToApply[j], effectCauser);
					}
				}
			}

			//FXEffects
			for (int j = 0; j < Effects.Length; j++)
			{
				FXManager.PlayFX(Effects[j], target.transform, target is Player);
			}
		}
		#endregion
	}
}