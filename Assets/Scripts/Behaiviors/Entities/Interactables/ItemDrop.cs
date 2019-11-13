using EoE.Information;
using EoE.UI;
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
			AddToInventory();

			if (prevStackSize != containedItem.stackSize)
			{
				DialogueController.ShowDialogue(new Dialogue(null, ("You picked up ", Color.white), ((prevStackSize - containedItem.stackSize) + "x " + containedItem.data.ItemName, Color.green), ("!", Color.white)));
			}
			else
			{
				DialogueController.ShowDialogue(new Dialogue(null, ("Not enought Inventory Space to pick up ", Color.white), (containedItem.stackSize + "x " + containedItem.data.ItemName, Color.green), ("!", Color.white)));
			}

			if (containedItem.stackSize == 0)
				Destroy(gameObject);
		}
		private void AddToInventory()
		{
			if (containedItem.data is ArmorItem)
			{
				Player.ArmorInventory.AddItem(containedItem);
			}
			else if (containedItem.data is WeaponItem)
			{
				Player.WeaponInventory.AddItem(containedItem);
			}
			else if (containedItem.data is SpellItem)
			{
				Player.SpellInventory.AddItem(containedItem);
			}
			else //Any other Item type
			{
				Player.ItemInventory.AddItem(containedItem);
			}
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