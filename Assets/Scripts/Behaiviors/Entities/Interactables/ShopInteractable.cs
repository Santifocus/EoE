using EoE.Information;
using EoE.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EoE.Entities
{
	public class ShopInteractable : Interactable
	{
		[SerializeField] private ShopInventory shopData = default;
		[SerializeField] private TextMeshPro infoSign = default;
		[SerializeField] private ColoredText[] signText = default;

		private ShopInventory shopDataInstance;
		private void Start()
		{
			shopDataInstance = Instantiate(shopData);
			canBeInteracted = true;
			infoSign.text = ColoredText.ToString(signText);
		}
		private void FixedUpdate()
		{
			if (isTarget && Player.Instance.Alive)
			{
				Vector3 facingDir = Vector3.Lerp(Player.Instance.actuallWorldPosition, PlayerCameraController.PlayerCamera.transform.position, 0.35f);
				Vector3 signDir = (infoSign.transform.position - facingDir).normalized;
				infoSign.transform.forward = signDir;
			}
		}
		protected override void Interact()
		{
			GameController.Shop.BuildShop(shopDataInstance);
		}

		protected override void MarkAsInteractTarget()
		{
			infoSign.gameObject.SetActive(true);
		}

		protected override void StopMarkAsInteractable()
		{
			infoSign.gameObject.SetActive(false);
		}
	}
}