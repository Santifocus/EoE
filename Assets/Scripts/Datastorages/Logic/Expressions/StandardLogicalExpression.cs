using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information.Logic
{
	public class StandardLogicalExpression : LogicComponent
	{
		public LogicComponent[] LogicalComponents = new LogicComponent[0];
		protected override bool InternalTrue
		{
			get
			{
				for (int i = 0; i < LogicalComponents.Length; i++)
					if (LogicalComponents[i].True)
						return true;

				return false;
			}
		}
	}
}