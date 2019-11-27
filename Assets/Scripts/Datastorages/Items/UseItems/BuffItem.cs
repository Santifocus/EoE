using EoE.Entities;
using System.Collections.Generic;

namespace EoE.Information
{
	public class BuffItem : Item
	{
		public Buff buffInfo;
		public bool stackable;

		protected override void OnUse(Entitie user)
		{
			if (stackable)
			{
				user.AddBuff(buffInfo, user);
			}
			else
			{
				if(!(user.TryReapplyBuff(buffInfo, user).Item1))
					user.AddBuff(buffInfo, user);
			}
		}
	}
}