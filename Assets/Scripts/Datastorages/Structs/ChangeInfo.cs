using EoE.Entities;
using EoE.Events;
using UnityEngine;

namespace EoE.Information
{
	public enum CauseType { Physical, Magic, Heal, DOT }
	public enum ElementType { None = 1, Fire = 2, Earth = 4, Water = 8, Electro = 16, Light = 32 }
	public struct ChangeInfo
	{
		public readonly Entitie attacker;
		public readonly CauseType cause;
		private readonly ElementType element;
		private readonly TargetStat targetStat;

		private readonly Vector3? impactPosition;
		private readonly Vector3? impactDirection;
		private readonly float baseDamageAmount;
		private readonly bool wasCritical;

		private readonly float? knockbackAmount;

		public ChangeInfo(Entitie attacker, CauseType cause, ElementType element, TargetStat targetStat, Vector3 impactPosition, Vector3 impactDirection, float baseDamageAmount, bool wasCritical, float? knockbackAmount = null)
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

		public ChangeInfo(Entitie attacker, CauseType cause, TargetStat targetStat, float baseDamageAmount)
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

			public ChangeResult(ChangeInfo basis, Entitie receiver, bool createDamageNumber, bool fromRegen = false)
			{
				//Calculate the damage that we want to cause
				finalChangeAmount = basis.baseDamageAmount;
				if (basis.targetStat == TargetStat.Health && receiver != basis.attacker && finalChangeAmount > 0)
				{
					if (basis.cause == CauseType.Physical)
					{
						finalChangeAmount = (((basis.attacker ? basis.attacker.EntitieLevel : 0) + GameController.CurrentGameSettings.PhysicalDamageLevelAdd) * basis.baseDamageAmount) / GameController.CurrentGameSettings.PhysicalDamageDivider;
						float defenseAmount = ((receiver.EntitieLevel + GameController.CurrentGameSettings.PhysicalDefenseLevelAdd) * receiver.curDefense) / GameController.CurrentGameSettings.PhysicalDefenseLevelDivider;
						finalChangeAmount -= defenseAmount;
					}
					else if (basis.cause == CauseType.Magic)
					{
						finalChangeAmount = (((basis.attacker ? basis.attacker.EntitieLevel : 0) + GameController.CurrentGameSettings.MagicDamageLevelAdd) * basis.baseDamageAmount) / GameController.CurrentGameSettings.MagicDamageDivider;
					}

					//If the multiplications caused the final damage change from positive to negative we want to set it to zero
					//This will prevent situations in which very weak attacks would heal the receiver
					if (finalChangeAmount < 0)
					{
						finalChangeAmount = 0;
					}
				}

				if (basis.cause != CauseType.Heal)
					finalChangeAmount *= GameController.CurrentGameSettings.GetEffectiveness(basis.element, receiver.SelfSettings.EntitieElement);
				if (basis.wasCritical)
					finalChangeAmount *= GameController.CurrentGameSettings.CritDamageMultiplier;

				//Finally we check if the receiver is actually invincible, if that is the case and the final damage is more then zero (Not a heal) then we set it to zero
				if (receiver.IsInvincible && basis.targetStat == TargetStat.Health && finalChangeAmount > 0)
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
				if (basis.targetStat == TargetStat.Health)
				{
					//Change true damage based on the entities true damage multiplier
					finalChangeAmount *= receiver.curTrueDamageDamageMultiplier;
					finalChangeAmount = Mathf.Max(finalChangeAmount, -(receiver.curMaxHealth - receiver.curHealth));
				}
				else if (basis.targetStat == TargetStat.Mana)
				{
					finalChangeAmount = Mathf.Max(finalChangeAmount, -(receiver.curMaxMana - receiver.curMana));
				}
				else if (receiver is Player)
				{
					finalChangeAmount = Mathf.Max(finalChangeAmount, -((receiver as Player).curMaxEndurance - (receiver as Player).curEndurance));
				}

				//FX for Player
				//We dont want to send VFX if the Player caused himself damage
				if (basis.targetStat == TargetStat.Health && basis.cause != CauseType.DOT)
				{
					if (basis.attacker is Player && !(receiver is Player))
					{
						EventManager.PlayerCausedDamageInvoke(receiver, basis.wasCritical);
					}

					if (receiver is Player && (finalChangeAmount > 0 || causedKnockback.HasValue) && !receiver.IsInvincible)
					{
						EventManager.PlayerTookDamageInvoke(finalChangeAmount, (basis.knockbackAmount / receiver.SelfSettings.EntitieMass) * (basis.wasCritical ? GameController.CurrentGameSettings.CritDamageMultiplier : 1));
					}
				}

				//Only show damage numbers if it is a change on health. Also, if the cause is regen, then check if we want to display regen numbers
				if (basis.targetStat == TargetStat.Health && finalChangeAmount != 0 && createDamageNumber && !(fromRegen && !GameController.CurrentGameSettings.ShowRegenNumbers))
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
					float sizeMultiplier = finalChangeAmount < 0 ? 1 : (Mathf.Clamp((finalChangeAmount / receiver.curMaxHealth) * 8, 0.75f, 4));

					EffectUtils.CreateDamageNumber(
						basis.impactPosition ?? receiver.actuallWorldPosition,
						colors,
						forceDirection * GameController.CurrentGameSettings.DamageNumberFlySpeed * sizeMultiplier,
						Mathf.Abs(finalChangeAmount),
						basis.wasCritical,
						sizeMultiplier);
				}
			}
		}
	}

}