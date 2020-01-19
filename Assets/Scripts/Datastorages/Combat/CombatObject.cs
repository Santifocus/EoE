using EoE.Entities;
using EoE.Information;
using System.Collections;
using UnityEngine;

namespace EoE.Combatery
{
	public enum EffectiveDirection { Local = 1, Center = 2, World = 4 }
	public enum InherritDirection { Local = 1, Target = 2, World = 4 }
	public enum DirectionBase { Forward = 1, Right = 2, Up = 4, Back = 8, Left = 16, Down = 32 }
	[System.Flags] public enum TargetMask { Self = (1 << 0), Allied = (1 << 1), Enemy = (1 << 2) }
	[System.Flags] public enum ColliderMask { Terrain = (1 << 0), Entities = (1 << 1) }
	[System.Flags] public enum EffectType { ImpulseVelocity = (1 << 0), FX = (1 << 1), AOE = (1 << 2), CreateProjectile = (1 << 3), HealOnCreator = (1 << 4), BuffOnCreator = (1 << 5), CreateRemenants = (1 << 6) }
	public class CombatObject : ScriptableObject
	{
		public float BasePhysicalDamage = 10;
		public float BaseMagicalDamage = 0;
		public float BaseKnockback = 0;
		public float BaseCritChance = 0;
		public ObjectCost Cost = new ObjectCost();

		//Helper functions
		public static (Vector3Int, bool) EnumDirToDir(DirectionBase direction)
		{
			switch (direction)
			{
				case DirectionBase.Forward:
					return (new Vector3Int(0, 0, 1), false);
				case DirectionBase.Back:
					return (new Vector3Int(0, 0, 1), true);
				case DirectionBase.Right:
					return (new Vector3Int(1, 0, 0), false);
				case DirectionBase.Left:
					return (new Vector3Int(1, 0, 0), true);
				case DirectionBase.Up:
					return (new Vector3Int(0, 1, 0), false);
				case DirectionBase.Down:
					return (new Vector3Int(0, 1, 0), true);
				default:
					return (new Vector3Int(0, 0, 1), false);
			}
		}
		public static bool IsAllowedEntitie(Entity target, Entity causer, TargetMask mask)
		{
			bool isSelf = causer == target;
			bool selfIsPlayer = causer is Player;
			bool otherIsPlayer = target is Player;

			//First rule out if the target is is the causer itself
			if (isSelf)
			{
				return HasMaskFlag(TargetMask.Self);
			}
			else
			{
				//Now check if the compared entities are both of the same team ie NonPlayer & NonPlayer ( / Player & Player, not going to happen because its not multiplayer)
				if (selfIsPlayer == otherIsPlayer)
				{
					return HasMaskFlag(TargetMask.Allied);
				}
				else //Player & NonPlayer / NonPlayer & Player
				{
					return HasMaskFlag(TargetMask.Enemy);
				}
			}

			bool HasMaskFlag(TargetMask flag)
			{
				return (flag | mask) == mask;
			}
		}
		public static Vector3 CalculateDirection(InherritDirection directionStyle, InherritDirection fallbackDirectionStyle, DirectionBase direction, Transform originTransform, Vector3 originOffset)
		{
			Vector3 calculatedDirection;
			(Vector3, bool) dirInfo = EnumDirToDir(direction);

			if (directionStyle == InherritDirection.Target)
			{
				calculatedDirection = DirectionFromStyle(fallbackDirectionStyle, originTransform, ref dirInfo);
			}
			else
			{
				calculatedDirection = DirectionFromStyle(directionStyle, originTransform, ref dirInfo);
			}

			return calculatedDirection;
		}
		public static Vector3 CalculateDirection(InherritDirection directionStyle, InherritDirection fallbackDirectionStyle, DirectionBase direction, Entity originEntitie, Vector3 originOffset)
		{
			Vector3 calculatedDirection;
			(Vector3, bool) dirInfo = EnumDirToDir(direction);
			if (directionStyle == InherritDirection.Target)
			{
				if (originEntitie.targetPosition.HasValue)
				{
					calculatedDirection = (originEntitie.targetPosition.Value - (originEntitie.actuallWorldPosition + originOffset)).normalized;
				}
				else
				{
					calculatedDirection = DirectionFromStyle(fallbackDirectionStyle, originEntitie.transform, ref dirInfo);
				}
			}
			else
			{
				calculatedDirection = DirectionFromStyle(directionStyle, originEntitie.transform, ref dirInfo);
			}
			return calculatedDirection;
		}
		private static Vector3 DirectionFromStyle(InherritDirection style, Transform originTransform, ref (Vector3, bool) dirInfo)
		{
			if (style == InherritDirection.World)
			{
				return dirInfo.Item1 * (dirInfo.Item2 ? -1 : 1);
			}
			else //style == InherritDirection.Local
			{
				if (dirInfo.Item1 == new Vector3Int(0, 0, 1))
				{
					return originTransform.forward * (dirInfo.Item2 ? -1 : 1);
				}
				else if (dirInfo.Item1 == new Vector3Int(1, 0, 0))
				{
					return originTransform.right * (dirInfo.Item2 ? -1 : 1);
				}
				else //dir.Item1 == new Vector3Int(0, 1, 0)
				{
					return originTransform.up * (dirInfo.Item2 ? -1 : 1);
				}
			}
		}
	}
	[System.Serializable]
	public class ObjectCost
	{
		public float Health;
		public float Mana;
		public float Endurance;
		public ConditionObject[] AdditionalConditions = new ConditionObject[0];
		public bool CanActivate(Entity target, float healthCostMultiplier, float manaCostMultiplier, float enduranceCostMultiplier)
		{
			for (int i = 0; i < AdditionalConditions.Length; i++)
			{
				if (!AdditionalConditions[i].ConditionMet())
					return false;
			}
			float totalHealthCost = Health * healthCostMultiplier;
			float totalManaCost = Mana * manaCostMultiplier;
			float totalEnduranceCost = Endurance * enduranceCostMultiplier;

			bool asPlayerCanAffordEndurance = (target is Player) ? (target as Player).curEndurance >= totalEnduranceCost : true;
			return (target.curHealth >= totalHealthCost) && (target.curMana >= totalManaCost) && asPlayerCanAffordEndurance;
		}
		public void Activate(Entity target, float healthCostMultiplier, float manaCostMultiplier, float enduranceCostMultiplier)
		{
			float totalHealthCost = Health * healthCostMultiplier;
			float totalManaCost = Mana * manaCostMultiplier;
			float totalEnduranceCost = Endurance * enduranceCostMultiplier;

			if (totalHealthCost != 0)
				target.ChangeHealth(new ChangeInfo(Player.Instance, CauseType.Magic, TargetStat.Health, totalHealthCost));
			if (totalManaCost != 0)
				target.ChangeMana(new ChangeInfo(Player.Instance, CauseType.Magic, TargetStat.Mana, totalManaCost));
			if (totalEnduranceCost != 0 && target is Player)
				(target as Player).ChangeEndurance(new ChangeInfo(Player.Instance, CauseType.Magic, TargetStat.Endurance, totalEnduranceCost));
		}
	}
	[System.Serializable]
	public class ProjectileInfo
	{
		public ProjectileData Projectile;

		public float ExecutionDelay = 0.2f;
		public int ExecutionCount = 1;
		public float ExecutionRepeatDelay = 0.2f;
	}
	[System.Serializable]
	public class CustomFXObject
	{
		public FXObject FX;

		public bool HasPositionOffset = false;
		public Vector3 PositionOffset = Vector3.zero;

		public bool HasRotationOffset = false;
		public Vector3 RotationOffset = Vector3.zero;

		public bool HasCustomScale = false;
		public Vector3 CustomScale = Vector3.one;
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

		public FXInstance[] Activate(Entity activator, CombatObject baseObject, float multiplier = 1, Transform overrideTransform = null, params Entity[] ignoredEntities)
		{
			//Impulse Velocity
			if (HasMaskFlag(EffectType.ImpulseVelocity))
			{
				if(activator is Player)
					Player.Instance.FallDamageCooldown = 0.2f; //Prevents fall damage
				Vector3 direction = CombatObject.CalculateDirection(ImpulseVelocityDirection, ImpulseVelocityFallbackDirection, ImpulseDirectionBase, activator, Vector3.zero);
				activator.entitieForceController.ApplyForce(direction * ImpulseVelocity * multiplier, 1 / ImpulseVelocityFallOffTime, true);
			}

			//FX
			//If we are going to create FX we want this array to be null because it will be re-created in the FXManager
			//otherwise we create a array with the size of 0 so we dont return null
			FXInstance[] createdFXInstances = (HasMaskFlag(EffectType.FX) ? (null) : (new FXInstance[0]));
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
				for(int i = 0; i < HealsOnUser.Length; i++)
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
					if(overrideTransform)
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