using EoE.Entities;
using EoE.Weapons;

namespace EoE.Information
{
	public class SpellItem : Item
	{
		public Spell targetSpell;
		protected override bool OnUse(Entitie user)
		{
			return user.CastSpell(targetSpell);
		}
	}
}