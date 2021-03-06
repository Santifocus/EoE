﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EoE.Behaviour.Entities;
using EoE.Information;

namespace EoE.Behaviour
{
	public class ShopInteractable : Interactable
	{
		[SerializeField] private ShopInventory shopData = default;
		[SerializeField] private TextMeshPro infoSign = default;
		[SerializeField] private ColoredText[] signText = default;
		[SerializeField] private TextBasedFX interactInfo = default;

		private ShopInventory shopDataInstance;
		private bool showingInteractInfo;
		private void Start()
		{
			shopDataInstance = Instantiate(shopData);
			canBeInteracted = true;
			infoSign.text = ColoredText.ToString(signText);
		}
		private void FixedUpdate()
		{
			if (isTarget && Player.Existant)
			{
				Vector3 facingDir = Vector3.Lerp(Player.Instance.ActuallWorldPosition, PlayerCameraController.PlayerCamera.transform.position, 0.35f);
				Vector3 signDir = (infoSign.transform.position - facingDir).normalized;
				infoSign.transform.forward = signDir;
			}
		}
		protected override void Interact()
		{
			if (!showingInteractInfo)
			{
				showingInteractInfo = true;
				FXInstance info = FXManager.ExecuteFX(interactInfo, Player.Instance.transform, true);
				GameController.BeginDelayedCall(() => { GameController.Shop.BuildShop(shopDataInstance); showingInteractInfo = false; },
												0,
												TimeType.Realtime,
												() => info.IsRemoved
												);
												
			}
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