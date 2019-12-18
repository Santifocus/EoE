using EoE.Information;
using System.Collections;
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
			//If the door is transitioning we cant interact with it
			//We also check if the required soulcount is reached
			if (transitioning || Player.Instance.TotalSoulCount < requiredSouls)
				return;

			//First check if the player has the required items
			//then remove the items that have the removeItem bool enabled
			for (bool remove = false; !remove; remove = !remove)
			{
				for (int i = 0; i < requiredItems.Length; i++)
				{
					//First we check if the player has this particular item, if not we can stop here,
					//If we find all items that should be removed then we can start removing them,
					//If we are in the second iteration of the removing loop we can skip the contains check
					if (remove || Player.Instance.Inventory.Contains(requiredItems[i].itemType, requiredItems[i].itemCount))
					{
						if (remove)
							Player.Instance.Inventory.RemoveStackSize(requiredItems[i].itemType, requiredItems[i].itemCount);
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
			animationControll.SetTrigger(state ? openAnimationName : closeAnimationName);
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
			public RequiredItem(Item itemType, int itemCount, bool removeItem)
			{
				this.itemType = itemType;
				this.itemCount = itemCount;
				this.removeItem = removeItem;
			}
		}
	}
}