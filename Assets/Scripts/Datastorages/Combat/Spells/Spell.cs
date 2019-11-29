using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Weapons
{
	[System.Flags] public enum SpellCollideMask { Terrain = (1 << 0), Entities = (1 << 1) }
	[System.Flags] public enum SpellTargetMask { Self = (1 << 0), Allied = (1 << 1), Enemy = (1 << 2) }
	[System.Flags] public enum SpellPart { Cast = (1 << 0), Start = (1 << 1), Projectile = (1 << 2) }
	[System.Flags] public enum SpellMovementRestrictionsMask { WhileCasting = (1 << 0), WhileShooting = (1 << 1) }
	public enum EffectiveDirection { Local = 1, Center = 2, World = 4 }
	public enum InherritDirection { Local = 1, Target = 2, World = 4}
	public enum DirectionBase { Forward = 1, Right = 2, Up = 4, Back = 8, Left = 16, Down = 32 }
	public enum BuffStackingStyle { Stack = 1, Reapply = 2, DoNothing = 4 }
	public class Spell : ScriptableObject
	{
		public float ManaCost = 10;
		public float BaseDamage = 10;
		public float BaseKnockback = 0;
		public float SpellCooldown = 3;

		public SpellPart ContainedParts = (SpellPart)7;
		public SpellMovementRestrictionsMask MovementRestrictions = (SpellMovementRestrictionsMask)3;

		public SpellCastPart CastInfo = new SpellCastPart();
		public SpellBeginningPart StartInfo = new SpellBeginningPart();

		public SpellProjectilePart[] ProjectileInfo = new SpellProjectilePart[0];
		public float[] DelayToNextProjectile = new float[0];

		public bool HasPart(SpellPart part)
		{
			return ((int)ContainedParts | (int)part) == ((int)ContainedParts);
		}
		//Helper functions
		public static bool IsAllowedEntitie(Entitie target, Entitie caster, SpellTargetMask mask)
		{
			bool isSelf = caster == target;
			bool selfIsPlayer = caster is Player;
			bool otherIsPlayer = target is Player;

			//First rule out if the target is self
			if (isSelf)
			{
				return HasMaskFlag(SpellTargetMask.Self);
			}
			else
			{
				//Now check if the compared entities are both of the same team ie NonPlayer & NonPlayer ( / Player & Player, not going to happen because its not multiplayer)
				if (selfIsPlayer == otherIsPlayer)
				{
					return HasMaskFlag(SpellTargetMask.Allied);
				}
				else //Player & NonPlayer / NonPlayer & Player
				{
					return HasMaskFlag(SpellTargetMask.Enemy);
				}
			}

			bool HasMaskFlag(SpellTargetMask flag)
			{
				return (flag | mask) == mask;
			}
		}
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
	}

	[System.Serializable]
	public class SpellCastPart
	{
		public float Duration = 3;
		public ParticleEffect[] ParticleEffects = new ParticleEffect[0];
		public SpellEffect[] StartEffects = new SpellEffect[0];
		public SpellEffect[] WhileEffects = new SpellEffect[0];
	}
	[System.Serializable]
	public class SpellBeginningPart
	{
		public ParticleEffect[] ParticleEffects = new ParticleEffect[0];
		public SpellEffect[] Effects = new SpellEffect[0];
	}
	[System.Serializable]
	public class SpellProjectilePart
	{
		public InherritDirection DirectionStyle = InherritDirection.Target;
		public InherritDirection FallbackDirectionStyle = InherritDirection.Local;
		public DirectionBase Direction = DirectionBase.Forward;

		public float Duration = 15;
		public float FlightSpeed = 25;
		public Vector3 CreateOffsetToCaster = Vector3.forward;
		public ParticleEffect[] ParticleEffects = new ParticleEffect[0];
		public SpellEffect[] StartEffects = new SpellEffect[0];
		public SpellEffect[] WhileEffects = new SpellEffect[0];

		//Collision
		public float EntitieHitboxSize = 1;
		public float TerrainHitboxSize = 0.5f;
		public SpellCollideMask CollideMask = (SpellCollideMask)3;
		public int Bounces = 0;
		public bool DestroyOnEntiteBounce = true;
		public bool CollisionEffectsOnBounce = true;

		public ParticleEffect[] CollisionParticleEffects = new ParticleEffect[0];
		public SpellEffect[] CollisionEffects = new SpellEffect[0];

		//Direct hit
		public ProjectileDirectHit DirectHit = new ProjectileDirectHit();
		[System.Serializable]
		public class ProjectileDirectHit
		{
			public SpellTargetMask AffectedTargets = SpellTargetMask.Enemy;

			public ElementType DamageElement = ElementType.None;
			public float DamageMultiplier = 0;
			public float CritChance = 0;

			public float KnockbackMultiplier = 0;
			public EffectiveDirection KnockbackOrigin = EffectiveDirection.Center;
			public DirectionBase KnockbackDirection = DirectionBase.Forward;

			public BuffStackingStyle BuffStackStyle = BuffStackingStyle.Reapply;
			public Buff[] BuffsToApply = new Buff[0];
			public FXObject[] Effects = new FXObject[0];
		}

		//Remenants
		public bool CreatesRemenants = false;
		public SpellRemenantsPart Remenants = new SpellRemenantsPart();
	}
	[System.Serializable]
	public class SpellRemenantsPart
	{
		public float Duration = 5;
		public ParticleEffect[] ParticleEffects = new ParticleEffect[0];
		public SpellEffect[] StartEffects = new SpellEffect[0];
		public SpellEffect[] WhileEffects = new SpellEffect[0];
		public bool TryGroundRemenants = true;
	}
}