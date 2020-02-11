using EoE.Controlls;
using EoE.Behaviour.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public abstract class ConditionObject : ScriptableObject
	{
		public bool Inverse = false;
		public bool True => Inverse != InternalTrue;
		protected abstract bool InternalTrue { get; }
	}
}