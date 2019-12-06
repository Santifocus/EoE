using EoE.Entities;
using EoE.Combatery;

namespace EoE.Information
{
	public class SpellItem : Item
	{
		public override InUIUses Uses => InUIUses.Equip | InUIUses.Drop | InUIUses.Back;
		public Spell targetSpell;
		protected override bool OnUse(Entitie user)
		{
			return user.CastSpell(targetSpell);
		}
		protected override bool OnEquip(Entitie user) => true;
	}
}