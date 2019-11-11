using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EoE.Information;

namespace EoE.UI
{
	public class InventorySlot : CMenuItem
	{
		[SerializeField] private Image iconDisplay = default;
		[SerializeField] private TextMeshProUGUI stackDisplay = default;
		[SerializeField] private Image onSelectBackground = default;
		[SerializeField] private Image onNotSelectBackground = default;

		private int inventoryIndex;
		private Inventory targetInventory;

		public void Setup(Inventory targetInventory, int inventoryIndex)
		{
			this.targetInventory = targetInventory;
			this.inventoryIndex = inventoryIndex;
			DeSelect();
		}

		public void UpdateDisplay()
		{
			bool empty = targetInventory[inventoryIndex] == null;
			if (!empty)
			{
				iconDisplay.sprite = targetInventory[inventoryIndex].data.ItemIcon;
				stackDisplay.text = targetInventory[inventoryIndex].stackSize.ToString();
			}

			iconDisplay.gameObject.SetActive(!empty);
			stackDisplay.gameObject.SetActive(!empty);
		}

		protected override void Select()
		{
			onSelectBackground.gameObject.SetActive(true);
			onNotSelectBackground.gameObject.SetActive(false);
		}
		protected override void DeSelect()
		{
			onSelectBackground.gameObject.SetActive(false);
			onNotSelectBackground.gameObject.SetActive(true);
		}
	}
}