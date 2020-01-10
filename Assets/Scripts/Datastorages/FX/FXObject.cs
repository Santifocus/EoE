using UnityEngine;

namespace EoE.Information
{
	public abstract class FXObject : ScriptableObject
	{
		public float TimeIn = 0;
		public float TimeOut = 0;
		public FinishConditions FinishConditions = new FinishConditions();
	}
	[System.Serializable]
	public class FinishConditions
	{
		public bool OnParentDeath = false;
		public bool OnTimeout = false;
		public float TimeStay = 1;
		public bool OnConditionMet = false;
		public ConditionObject Condition;
	}
}