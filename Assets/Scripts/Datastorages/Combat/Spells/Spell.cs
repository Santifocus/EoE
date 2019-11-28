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

		public float HitboxSize = 1;
		public float Duration = 15;
		public float FlightSpeed = 25;
		public Vector3 CreateOffsetToCaster = Vector3.forward;
		public ParticleEffect[] ParticleEffects = new ParticleEffect[0];
		public SpellEffect[] StartEffects = new SpellEffect[0];
		public SpellEffect[] WhileEffects = new SpellEffect[0];

		//Collision
		public SpellCollideMask CollideMask = (SpellCollideMask)3;
		public int Bounces = 0;
		public bool BounceOffEntities = false;
		public bool CollisionEffectsOnBounce = true;

		public ParticleEffect[] CollisionParticleEffects = new ParticleEffect[0];
		public SpellEffect[] CollisionEffects = new SpellEffect[0];

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