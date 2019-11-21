using EoE.Entities;
using EoE.Information;
using EoE.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Weapons
{
	public class CasterProjectile : MonoBehaviour
	{
		[SerializeField] private Rigidbody body = default;
		[SerializeField] private SphereCollider coll = default;
		private CasterSettings data;
		private Caster parent;
		public void Setup(CasterSettings data, Caster parent, Vector3 projectileDirection)
		{
			this.data = data;
			this.parent = parent;
			transform.forward = projectileDirection;
			body.velocity = transform.forward * data.ProjectileFlightSpeed; 
			coll.radius = data.ProjectileHitboxSize / 2;
		}

		private void OnTriggerEnter(Collider other)
		{
			if(other.gameObject.layer == ConstantCollector.ENTITIE_LAYER)
			{
				Player hitPlayer = other.gameObject.GetComponent<Player>();
				if (hitPlayer)
				{
					float? knockBack = data.ProjectileKnockback > 0 ? ((float?)data.ProjectileKnockback) : null;
					hitPlayer.ChangeHealth(new ChangeInfo(parent, CauseType.Magic, data.ProjectileElement, coll.ClosestPoint(hitPlayer.actuallWorldPosition), transform.forward, data.BaseMagicDamage * data.ProjectileDamageMultiplier, Random.value < data.ProjectileCritChance, knockBack));

					ExplodeProjectile();
				}
			}
			else if(other.gameObject.layer == ConstantCollector.TERRAIN_LAYER)
			{
				ExplodeProjectile();
			}
		}

		private void ExplodeProjectile()
		{
			FXManager.PlayFX(data.ProjectileDestroyParticles, transform);
			float toPlayerDist = (Player.Instance.actuallWorldPosition - transform.position).magnitude;
			float multiplier = 1 - Mathf.Clamp01(toPlayerDist / data.MaxDistanceImpactShake);
			FXManager.PlayFX(data.ProjectileImpactShake, transform, multiplier);
			Destroy(gameObject);
		}
	}
}