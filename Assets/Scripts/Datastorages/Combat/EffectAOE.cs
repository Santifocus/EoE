using EoE.Entities;
using EoE.Information;
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
		public void Activate(Entity effectCauser, Transform origin, CombatObject infoBase, EffectOverrides effectOverrides = null, params Entity[] ignoredEntities)
		{
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
			List<int> ignoredIndexes = new List<int>(ignoredEntities.Length);

			for (int i = 0; i < Entity.AllEntities.Count; i++)
			{
				bool isIgnored = false;
				for(int j = 0; j < ignoredEntities.Length; j++)
				{
					if(Entity.AllEntities[i] == ignoredEntities[j])
					{
						isIgnored = true;
						ignoredIndexes.Add(i);
						break;
					}

				}
				if (isIgnored)
					continue;

				if ((Entity.AllEntities[i].actuallWorldPosition - origin.position).sqrMagnitude < outerSphereDist)
				{
					//Check if this entitie should be a targetable entitie
					if (CombatObject.IsAllowedEntitie(Entity.AllEntities[i], effectCauser, AffectedTargets))
						requiredCapacity++;
				}
			}
			CollectedEntitieData[] eligibleTargets = new CollectedEntitieData[requiredCapacity];

			int addedEntities = 0;
			int checkedIgnoredIndexes = 0;
			for (int i = 0; i < Entity.AllEntities.Count; i++)
			{
				if (ignoredIndexes.Count > checkedIgnoredIndexes && ignoredIndexes[checkedIgnoredIndexes] == i)
				{
					checkedIgnoredIndexes++;
					continue;
				}

				Vector3 dif = Entity.AllEntities[i].actuallWorldPosition - origin.position;
				float sqrDist = dif.sqrMagnitude;

				//Generally onbly allow to keep going if the distance is smaller then the other sphere distance
				if (sqrDist < outerSphereDist)
				{
					//Check if this entitie should be a targetable entitie
					if (!CombatObject.IsAllowedEntitie(Entity.AllEntities[i], effectCauser, AffectedTargets))
						continue;

					CollectedEntitieData data = new CollectedEntitieData()
					{
						Target = Entity.AllEntities[i],
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

			//Override implementation
			ElementType effectElement = (effectOverrides == null) ? DamageElement : (effectOverrides.OverridenElement.HasValue ? effectOverrides.OverridenElement.Value : DamageElement);
			CauseType effectCause = (effectOverrides == null) ? CauseType : (effectOverrides.OverridenCauseType.HasValue ? effectOverrides.OverridenCauseType.Value : CauseType);
			float overrideDamageMultiplier = effectOverrides == null ? 1 : effectOverrides.ExtraDamageMultiplier;
			float overrideKnockbackMultiplier = effectOverrides == null ? 1 : effectOverrides.ExtraKnockbackMultiplier;
			float overrideCritChanceMultiplier = effectOverrides == null ? 1 : effectOverrides.ExtraCritChanceMultiplier;
			float overrideEffectMultiplier = effectOverrides == null ? 1 : effectOverrides.EffectMultiplier;

			//Now we calculate damage and knockback
			float baseDamage = (effectCause == CauseType.Physical ? infoBase.BasePhysicalDamage : infoBase.BaseMagicalDamage) * DamageMultiplier;
			float baseKnockBack = infoBase.BaseKnockback * KnockbackMultiplier;
			float critChance = infoBase.BaseCritChance * CritChanceMultiplier;

			baseDamage *= overrideDamageMultiplier;
			baseKnockBack *= overrideKnockbackMultiplier;
			critChance *= overrideCritChanceMultiplier;

			bool isCrit = Utils.Chance01(critChance);

			int targetCount = HasMaximumHits ? System.Math.Min(eligibleTargets.Length, MaximumHits) : eligibleTargets.Length;
			for (int i = 0; i < targetCount; i++)
			{
				//Damage / Knockback
				float damage = baseDamage * eligibleTargets[i].Multiplier;
				float knockback = baseKnockBack * eligibleTargets[i].Multiplier;

				eligibleTargets[i].Target.ChangeHealth(new ChangeInfo(
												effectCauser,
												effectCause,
												effectElement,
												TargetStat.Health,
												eligibleTargets[i].Target.actuallWorldPosition,
												new Vector3(eligibleTargets[i].ApplyDirection.x * KnockbackAxisMultiplier.x, 
															eligibleTargets[i].ApplyDirection.y * KnockbackAxisMultiplier.y, 
															eligibleTargets[i].ApplyDirection.z * KnockbackAxisMultiplier.z),
												damage,
												isCrit,
												(knockback != 0) ? (float?)knockback : (null)
												));

				float effectMultiplier = eligibleTargets[i].Multiplier * overrideEffectMultiplier;
				//Buffs
				for (int j = 0; j < BuffsToApply.Length; j++)
				{
					Buff.ApplyBuff(BuffsToApply[j], eligibleTargets[i].Target, effectCauser, effectMultiplier, BuffStackStyle);
				}

				//FXEffects
				for (int j = 0; j < Effects.Length; j++)
				{
					FXManager.PlayFX(Effects[j].FX, eligibleTargets[i].Target.transform, eligibleTargets[i].Target is Player, effectMultiplier);
				}
			}
		}
		private struct CollectedEntitieData
		{
			public Entity Target;
			public float Multiplier;
			public float SqrDist;
			public Vector3 ApplyDirection;
		}
		#endregion
	}
}