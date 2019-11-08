using EoE.Information;
using EoE.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Entities
{
	public class ItemDrop : Interactable
	{
		[SerializeField] private Rigidbody body = default;
		private InventoryItem containedItem;
		protected override void Interact()
		{
			int prevStackSize = containedItem.stackSize;
			Player.PlayerInventory.AddItem(containedItem);

			if(prevStackSize != containedItem.stackSize)
			{
				DialogueController.ShowDialogue(new Dialogue(null, ("You picked up ", Color.white), ((prevStackSize - containedItem.stackSize) + "x " + containedItem.data.ItemName, Color.green), ("!", Color.white)));
			}

			if (containedItem.stackSize == 0)
				Destroy(gameObject);
		}
		protected override void MarkAsInteractTarget()
		{

		}
		protected override void StopMarkAsInteractable()
		{

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
		}
	}
}