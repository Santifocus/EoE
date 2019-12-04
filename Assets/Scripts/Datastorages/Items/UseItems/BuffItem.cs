using EoE.Entities;
using System.Collections.Generic;

namespace EoE.Information
{
	public class BuffItem : Item
	{
		public override InUIUses Uses => InUIUses.Use | InUIUses.Equip;
		public Buff[] buffsToApply;
		public bool stackable;

		protected override bool OnUse(Entitie user)
		{
			if (stackable)
			{
				for(int i = 0; i < buffsToApply.Length; i++)
				{
					user.AddBuff(buffsToApply[i], user);
				}
			}
			else
			{
				for (int i = 0; i < buffsToApply.Length; i++)
				{
					if (!(user.TryReapplyBuff(buffsToApply[i], user).Item1))
						user.AddBuff(buffsToApply[i], user);
				}
			}
			return true;
		}
	}
}