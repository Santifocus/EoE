using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EoE.Behaviour.Entities;
using EoE.Information;

namespace EoE.Behaviour
{
	public class ItemDrop : Interactable
	{
		private const float FAILED_PICKUP_DELAY = 0.5f;

		public Rigidbody body = default;
		public Collider[] colls { get; private set; }
		[SerializeField] private FXObject[] EffectsOnInteract = default;
		[SerializeField] private TextMeshPro infoDisplay = default;
		[SerializeField] private Vector3 infoDisplayOffset = new Vector3(0, 2, 0);
		[SerializeField] private Color amountColor = Color.red;
		[SerializeField] private Notification failedPickUpNotification = default;

		private InventoryItem containedItem;
		private float FailedPickUpDelay;
		public void SetupItemDrop(InventoryItem containedItem, bool stopVelocity)
		{
			canBeInteracted = true;
			this.containedItem = containedItem;

			if (stopVelocity)
			{
				body.constraints = RigidbodyConstraints.FreezeRotation & RigidbodyConstraints.FreezePosition;
			}
			else
			{
				body.velocity = Random.insideUnitSphere.normalized * GameController.CurrentGameSettings.ItemDropRandomVelocityStrenght;
			}

			infoDisplay.gameObject.SetActive(false);
			string amountColHex = ColorUtility.ToHtmlStringRGBA(amountColor);
			infoDisplay.text = "Pick up <color=#" + amountColHex + ">" + containedItem.stackSize + "x </color>" + containedItem.data.ItemName + " [A]";

			colls = GetComponentsInChildren<Collider>();
		}
		protected override void Interact()
		{
			int preStacksize = containedItem.stackSize;
			Player.Instance.Inventory.AddItem(containedItem);

			if (preStacksize != containedItem.stackSize)
			{
				for (int i = 0; i < EffectsOnInteract.Length; i++)
				{
					if (EffectsOnInteract[i] is TextBasedFX)
					{
						(string, string)[] replaceInstructions = new (string, string)[2]
						{
							("{Name}", containedItem.data.ItemName.text.ToString()),
							("{Amount}", (preStacksize - containedItem.stackSize).ToString())
						};
						FXManager.ExecuteFX((EffectsOnInteract[i] as TextBasedFX).CreateInstructedNotification(replaceInstructions), Player.Instance.transform, true);
						continue;
					}
					FXManager.ExecuteFX(EffectsOnInteract[i], transform, true);
				}
			}
			else
			{
				if (FailedPickUpDelay <= 0)
				{
					FXManager.ExecuteFX(failedPickUpNotification, Player.Instance.transform, true);
					FailedPickUpDelay = FAILED_PICKUP_DELAY;
				}
			}

			if (containedItem.stackSize == 0)
			{
				Destroy(gameObject);
			}
		}
		private void LateUpdate()
		{
			if (FailedPickUpDelay > 0)
				FailedPickUpDelay -= Time.deltaTime;

			if (!isTarget || !Player.Existant)
				return;

			infoDisplay.transform.position = transform.position + infoDisplayOffset;
			Vector3 facingDir = Vector3.Lerp(Player.Instance.ActuallWorldPosition, PlayerCameraController.PlayerCamera.transform.position, 0.45f);
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