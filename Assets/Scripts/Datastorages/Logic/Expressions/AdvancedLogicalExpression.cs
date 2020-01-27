using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information.Logic
{
	public class AdvancedLogicalExpression : LogicComponent
	{
		public int MinimumTrue = 0;
		public int MaximumTrue = 0;
		public LogicalElement[] Elements = new LogicalElement[0];

		protected override bool InternalTrue
		{
			get
			{
				int trueCount = 0;
				for (int i = 0; i < Elements.Length; i++)
				{
					if (Elements[i].True)
					{
						trueCount++;
						if (trueCount > MaximumTrue)
							break;
					}
				}

				return (trueCount >= MinimumTrue && trueCount <= MaximumTrue) != Inverse;
			}
		}
	}
	[System.Serializable]
	public class LogicalElement
	{
		public bool Inverse = false;
		public int MinimumTrue = 0;
		public int MaximumTrue = 0;
		public LogicComponent[] LogicalConditions = new LogicComponent[0];

		public bool True
		{
			get
			{
				int trueCount = 0;
				for(int i = 0; i < LogicalConditions.Length; i++)
				{
					if (LogicalConditions[i].True)
					{
						trueCount++;
						if (trueCount > MaximumTrue)
							break;
					}
				}

				return (trueCount >= MinimumTrue && trueCount <= MaximumTrue) != Inverse;
			}
		}
	}
}