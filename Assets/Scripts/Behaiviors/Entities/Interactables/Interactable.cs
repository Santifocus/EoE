using UnityEngine;

namespace EoE.Entities
{
	public abstract class Interactable : MonoBehaviour
	{
		public static Interactable MarkedInteractable;
		protected bool isTarget { get; private set; }
		protected bool canBeInteracted;
		public void MarkAsInteractTargetBase()
		{
			isTarget = true;
			MarkedInteractable = this;
			MarkAsInteractTarget();
		}
		public void StopMarkAsInteractableBase()
		{
			isTarget = false;
			if (MarkedInteractable == this)
				MarkedInteractable = null;
			StopMarkAsInteractable();
		}
		public bool TryInteract()
		{
			if (!canBeInteracted)
				return false;

			Interact();
			return true;
		}
		protected abstract void Interact();
		protected abstract void MarkAsInteractTarget();
		protected abstract void StopMarkAsInteractable();
	}
}