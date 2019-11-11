using EoE.Controlls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EoE.Information;
using EoE.Entities;

namespace EoE.UI
{
	public class DropMenu : MonoBehaviour
	{
		private enum SelectedPart { Drop, Cancel, Count }

		[SerializeField] private TextMeshProUGUI dropCount = default;
		[SerializeField] private GameObject dropCountSelected = default;
		[SerializeField] private GameObject dropCountNotSelected = default;

		[SerializeField] private GameObject dropOptionSelected = default;
		[SerializeField] private GameObject dropOptionNotSelected = default;

		[SerializeField] private GameObject cancelOptionSelected = default;
		[SerializeField] private GameObject cancelOptionNotSelected = default;

		private InventoryMenu parent;
		private SelectedPart selectedPart;
		private int curDropCount;
		private int allowedMax => parent.targetInventory[parent.curSlotIndex].stackSize;

		public void Setup(InventoryMenu parent)
		{
			this.parent = parent;
		}
		public void Show()
		{
			selectedPart = SelectedPart.Count;
			curDropCount = 1;
			dropCount.text = curDropCount.ToString();

			gameObject.SetActive(true);
			UpdateSelectedState();
		}
		private void Update()
		{
			int sideInput = InputController.MenuRight.Down ? 1 : (InputController.MenuLeft.Down ? -1 : 0);
			if (sideInput != 0)
			{
				if (selectedPart == SelectedPart.Count)
				{
					curDropCount += sideInput;

					if (curDropCount > allowedMax)
						curDropCount = 1;
					else if (curDropCount == 0)
						curDropCount = allowedMax;

					dropCount.text = curDropCount.ToString();
				}
				else
				{
					if (selectedPart == SelectedPart.Cancel)
						selectedPart = SelectedPart.Drop;
					else
						selectedPart = SelectedPart.Cancel;

					UpdateSelectedState();
				}
			}
			else if (InputController.MenuUp.Down || InputController.MenuDown.Down)
			{
				if (selectedPart == SelectedPart.Count)
					selectedPart = SelectedPart.Drop;
				else
					selectedPart = SelectedPart.Count;

				UpdateSelectedState();
			}
			else if (InputController.MenuEnter.Down)
			{
				bool fullyDropped = false;
				if(selectedPart != SelectedPart.Cancel) 
				{
					parent.targetInventory[parent.curSlotIndex].data.CreateItemDrop(Player.Instance.actuallWorldPosition, curDropCount, true);
					parent.targetInventory[parent.curSlotIndex].stackSize -= curDropCount;
					fullyDropped = parent.targetInventory[parent.curSlotIndex].stackSize == 0;
					parent.targetInventory.ForceUpdate();
				}
				parent.HideDropMenu();
				if (fullyDropped)
				{
					parent.MenuBack();
				}
			}
			else if (InputController.MenuBack.Down)
			{
				parent.HideDropMenu();
			}
		}
		private void UpdateSelectedState()
		{
			dropOptionSelected.SetActive(selectedPart == SelectedPart.Drop);
			dropOptionNotSelected.SetActive(selectedPart != SelectedPart.Drop);

			dropCountSelected.SetActive(selectedPart == SelectedPart.Count);
			dropCountNotSelected.SetActive(selectedPart != SelectedPart.Count);

			cancelOptionSelected.SetActive(selectedPart == SelectedPart.Cancel);
			cancelOptionNotSelected.SetActive(selectedPart != SelectedPart.Cancel);
		}
		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}