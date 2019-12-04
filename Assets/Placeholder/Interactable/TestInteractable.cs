using EoE.UI;
using System.Collections;
using UnityEngine;

namespace EoE.Entities
{
	public class TestInteractable : Interactable
	{
		private static Dialogue.OnFinishDialogue soulCountInformInProgress;
		private static bool shouldSendSoulCountInform;
		private static bool setupStaticDoorInformer;

		[SerializeField] private int requiredSouls = default;
		[SerializeField] private float openTime = default;
		[SerializeField] private MeshRenderer rend = default;
		[SerializeField] private Vector3 rotationWhenOpen = default;
		[SerializeField] private Vector3 rotationWhenClosed = default;

		[SerializeField] private Collider coll = default;

		[SerializeField] private Vector3 offsetWhenOpen = default;
		[SerializeField] private Vector3 offsetWhenClosed = default;
		private bool openState;
		private Vector3 basePos;

		private void Start()
		{
			canBeInteracted = true;
			basePos = rend.transform.position;

			if (!setupStaticDoorInformer)
				SetupStaticDoorInformer();
		}
		private static void SetupStaticDoorInformer()
		{
			setupStaticDoorInformer = true;
			shouldSendSoulCountInform = true;
			soulCountInformInProgress += AllowSendSoulCountInform;
		}
		private static void AllowSendSoulCountInform()
		{
			shouldSendSoulCountInform = true;
		}
		protected override void Interact()
		{
			if (Player.Instance.TotalSoulCount >= requiredSouls)
				StartCoroutine(ChangeState(!openState));
			else if (shouldSendSoulCountInform)
			{
				shouldSendSoulCountInform = false;
				DialogueController.ShowDialogue(new Dialogue(soulCountInformInProgress, ("You cannot open this door, you need to have at least ", Color.white), (requiredSouls + " Souls", Color.red), (" to open it!", Color.white)));
			}
		}

		private IEnumerator ChangeState(bool openState)
		{
			canBeInteracted = false;
			float processTime = 0;
			coll.enabled = false;

			while (processTime < openTime)
			{
				yield return new WaitForEndOfFrame();
				processTime += Time.deltaTime;
				float progress = processTime / openTime;
				rend.transform.eulerAngles = Vector3.Lerp(openState ? rotationWhenClosed : rotationWhenOpen, openState ? rotationWhenOpen : rotationWhenClosed, progress);
				rend.transform.position = basePos + Vector3.Lerp(openState ? offsetWhenClosed : offsetWhenOpen, openState ? offsetWhenOpen : offsetWhenClosed, progress);
			}

			coll.enabled = true;

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