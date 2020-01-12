using EoE.Entities;
using UnityEngine;

namespace EoE.Information
{
	public class ArmorItem : Item
	{
		public override InUIUses Uses => InUIUses.Equip | InUIUses.Drop | InUIUses.Back;
		[SerializeField] private Buff buffToApply = default;
		protected override bool OnEquip(Entity user)
		{
			if (user.ArmorBuff != null)
			{
				user.RemoveBuff(user.ArmorBuff);
			}
			user.ArmorBuff = user.AddBuff(buffToApply, user);
			return true;
		}
		protected override bool OnUnEquip(Entity user)
		{
			user.RemoveBuff(user.ArmorBuff);
			return true;
		}
	}
}