using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Entities
{
	public class Door : Interactable
	{
		[SerializeField] private bool startState = false;
		[SerializeField] private MeshRenderer rend = default;
		[SerializeField] private Color notMarkedColor = Color.white;
		[SerializeField] private Color markedColor = Color.red;

		[Space(10)]
		[Header("Animation")]
		[SerializeField] private Animator animationControll = default;
		[SerializeField] private string openAnimationName = "Open";
		[SerializeField] private float openAnimationTime = 1;
		[SerializeField] private string closeAnimationName = "Close";
		[SerializeField] private float closeAnimationTime = 1;

		[Space(10)]
		[Header("Requirements")]
		[SerializeField] private int requiredSouls = 0;
		[SerializeField] private RequiredItem[] requiredItems = default;

		private bool transitioning;
		private bool open;
		private void Start()
		{
			open = startState;
			rend.material.color = notMarkedColor;
		}

		protected override void Interact()
		{
			if (transitioning || Player.TotalSoulCount < requiredSouls)
				return;

			//First check if the player has the required items
			//then remove the items that have the removeItem bool enabled
			for (bool remove = false; !remove; remove = !remove)
			{
				for (int i = 0; i < requiredItems.Length; i++)
				{
					Inventory targetInventory;

					//First find the inventory we are checking
					if (requiredItems[i].itemType is WeaponItem)
						targetInventory = Player.WeaponInventory;
					else if (requiredItems[i].itemType is ArmorItem)
						targetInventory = Player.ArmorInventory;
					else if (requiredItems[i].itemType is SpellItem)
						targetInventory = Player.SpellInventory;
					else
						targetInventory = Player.ItemInventory;

					//Now check if in the targetinventory contains the required item and the stacksize
					//if not stop here, if yes then if this is the remove loop then take the item out aswell
					//if this is the remove loop then we already confirmed that the item is in the invenory so we can skip the check
					if (remove || targetInventory.Contains(requiredItems[i].itemType, requiredItems[i].itemCount))
					{
						if(remove)
							targetInventory.RemoveStackSize(requiredItems[i].itemType, requiredItems[i].itemCount);
					}
					else
					{
						return;
					}
				}
			}

			StartCoroutine(TransitionDoorState(!open));
		}

		private IEnumerator TransitionDoorState(bool state)
		{
			animationControll.SetTrigger(state ? "Open" : "Close");
			transitioning = true;
			yield return new WaitForSeconds(state ? openAnimationTime : closeAnimationTime);
			transitioning = false;
			open = state;
		}

		protected override void MarkAsInteractTarget()
		{
			rend.material.color = markedColor;
		}

		protected override void StopMarkAsInteractable()
		{
			rend.material.color = notMarkedColor;
		}

		[System.Serializable]
		private class RequiredItem
		{
			public Item itemType;
			public int itemCount;
			public bool removeItem;
		}
	}
}