using EoE.Combatery;
using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	public abstract class Ultimate : CombatObject
	{
		public Sprite UltimateIcon = default;
		public void Activate()
		{
			CostActivation();
			ActivateInternal();
		}
		protected virtual void CostActivation()
		{
			Cost.PayCost(Player.Instance, 1, 1, 1);
		}
		protected abstract void ActivateInternal();
	}
}