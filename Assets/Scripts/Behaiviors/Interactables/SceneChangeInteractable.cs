using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EoE.Entities
{
	public class SceneChangeInteractable : Interactable
	{
		[SerializeField] private ColoredText[] infoText = default;
		[SerializeField] private TextMeshPro infoDisplay = default;
		[SerializeField] private Vector3 infoDisplayOffset = new Vector3(0, 2, 0);

		[SerializeField] protected int targetSceneIndex = 1;
		[SerializeField] protected bool doLoadingScreen = false;
		private void Start()
		{
			canBeInteracted = true;
			infoDisplay.text = ColoredText.ToString(infoText);
			infoDisplay.gameObject.SetActive(false);
		}
		protected override void Interact()
		{
			SceneLoader.TransitionToScene(targetSceneIndex, doLoadingScreen);
		}
		private void LateUpdate()
		{
			if (!isTarget || !Player.Existant)
				return;

			infoDisplay.transform.position = transform.position + infoDisplayOffset;
			Vector3 facingDir = Vector3.Lerp(Player.Instance.actuallWorldPosition, PlayerCameraController.PlayerCamera.transform.position, 0.45f);
			Vector3 signDir = (infoDisplay.transform.position - facingDir).normalized;
			infoDisplay.transform.forward = signDir;
		}

		protected override void MarkAsInteractTarget()
		{
			infoDisplay.gameObject.SetActive(true);
		}
		protected override void StopMarkAsInteractable()
		{
			infoDisplay.gameObject.SetActive(false);
		}
	}
}