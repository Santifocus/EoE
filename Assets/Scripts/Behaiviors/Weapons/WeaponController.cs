using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Weapons
{
	public class WeaponController : MonoBehaviour
	{
		[SerializeField] private WeaponHitbox[] weaponHitboxes = default;
		private bool curActive;
		public bool Active { get => curActive; set => ChangeWeaponState(value); }
		private void ChangeWeaponState(bool state)
		{
			curActive = state;
			for(int i = 0; i < weaponHitboxes.Length; i++)
			{
				weaponHitboxes[i].Active = true;
			}
		}
		public void HitObject(GameObject hit)
		{

		}
	}
}