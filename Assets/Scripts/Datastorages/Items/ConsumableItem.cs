using EoE.Entities;
using UnityEngine;

namespace EoE.Information
{
	public enum BuffStackingStyle { Stack = 1, Reapply = 2, DoNothing = 4 }
	public class ConsumableItem : Item
	{
		public override InUIUses Uses => InUIUses.Use | InUIUses.Equip | InUIUses.Drop | InUIUses.Back;

		[Header("Heal Effect")]
		public HealTargetInfo[] HealEffects;

		[Space(5)]
		[Header("Buffs")]
		public Buff[] BuffsToApply;
		public BuffStackingStyle StackingStyle = BuffStackingStyle.Reapply;

		protected override bool OnUse(Entitie user)
		{
			//Activate Healeffects
			for (int i = 0; i < HealEffects.Length; i++)
			{
				HealEffects[i].Activate(user);
			}
			//Apply buffs
			for (int i = 0; i < BuffsToApply.Length; i++)
			{
				if (StackingStyle == BuffStackingStyle.Stack)
				{
					user.AddBuff(BuffsToApply[i], user);
				}
				else if (StackingStyle == BuffStackingStyle.Reapply)
				{
					if (!(user.TryReapplyBuff(BuffsToApply[i], user).Item1))
					{
						user.AddBuff(BuffsToApply[i], user);
					}
				}
				else //effect.BuffStackStyle == BuffStackingStyle.DoNothing
				{
					if (!(user.HasBuffActive(BuffsToApply[i], user).HasValue))
					{
						user.AddBuff(BuffsToApply[i], user);
					}
				}
			}
			return true;
		}

		protected override bool OnEquip(Entitie user)
		{
			return true;
		}
	}
	[System.Serializable]
	public class HealTargetInfo
	{
		public TargetStat HealType;
		public bool Percent;
		public float Amount;

		public void Activate(Entitie user)
		{
			if (HealType == TargetStat.Endurance && !(user is Player))
				return;

			float targetStatAmount = HealType == TargetStat.Health ? user.curMaxHealth : (HealType == TargetStat.Mana ? user.curMaxMana : (user as Player).curMaxEndurance);
			float amount = Percent ? (targetStatAmount * (Amount / 100)) : Amount;
			amount *= -1;

			if (HealType == TargetStat.Health)
			{
				user.ChangeHealth(new ChangeInfo(user, amount > 0 ? CauseType.Magic : CauseType.Heal, HealType, amount));
			}
			else if (HealType == TargetStat.Mana)
			{
				user.ChangeMana(new ChangeInfo(user, amount > 0 ? CauseType.Magic : CauseType.Heal, HealType, amount));
			}
			else //t == HealTargetType.Endurance
			{
				(user as Player).ChangeEndurance(new ChangeInfo(user, amount > 0 ? CauseType.Magic : CauseType.Heal, HealType, amount));
			}
		}
	}
}