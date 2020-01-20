using EoE.Combatery;
using EoE.Entities;

namespace EoE.Information
{
	public class SpellItem : Item
	{
		public override InUIUses Uses => InUIUses.Equip | InUIUses.Drop | InUIUses.Back;
		public Spell TargetSpell;
		[UnityEngine.Serialization.FormerlySerializedAs("TargetComound")]public ActivationCompound TargetCompound;
		protected override bool OnUse(Entity user)
		{
			return user.ActivateCompound(TargetCompound);
		}
		protected override bool OnEquip(Entity user) => true;
	}
}