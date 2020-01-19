using EoE.Combatery;
using EoE.Entities;
using EoE.UI;

namespace EoE.Information
{
	public class WeaponItem : Item
	{
		public override InUIUses Uses => InUIUses.Equip | InUIUses.Drop | InUIUses.Back;
		public Weapon TargetWeapon;
		protected override bool OnUse(Entity user)
		{
			if (WeaponController.Instance)
				WeaponController.Instance.StartAttack();
			return false;
		}
		protected override bool OnEquip(Entity user)
		{
			if (WeaponController.Instance)
				WeaponController.Instance.Remove();
			WeaponController newWeapon = Instantiate(TargetWeapon.WeaponPrefab, Storage.ParticleStorage);
			newWeapon.Setup(TargetWeapon);

			return true;
		}
		protected override bool OnUnEquip(Entity user)
		{
			if (WeaponController.Instance)
				WeaponController.Instance.Remove();
			ComboDisplayController.Instance.StopCombo();
			return true;
		}
	}
}