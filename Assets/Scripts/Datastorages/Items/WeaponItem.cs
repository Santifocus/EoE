using EoE.Entities;

namespace EoE.Information
{
	public class WeaponItem : Item
	{
		public override InUIUses Uses => InUIUses.Equip | InUIUses.Drop | InUIUses.Back;
		protected override bool OnUse(Entitie user)
		{
			return false;
		}
	}
}