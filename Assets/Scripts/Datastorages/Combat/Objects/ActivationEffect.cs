using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	[System.Flags] public enum EffectType 
	{ 
		ImpulseVelocity = (1 << 0),
		FX = (1 << 1),
		AOE = (1 << 2),
		CreateProjectile = (1 << 3),
		HealOnCreator = (1 << 4),
		BuffOnCreator = (1 << 5),
		CreateRemenants = (1 << 6),
		PlayerItemChange = (1 << 7)
	}
	[System.Serializable]
	public class ActivationEffect
	{
		public float ChanceToActivate = 1;

		public EffectType ContainedEffectType = 0;

		//Impulse Velocity
		public float ImpulseVelocity = 5;
		public float ImpulseVelocityFallOffTime = 0.5f;
		public InherritDirection ImpulseVelocityDirection = InherritDirection.Target;
		public InherritDirection ImpulseVelocityFallbackDirection = InherritDirection.Local;
		public DirectionBase ImpulseDirectionBase = DirectionBase.Forward;

		//FX
		public CustomFXObject[] FXObjects = new CustomFXObject[0];

		//AOE
		public EffectAOE[] AOEEffects = new EffectAOE[0];

		//Projectile
		public ProjectileInfo[] ProjectileInfos = new ProjectileInfo[0];

		//HealEffects
		public HealTargetInfo[] HealsOnUser = new HealTargetInfo[0];

		//Buffs
		public BuffStackingStyle StackingStyle = BuffStackingStyle.Reapply;
		public Buff[] BuffsOnUser = new Buff[0];

		//Remenants
		public RemenantsData[] CreatedRemenants = new RemenantsData[0];

		//PlayerItemChange
		public PlayerItemChange[] PlayerItemChanges = new PlayerItemChange[0];

		public FXInstance[] Activate(Entity activator, CombatObject baseObject, float multiplier = 1, Transform overrideTransform = null, params Entity[] ignoredEntities)
		{
			//If we are going to create FX we want this array to be null because it will be re-created in the FXManager
			//otherwise we create a array with the size of 0 so we dont return null
			FXInstance[] createdFXInstances = (HasMaskFlag(EffectType.FX) ? (null) : (new FXInstance[0]));

			//If the chance to activate doesnt work out we stop here and return the empty FXInstance array
			if (!Utils.Chance01(ChanceToActivate))
			{
				return createdFXInstances;
			}

			//Impulse Velocity
			if (HasMaskFlag(EffectType.ImpulseVelocity))
			{
				if (activator is Player)
					Player.Instance.FallDamageCooldown = 0.2f; //Prevents fall damage
				Vector3 direction = CombatObject.CalculateDirection(ImpulseVelocityDirection, ImpulseVelocityFallbackDirection, ImpulseDirectionBase, activator, Vector3.zero);
				activator.entitieForceController.ApplyForce(direction * ImpulseVelocity * multiplier, 1 / ImpulseVelocityFallOffTime, true);
			}

			//FX
			if (HasMaskFlag(EffectType.FX))
			{
				FXManager.ExecuteFX(FXObjects, overrideTransform ?? activator.transform, activator is Player, out createdFXInstances, multiplier);
			}

			//AOE
			if (HasMaskFlag(EffectType.AOE))
			{
				EffectOverrides overrides = new EffectOverrides()
				{
					ExtraDamageMultiplier = multiplier,
					ExtraKnockbackMultiplier = multiplier,
					ExtraCritChanceMultiplier = multiplier,
					EffectMultiplier = multiplier,
				};
				for (int i = 0; i < AOEEffects.Length; i++)
				{
					AOEEffects[i].Activate(activator, overrideTransform ?? activator.transform, baseObject, overrides, ignoredEntities);
				}
			}

			//CreateProjectile
			if (HasMaskFlag(EffectType.CreateProjectile))
			{
				GameController.Instance.StartCoroutine(ProjectileCreation(activator, baseObject, overrideTransform, multiplier));
			}

			//HealOnCreator
			if (HasMaskFlag(EffectType.HealOnCreator))
			{
				for (int i = 0; i < HealsOnUser.Length; i++)
				{
					HealsOnUser[i].Activate(activator, multiplier);
				}
			}

			//BuffOnCreator
			if (HasMaskFlag(EffectType.BuffOnCreator))
			{
				for (int i = 0; i < BuffsOnUser.Length; i++)
				{
					Buff.ApplyBuff(BuffsOnUser[i], activator, activator, multiplier, StackingStyle);
				}
			}

			//CreateRemenants
			if (HasMaskFlag(EffectType.CreateRemenants))
			{
				for (int i = 0; i < CreatedRemenants.Length; i++)
				{
					Remenants.CreateRemenants(baseObject, CreatedRemenants[i], activator, (overrideTransform ?? activator.transform).position, multiplier);
				}
			}

			//Change Player Items
			if (HasMaskFlag(EffectType.PlayerItemChange))
			{
				for (int i = 0; i < PlayerItemChanges.Length; i++)
				{
					PlayerItemChanges[i].Activate();
				}
			}
			return createdFXInstances;
		}

		#region ProjectileCreation
		private IEnumerator ProjectileCreation(Entity activator, CombatObject baseObject, Transform overrideTransform, float multiplier)
		{
			for (int i = 0; i < ProjectileInfos.Length; i++)
			{
				float timer = 0;
				while (timer < ProjectileInfos[i].ExecutionDelay)
				{
					yield return new WaitForEndOfFrame();
					timer += Time.deltaTime;
					if (activator.IsStunned)
						goto ProjectileCreationFinished;
				}

				for (int j = 0; j < ProjectileInfos[i].ExecutionCount; j++)
				{
					if (overrideTransform)
						CreateProjectile(activator, overrideTransform, baseObject, ProjectileInfos[i].Projectile, multiplier);
					else
						CreateProjectile(activator, baseObject, ProjectileInfos[i].Projectile, multiplier);

					if (j < ProjectileInfos[i].ExecutionCount - 1)
					{
						float repeatTimer = 0;
						while (repeatTimer < ProjectileInfos[i].ExecutionRepeatDelay)
						{
							yield return new WaitForEndOfFrame();
							repeatTimer += Time.deltaTime;
							if (activator.IsStunned)
								goto ProjectileCreationFinished;
						}
					}
				}
			}
		ProjectileCreationFinished:;
		}
		private void CreateProjectile(Entity activator, CombatObject baseObject, ProjectileData data, float multiplier)
		{
			//Calculate the spawnoffset
			Vector3 spawnOffset = data.CreateOffsetToCaster.x * activator.transform.right + data.CreateOffsetToCaster.y * activator.transform.up + data.CreateOffsetToCaster.z * activator.transform.forward;

			//First find out what direction the projectile should fly
			Vector3 direction = CombatObject.CalculateDirection(data.DirectionStyle,
																data.FallbackDirectionStyle,
																data.Direction,
																activator,
																spawnOffset
																);
			Projectile.CreateProjectile(baseObject, data, activator, direction, activator.actuallWorldPosition + spawnOffset, multiplier);
		}
		private void CreateProjectile(Entity activator, Transform originTransform, CombatObject baseObject, ProjectileData data, float multiplier)
		{
			//Calculate the spawnoffset
			Vector3 spawnOffset = data.CreateOffsetToCaster.x * originTransform.right + data.CreateOffsetToCaster.y * originTransform.up + data.CreateOffsetToCaster.z * originTransform.forward;

			//First find out what direction the projectile should fly
			Vector3 direction = CombatObject.CalculateDirection(data.DirectionStyle,
																data.FallbackDirectionStyle,
																data.Direction,
																originTransform,
																spawnOffset
																);
			Projectile.CreateProjectile(baseObject, data, activator, direction, originTransform.transform.position + spawnOffset, multiplier);
		}
		#endregion

		public bool HasMaskFlag(EffectType flag)
		{
			return (flag | ContainedEffectType) == ContainedEffectType;
		}
	}
}