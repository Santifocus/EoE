using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	[System.Flags] public enum ActionType { Casting = 1, Attacking = 2 }
	[System.Flags] public enum MovementType { Walk = 1, Turn = 2 }
	public class ActivationCompound : CombatObject
	{
		public bool NoDefinedActionType = false;
		public ActionType CompoundActionType = ActionType.Casting;
		public int CostActivationIndex = 0;
		public float CausedCooldown = 1;
		public bool CancelFromStun = true;
		public bool CancelIfCostActivationIsImpossible = true;

		public ActivationElement[] Elements = new ActivationElement[1];
	}
	[System.Serializable]
	public class ActivationElement
	{
		public ActionType ActionRestrictions = (ActionType)(-1);
		public MovementType MovementRestrictions = MovementType.Turn;

		public float ElementDuration = 1;
		public float WhileTickTime = 0.1f;
		public ActivationEffect[] StartEffects = new ActivationEffect[0];
		public ActivationEffect[] WhileEffects = new ActivationEffect[0];

		public ConditionObject[] PauseConditions = new ConditionObject[0];
		public ConditionObject[] YieldConditions = new ConditionObject[0];
		public ConditionObject[] StopConditions = new ConditionObject[0];

		public bool ShouldRestrictAction(ActionType restriction)
		{
			return ((int)ActionRestrictions | (int)restriction) == ((int)ActionRestrictions);
		}
		public bool ShouldRestrictMovement(MovementType restriction)
		{
			return ((int)MovementRestrictions | (int)restriction) == ((int)MovementRestrictions);
		}
	}
}