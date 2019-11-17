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
				user.AddBuff(buffInfo, user);
			else
			{
				List<BuffInstance> targetList = buffInfo.Permanent ? user.permanentBuffs : user.nonPermanentBuffs;

				int? foundCopy = null;
				for (int i = 0; i < targetList.Count; i++)
				{
					if (targetList[i].Base == buffInfo)
					{
						foundCopy = i;
						break;
					}
				}

				//If we find a copy we reset the buff to its start state by removing and then adding it
				//If we didnt find it we just add it
				if (foundCopy.HasValue)
				{
					user.RemoveBuff(foundCopy.Value, buffInfo.Permanent);
				}
				user.AddBuff(buffInfo, user);
			}
		}
	}
}