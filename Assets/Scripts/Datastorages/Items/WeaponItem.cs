using EoE.Entities;

namespace EoE.Information
{
	public class WeaponItem : Item
	{
		public override InUIUses Uses => InUIUses.Equip;
		protected override bool OnUse(Entitie user)
		{
			return false;
		}
	}
}