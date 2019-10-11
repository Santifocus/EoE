using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public enum CauseType { Physical, Magic, Heal }
	public enum ElementType { None = 0, Fire = 1, Earth = 2, Water = 3, Electro = 4, Light = 5 }
	public struct InflictionInfo
	{
		public readonly Entitie attacker;
		private readonly CauseType cause;
		private readonly ElementType element;

		private readonly Vector3 impactPosition;
		private readonly float baseDamageAmount;
		private readonly bool wasCritical;

		private readonly bool causeKnockback;
		private readonly float knockbackAmount;

		public InflictionInfo(Entitie attacker, CauseType cause, ElementType element, Vector3 impactPosition, float baseDamageAmount, bool wasCritical, bool causeKnockback = false, float knockbackAmount = 0)
		{
			this.attacker = attacker;
			this.cause = cause;
			this.element = element;
			this.impactPosition = impactPosition;
			this.baseDamageAmount = baseDamageAmount;
			this.causeKnockback = causeKnockback;
			this.knockbackAmount = knockbackAmount;
			this.wasCritical = wasCritical;
		}

		public struct InflictionResult
		{
			public readonly float finalDamage;
			public readonly Vector3 forceDirection;
			public readonly Vector3 causedKnockback;

			public InflictionResult(InflictionInfo basis, Entitie receiver, bool createDamageNumber, bool fromRegen = false)
			{
				finalDamage = basis.baseDamageAmount;
				if(basis.cause != CauseType.Heal)
					finalDamage *= GameController.CurrentGameSettings.GetEffectiveness(basis.element, receiver.SelfSettings.EntitieElement) ;

				if (basis.causeKnockback)
				{
					forceDirection = ((receiver.SelfSettings.MassCenter + receiver.transform.position) - basis.impactPosition).normalized;
					causedKnockback = forceDirection * basis.knockbackAmount / receiver.SelfSettings.EntitieMass;
				}
				else
				{
					forceDirection = Vector3.up;
					causedKnockback = Vector3.zero;
				}

				if (createDamageNumber && !(fromRegen && !GameController.CurrentGameSettings.ShowRegenNumbers))
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
						default:
							colors = null;
							break;
					}

					GameController.Instance.CreateDamageNumber(basis.impactPosition, colors, forceDirection * GameController.CurrentGameSettings.DamageNumberFlySpeed, Mathf.Abs(finalDamage), basis.wasCritical);
				}
			}
		}
	}

}