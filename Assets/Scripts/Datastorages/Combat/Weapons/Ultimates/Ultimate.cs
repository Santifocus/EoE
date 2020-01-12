using EoE.Combatery;
using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public abstract class Ultimate : ScriptableObject
	{
		public Sprite UltimateIcon = default;
		public CombatObject BaseSettings;
        public virtual bool CanActivate()
		{
			return BaseSettings.IsActivatable(Player.Instance, 1, 1, 1);
		}
		public void Activate()
		{
			CostActivation();
			ActivateInternal();
		}
		protected virtual void CostActivation()
		{
			BaseSettings.ActivateCost(Player.Instance, 1, 1, 1);
		}
		protected abstract void ActivateInternal();
	}
}