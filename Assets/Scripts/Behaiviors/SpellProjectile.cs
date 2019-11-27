using EoE.Entities;
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
		[SerializeField] private SphereCollider coll = default;

		private Entitie creator;
		private Spell info;
		private int index;
		private int remainingBounces;
		private float remainingLifeTime;
		private float bounceCooldown;

		private float delayToWhileCast;
		private bool isRemenants;

		private void Start()
		{
			AllSpellProjectiles.Add(this);
		}
		public void Setup(Spell info, int index, Entitie creator, Vector3 direction)
		{
			Physics.IgnoreCollision(creator.coll, coll);

			coll.radius = info.ProjectileInfo[index].HitboxSize;
			transform.forward = direction;
			body.velocity = direction * info.ProjectileInfo[index].FlightSpeed;
			delayToWhileCast = GameController.CurrentGameSettings.SpellEffectTickSpeed;

			this.creator = creator;
			this.info = info;
			this.index = index;
			this.remainingBounces = info.ProjectileInfo[index].Bounces;
			this.remainingLifeTime = info.ProjectileInfo[index].Duration;
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
		private void OnCollisionEnter(Collision collision)
		{
			if (bounceCooldown > 0 || isRemenants)
				return;

			if(collision.gameObject.layer == ConstantCollector.ENTITIE_LAYER && HasCollisionFlag(SpellCollideMask.Entities))
			{
				if (info.ProjectileInfo[index].BounceOffEntities && remainingBounces > 0)
				{
					Bounce(collision.contacts[0].normal);
				}
				else
				{
					FinishProjectileFlight();
				}
			}
			else if(HasCollisionFlag(SpellCollideMask.Terrain))// && collision.gameObject.layer == ConstantCollector.TERRAIN_LAYER
			{
				if (remainingBounces > 0)
				{
					Bounce(collision.contacts[0].normal);
				}
				else
				{
					FinishProjectileFlight();
				}
			}
		}
		private bool HasCollisionFlag(SpellCollideMask flag)
		{
			return (info.ProjectileInfo[index].CollideMask | flag) == info.ProjectileInfo[index].CollideMask;
		}
		private void Bounce(Vector3 normal)
		{
			remainingBounces--;
			bounceCooldown = BOUNCE_COOLDOWN;
			body.velocity = Vector3.Reflect(body.velocity, normal);

			if (info.ProjectileInfo[index].CollisionEffectsOnBounce)
				ActivateSpellEffects(info.ProjectileInfo[index].CollisionEffects);
		}
		private void FinishProjectileFlight()
		{
			ActivateSpellEffects(info.ProjectileInfo[index].CollisionEffects);
			if (info.ProjectileInfo[index].CreatesRemenants)
			{
				body.velocity = Vector3.zero;
				ActivateSpellEffects(info.ProjectileInfo[index].Remenants.StartEffects);
				this.remainingLifeTime = info.ProjectileInfo[index].Remenants.Duration;

				if (info.ProjectileInfo[index].Remenants.TryGroundRemenants)
				{
					Fall();
				}
			}
			else
			{
				Destroy(gameObject);
			}
		}
		private void ActivateSpellEffects(SpellEffect[] effects)
		{
			for (int i = 0; i < effects.Length; i++)
				Entitie.ActivateSpellEffect(creator, effects[i], transform, info);
		}
		public void Fall()
		{
			body.useGravity = true;
			body.constraints = ~RigidbodyConstraints.FreezePositionY;
		}
		private void OnDestroy()
		{
			AllSpellProjectiles.Remove(this);
		}
	}
}