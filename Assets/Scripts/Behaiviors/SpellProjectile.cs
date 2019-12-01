using EoE.Entities;
using EoE.Information;
using EoE.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Weapons
{
	public class SpellProjectile : MonoBehaviour
	{
		private const float BOUNCE_COOLDOWN = 0.2f;
		public static List<SpellProjectile> AllSpellProjectiles = new List<SpellProjectile>();
		[SerializeField] private Rigidbody body = default;
		[SerializeField] private SphereCollider terrainColl = default;
		[SerializeField] private SphereCollider entitieColl = default;
		[SerializeField] private SphereCollider entitieTriggerColl = default;

		private Entitie creator;
		private Spell info;
		private int index;
		private int remainingBounces;
		private float remainingLifeTime;
		private float bounceCooldown;

		private float delayToWhileCast;
		private bool isDead;
		private bool isRemenants;

		private List<FXInstance> boundEffects = new List<FXInstance>();

		private void Start()
		{
			AllSpellProjectiles.Add(this);
		}
		public void Setup(Spell info, int index, Entitie creator, Vector3 direction)
		{
			transform.forward = direction;
			body.velocity = direction * info.ProjectileInfo[index].FlightSpeed;
			delayToWhileCast = GameController.CurrentGameSettings.SpellEffectTickSpeed;

			this.creator = creator;
			this.info = info;
			this.index = index;
			this.remainingBounces = info.ProjectileInfo[index].Bounces;
			this.remainingLifeTime = info.ProjectileInfo[index].Duration;

			SetupCollider();

			CreateParticles(info.ProjectileInfo[index].ParticleEffects, true);
		}
		private void SetupCollider()
		{
			Physics.IgnoreCollision(creator.coll, entitieColl);

			if((info.ProjectileInfo[index].CollideMask | SpellCollideMask.Terrain) == info.ProjectileInfo[index].CollideMask)
			{
				terrainColl.radius = info.ProjectileInfo[index].TerrainHitboxSize;
			}
			else
			{
				terrainColl.enabled = false;
			}

			if ((info.ProjectileInfo[index].CollideMask | SpellCollideMask.Entities) == info.ProjectileInfo[index].CollideMask)
			{
				entitieColl.radius = entitieTriggerColl.radius = info.ProjectileInfo[index].EntitieHitboxSize;
			}
			else
			{
				entitieColl.enabled = false;
			}
		}
		private void Update()
		{
			delayToWhileCast -= Time.deltaTime;
			remainingLifeTime -= Time.deltaTime;

			if (isRemenants)
			{
				if (delayToWhileCast <= 0)
				{
					delayToWhileCast += GameController.CurrentGameSettings.SpellEffectTickSpeed;
					ActivateSpellEffects(info.ProjectileInfo[index].Remenants.WhileEffects);
				}
			}
			else
			{
				if (bounceCooldown > 0)
					bounceCooldown -= Time.deltaTime;

				if (delayToWhileCast <= 0)
				{
					delayToWhileCast += GameController.CurrentGameSettings.SpellEffectTickSpeed;
					ActivateSpellEffects(info.ProjectileInfo[index].WhileEffects);
				}
			}

			if(remainingLifeTime < 0)
			{
				if (isRemenants)
				{
					Destroy(gameObject);
				}
				else
				{
					FinishProjectileFlight();
				}
			}
		}
		private void FixedUpdate()
		{
			if(!isRemenants)
				transform.forward = body.velocity.normalized;
		}
		private void OnTriggerEnter(Collider other)
		{
			if (isRemenants)
				return;

			Entitie hit = other.GetComponent<Entitie>();
			if (Spell.IsAllowedEntitie(hit, creator, info.ProjectileInfo[index].DirectHit.AffectedTargets))
			{
				DirectTargetHit(hit);
			}
		}
		private void OnCollisionEnter(Collision collision)
		{
			if (bounceCooldown > 0 || isRemenants)
				return;

			if((info.ProjectileInfo[index].DestroyOnEntiteBounce) && (collision.gameObject.layer == ConstantCollector.ENTITIE_LAYER))
			{
				FinishProjectileFlight();
				return;
			}

			if (remainingBounces > 0)
			{
				OnBounce();
			}
			else
			{
				FinishProjectileFlight();
			}
		}
		private void OnBounce()
		{
			remainingBounces--;
			bounceCooldown = BOUNCE_COOLDOWN;

			if (info.ProjectileInfo[index].CollisionEffectsOnBounce)
			{
				ActivateSpellEffects(info.ProjectileInfo[index].CollisionEffects);
				CreateParticles(info.ProjectileInfo[index].CollisionParticleEffects);
			}
		}
		private void DirectTargetHit(Entitie hit)
		{
			float damage = info.BaseDamage * info.ProjectileInfo[index].DirectHit.DamageMultiplier;
			float knockback = info.BaseKnockback * info.ProjectileInfo[index].DirectHit.KnockbackMultiplier;
			Vector3 knockbackDirection = transform.forward;

			switch (info.ProjectileInfo[index].DirectHit.KnockbackOrigin)
			{
				case EffectiveDirection.Local:
					break;
				case EffectiveDirection.Center:
					{
						knockbackDirection = (hit.actuallWorldPosition - transform.position).normalized;
						break;
					}
				case EffectiveDirection.World:
					{
						(Vector3Int, bool) dir = Spell.EnumDirToDir(info.ProjectileInfo[index].DirectHit.KnockbackDirection);
						knockbackDirection = dir.Item1 * (dir.Item2 ? -1 : 1);
						break;
					}
			}

			//Apply damage
			hit.ChangeHealth(new ChangeInfo(creator, 
											CauseType.Magic, 
											info.ProjectileInfo[index].DirectHit.DamageElement, 
											TargetStat.Health, 
											hit.coll.ClosestPoint(transform.position), 
											knockbackDirection, 
											damage, 
											Random.value < info.ProjectileInfo[index].DirectHit.CritChance, 
											knockback == 0 ? null : (float?)knockback));
			//Give buffs
			for (int i = 0; i < info.ProjectileInfo[index].DirectHit.BuffsToApply.Length; i++)
			{
				if (info.ProjectileInfo[index].DirectHit.BuffStackStyle == BuffStackingStyle.Stack)
				{
					hit.AddBuff(info.ProjectileInfo[index].DirectHit.BuffsToApply[i], creator);
				}
				else if (info.ProjectileInfo[index].DirectHit.BuffStackStyle == BuffStackingStyle.Reapply)
				{
					if (!(hit.TryReapplyBuff(info.ProjectileInfo[index].DirectHit.BuffsToApply[i], creator).Item1))
					{
						hit.AddBuff(info.ProjectileInfo[index].DirectHit.BuffsToApply[i], creator);
					}
				}
				else //effect.BuffStackStyle == BuffStackingStyle.DoNothing
				{
					if (!(hit.HasBuffActive(info.ProjectileInfo[index].DirectHit.BuffsToApply[i], creator)))
					{
						hit.AddBuff(info.ProjectileInfo[index].DirectHit.BuffsToApply[i], creator);
					}
				}
			}

			//FX
			for (int i = 0; i < info.ProjectileInfo[index].DirectHit.Effects.Length; i++)
			{
				FXManager.PlayFX(info.ProjectileInfo[index].DirectHit.Effects[i], hit.transform, hit is Player);
			}
		}
		private void FinishProjectileFlight()
		{
			if (isDead || isRemenants)
				return;

			ActivateSpellEffects(info.ProjectileInfo[index].CollisionEffects);
			CreateParticles(info.ProjectileInfo[index].CollisionParticleEffects);
			if (info.ProjectileInfo[index].CreatesRemenants)
			{
				isRemenants = true;
				body.velocity = Vector3.zero;
				StopBoundParticles();
				ActivateSpellEffects(info.ProjectileInfo[index].Remenants.StartEffects);
				CreateParticles(info.ProjectileInfo[index].Remenants.ParticleEffects, true);

				this.remainingLifeTime = info.ProjectileInfo[index].Remenants.Duration;

				if (info.ProjectileInfo[index].Remenants.TryGroundRemenants)
				{
					Fall();
				}
			}
			else
			{
				body.velocity = Vector3.zero;
				isDead = true;
				Destroy(gameObject);
			}
		}
		private void ActivateSpellEffects(SpellEffect[] effects)
		{
			for (int i = 0; i < effects.Length; i++)
				Entitie.ActivateSpellEffect(creator, effects[i], transform, info);
		}
		private void CreateParticles(ParticleEffect[] particles, bool bind = false)
		{
			for (int i = 0; i < particles.Length; i++)
			{
				FXInstance instance = FXManager.PlayFX(particles[i], transform, false);
				if (bind)
					boundEffects.Add(instance);
			}
		}
		private void StopBoundParticles()
		{
			for (int i = 0; i < boundEffects.Count; i++)
			{
				boundEffects[i].FinishFX();
			}
			boundEffects = new List<FXInstance>();
		}
		public void Fall()
		{
			entitieColl.enabled = entitieTriggerColl.enabled = false;

			terrainColl.enabled = true;
			terrainColl.material = null;
			terrainColl.radius = 0.5f;

			body.useGravity = true;
			body.constraints = ~RigidbodyConstraints.FreezePositionY;
		}
		private void OnDestroy()
		{
			StopBoundParticles();
			AllSpellProjectiles.Remove(this);
		}
	}
}