using EoE.Entities;
using EoE.Information;
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

		public CustomFXObject[] Effects = new CustomFXObject[0];
		#region Activation
		public void Activate(Entity effectCauser, Entity target, CombatData infoBase, Vector3 forceDirection, Vector3 hitPoint, EffectOverrides effectOverrides = null)
		{
			ElementType effectElement = (effectOverrides == null) ? DamageElement : (effectOverrides.OverridenElement.HasValue ? effectOverrides.OverridenElement.Value : DamageElement);
			CauseType effectCause = (effectOverrides == null) ? CauseType : (effectOverrides.OverridenCauseType.HasValue ? effectOverrides.OverridenCauseType.Value : CauseType);

			//Damage / Knockback
			float baseDamage = (effectCause == CauseType.Physical ? (effectCauser.curPhysicalDamage) : (effectCause == CauseType.Magic ? effectCauser.curMagicalDamage : 0)) +
								(effectCause == CauseType.Physical ? infoBase.BasePhysicalDamage : effectCauser.curMagicalDamage);
			bool wasCrit = Utils.Chance01(CritChanceMultiplier * infoBase.BaseCritChance * (effectOverrides == null ? 1 : effectOverrides.ExtraCritChanceMultiplier));

			float damage = baseDamage * DamageMultiplier * (effectOverrides == null ? 1 : effectOverrides.ExtraDamageMultiplier);
			float knockback = KnockbackMultiplier * infoBase.BaseKnockback * (effectOverrides == null ? 1 : effectOverrides.ExtraKnockbackMultiplier);

			target.ChangeHealth(new ChangeInfo(
								effectCauser,
								effectCause,
								effectElement,
								TargetStat.Health,
								hitPoint,
								new Vector3(forceDirection.x * KnockbackAxisMultiplier.x, forceDirection.y * KnockbackAxisMultiplier.y, forceDirection.z * KnockbackAxisMultiplier.z),
								damage,
								wasCrit,
								(knockback > 0) ? (float?)knockback : (null)
								));

			//Buffs
			for (int j = 0; j < BuffsToApply.Length; j++)
			{
				Buff.ApplyBuff(BuffsToApply[j], target, effectCauser, BuffStackStyle);
			}

			//FXEffects
			for (int j = 0; j < Effects.Length; j++)
			{
				FXManager.PlayFX(Effects[j], target.transform, target is Player);
			}
		}
		#endregion
	}
	public class EffectOverrides
	{
		public float ExtraDamageMultiplier;
		public float ExtraCritChanceMultiplier;
		public float ExtraKnockbackMultiplier;
		public ElementType? OverridenElement;
		public CauseType? OverridenCauseType;
	}
}