using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public abstract class Ultimate : ScriptableObject
	{
		public static bool PlayerUsingUltimate = false;

		public Sprite UltimateIcon = default;
		public float HealthCost = 0;
		public float ManaCost = 0;
		public float EnduranceCost = 0;
        public virtual bool CanActivate()
		{
			return (Player.Instance.curHealth >= HealthCost && Player.Instance.curMana >= ManaCost && Player.Instance.curEndurance >= EnduranceCost);
		}
		public void Activate()
		{
			PlayerUsingUltimate = true;
			ActivateUltimate();
		}
		protected abstract void ActivateUltimate();
	}
}