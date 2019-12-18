using EoE.Information;
using TMPro;
using UnityEngine;

namespace EoE.Entities
{
	public class ItemDrop : Interactable
	{
		[SerializeField] private Rigidbody body = default;
		[SerializeField] private TextMeshPro infoDisplayPrefab = default;
		[SerializeField] private Vector3 infoDisplayOffset = new Vector3(0, 2, 0);
		[SerializeField] private Color amountColor = Color.red;
		private TextMeshPro infoDisplay;
		private InventoryItem containedItem;
		protected override void Interact()
		{
			int preStacksize = containedItem.stackSize;
			Player.Instance.Inventory.AddItem(containedItem);

			if (preStacksize != containedItem.stackSize)
			{
				for (int i = 0; i < Player.PlayerSettings.EffectsOnItemPickup.Length; i++)
				{
					FXManager.PlayFX(Player.PlayerSettings.EffectsOnItemPickup[i], transform, true);
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
			if (!isTarget || !Player.Instance.Alive)
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
			infoDisplay.text = "Pick up <color=#" + amountColHex + ">" + containedItem.stackSize + "x </color>" + containedItem.data.ItemName + "[A]";
		}
	}
}