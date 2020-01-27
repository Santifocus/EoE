using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information.Logic
{
	public abstract class LogicComponent : ScriptableObject
	{
		public bool Inverse = false;
		public bool True => InternalTrue != Inverse;
		protected abstract bool InternalTrue { get; }
	}
}