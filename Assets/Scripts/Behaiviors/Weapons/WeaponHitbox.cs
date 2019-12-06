using EoE.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	public class WeaponHitbox : MonoBehaviour
	{
		public TrailRenderer trail;

		private WeaponController controller;
		private bool curActive;
		private Collider coll;
		public bool Active { get => curActive; set => ChangeWeaponState(value); }
		private List<Collider> ignoredColliders = new List<Collider>();
		private void OnValidate()
		{
			Collider[] allCollider = GetComponents<Collider>();
			if (allCollider.Length > 1)
			{
				Debug.LogError("Cannot attach more than one collider to a single weapon hitbox!");
				for(int i = 1; i < allCollider.Length; i++)
				{
					DestroyImmediate(allCollider[i]);
				}
			}
		}
		public void Setup(WeaponController controller)
		{
			this.controller = controller;
			if (trail)
				trail.enabled = false;
			coll = GetComponent<Collider>();
			coll.enabled = false;
			Physics.IgnoreCollision(coll, Player.Instance.coll);
		}

		private void ChangeWeaponState(bool state)
		{
			coll.enabled = state;
			curActive = state;
			if (trail)
				trail.enabled = state;
		}

		public void IgnoreCollider(Collider other)
		{
			Physics.IgnoreCollision(coll, other);
			ignoredColliders.Add(other);
		}

		public void ResetCollisionIgnores()
		{
			for(int i = 0; i < ignoredColliders.Count; i++)
			{
				Physics.IgnoreCollision(coll, ignoredColliders[i], false);
			}
			ignoredColliders = new List<Collider>();
		}

		private void OnCollisionEnter(Collision collision)
		{
			controller.HitObject(collision.contacts[0].point, collision.collider);
		}
	}
}