using EoE.Entities;
using UnityEngine;

namespace EoE.Information
{
	public class HealItem : Item
	{
		public override InUIUses Uses => InUIUses.Use | InUIUses.Equip | InUIUses.Drop | InUIUses.Back;
		public HealTargetInfo[] healEffects;
		protected override bool OnUse(Entitie user)
		{
			for (int i = 0; i < healEffects.Length; i++)
			{
				healEffects[i].Activate(user);
			}
			return true;
		}
	}
}