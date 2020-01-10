using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	public class RemenantsData : ScriptableObject
	{
		public bool TryGroundRemenants = true;
		public float Duration = 5;
		public ActivationEffect[] StartEffects = new ActivationEffect[0];
		public ActivationEffect[] WhileEffects = new ActivationEffect[0];
		public float WhileTickTime = 0.1f;
		public ActivationEffect[] OnEndEffects = new ActivationEffect[0];
	}
}