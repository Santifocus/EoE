using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	[System.Flags] public enum SpellPart { Cast = (1 << 0), Start = (1 << 1), Projectile = (1 << 2) }
	[System.Flags] public enum SpellMovementRestrictionsMask { WhileCasting = (1 << 0), WhileShooting = (1 << 1) }
	public class Spell : CombatObject
	{
		public float SpellCooldown = 3;

		public SpellPart ContainedParts = (SpellPart)(-1);
		public SpellMovementRestrictionsMask MovementRestrictions = (SpellMovementRestrictionsMask)(-1);
		public SpellPart SubtractManaAtPart = SpellPart.Start;

		public SpellCastPart CastInfo = new SpellCastPart();
		public SpellBeginningPart StartInfo = new SpellBeginningPart();

		public SpellProjectilePart[] ProjectileInfo = new SpellProjectilePart[0];
		public float[] DelayToNextProjectile = new float[0];

		public bool HasSpellPart(SpellPart part)
		{
			return ((int)ContainedParts | (int)part) == ((int)ContainedParts);
		}
	}

	[System.Serializable]
	public class SpellCastPart
	{
		public float Duration = 3;
		public CustomFXObject[] CustomEffects = new CustomFXObject[0];
		public EffectAOE[] StartEffects = new EffectAOE[0];
		public EffectAOE[] WhileEffects = new EffectAOE[0];
	}
	[System.Serializable]
	public class SpellBeginningPart
	{
		public CustomFXObject[] CustomEffects = new CustomFXObject[0];
		public EffectAOE[] Effects = new EffectAOE[0];
	}
	[System.Serializable]
	public class SpellProjectilePart
	{
		//Execution info
		public int ExecutionCount = 1;
		public float DelayPerExecution = 0.1f;

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
	public class SpellRemenantsPart
	{
		public float Duration = 5;
		public ParticleEffect[] ParticleEffects = new ParticleEffect[0];
		public EffectAOE[] StartEffects = new EffectAOE[0];
		public EffectAOE[] WhileEffects = new EffectAOE[0];
		public bool TryGroundRemenants = true;
	}
}