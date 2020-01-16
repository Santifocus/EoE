using EoE.Information;
using TMPro;
using UnityEngine;

namespace EoE.Entities
{
	public class ItemDrop : Interactable
	{
		private const float FAILED_PICKUP_DELAY = 0.5f;

		[SerializeField] private Rigidbody body = default;
		[SerializeField] private TextMeshPro infoDisplayPrefab = default;
		[SerializeField] private Vector3 infoDisplayOffset = new Vector3(0, 2, 0);
		[SerializeField] private Color amountColor = Color.red;
		[SerializeField] private Notification failedPickUpNotification = default;
		private TextMeshPro infoDisplay;
		private InventoryItem containedItem;
		private float FailedPickUpDelay;
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
						FXManager.PlayFX((EffectsOnInteract[i] as TextBasedFX).CreateInstructedNotification(replaceInstructions), Player.Instance.transform, true);
						continue;
					}
					FXManager.PlayFX(EffectsOnInteract[i], transform, true);
				}
			}
			else
			{
				if (FailedPickUpDelay <= 0)
				{
					FXManager.PlayFX(failedPickUpNotification, Player.Instance.transform, true);
					FailedPickUpDelay = FAILED_PICKUP_DELAY;
				}
			}

			if (containedItem.stackSize == 0)
			{
				Destroy(infoDisplay.gameObject);
				Destroy(gameObject);
			}
		}
		private void LateUpdate()
		{
			if (FailedPickUpDelay > 0)
				FailedPickUpDelay -= Time.deltaTime;

			if (!isTarget || !Player.Targetable)
				return;

			infoDisplay.transform.position = transform.position + infoDisplayOffset;
			Vector3 facingDir = Vector3.Lerp(Player.Instance.actuallWorldPosition, PlayerCameraController.PlayerCamera.transform.position, 0.35f);
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

			infoDisplay = Instantiate(infoDisplayPrefab, Storage.ParticleStorage);
			infoDisplay.gameObject.SetActive(false);

			string amountColHex = ColorUtility.ToHtmlStringRGBA(amountColor);
			infoDisplay.text = "Pick up <color=#" + amountColHex + ">" + containedItem.stackSize + "x </color>" + containedItem.data.ItemName + " [A]";
		}
	}
}