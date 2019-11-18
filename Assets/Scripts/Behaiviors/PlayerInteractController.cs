using UnityEngine;

namespace EoE.Entities
{
	public class PlayerInteractController : MonoBehaviour
	{
		[SerializeField] private SphereCollider interactableSphere = default;

		private Collider currInteractTarget;

		private float? framesLowestDistance;
		private Collider curFrameTarget;
		private void FixedUpdate()
		{
			if (framesLowestDistance.HasValue)
			{
				framesLowestDistance = null;
				if (currInteractTarget != curFrameTarget)
				{
					//Tell old target its not targeted anymore
					if (Interactable.MarkedInteractable)
						Interactable.MarkedInteractable.StopMarkAsInteractableBase();

					//Inform the new target it is now the interact target
					currInteractTarget = curFrameTarget;
					Interactable target = currInteractTarget.GetComponent<Interactable>();
					target.MarkAsInteractTargetBase();
				}
			}
			else
			{
				if (Interactable.MarkedInteractable)
					Interactable.MarkedInteractable.StopMarkAsInteractableBase();
			}
			curFrameTarget = null;
			currInteractTarget = null;
		}
		private void OnTriggerStay(Collider other)
		{
			if (!framesLowestDistance.HasValue)
			{
				if (other.GetComponent<Interactable>().IsInteractable)
				{
					framesLowestDistance = (interactableSphere.bounds.center - other.bounds.center).sqrMagnitude;
					curFrameTarget = other;
				}
			}
			else
			{
				float selfDist = (interactableSphere.bounds.center - other.bounds.center).sqrMagnitude;
				if (selfDist < framesLowestDistance.Value)
				{
					if (other.GetComponent<Interactable>().IsInteractable)
					{
						framesLowestDistance = selfDist;
						curFrameTarget = other;
					}
				}
			}
		}
	}
}