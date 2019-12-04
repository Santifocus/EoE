using EoE.Entities;
using EoE.Weapons;

namespace EoE.Information
{
	public class SpellItem : Item
	{
		public override InUIUses Uses => InUIUses.Equip;
		public Spell targetSpell;
		protected override bool OnUse(Entitie user)
		{
			return user.CastSpell(targetSpell);
		}
		protected override bool OnEquip(Entitie user) => true;
	}
}