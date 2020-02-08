using EoE.Combatery;
using EoE.Entities;
using UnityEngine;

namespace EoE.Information
{
	public class ConsumableItem : Item
	{
		public override InUIUses Uses => InUIUses.Use | InUIUses.Equip | InUIUses.Drop | InUIUses.Back;

		[Header("Heal Effect")]
		public HealTargetInfo[] HealEffects;

		[Space(5)]
		[Header("Buffs")]
		public Buff[] BuffsToApply;
		public BuffStackingStyle StackingStyle = BuffStackingStyle.Reapply;

		protected override bool OnUse(Entity user)
		{
			//Activate Healeffects
			for (int i = 0; i < HealEffects.Length; i++)
			{
				HealEffects[i].Activate(user);
			}
			//Apply buffs
			for (int i = 0; i < BuffsToApply.Length; i++)
			{
				Buff.ApplyBuff(BuffsToApply[i], user, user, 1, StackingStyle);
			}
			return true;
		}

		protected override bool OnEquip(Entity user)
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

		public void Activate(Entity user, float multiplier = 1)
		{
			if (HealType == TargetStat.Stamina && !(user is Player))
				return;

			float targetStatAmount = HealType == TargetStat.Health ? user.curMaxHealth : (HealType == TargetStat.Mana ? user.curMaxMana : (user as Player).CurMaxStamina);
			float amount = Percent ? (targetStatAmount * (Amount / 100)) : Amount;
			amount *= -multiplier;

			if (HealType == TargetStat.Health)
			{
				user.ChangeHealth(new ChangeInfo(user, (amount > 0) ? CauseType.Magic : CauseType.Heal, TargetStat.Health, amount));
			}
			else if (HealType == TargetStat.Mana)
			{
				user.ChangeMana(new ChangeInfo(user, (amount > 0) ? CauseType.Magic : CauseType.Heal, TargetStat.Mana, amount));
			}
			else
			{
				if (HealType == TargetStat.Stamina)
					(user as Player).ChangeStamina(new ChangeInfo(user, (amount > 0) ? CauseType.Magic : CauseType.Heal, TargetStat.Stamina, amount));
				else if (HealType == TargetStat.UltimateCharge && WeaponController.Instance)
					WeaponController.Instance.AddUltimateCharge(-amount);
			}
		}
	}
}