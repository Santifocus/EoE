using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	public enum EffectiveDirection { Local = 1, Center = 2, World = 4 }
	public enum InherritDirection { Local = 1, Target = 2, World = 4 }
	public enum DirectionBase { Forward = 1, Right = 2, Up = 4, Back = 8, Left = 16, Down = 32 }
	public enum BuffStackingStyle { Stack = 1, Reapply = 2, DoNothing = 4 }
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
	}
	[System.Serializable]
	public class ProjectileData
	{
		//Execution info
		public int ExecutionCount = 1;
		public float DelayPerExecution = 0.2f;

		//Direction
		public InherritDirection DirectionStyle = InherritDirection.Target;
		public InherritDirection FallbackDirectionStyle = InherritDirection.Local;
		public DirectionBase Direction = DirectionBase.Forward;

		//Flight
		public float Duration = 15;
		public float FlightSpeed = 25;
		public Vector3 CreateOffsetToCaster = Vector3.forward;
		public CustomFXObject[] CustomEffects = new CustomFXObject[0];
		public EffectAOE[] StartEffects = new EffectAOE[0];
		public EffectAOE[] WhileEffects = new EffectAOE[0];

		//Collision
		public float EntitieHitboxSize = 1;
		public float TerrainHitboxSize = 0.5f;
		public ColliderMask CollideMask = (ColliderMask)(-1);
		public int Bounces = 0;
		public bool DestroyOnEntiteBounce = true;
		public bool CollisionEffectsOnBounce = true;

		public CustomFXObject[] CollisionCustomEffects = new CustomFXObject[0];
		public EffectAOE[] CollisionEffects = new EffectAOE[0];

		//Direct hit
		public EffectSingle DirectHit;

		//Remenants
		public bool CreatesRemenants = false;
		public SpellRemenantsPart Remenants = new SpellRemenantsPart();
	}
	[System.Serializable]
	public class CustomFXObject
	{
#if UNITY_EDITOR
		public bool openInInspector = false;
#endif
		public FXObject FX;

		public bool HasCustomOffset = false;
		public Vector3 CustomOffset = Vector3.zero;

		public bool HasCustomRotationOffset = false;
		public Vector3 CustomRotation = Vector3.zero;

		public bool HasCustomScale = false;
		public Vector3 CustomScale = Vector3.one;
	}
}