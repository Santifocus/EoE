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
		public void FixedUpdate()
		{
			currentTotalForce = Vector3.zero;
			for (int i = 0; i < currentForces.Count; i++)
			{
				Vector3 sForce = currentForces[i].UpdateForce();
				currentTotalForce += sForce;
				if (sForce.sqrMagnitude < FORCE_REMOVE_THRESHOLD)
				{
					currentForces[i].OnRemove();
					currentForces.RemoveAt(i);
					i--;
				}
			}
		}
		public SingleForce ApplyForce(Vector3 Force, float Drag, bool linearDrag = false, System.Action deleteCall = null)
		{
			SingleForce force = new SingleForce(Force, Drag, linearDrag, deleteCall);
			currentForces.Add(force);
			return force;
		}
		public bool ForceRemoveForce(SingleForce force)
		{
			if (currentForces.Contains(force))
			{
				currentTotalForce -= force.Force;
				force.OnRemove();
				currentForces.Remove(force);
				return true;
			}
			else
			{
				return false;
			}
		}
		public class SingleForce
		{
			public Vector3 Force { get; private set; }
			public Vector3 StartForce;

			public float Drag { get; private set; }
			public bool LinearDrag { get; private set; }
			public float PassedTime { get; private set; }

			public System.Action deleteCall { get; private set; }

			public SingleForce(Vector3 Force, float Drag, bool LinearDrag, System.Action deleteCall)
			{
				this.StartForce = this.Force = Force;
				this.Drag = Drag;
				this.LinearDrag = LinearDrag;
				this.deleteCall = deleteCall;
			}

			public Vector3 UpdateForce()
			{
				if (LinearDrag)
				{
					PassedTime += Time.fixedDeltaTime;
					Force = Vector3.Lerp(StartForce, Vector3.zero, PassedTime * Drag);
				}
				else
				{
					Force = Vector3.Lerp(Force, Vector3.zero, Time.fixedDeltaTime * Drag);
				}
				return Force;
			}

			public void OnRemove()
			{
				if (deleteCall != null)
					deleteCall?.Invoke();
			}
		}
	}
}