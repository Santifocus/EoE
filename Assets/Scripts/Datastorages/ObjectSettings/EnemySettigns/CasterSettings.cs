using EoE.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class CasterSettings : EnemySettings
	{
		//Projectile
		public float ProjectileChargeTime = 2.5f;
		public float ProjectileDamageMultiplier = 1;
		public ElementType ProjectileElement = ElementType.Electro;
		public float ProjectileKnockback = 5;
		public float ProjectileCritChance = 0;
		public float ProjectileManaCost = 5;
		public float ProjectileFlightSpeed = 25;
		public float ProjectileSpellCooldown = 2;
		public float ProjectileHitboxSize = 1.5f;
		public Vector3 ProjectileSpawnOffset = default;
		public CasterProjectile ProjectilePrefab = null;
		public FXObject[] CastingAnnouncement = null;

		public ParticleEffect ProjectileFlyParticles = null;
		public ParticleEffect ProjectileDestroyParticles = null;

		public float MaxDistanceImpactShake = 10;
		public ScreenShake ProjectileImpactShake = null;
	}
}