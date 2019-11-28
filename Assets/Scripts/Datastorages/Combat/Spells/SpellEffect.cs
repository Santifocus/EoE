using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Weapons
{
	public class SpellEffect : ScriptableObject
	{
		public SpellTargetMask AffectedTargets = SpellTargetMask.Enemy;

		public float BaseEffectRadius = 5;
		public float ZeroOutDistance = 7;

		public bool HasMaximumHits = false;
		public int MaximumHits = 0;

		public ElementType DamageElement = ElementType.None;
		public float CritChance = 0;
		public float DamageMultiplier = 1;

		public float KnockbackMultiplier = 1;
		public EffectiveDirection KnockbackOrigin = EffectiveDirection.Center;
		public DirectionBase KnockbackDirection = DirectionBase.Forward;

		public BuffStackingStyle BuffStackStyle = BuffStackingStyle.Reapply;
		public Buff[] BuffsToApply = new Buff[0];

		public FXObject[] Effects = new FXObject[0];
	}
}