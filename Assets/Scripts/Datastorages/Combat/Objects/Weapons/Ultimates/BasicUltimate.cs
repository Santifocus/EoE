using EoE.Combatery;
using EoE.Behaviour.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class BasicUltimate : Ultimate
	{
		public ActivationEffect[] ActivationEffects = new ActivationEffect[0];
		protected override void ActivateInternal()
		{
			for(int i = 0; i < ActivationEffects.Length; i++)
				ActivationEffects[i].Activate(Player.Instance, this);
		}
	}
}