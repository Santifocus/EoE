using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Entities
{
	public abstract class Interactable : MonoBehaviour
	{
		public static Interactable MarkedInteractable;
		public void MarkAsInteractTargetBase()
		{
			MarkedInteractable = this;
			MarkAsInteractTarget();
		}
		public void StopMarkAsInteractableBase()
		{
			if(MarkedInteractable == this)
				MarkedInteractable = null;
			StopMarkAsInteractable();
		}
		public abstract void Interact();
		protected abstract void MarkAsInteractTarget();
		protected abstract void StopMarkAsInteractable();
	}
}