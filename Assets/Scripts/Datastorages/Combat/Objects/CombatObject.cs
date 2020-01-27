using EoE.Entities;
using EoE.Information;
using EoE.Information.Logic;
using System.Collections;
using UnityEngine;

namespace EoE.Combatery
{
	public enum EffectiveDirection { Local = 1, Center = 2, World = 4 }
	public enum InherritDirection { Local = 1, Target = 2, World = 4 }
	public enum DirectionBase { Forward = 1, Right = 2, Up = 4, Back = 8, Left = 16, Down = 32 }
	[System.Flags] public enum ActionType { Casting = 1, Attacking = 2 }
	[System.Flags] public enum MovementType { Walk = 1, Turn = 2 }
	[System.Flags] public enum TargetMask { Self = (1 << 0), Allied = (1 << 1), Enemy = (1 << 2) }
	[System.Flags] public enum ColliderMask { Terrain = (1 << 0), Entities = (1 << 1) }
	[System.Flags] public enum EffectType { ImpulseVelocity = (1 << 0), FX = (1 << 1), AOE = (1 << 2), CreateProjectile = (1 << 3), HealOnCreator = (1 << 4), BuffOnCreator = (1 << 5), CreateRemenants = (1 << 6) }
	public class CombatObject : ScriptableObject
	{
		public bool NoDefinedActionType = false;
		public ActionType ActionType = ActionType.Casting;

		public float BasePhysicalDamage = 0;
		public float BaseMagicalDamage = 0;
		public float BaseKnockback = 0;
		public float BaseCritChance = 0;
		public ObjectCost Cost = new ObjectCost();
		public virtual bool CanActivate(Entity target, float healthCostMultiplier, float manaCostMultiplier, float enduranceCostMultiplier)
		{
			return AllowedAction(target) && (Cost.CanAfford(target, healthCostMultiplier, manaCostMultiplier, enduranceCostMultiplier));
		}
		public bool AllowedAction(Entity target)
		{
			if (!NoDefinedActionType)
			{
				return !IsStoppedAction(target) && !IsActionOnCooldown(target);
			}
			else
				return true;
		}
		public bool IsStoppedAction(Entity target)
		{
			if (!NoDefinedActionType)
			{
				if (ActionType == ActionType.Casting)
				{
					return target.IsCastingStopped;
				}
				else//ActionType == ActionType.Attacking
				{
					return target.IsAttackStopped;
				}
			}
			else
				return false;
		}
		public bool IsActionOnCooldown(Entity target)
		{
			if (!NoDefinedActionType)
			{
				if (ActionType == ActionType.Casting)
				{
					return target.CastingCooldown > 0;
				}
				else//ActionType == ActionType.Attacking
				{
					return target.AttackCooldown > 0;
				}
			}
			else
				return false;
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
		public LogicComponent[] AdditionalConditions = new LogicComponent[0];
		public bool CanAfford(Entity target, float healthCostMultiplier, float manaCostMultiplier, float enduranceCostMultiplier)
		{
			for (int i = 0; i < AdditionalConditions.Length; i++)
			{
				if (!AdditionalConditions[i].True)
					return false;
			}
			float totalHealthCost = Health * healthCostMultiplier;
			float totalManaCost = Mana * manaCostMultiplier;
			float totalEnduranceCost = Endurance * enduranceCostMultiplier;

			bool asPlayerCanAffordEndurance = (target is Player) ? (target as Player).curEndurance >= totalEnduranceCost : true;
			return (target.curHealth >= totalHealthCost) && (target.curMana >= totalManaCost) && asPlayerCanAffordEndurance;
		}
		public void PayCost(Entity target, float healthCostMultiplier, float manaCostMultiplier, float enduranceCostMultiplier)
		{
			float totalHealthCost = Health * healthCostMultiplier;
			float totalManaCost = Mana * manaCostMultiplier;
			float totalEnduranceCost = Endurance * enduranceCostMultiplier;

			if (totalHealthCost != 0)
				target.ChangeHealth(new ChangeInfo(null, CauseType.Magic, TargetStat.Health, totalHealthCost));
			if (totalManaCost != 0)
				target.ChangeMana(new ChangeInfo(null, CauseType.Magic, TargetStat.Mana, totalManaCost));
			if ((totalEnduranceCost != 0) && (target is Player))
				(target as Player).ChangeEndurance(new ChangeInfo(null, CauseType.Magic, TargetStat.Endurance, totalEnduranceCost));
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
	public class RestrictionData
	{
		public ActionType ActionRestrictions = (ActionType)(-1);
		public MovementType MovementRestrictions = (MovementType)(-1);

		public void ApplyRestriction(Entity target, bool state)
		{
			int change = state ? 1 : -1;
			if (ShouldRestrictAction(ActionType.Casting))
				target.CastingStops += change;
			if (ShouldRestrictAction(ActionType.Attacking))
				target.AttackStops += change;

			if (ShouldRestrictMovement(MovementType.Walk))
				target.MovementStops += change;
			if (ShouldRestrictMovement(MovementType.Turn))
				target.TurnStops += change;
		}
		public bool ShouldRestrictAction(ActionType restriction)
		{
			return ((int)ActionRestrictions | (int)restriction) == ((int)ActionRestrictions);
		}
		public bool ShouldRestrictMovement(MovementType restriction)
		{
			return ((int)MovementRestrictions | (int)restriction) == ((int)MovementRestrictions);
		}
	}
}