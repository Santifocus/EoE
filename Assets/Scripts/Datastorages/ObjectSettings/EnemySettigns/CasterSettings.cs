using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class CasterSettings : EnemySettings
	{
		//Projectile
		public float ProejctileDamageMultiplier;
		public float ProjectileManaCost;
		public float ProjectileFlightSpeed;
		public FXInstance[] CastingAnnouncement;
		public ParticleEffect ProjectileParticles;
	}
}