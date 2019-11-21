﻿using EoE.Entities;
using UnityEngine;

namespace EoE.Information
{
	public class HealItem : Item
	{
		public enum HealTargetType { Health, Mana, Endurance }
		public HealTargetInfo[] healEffects;
		protected override void OnUse(Entitie user)
		{
			for (int i = 0; i < healEffects.Length; i++)
			{
				HealTargetType t = healEffects[i].HealType;
				if (t == HealTargetType.Endurance && !(user is Player))
					continue;

				float targetStatAmount = t == HealTargetType.Health ? user.curMaxHealth : (t == HealTargetType.Mana ? user.curMaxMana : (user as Player).useableEndurance);
				float amount = healEffects[i].Percent ? (targetStatAmount * (healEffects[i].Amount / 100)) : healEffects[i].Amount;
				amount *= -1;

				if (t == HealTargetType.Health)
				{
					user.ChangeHealth(new ChangeInfo(user, amount > 0 ? CauseType.Magic : CauseType.Heal, TargetStat.Health, amount));
				}
				else if (t == HealTargetType.Mana)
				{
					user.ChangeMana(new ChangeInfo(user, amount > 0 ? CauseType.Magic : CauseType.Heal, TargetStat.Mana, amount));
				}
				else //t == HealTargetType.Endurance
				{
					(user as Player).ChangeEndurance(new ChangeInfo(user, amount > 0 ? CauseType.Magic : CauseType.Heal, TargetStat.Endurance, amount));
				}
			}
		}

		[System.Serializable]
		public class HealTargetInfo
		{
			public HealTargetType HealType;
			public bool Percent;
			public float Amount;
		}
	}
}