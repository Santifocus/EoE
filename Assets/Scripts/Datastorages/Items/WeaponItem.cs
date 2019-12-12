using EoE.Combatery;
using EoE.Entities;
using UnityEngine;

namespace EoE.Information
{
	public class WeaponItem : Item
	{
		public override InUIUses Uses => InUIUses.Equip | InUIUses.Drop | InUIUses.Back;
		public Weapon TargetWeapon;
		protected override bool OnUse(Entitie user)
		{
			if (WeaponController.PlayerWeaponController)
				WeaponController.PlayerWeaponController.StartAttack();
			return false;
		}
		protected override bool OnEquip(Entitie user)
		{
			if (WeaponController.PlayerWeaponController)
				Destroy(WeaponController.PlayerWeaponController.gameObject);
			WeaponController newWeapon = Instantiate(TargetWeapon.WeaponPrefab, Storage.ParticleStorage);
			newWeapon.Setup(TargetWeapon);

			return true;
		}
		protected override bool OnUnEquip(Entitie user)
		{
			if (WeaponController.PlayerWeaponController)
				Destroy(WeaponController.PlayerWeaponController.gameObject);
			return true;
		}
	}
}