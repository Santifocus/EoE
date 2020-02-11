using EoE.Combatery;
using EoE.Behaviour.Entities;

namespace EoE.Information
{
	public class ActivationCompoundItem : Item
	{
		public override InUIUses Uses => InUIUses.Equip | InUIUses.Drop | InUIUses.Back;
		public ActivationCompound TargetCompound;
		protected override bool OnUse(Entity user)
		{
			return user.ActivateCompound(TargetCompound);
		}
		protected override bool OnEquip(Entity user) => true;
	}
}