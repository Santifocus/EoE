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
			WeaponController.Instance.ForceAttackStart(AttackData);
		}

		public override bool CanActivate()
		{
			return base.CanActivate() && WeaponController.Instance;
		}
	}
}