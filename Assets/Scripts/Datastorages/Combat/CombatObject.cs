using EoE.Entities;
using EoE.Information;
using UnityEngine;

namespace EoE.Combatery
{
	public enum EffectiveDirection { Local = 1, Center = 2, World = 4 }
	public enum InherritDirection { Local = 1, Target = 2, World = 4 }
	public enum DirectionBase { Forward = 1, Right = 2, Up = 4, Back = 8, Left = 16, Down = 32 }
	[System.Flags] public enum TargetMask { Self = (1 << 0), Allied = (1 << 1), Enemy = (1 << 2) }
	[System.Flags] public enum ColliderMask { Terrain = (1 << 0), Entities = (1 << 1) }
	public class CombatObject : ScriptableObject
	{
		public float BaseDamage = 10;
		public float BaseManaCost = 0;
		public float BaseEnduranceCost = 0;
		public float BaseKnockback = 0;
		public float BaseCritChance = 0;

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
}