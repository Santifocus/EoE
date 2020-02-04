using EoE.Combatery;
using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class AttackUltimate : BasicUltimate
	{
		public AttackSequence AttackData = new AttackSequence();
		protected override void CostActivation() { } //Ignore the price of the first attack sequence part, it will be activated from the weapon controller
		protected override void ActivateInternal()
		{
			base.ActivateInternal();
			WeaponController.Instance.overrideBaseObject = this;
			WeaponController.Instance.ForceAttackStart(AttackData);

			//Now we check if the attack sequence start was successfull, if not we do nothing, otherwise we start a delayed call that will end the moment the attack sequence is over,
			//when it is, the overrideBaseObject will be reset
			if (WeaponController.Instance.InAttackSequence) 
			{
				System.Func<bool> condition = new System.Func<bool>(() => !WeaponController.Instance.InAttackSequence);
				GameController.BeginDelayedCall(() => { WeaponController.Instance.overrideBaseObject = null; }, 0, TimeType.ScaledDeltaTime, condition);
			}
		}

		public override bool CanActivate(Entity target, float healthCostMultiplier, float manaCostMultiplier, float staminaCostMultiplier)
		{
			return base.CanActivate(target, healthCostMultiplier, manaCostMultiplier, staminaCostMultiplier) && WeaponController.Instance;
		}
	}
}