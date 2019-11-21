using EoE.Entities;
using EoE.Information;
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
		public void Setup(CasterSettings data, Caster parent)
		{
			this.data = data;
			this.parent = parent;
			transform.forward = parent.transform.forward;
			body.velocity = transform.forward * data.ProjectileFlightSpeed;
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

					FXManager.PlayFX(data.ProjectileDestroyParticles, transform);
					Destroy(gameObject);
				}
			}
			else if(other.gameObject.layer == ConstantCollector.TERRAIN_LAYER)
			{
				FXManager.PlayFX(data.ProjectileDestroyParticles, transform);
				Destroy(gameObject);
			}
		}
	}
}