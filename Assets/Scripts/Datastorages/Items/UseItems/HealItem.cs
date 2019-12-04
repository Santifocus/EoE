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
				TargetStat t = healEffects[i].HealType;
				if (t == TargetStat.Endurance && !(user is Player))
					continue;

				float targetStatAmount = t == TargetStat.Health ? user.curMaxHealth : (t == TargetStat.Mana ? user.curMaxMana : (user as Player).curMaxEndurance);
				float amount = healEffects[i].Percent ? (targetStatAmount * (healEffects[i].Amount / 100)) : healEffects[i].Amount;
				amount *= -1;

				if (t == TargetStat.Health)
				{
					user.ChangeHealth(new ChangeInfo(user, amount > 0 ? CauseType.Magic : CauseType.Heal, t, amount));
				}
				else if (t == TargetStat.Mana)
				{
					user.ChangeMana(new ChangeInfo(user, amount > 0 ? CauseType.Magic : CauseType.Heal, t, amount));
				}
				else //t == HealTargetType.Endurance
				{
					(user as Player).ChangeEndurance(new ChangeInfo(user, amount > 0 ? CauseType.Magic : CauseType.Heal, t, amount));
				}
			}
			return true;
		}

		[System.Serializable]
		public class HealTargetInfo
		{
			public TargetStat HealType;
			public bool Percent;
			public float Amount;
		}
	}
}