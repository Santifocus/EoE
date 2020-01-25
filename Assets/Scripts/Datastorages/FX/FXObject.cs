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
		public ConditionObject[] Conditions = new ConditionObject[0];
		public bool ConditionMet()
		{
			for (int i = 0; i < Conditions.Length; i++)
				if (Conditions[i].ConditionMet())
					return true;

			return false;
		}
	}
}