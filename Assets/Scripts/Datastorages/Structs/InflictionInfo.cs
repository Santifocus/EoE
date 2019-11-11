using EoE.Entities;
using EoE.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public enum CauseType { Physical, Magic, Heal, DOT }
	public enum ElementType { None = 0, Fire = 1, Earth = 2, Water = 3, Electro = 4, Light = 5 }
	public struct InflictionInfo
	{
		public readonly Entitie attacker;
		private readonly CauseType cause;
		private readonly ElementType element;

		private readonly Vector3 impactPosition;
		private readonly Vector3 impactDirection;
		private readonly float baseDamageAmount;
		private readonly bool wasCritical;

		private readonly bool causeKnockback;
		private readonly float knockbackAmount;

		public InflictionInfo(Entitie attacker, CauseType cause, ElementType element, Vector3 impactPosition, Vector3 impactDirection, float baseDamageAmount, bool wasCritical, bool causeKnockback = false, float knockbackAmount = 0)
		{
			this.attacker = attacker;
			this.cause = cause;
			this.element = element;
			this.impactPosition = impactPosition;
			this.impactDirection = impactDirection;
			this.baseDamageAmount = baseDamageAmount;
			this.causeKnockback = causeKnockback;
			this.knockbackAmount = knockbackAmount;
			this.wasCritical = wasCritical;
		}

		public struct InflictionResult
		{
			public readonly float finalDamage;
			public readonly Vector3 forceDirection;
			public readonly Vector3? causedKnockback;

			public InflictionResult(InflictionInfo basis, Entitie receiver, bool createDamageNumber, bool fromRegen = false)
			{
				//Calculate the damage that we want to cause
				finalDamage = (((basis.attacker ? basis.attacker.EntitieLevel : 1) + GameController.CurrentGameSettings.DamageLevelAdd) * basis.baseDamageAmount) / GameController.CurrentGameSettings.DamageDivider;
				if (basis.cause == CauseType.Physical)
				{
					float defenseAmount = ((receiver.EntitieLevel + GameController.CurrentGameSettings.DefenseLevelAdd) * receiver.curDefense) / GameController.CurrentGameSettings.DefenseLevelDivider;
					finalDamage -= defenseAmount;
				}

				if (basis.cause != CauseType.Heal)
					finalDamage *= GameController.CurrentGameSettings.GetEffectiveness(basis.element, receiver.SelfSettings.EntitieElement);


				if (basis.causeKnockback)
				{
					forceDirection = basis.impactDirection;
					causedKnockback = forceDirection * basis.knockbackAmount / receiver.SelfSettings.EntitieMass;
				}
				else
				{
					forceDirection = Vector3.up;
					causedKnockback = null;
				}
				if (basis.wasCritical)
				{
					if(causedKnockback.HasValue)
						causedKnockback *= GameController.CurrentGameSettings.CritDamageMultiplier;

					finalDamage *= GameController.CurrentGameSettings.CritDamageMultiplier;
				}

				//VFX for Player
				if(basis.attacker is Player && basis.wasCritical && (basis.cause == CauseType.Physical || basis.cause == CauseType.Magic))
				{
					//Time dilation
					if (Player.PlayerSettings.SlowOnCriticalHit)
					{
						EffectUtils.TimeDilation(
							0,
							Player.PlayerSettings.SlowOnCritScale,
							Player.PlayerSettings.SlowOnCritTimeIn,
							Player.PlayerSettings.SlowOnCritTimeStay,
							false,
							Player.PlayerSettings.SlowOnCritTimeOut);
					}

					//Screen shake
					if (Player.PlayerSettings.ScreenShakeOnCrit)
					{
						EffectUtils.ShakeScreen(
							Player.PlayerSettings.ShakeTimeOnCrit,
							Player.PlayerSettings.OnCritShakeAxisIntensity,
							Player.PlayerSettings.OnCritShakeAngleIntensity);
					}

					//Controller rumble
					if (Player.PlayerSettings.RumbleOnCrit)
					{
						EffectUtils.RumbleController(
							Player.PlayerSettings.RumbleOnCritTime,
							Player.PlayerSettings.RumbleOnCritLeftIntensityStart,
							Player.PlayerSettings.RumbleOnCritRightIntensityStart,
							Player.PlayerSettings.RumbleOnCritLeftIntensityEnd,
							Player.PlayerSettings.RumbleOnCritRightIntensityEnd);
					}
				}

				//We dont want to overheal, but will allow overkill for bettet VFX
				finalDamage = Mathf.Max(finalDamage, -(receiver.curMaxHealth - receiver.curHealth));

				if (finalDamage != 0 && createDamageNumber && !(fromRegen && !GameController.CurrentGameSettings.ShowRegenNumbers))
				{
					Gradient colors;
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
								if(finalDamage < 0)
									colors = GameController.CurrentGameSettings.HealColors;
								else if(basis.element != ElementType.None)
									colors = GameController.CurrentGameSettings.PhysicalDamageColors;
								else
									colors = GameController.CurrentGameSettings.MagicalDamageColors;
							}
							break;
						default:
							colors = null;
							break;
					}

					if(!(receiver.IsInvincible && finalDamage > 0))
						Utils.EffectUtils.CreateDamageNumber(
							basis.impactPosition, 
							colors, 
							forceDirection * GameController.CurrentGameSettings.DamageNumberFlySpeed * (basis.wasCritical ? 2 : 1), 
							Mathf.Abs(finalDamage), 
							basis.wasCritical, 
							basis.wasCritical ? 2 : 1);
				}
			}
		}
	}

}