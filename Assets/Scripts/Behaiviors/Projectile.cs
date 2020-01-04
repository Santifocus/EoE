using EoE.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	public class Projectile : MonoBehaviour
	{
		private const float BOUNCE_COOLDOWN = 0.2f;
		public static List<Projectile> AllProjectiles = new List<Projectile>();
		[SerializeField] private Rigidbody body = default;
		[SerializeField] private SphereCollider terrainColl = default;
		[SerializeField] private SphereCollider entitieColl = default;
		[SerializeField] private SphereCollider entitieTriggerColl = default;

		private Entitie creator;
		private CombatObject baseData;
		private ProjectileData info;
		private int remainingBounces;
		private float remainingLifeTime;
		private float bounceCooldown;

		private float delayToWhileCast;
		private bool isDead;
		private bool isRemenants;

		private List<FXInstance> boundEffects = new List<FXInstance>();

		private void Start()
		{
			AllProjectiles.Add(this);
		}
		public static Projectile CreateProjectile(CombatObject baseData, ProjectileData info, Entitie creator, Vector3 direction, Vector3 spawnPos)
		{
			Projectile projectile = Instantiate(GameController.ProjectilePrefab, Storage.ProjectileStorage);
			projectile.transform.position = spawnPos;

			projectile.baseData = baseData;
			projectile.info = info;
			projectile.creator = creator;

			projectile.transform.forward = direction;
			projectile.body.velocity = direction * info.FlightSpeed;

			projectile.delayToWhileCast = GameController.CurrentGameSettings.WhileEffectTickSpeed;

			projectile.remainingBounces = info.Bounces;
			projectile.remainingLifeTime = info.Duration;

			projectile.SetupCollider();
			projectile.CreateFX(info.VisualStartEffects, true);

			return projectile;
		}
		private void SetupCollider()
		{
			Physics.IgnoreCollision(creator.coll, entitieColl);

			if ((info.CollideMask | ColliderMask.Terrain) == info.CollideMask)
			{
				terrainColl.radius = info.TerrainHitboxSize;
			}
			else
			{
				terrainColl.enabled = false;
			}

			if ((info.CollideMask | ColliderMask.Entities) == info.CollideMask)
			{
				entitieColl.radius = entitieTriggerColl.radius = info.EntitieHitboxSize;
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
					delayToWhileCast += GameController.CurrentGameSettings.WhileEffectTickSpeed;
					ActivateSpellEffects(info.Remenants.WhileEffects);
				}
			}
			else
			{
				if (bounceCooldown > 0)
					bounceCooldown -= Time.deltaTime;

				if (delayToWhileCast <= 0)
				{
					delayToWhileCast += GameController.CurrentGameSettings.WhileEffectTickSpeed;
					ActivateSpellEffects(info.WhileEffects);
				}
			}

			if (remainingLifeTime < 0)
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
				body.velocity = transform.forward * info.FlightSpeed;
			}
		}
		private void OnTriggerEnter(Collider other)
		{
			if (isRemenants)
				return;

			Entitie hit = other.GetComponent<Entitie>();
			if (info.DirectHit != null)
			{
				if (CombatObject.IsAllowedEntitie(hit, creator, info.DirectHit.AffectedTargets))
				{
					DirectTargetHit(hit);
				}
			}
		}
		private void OnCollisionEnter(Collision collision)
		{
			if (bounceCooldown > 0 || isRemenants)
				return;

			if ((info.DestroyOnEntiteBounce) && (collision.gameObject.layer == ConstantCollector.ENTITIE_LAYER))
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

			if (info.CollisionEffectsOnBounce)
			{
				ActivateSpellEffects(info.CollisionEffectsAOE);
				CreateFX(info.VisualCollisionEffects);
			}
		}
		private void DirectTargetHit(Entitie hit)
		{
			Vector3 knockbackDirection = transform.forward;
			switch (info.DirectHit.KnockbackOrigin)
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
						(Vector3Int, bool) dir = CombatObject.EnumDirToDir(info.DirectHit.KnockbackDirection);
						knockbackDirection = dir.Item1 * (dir.Item2 ? -1 : 1);
						break;
					}
			}
			info.DirectHit.Activate(creator, hit, baseData, knockbackDirection, hit.coll.ClosestPoint(transform.position));
		}
		private void FinishProjectileFlight()
		{
			if (isDead || isRemenants)
				return;

			ActivateSpellEffects(info.CollisionEffectsAOE);
			CreateFX(info.VisualCollisionEffects);
			if (info.CreatesRemenants)
			{
				isRemenants = true;
				body.velocity = Vector3.zero;
				StopBoundParticles();
				ActivateSpellEffects(info.Remenants.StartEffects);

				for (int i = 0; i < info.Remenants.VisualEffects.Length; i++)
				{
					FXInstance instance = FXManager.PlayFX(info.Remenants.VisualEffects[i], transform, false);
					boundEffects.Add(instance);
				}

				this.remainingLifeTime = info.Remenants.Duration;

				if (info.Remenants.TryGroundRemenants)
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
				effects[i].Activate(creator, transform, baseData);
		}
		private void CreateFX(CustomFXObject[] effects, bool bind = false)
		{
			for (int i = 0; i < effects.Length; i++)
			{
				FXInstance instance = FXManager.PlayFX(effects[i], transform, false, 1);
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
			AllProjectiles.Remove(this);
		}
	}
}