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

		private Entity creator;
		private CombatData baseData;
		private ProjectileData info;
		private int remainingBounces;
		private float remainingLifeTime;
		private float bounceCooldown;

		private float delayToWhileCast;
		private bool isDead;
		private Vector3 spawnPos;

		private List<FXInstance> boundEffects = new List<FXInstance>();

		private void Start()
		{
			AllProjectiles.Add(this);
		}
		public static Projectile CreateProjectile(CombatData baseData, ProjectileData info, Entity creator, Vector3 direction, Vector3 spawnPos)
		{
			Projectile projectile = Instantiate(GameController.ProjectilePrefab, Storage.ProjectileStorage);
			projectile.transform.position = projectile.spawnPos = spawnPos;

			projectile.baseData = baseData;
			projectile.info = info;
			projectile.creator = creator;

			projectile.transform.forward = direction;
			projectile.body.velocity = direction * info.FlightSpeed;

			projectile.delayToWhileCast = info.WhileTickTime;

			projectile.remainingBounces = info.Bounces;
			projectile.remainingLifeTime = info.Duration;

			projectile.SetupCollider();
			projectile.ActivateActivationEffects(info.StartEffects, true);

			return projectile;
		}
		private void SetupCollider()
		{
			Physics.IgnoreCollision(creator.coll, entitieColl);
			Physics.IgnoreCollision(creator.coll, entitieTriggerColl);

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

			if (bounceCooldown > 0)
				bounceCooldown -= Time.deltaTime;

			if (delayToWhileCast <= 0)
			{
				delayToWhileCast += info.WhileTickTime;
				ActivateActivationEffects(info.WhileEffects, true);
			}

			if (remainingLifeTime < 0)
			{
				FinishProjectileFlight();
			}
		}
		private void FixedUpdate()
		{
			float speed = body.velocity.magnitude;
			if(speed > 0)
				transform.forward = body.velocity / speed;
			body.velocity = transform.forward * info.FlightSpeed;
		}
		private void OnTriggerEnter(Collider other)
		{
			if (info.DirectHit == null)
				return;

			if (other.gameObject.layer == ConstantCollector.ENTITIE_LAYER)
			{
				Entity hit = other.GetComponent<Entity>();
				if (CombatObject.IsAllowedEntitie(hit, creator, info.DirectHit.AffectedTargets))
				{
					DirectTargetHit(hit);
				}
			}
			else //other.gameObject.layer == ConstantCollector.SHIELD_LAYER
			{
				Shield hit = other.GetComponent<Shield>();
				if (hit.creator != creator)
				{
					bool isCrit = Utils.Chance01(info.DirectHit.CritChanceMultiplier * baseData.BaseCritChance);
					float damage = info.DirectHit.DamageMultiplier * baseData.BaseMagicalDamage * (isCrit ? GameController.CurrentGameSettings.CritDamageMultiplier : 1);
					hit.HitShield(damage);

					if (hit.info.ReflectProjectiles)
						Reflect(hit.creator);
				}
			}
		}
		private void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.layer != ConstantCollector.SHIELD_LAYER)
			{
				if (bounceCooldown > 0)
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
			else
			{
				Shield hit = collision.gameObject.GetComponent<Shield>();
				if (hit.creator != creator)
				{
					if (hit.info.ReflectProjectiles)
					{
						Reflect(hit.creator);
					}
					else
					{
						for (int i = 0; i < info.CollisionEffects.Length; i++)
						{
							info.CollisionEffects[i].Activate(creator, baseData, transform, null, hit.creator);
						}
						isDead = true;
						Destroy(gameObject);
					}

					//Apply damage to the shield
					float totalDamage = 0;
					for (int i = 0; i < info.CollisionEffects.Length; i++)
					{
						if (info.CollisionEffects[i].HasMaskFlag(EffectType.AOE))
						{
							for(int j = 0; j < info.CollisionEffects[i].AOEEffects.Length; j++)
							{
								bool wasCrit = Utils.Chance01(info.CollisionEffects[i].AOEEffects[j].CritChanceMultiplier * baseData.BaseCritChance);
								totalDamage += info.CollisionEffects[i].AOEEffects[j].DamageMultiplier * (wasCrit ? GameController.CurrentGameSettings.CritDamageMultiplier : 1) * baseData.BaseMagicalDamage;
							}
						}
					}
					hit.HitShield(totalDamage);
				}
			}
		}
		private void OnBounce()
		{
			remainingBounces--;
			bounceCooldown = BOUNCE_COOLDOWN;

			if (info.CollisionEffectsOnBounce)
			{
				ActivateActivationEffects(info.CollisionEffects, false);
			}
		}
		private void DirectTargetHit(Entity hit)
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
			if (isDead)
				return;

			ActivateActivationEffects(info.CollisionEffects, false);
			isDead = true;
			Destroy(gameObject);
		}
		private void ActivateActivationEffects(ActivationEffect[] activationEffects, bool binding)
		{
			for (int i = 0; i < activationEffects.Length; i++)
			{
				FXInstance[] fxInstances = activationEffects[i].Activate(creator, baseData, transform);
				if (binding)
					boundEffects.AddRange(fxInstances);
			}
		}
		private void StopBoundFX()
		{
			for (int i = 0; i < boundEffects.Count; i++)
			{
				if(boundEffects[i] != null)
					boundEffects[i].FinishFX();
			}
			boundEffects = new List<FXInstance>();
		}
		private void Reflect(Entity newOwner)
		{
			body.velocity = transform.forward * body.velocity.magnitude * -1;
			Physics.IgnoreCollision(creator.coll, entitieColl, false);
			Physics.IgnoreCollision(creator.coll, entitieTriggerColl, false);

			creator = newOwner;
			Physics.IgnoreCollision(creator.coll, entitieColl);
			Physics.IgnoreCollision(creator.coll, entitieTriggerColl);
		}
		private void OnDestroy()
		{
			StopBoundFX();
			AllProjectiles.Remove(this);
		}
		private void OnDrawGizmos()
		{
			if (info == null)
				return;
			if(info.Duration - remainingLifeTime < 0.5f)
				DrawEffectSpheres(info.StartEffects, spawnPos);

			DrawEffectSpheres(info.WhileEffects, transform.position);
		}
		private void DrawEffectSpheres(ActivationEffect[] arrayTarget, Vector3 targetPos)
		{
			for (int i = 0; i < arrayTarget.Length; i++)
			{
				if (arrayTarget[i].HasMaskFlag(EffectType.AOE))
				{
					for (int j = 0; j < arrayTarget[i].AOEEffects.Length; j++)
					{
						Gizmos.color = new Color(0, 1, 0, 0.3f);
						Gizmos.DrawSphere(targetPos, arrayTarget[i].AOEEffects[j].BaseEffectRadius);
						if (arrayTarget[i].AOEEffects[j].ZeroOutDistance > arrayTarget[i].AOEEffects[j].BaseEffectRadius)
						{
							Gizmos.color = Color.red;
							Gizmos.DrawWireSphere(targetPos, arrayTarget[i].AOEEffects[j].ZeroOutDistance);
						}
					}
				}
			}
		}
	}
}