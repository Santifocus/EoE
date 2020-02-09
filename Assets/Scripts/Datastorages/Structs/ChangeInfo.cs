using EoE.Entities;
using EoE.Events;
using UnityEngine;

namespace EoE.Information
{
	public enum CauseType { Physical, Magic, Heal, DOT }
	public enum ElementType { None = (1 << 0), Fire = (1 << 1), Earth = (1 << 2), Water = (1 << 3), Electro = (1 << 4), Light = (1 << 5) , Dark = (1 << 6) }
	public struct ChangeInfo
	{
		public readonly Entity attacker;
		public readonly CauseType cause;
		private readonly ElementType element;
		private readonly TargetStat targetStat;

		private readonly Vector3? impactPosition;
		private readonly Vector3? impactDirection;
		private readonly float baseDamageAmount;
		private readonly bool wasCritical;

		private readonly float? knockbackAmount;

		public ChangeInfo(Entity attacker, CauseType cause, ElementType element, TargetStat targetStat, Vector3 impactPosition, Vector3 impactDirection, float baseDamageAmount, bool wasCritical, float? knockbackAmount = null)
		{
			this.attacker = attacker;
			this.cause = cause;
			this.element = element;
			this.targetStat = targetStat;
			this.impactPosition = impactPosition;
			this.impactDirection = impactDirection;
			this.baseDamageAmount = baseDamageAmount;
			this.knockbackAmount = knockbackAmount;
			this.wasCritical = wasCritical;
		}

		public ChangeInfo(Entity attacker, CauseType cause, TargetStat targetStat, float baseDamageAmount)
		{
			this.attacker = attacker;
			this.cause = cause;
			this.element = ElementType.None;
			this.targetStat = targetStat;
			this.impactPosition = null;
			this.impactDirection = null;
			this.baseDamageAmount = baseDamageAmount;
			this.knockbackAmount = null;
			this.wasCritical = false;
		}

		public struct ChangeResult
		{
			public readonly float finalChangeAmount;
			public readonly Vector3 forceDirection;
			public readonly Vector3? causedKnockback;

			public ChangeResult(ChangeInfo basis, Entity receiver, bool createDamageNumber, bool fromRegen = false)
			{
				bool changeOnHealth = basis.targetStat == TargetStat.Health;
				float attackerExtraDamage = 0;
				if (changeOnHealth && basis.attacker)
					attackerExtraDamage = (basis.cause == CauseType.Physical) ? basis.attacker.CurPhysicalDamage : ((basis.cause == CauseType.Magic) ? basis.attacker.CurMagicalDamage : 0);

				finalChangeAmount = basis.baseDamageAmount + attackerExtraDamage;

				//First we check if the receiver is invincible, if that is the case & the final damage is more then zero (Not a heal) then we set it to zero
				if (receiver.IsInvincible && changeOnHealth && (finalChangeAmount > 0))
					finalChangeAmount = 0;

				//Calculate the damage that we want to cause based on formulas set in the GameSettings
				if (changeOnHealth && receiver != basis.attacker && finalChangeAmount > 0)
				{
					if (basis.cause == CauseType.Physical)
					{
						finalChangeAmount = (((basis.attacker ? basis.attacker.EntitieLevel : 0) + GameController.CurrentGameSettings.PhysicalDamageLevelAdd) * basis.baseDamageAmount) / GameController.CurrentGameSettings.PhysicalDamageDivider;
						float defenseAmount = ((receiver.EntitieLevel + GameController.CurrentGameSettings.PhysicalDefenseLevelAdd) * receiver.CurDefense) / GameController.CurrentGameSettings.PhysicalDefenseLevelDivider;
						finalChangeAmount -= defenseAmount;
					}
					else if (basis.cause == CauseType.Magic)
					{
						finalChangeAmount = (((basis.attacker ? basis.attacker.EntitieLevel : 0) + GameController.CurrentGameSettings.MagicDamageLevelAdd) * basis.baseDamageAmount) / GameController.CurrentGameSettings.MagicDamageDivider;
					}

					//If the chages caused the final damage change from positive to negative we want to set it to the set minimum damage
					//This will prevent situations in which very weak attacks would heal the receiver
					if (finalChangeAmount < 0)
					{
						finalChangeAmount = GameController.CurrentGameSettings.MinDamage;
					}
				}

				//Generall multipliers
				if (basis.cause != CauseType.Heal)
				{
					finalChangeAmount *= GameController.CurrentGameSettings.GetEffectiveness(basis.element, receiver.SelfSettings.EntitieElement);
				}
				if (basis.wasCritical)
				{
					finalChangeAmount *= GameController.CurrentGameSettings.CritDamageMultiplier;
				}

				//Calculate the caused knockback if it has a value
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

				//We dont want to overheal, but will allow overkill for better FX
				if (changeOnHealth)
				{
					//Change true damage based on the entities true damage multiplier
					finalChangeAmount *= receiver.CurTrueDamageDamageMultiplier;
					finalChangeAmount = Mathf.Max(finalChangeAmount, -(receiver.CurMaxHealth - receiver.CurHealth));
				}
				else if (basis.targetStat == TargetStat.Mana)
				{
					finalChangeAmount = Mathf.Max(finalChangeAmount, -(receiver.CurMaxMana - receiver.CurMana));
				}
				else if (receiver is Player) //&& basis.targetStat == TargetStat.Stamina
				{
					finalChangeAmount = Mathf.Max(finalChangeAmount, -((receiver as Player).CurMaxStamina - (receiver as Player).CurStamina));
				}

				//Event call for: Player receive damage OR Player caused damage
				if (changeOnHealth && basis.cause != CauseType.DOT)
				{
					//We dont want to send FX if the Player caused himself damage
					if (basis.attacker is Player && !(receiver is Player))
					{
						EventManager.PlayerCausedDamageInvoke(receiver, basis, basis.wasCritical);
					}

					if (receiver is Player && (finalChangeAmount > 0 || causedKnockback.HasValue) && !receiver.IsInvincible)
					{
						EventManager.PlayerTookDamageInvoke(finalChangeAmount, (basis.knockbackAmount / receiver.SelfSettings.EntitieMass) * (basis.wasCritical ? GameController.CurrentGameSettings.CritDamageMultiplier : 1));
					}
				}

				bool notZero = finalChangeAmount != 0;
				bool isRegenButAllowed = !(fromRegen && !GameController.CurrentGameSettings.ShowRegenNumbers);

				//Only show damage numbers if it is a change on health. Also, if the cause is regen, then check if we want to display regen numbers
				if (changeOnHealth && notZero && createDamageNumber && isRegenButAllowed)
				{
					Gradient colors;
					if ((finalChangeAmount < 0) || (basis.cause == CauseType.Heal))
					{
						colors = GameController.CurrentGameSettings.HealColors;
					}
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
							case CauseType.DOT:
								{
									if (basis.element == ElementType.None)
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
					float sizeMultiplier = finalChangeAmount < 0 ? 1 : (Mathf.Clamp((finalChangeAmount / receiver.CurMaxHealth) * 8, 0.75f, 4));

					EffectManager.CreateDamageNumber(	basis.impactPosition ?? receiver.actuallWorldPosition,
													colors,
													forceDirection * GameController.CurrentGameSettings.DamageNumberFlySpeed * sizeMultiplier,
													Mathf.Abs(finalChangeAmount),
													basis.wasCritical,
													sizeMultiplier
													);
				}
			}
		}
	}

}