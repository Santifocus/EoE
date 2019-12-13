﻿using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	public class EffectAOE : ScriptableObject
	{
		public TargetMask AffectedTargets = TargetMask.Enemy;
		public ElementType DamageElement = ElementType.None;
		public CauseType CauseType = CauseType.Magic;

		public float BaseEffectRadius = 5;
		public float ZeroOutDistance = 7;

		public bool HasMaximumHits = false;
		public int MaximumHits = 0;

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
		public void ActivateEffectAOE(Entitie effectCauser, Transform origin, CombatObject infoBase, EffectOverrides effectOverrides = null)
		{
			ElementType effectElement = (effectOverrides == null) ? DamageElement : (effectOverrides.OverridenElement.HasValue ? effectOverrides.OverridenElement.Value : DamageElement);
			CauseType effectCause = (effectOverrides == null) ? CauseType : (effectOverrides.OverridenCauseType.HasValue ? effectOverrides.OverridenCauseType.Value : CauseType);

			//If the effect is not applying force based on its center we can find out the direction here
			//so we dont have to recalculate it everytime a new eligible target is added
			Vector3 localDirection = Vector3.up;
			if (KnockbackOrigin != EffectiveDirection.Center)
			{
				(Vector3Int, bool) dirInfo = CombatObject.EnumDirToDir(KnockbackDirection);
				if (KnockbackOrigin == EffectiveDirection.World)
				{
					localDirection = dirInfo.Item1 * (dirInfo.Item2 ? -1 : 1);
				}
				else //effect.KnockbackDirection == EffectiveDirection.Local
				{
					if (dirInfo.Item1 == new Vector3Int(0, 0, 1))
					{
						localDirection = origin.forward * (dirInfo.Item2 ? -1 : 1);
					}
					else if (dirInfo.Item1 == new Vector3Int(1, 0, 0))
					{
						localDirection = origin.right * (dirInfo.Item2 ? -1 : 1);
					}
					else //dir.Item1 == new Vector3Int(0, 1, 0)
					{
						localDirection = origin.up * (dirInfo.Item2 ? -1 : 1);
					}
				}
			}

			//First find the targets that are eligible
			float innerSphereDist = BaseEffectRadius * BaseEffectRadius;
			float outerSphereDist = ZeroOutDistance * ZeroOutDistance;

			//In order to dodge using a list so we dont spam the garbage collector we first find out how many entities will be added
			//And then build a array with that size, for that we could use Linq aswell but this methode is faster (probably)
			int requiredCapacity = 0;
			for (int i = 0; i < Entitie.AllEntities.Count; i++)
			{
				if ((Entitie.AllEntities[i].actuallWorldPosition - origin.position).sqrMagnitude < outerSphereDist)
				{
					//Check if this entitie should be a targetable entitie
					if (CombatObject.IsAllowedEntitie(Entitie.AllEntities[i], effectCauser, AffectedTargets))
						requiredCapacity++;
				}
			}
			CollectedEntitieData[] eligibleTargets = new CollectedEntitieData[requiredCapacity];

			int addedEntities = 0;
			for (int i = 0; i < Entitie.AllEntities.Count; i++)
			{
				Vector3 dif = Entitie.AllEntities[i].actuallWorldPosition - origin.position;
				float sqrDist = dif.sqrMagnitude;

				//Generally onbly allow to keep going if the distance is smaller then the other sphere distance
				if (sqrDist < outerSphereDist)
				{
					//Check if this entitie should be a targetable entitie
					if (!CombatObject.IsAllowedEntitie(Entitie.AllEntities[i], effectCauser, AffectedTargets))
						continue;

					CollectedEntitieData data = new CollectedEntitieData()
					{
						Target = Entitie.AllEntities[i],
						SqrDist = sqrDist
					};

					//If the target is in the outer sphere and we want to normalize the direction based on the center
					//then we can save one square root by only calculating it in the multiplier calculation
					float? distance = null;

					//Calculate the multiplier
					if (sqrDist <= innerSphereDist)
					{
						//Max is 1 and inner sphere will always be max
						data.Multiplier = 1;
					}
					else //(sqrDist < outerSphereDist) && (sqrDist > innerSphereDist)
					{
						distance = Mathf.Sqrt(sqrDist);
						float outerDistance = distance.Value - BaseEffectRadius;
						//The divider will never be zero because of previous if statements so we dont have to catch it
						data.Multiplier = outerDistance / (ZeroOutDistance - BaseEffectRadius);
					}

					//Now find out in what direction we want to apply forces
					if (KnockbackOrigin == EffectiveDirection.Center)
					{
						if (!distance.HasValue)
							distance = Mathf.Sqrt(sqrDist);

						data.ApplyDirection = (distance.Value > 0) ? (dif / distance.Value) : (Vector3.up);
					}
					else//effect.KnockbackDirection == EffectiveDirection.Local || effect.KnockbackDirection == EffectiveDirection.World
					{
						data.ApplyDirection = localDirection;
					}
					eligibleTargets[addedEntities] = data;
					addedEntities++;
				}
			}

			//If there is a limited amount of hits we sort based on distance and later we only use the first 'effect.MaximumHits' results in the list
			if ((HasMaximumHits) && (eligibleTargets.Length > MaximumHits))
			{
				System.Array.Sort(eligibleTargets, (x, y) => x.SqrDist.CompareTo(y.SqrDist));
			}

			//Now we have to apply effects: Damage, Knockback, Buffs and FXEffects
			int targetCount = HasMaximumHits ? System.Math.Min(eligibleTargets.Length, MaximumHits) : eligibleTargets.Length;
			bool effectWasCrit = Utils.Chance01(CritChanceMultiplier * infoBase.BaseCritChance * (effectOverrides == null ? 1 : effectOverrides.ExtraCritChanceMultiplier));

			float casterBaseDamage = CauseType == CauseType.Physical ? (effectCauser.curPhysicalDamage) : (CauseType == CauseType.Magic ? effectCauser.curMagicalDamage : 0);
			float baseKnockBack = KnockbackMultiplier * infoBase.BaseKnockback;

			for (int i = 0; i < targetCount; i++)
			{
				//Damage / Knockback
				float damage = (infoBase.BaseDamage + casterBaseDamage) * DamageMultiplier * eligibleTargets[i].Multiplier * (effectOverrides == null ? 1 : effectOverrides.ExtraDamageMultiplier);
				float? knockbackAmount = (baseKnockBack != 0) ? (float?)(baseKnockBack * eligibleTargets[i].Multiplier * (effectOverrides == null ? 1 : effectOverrides.ExtraKnockbackMultiplier)) : (null);

				eligibleTargets[i].Target.ChangeHealth(new ChangeInfo(
					effectCauser,
					CauseType,
					DamageElement,
					TargetStat.Health,
					eligibleTargets[i].Target.actuallWorldPosition,
					new Vector3(eligibleTargets[i].ApplyDirection.x * KnockbackAxisMultiplier.x, eligibleTargets[i].ApplyDirection.y * KnockbackAxisMultiplier.y, eligibleTargets[i].ApplyDirection.z * KnockbackAxisMultiplier.z),
					damage,
					effectWasCrit,
					knockbackAmount
					));

				//Buffs
				for (int j = 0; j < BuffsToApply.Length; j++)
				{
					if (BuffStackStyle == BuffStackingStyle.Stack)
					{
						eligibleTargets[i].Target.AddBuff(BuffsToApply[j], effectCauser);
					}
					else if (BuffStackStyle == BuffStackingStyle.Reapply)
					{
						if (!(eligibleTargets[i].Target.TryReapplyBuff(BuffsToApply[j], effectCauser).Item1))
						{
							eligibleTargets[i].Target.AddBuff(BuffsToApply[j], effectCauser);
						}
					}
					else //effect.BuffStackStyle == BuffStackingStyle.DoNothing
					{
						if (!(eligibleTargets[i].Target.HasBuffActive(BuffsToApply[j], effectCauser).HasValue))
						{
							eligibleTargets[i].Target.AddBuff(BuffsToApply[j], effectCauser);
						}
					}
				}

				//FXEffects
				for (int j = 0; j < Effects.Length; j++)
				{
					FXManager.PlayFX(Effects[j].FX, eligibleTargets[i].Target.transform, eligibleTargets[i].Target is Player, eligibleTargets[i].Multiplier);
				}
			}
		}
		private struct CollectedEntitieData
		{
			public Entitie Target;
			public float Multiplier;
			public float SqrDist;
			public Vector3 ApplyDirection;
		}
		#endregion
	}
}