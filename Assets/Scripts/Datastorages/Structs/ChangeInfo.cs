using EoE.Entities;
using EoE.Events;
using EoE.Utils;
using UnityEngine;

namespace EoE.Information
{
	public enum CauseType { Physical, Magic, Heal, DOT }
	public enum ElementType { None = 0, Fire = 1, Earth = 2, Water = 3, Electro = 4, Light = 5 }
	public struct ChangeInfo
	{
		public readonly Entitie attacker;
		private readonly CauseType cause;
		private readonly ElementType element;

		private readonly Vector3? impactPosition;
		private readonly Vector3? impactDirection;
		private readonly float baseDamageAmount;
		private readonly bool wasCritical;

		private readonly float? knockbackAmount;

		private readonly bool changeOnHealth;

		public ChangeInfo(Entitie attacker, CauseType cause, ElementType element, Vector3 impactPosition, Vector3 impactDirection, float baseDamageAmount, bool wasCritical, float? knockbackAmount = null, bool changeOnHealth = true)
		{
			this.attacker = attacker;
			this.cause = cause;
			this.element = element;
			this.impactPosition = impactPosition;
			this.impactDirection = impactDirection;
			this.baseDamageAmount = baseDamageAmount;
			this.knockbackAmount = knockbackAmount;
			this.wasCritical = wasCritical;
			this.changeOnHealth = changeOnHealth;
		}

		public ChangeInfo(Entitie attacker, CauseType cause, float baseDamageAmount, bool changeOnHealth = true)
		{
			this.attacker = attacker;
			this.cause = cause;
			this.element = ElementType.None;
			this.impactPosition = null;
			this.impactDirection = null;
			this.baseDamageAmount = baseDamageAmount;
			this.knockbackAmount = null;
			this.wasCritical = false;
			this.changeOnHealth = changeOnHealth;
		}

		public struct ChangeResult
		{
			public readonly float finalChangeAmount;
			public readonly Vector3 forceDirection;
			public readonly Vector3? causedKnockback;

			public ChangeResult(ChangeInfo basis, Entitie receiver, bool createDamageNumber, bool fromRegen = false)
			{
				//Calculate the damage that we want to cause
				if (receiver != basis.attacker)
				{
					finalChangeAmount = (((basis.attacker ? basis.attacker.EntitieLevel : 1) + GameController.CurrentGameSettings.DamageLevelAdd) * basis.baseDamageAmount) / GameController.CurrentGameSettings.DamageDivider;
					if (basis.cause == CauseType.Physical)
					{
						float defenseAmount = ((receiver.EntitieLevel + GameController.CurrentGameSettings.DefenseLevelAdd) * receiver.curDefense) / GameController.CurrentGameSettings.DefenseLevelDivider;
						finalChangeAmount -= defenseAmount;
					}
				}
				else
				{
					finalChangeAmount = basis.baseDamageAmount;
				}

				if (basis.cause != CauseType.Heal)
					finalChangeAmount *= GameController.CurrentGameSettings.GetEffectiveness(basis.element, receiver.SelfSettings.EntitieElement);
				if (basis.wasCritical)
					finalChangeAmount *= GameController.CurrentGameSettings.CritDamageMultiplier;

				//Finally we check if the receiver is actually invincible, if that is the case and the final damage is more then zero (Not a heal) then we set it to zero
				if (receiver.IsInvincible && basis.changeOnHealth && finalChangeAmount > 0)
					finalChangeAmount = 0;

				//Calculate the caused knockback if there is one
				if (basis.knockbackAmount.HasValue && basis.impactDirection.HasValue)
				{
					forceDirection = basis.impactDirection.Value;
					causedKnockback = forceDirection * basis.knockbackAmount / receiver.SelfSettings.EntitieMass;
					if (basis.wasCritical)
						causedKnockback *= GameController.CurrentGameSettings.CritDamageMultiplier;
				}
				else
				{
					forceDirection = Vector3.up;
					causedKnockback = null;
				}


				//We dont want to overheal, but will allow overkill for bettet VFX
				finalChangeAmount = Mathf.Max(finalChangeAmount, -(receiver.curMaxHealth - receiver.curHealth));

				//VFX for Player
				//We dont want to send VFX if the Player caused himself damage
				if (basis.changeOnHealth)
				{
					if (basis.attacker is Player && !(receiver is Player))
					{
						EventManager.PlayerCausedDamageInvoke(receiver, basis.wasCritical);
					}

					if(receiver is Player)
					{
						EventManager.PlayerTookDamageInvoke(finalChangeAmount, (basis.knockbackAmount / receiver.SelfSettings.EntitieMass) * (basis.wasCritical ? GameController.CurrentGameSettings.CritDamageMultiplier : 1));
					}
				}

				//Only show damage numbers if it is a change on health. Also, if the cause is regen, then check if we want to display regen numbers
				if (basis.changeOnHealth && finalChangeAmount != 0 && createDamageNumber && !(fromRegen && !GameController.CurrentGameSettings.ShowRegenNumbers))
				{
					Gradient colors;
					if (finalChangeAmount < 0)
						colors = GameController.CurrentGameSettings.HealColors;
					else
					{
						switch (basis.cause)
						{
							case CauseType.Physical:
								colors = GameController.CurrentGameSettings.PhysicalDamageColors;
								break;
							case CauseType.Magic:
								colors = GameController.CurrentGameSettings.MagicalDamageColors;
								break;
							case CauseType.Heal:
								colors = GameController.CurrentGameSettings.HealColors;
								break;
							case CauseType.DOT:
								{
									if (basis.element != ElementType.None)
										colors = GameController.CurrentGameSettings.PhysicalDamageColors;
									else
										colors = GameController.CurrentGameSettings.MagicalDamageColors;
								}
								break;
							default:
								colors = null;
								break;
						}
					}

					EffectUtils.CreateDamageNumber(
						basis.impactPosition ?? receiver.actuallWorldPosition,
						colors,
						forceDirection * GameController.CurrentGameSettings.DamageNumberFlySpeed * (basis.wasCritical ? 2 : 1),
						Mathf.Abs(finalChangeAmount),
						basis.wasCritical,
						basis.wasCritical ? 2 : 1);
				}
			}
		}
	}

}