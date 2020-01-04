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
	[System.Flags] public enum EffectType { ImpulseVelocity = (1 << 0), FX = (1 << 1), AOE = (1 << 2), CreateProjectile = (1 << 3), HealOnUser = (1 << 4), BuffOnUser = (1 << 5) }
	public class CombatObject : ScriptableObject
	{
		public float BaseDamage = 10;
		public float BaseHealthCost = 0;
		public float BaseManaCost = 0;
		public float BaseEnduranceCost = 0;
		public float BaseKnockback = 0;
		public float BaseCritChance = 0;

		public bool CheckIfCanActivateCost(Entitie target, float healthCostMultiplier, float manaCostMultiplier, float enduranceCostMultiplier)
		{
			float totalHealthCost = BaseHealthCost * healthCostMultiplier;
			float totalManaCost = BaseManaCost * manaCostMultiplier;
			float totalEnduranceCost = BaseEnduranceCost * enduranceCostMultiplier;

			bool asPlayerCanAffordEndurance = (target is Player) ? (target as Player).curEndurance >= totalEnduranceCost : true;
			return (target.curHealth >= totalHealthCost) && (target.curMana >= totalManaCost) && asPlayerCanAffordEndurance;
		}
		public void ActivateCost(Entitie target, float healthCostMultiplier, float manaCostMultiplier, float enduranceCostMultiplier)
		{
			float totalHealthCost = BaseHealthCost * healthCostMultiplier;
			float totalManaCost = BaseManaCost * manaCostMultiplier;
			float totalEnduranceCost = BaseEnduranceCost * enduranceCostMultiplier;

			if (totalHealthCost != 0)
				target.ChangeHealth(new ChangeInfo(Player.Instance, CauseType.Magic, TargetStat.Health, totalHealthCost));
			if (totalManaCost != 0)
				target.ChangeMana(new ChangeInfo(Player.Instance, CauseType.Magic, TargetStat.Mana, totalManaCost));
			if (totalEnduranceCost != 0 && target is Player)
				(target as Player).ChangeEndurance(new ChangeInfo(Player.Instance, CauseType.Magic, TargetStat.Endurance, totalEnduranceCost));
		}

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
		public static bool IsAllowedEntitie(Entitie target, Entitie causer, TargetMask mask)
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
		public static Vector3 CalculateDirection(InherritDirection directionStyle, InherritDirection fallbackDirectionStyle, DirectionBase direction, Entitie originEntitie, Vector3 originOffset)
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
					calculatedDirection = DirectionFromStyle(fallbackDirectionStyle);
				}
			}
			else
			{
				calculatedDirection = DirectionFromStyle(directionStyle);
			}
			return calculatedDirection;
			Vector3 DirectionFromStyle(InherritDirection style)
			{
				if (style == InherritDirection.World)
				{
					return dirInfo.Item1 * (dirInfo.Item2 ? -1 : 1);
				}
				else //style == InherritDirection.Local
				{
					if (dirInfo.Item1 == new Vector3Int(0, 0, 1))
					{
						return originEntitie.transform.forward * (dirInfo.Item2 ? -1 : 1);
					}
					else if (dirInfo.Item1 == new Vector3Int(1, 0, 0))
					{
						return originEntitie.transform.right * (dirInfo.Item2 ? -1 : 1);
					}
					else //dir.Item1 == new Vector3Int(0, 1, 0)
					{
						return originEntitie.transform.up * (dirInfo.Item2 ? -1 : 1);
					}
				}
			}
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
		public HealTargetInfo[] HealEffects = new HealTargetInfo[0];

		//Buffs
		public BuffStackingStyle StackingStyle = BuffStackingStyle.Reapply;
		public Buff[] BuffsToApply = new Buff[0];

		public void Activate(Entitie activator, CombatObject baseObject)
		{
			if (HasMaskFlag(EffectType.ImpulseVelocity))
			{
				Vector3 direction = CombatObject.CalculateDirection(ImpulseVelocityDirection, ImpulseVelocityFallbackDirection, ImpulseDirectionBase, activator, Vector3.zero);
				activator.entitieForceController.ApplyForce(direction * ImpulseVelocity, 1 / ImpulseVelocityFallOffTime, true);
			}
			if (HasMaskFlag(EffectType.FX))
			{
				for (int i = 0; i < FXObjects.Length; i++)
				{
					FXManager.PlayFX(FXObjects[i], activator.transform, activator is Player);
				}
			}
			if (HasMaskFlag(EffectType.AOE))
			{
				for (int i = 0; i < AOEEffects.Length; i++)
				{
					AOEEffects[i].Activate(activator, activator.transform, baseObject);
				}
			}
			if (HasMaskFlag(EffectType.CreateProjectile))
			{
				GameController.Instance.StartCoroutine(ProjectileCreation(activator, baseObject));
			}
			if (HasMaskFlag(EffectType.HealOnUser))
			{
				for(int i = 0; i < HealEffects.Length; i++)
				{
					HealEffects[i].Activate(activator);
				}
			}
			if (HasMaskFlag(EffectType.BuffOnUser))
			{
				for(int i = 0; i < BuffsToApply.Length; i++)
				{
					Buff.ApplyBuff(BuffsToApply[i], activator, activator, StackingStyle);
				}
			}
		}

		#region ProjectileCreation
		private IEnumerator ProjectileCreation(Entitie activator, CombatObject baseObject)
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
					CreateProjectile(activator, baseObject, ProjectileInfos[i].Projectile);
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
		private void CreateProjectile(Entitie activator, CombatObject baseObject, ProjectileData data)
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
			Projectile.CreateProjectile(baseObject, data, activator, direction, activator.actuallWorldPosition + spawnOffset);
		}
		#endregion

		public bool HasMaskFlag(EffectType flag)
		{
			return (flag | ContainedEffectType) == ContainedEffectType;
		}
	}
}