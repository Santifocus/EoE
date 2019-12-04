using EoE.Entities;
using UnityEngine;

namespace EoE.Information
{
	public class ArmorItem : Item
	{
		public override InUIUses Uses => InUIUses.Equip;
		[SerializeField] private Buff buffToApply = default;
		protected override bool OnEquip(Entitie user)
		{
			if(user.ArmorBuff != null)
			{
				user.RemoveBuff(user.ArmorBuff);
			}
			user.ArmorBuff = user.AddBuff(buffToApply, user);
			return true;
		}
	}
}