using EoE.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class CasterSettings : EnemySettings
	{
		//Projectile
		public float ProjectileChargeTime;
		public float ProjectileDamageMultiplier;
		public ElementType ProjectileElement;
		public float ProjectileKnockback;
		public float ProjectileCritChance;
		public float ProjectileManaCost;
		public float ProjectileFlightSpeed;
		public float ProjectileSpellCooldown;
		public float ProjectileHitboxSize;
		public Vector3 ProjectileSpawnOffset;
		public CasterProjectile ProjectilePrefab;
		public FXInstance[] CastingAnnouncement;
		public ParticleEffect ProjectileFlyParticles;
		public ParticleEffect ProjectileDestroyParticles;
	}
}