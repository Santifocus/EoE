using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Weapons
{
	public class WeaponHitbox : MonoBehaviour
	{
		public TrailRenderer trail;

		private WeaponController controller;
		private bool curActive;
		private Collider coll;
		public bool Active { get => curActive; set => ChangeWeaponState(value); }
		public void Setup(WeaponController controller)
		{
			this.controller = controller;
			if (trail)
				trail.enabled = false;
			if(GetComponents<Collider>().Length > 1)
				Debug.LogError("Cannot attach more than one collider to a single weapon hitbox!");
			coll = GetComponent<Collider>();
			coll.enabled = false;
		}

		private void ChangeWeaponState(bool state)
		{
			coll.enabled = state;
			curActive = state;
			if(trail)
				trail.enabled = state;
		}

		private void OnTriggerEnter(Collider other)
		{
			controller.HitObject(coll.bounds.center, other.gameObject);
		}
	}
}