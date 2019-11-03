using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Entities
{
	public class TestInteractable : Interactable
	{
		[SerializeField] private float openTime = default;
		[SerializeField] private MeshRenderer rend = default;
		[SerializeField] private Vector3 rotationWhenOpen = default;
		[SerializeField] private Vector3 rotationWhenClosed = default;

		[SerializeField] private Vector3 offsetWhenOpen = default;
		[SerializeField] private Vector3 offsetWhenClosed = default;
		private bool openState;
		private Vector3 basePos;

		private void Start()
		{
			canBeInteracted = true;
			basePos = rend.transform.position;
		}
		protected override void Interact()
		{
			StartCoroutine(ChangeState(!openState));
		}

		private IEnumerator ChangeState(bool openState)
		{
			canBeInteracted = false;
			float processTime = 0;

			while(processTime < openTime)
			{
				yield return new WaitForEndOfFrame();
				processTime += Time.deltaTime;
				float progress = processTime / openTime;
				rend.transform.eulerAngles = Vector3.Lerp(openState ? rotationWhenClosed : rotationWhenOpen, openState ? rotationWhenOpen : rotationWhenClosed, progress);
				rend.transform.position = basePos + Vector3.Lerp(openState ? offsetWhenClosed : offsetWhenOpen, openState ? offsetWhenOpen : offsetWhenClosed, progress);
			}

			this.openState = openState;
			canBeInteracted = true;
		}

		protected override void MarkAsInteractTarget()
		{
			rend.material.color = Color.Lerp(Color.red, Color.white, 0.75f);
		}

		protected override void StopMarkAsInteractable()
		{
			rend.material.color = Color.white;
		}
	}
}