using EoE.Entities;
using EoE.Information;
using EoE.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
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

			CreateFX(info.ProjectileInfo[index].CustomEffects, true);
		}
		private void SetupCollider()
		{
			Physics.IgnoreCollision(creator.coll, entitieColl);

			if((info.ProjectileInfo[index].CollideMask | ColliderMask.Terrain) == info.ProjectileInfo[index].CollideMask)
			{
				terrainColl.radius = info.ProjectileInfo[index].TerrainHitboxSize;
			}
			else
			{
				terrainColl.enabled = false;
			}

			if ((info.ProjectileInfo[index].CollideMask | ColliderMask.Entities) == info.ProjectileInfo[index].CollideMask)
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
			if (!isRemenants)
			{
				transform.forward = body.velocity.normalized;
				body.velocity = transform.forward * info.ProjectileInfo[index].FlightSpeed;
			}
		}
		private void OnTriggerEnter(Collider other)
		{
			if (isRemenants)
				return;

			Entitie hit = other.GetComponent<Entitie>();
			if (info.ProjectileInfo[index].DirectHit != null)
			{
				if (CombatObject.IsAllowedEntitie(hit, creator, info.ProjectileInfo[index].DirectHit.AffectedTargets))
				{
					DirectTargetHit(hit);
				}
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
				CreateFX(info.ProjectileInfo[index].CollisionCustomEffects);
			}
		}
		private void DirectTargetHit(Entitie hit)
		{
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
						(Vector3Int, bool) dir = CombatObject.EnumDirToDir(info.ProjectileInfo[index].DirectHit.KnockbackDirection);
						knockbackDirection = dir.Item1 * (dir.Item2 ? -1 : 1);
						break;
					}
			}
			info.ProjectileInfo[index].DirectHit.ActivateEffectSingle(creator, hit, info, knockbackDirection, hit.coll.ClosestPoint(transform.position));
		}
		private void FinishProjectileFlight()
		{
			if (isDead || isRemenants)
				return;

			ActivateSpellEffects(info.ProjectileInfo[index].CollisionEffects);
			CreateFX(info.ProjectileInfo[index].CollisionCustomEffects);
			if (info.ProjectileInfo[index].CreatesRemenants)
			{
				isRemenants = true;
				body.velocity = Vector3.zero;
				StopBoundParticles();
				ActivateSpellEffects(info.ProjectileInfo[index].Remenants.StartEffects);

				for(int i = 0; i < info.ProjectileInfo[index].Remenants.ParticleEffects.Length; i++)
				{
					FXInstance instance = FXManager.PlayFX(info.ProjectileInfo[index].Remenants.ParticleEffects[i], transform, false);
					boundEffects.Add(instance);
				}

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
		private void ActivateSpellEffects(EffectAOE[] effects)
		{
			for (int i = 0; i < effects.Length; i++)
				effects[i].ActivateEffectAOE(creator, transform, info);
		}
		private void CreateFX(CustomFXObject[] effects, bool bind = false)
		{
			for (int i = 0; i < effects.Length; i++)
			{
				FXInstance instance = FXManager.PlayFX(	effects[i].FX, 
														transform, 
														false, 
														1, 
														effects[i].HasCustomOffset ? ((Vector3?)effects[i].CustomOffset) : null,
														effects[i].HasCustomRotationOffset ? ((Vector3?)effects[i].CustomRotation) : null,
														effects[i].HasCustomScale ? ((Vector3?)effects[i].CustomScale) : null
														);
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