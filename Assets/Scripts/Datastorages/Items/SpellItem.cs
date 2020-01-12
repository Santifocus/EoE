using EoE.Combatery;
using EoE.Entities;

namespace EoE.Information
{
	public class SpellItem : Item
	{
		public override InUIUses Uses => InUIUses.Equip | InUIUses.Drop | InUIUses.Back;
		public Spell TargetSpell;
		protected override bool OnUse(Entity user)
		{
			return user.CastSpell(TargetSpell);
		}
		protected override bool OnEquip(Entity user) => true;
	}
}