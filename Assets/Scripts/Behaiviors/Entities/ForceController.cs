using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Entities 
{
	public class ForceController
	{
		private const float FORCE_REMOVE_THRESHOLD = 0.2f;
		public Vector3 currentTotalForce { get; private set; }
		private List<SingleForce> currentForces;

		public ForceController()
		{
			currentForces = new List<SingleForce>();
		}
		public void Update()
		{
			currentTotalForce = Vector3.zero;
			for(int i = 0; i < currentForces.Count; i++)
			{
				Vector3 sForce = currentForces[i].UpdateForce();
				currentTotalForce += sForce;
				if(sForce.sqrMagnitude < FORCE_REMOVE_THRESHOLD)
				{
					currentForces.RemoveAt(i);
					i--;
				}
			}
		}
		public void ApplyForce(Vector3 Force, float Drag)
		{
			currentForces.Add(new SingleForce(Force, Drag));
		}

		private class SingleForce
		{
			private Vector3 Force;
			private float Drag;

			public SingleForce(Vector3 Force, float Drag)
			{
				this.Force = Force;
				this.Drag = Drag;
			}

			public Vector3 UpdateForce()
			{
				Force = Vector3.Lerp(Force, Vector3.zero, Time.fixedDeltaTime * Drag);
				return Force;
			}
		}
	}
}