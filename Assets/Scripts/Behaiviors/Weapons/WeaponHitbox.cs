using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Weapons
{
	public class WeaponHitbox : MonoBehaviour
	{
		[SerializeField] private TrailRenderer trail;

		private WeaponController controller;
		private bool curActive;
		public bool Active { get => curActive; set => ChangeWeaponState(value); }
		public void Setup(WeaponController controller)
		{
			this.controller = controller;
			trail.enabled = false;
			gameObject.layer = ConstantCollector.WEAPON_LAYER;
			GetComponent<Collider>().isTrigger = true;
		}

		private void ChangeWeaponState(bool state)
		{
			curActive = state;
			trail.enabled = state;
		}
		private void OnTriggerEnter(Collider coll)
		{
			controller.HitObject(coll.gameObject);
		}
	}
}