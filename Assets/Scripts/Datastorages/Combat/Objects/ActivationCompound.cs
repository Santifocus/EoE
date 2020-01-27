using EoE.Information;
using EoE.Information.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	public class ActivationCompound : CombatObject
	{
		public int CostActivationIndex = 0;
		public float CausedCooldown = 1;
		public bool CancelFromStun = true;
		public bool CancelIfCostActivationIsImpossible = true;

		public ActivationElement[] Elements = new ActivationElement[1];
	}
	[System.Serializable]
	public class ActivationElement
	{
		public RestrictionData Restrictions = new RestrictionData();

		public float ElementDuration = 1;
		public float WhileTickTime = 0.1f;
		public ActivationEffect[] StartEffects = new ActivationEffect[0];
		public ActivationEffect[] WhileEffects = new ActivationEffect[0];

		public ActivationEffect[] AtTargetStartEffects = new ActivationEffect[0];
		public ActivationEffect[] AtTargetWhileEffects = new ActivationEffect[0];

		public LogicComponent PauseCondition;
		public LogicComponent YieldCondition;
		public LogicComponent StopCondition;
	}
}