using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Weapons
{
	public class WeaponHitbox : MonoBehaviour
	{
		[SerializeField] private WeaponController controller;

		private void Start()
		{
			gameObject.layer = ConstantCollector.WEAPON_LAYER;
			GetComponent<Collider>().isTrigger = true;
		}

		private void OnTriggerEnter(Collider coll)
		{
			Entitie hit = coll.GetComponent<Entitie>();

		}
	}
}