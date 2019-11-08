using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EoE.UI
{
	public enum InventoryMenuFunction { UseItem, EquipWeapon, EquipArmor, EquipItem, Delete }
	public class InventorySlotMenuItem : MonoBehaviour
	{
		private static InventorySlotMenuItem SelectedMenuItem;
		private bool selected;
		[HideInInspector] public InventoryMenuFunction function { get; private set; }

		[SerializeField] private TextMeshProUGUI functionText = default;
		[SerializeField] private Image selectedBackground = default;
		[SerializeField] private Image nonSelectedBackground = default;
		public void Setup(InventoryMenuFunction function)
		{
			this.function = function;

			switch (function)
			{
				case InventoryMenuFunction.UseItem:
					functionText.text = "Use Item";
					break;
				case InventoryMenuFunction.EquipWeapon:
					functionText.text = "Equip Weapon";
					break;
				case InventoryMenuFunction.EquipArmor:
					functionText.text = "Equip Armor";
					break;
				case InventoryMenuFunction.EquipItem:
					functionText.text = "Equip Item";
					break;
				case InventoryMenuFunction.Delete:
					functionText.text = "Delete";
					break;
			}
		}

		public void Select()
		{
			if (SelectedMenuItem != null)
				SelectedMenuItem.DeSelect();

			SelectedMenuItem = this;
			selected = true;
			SetBackgroundState();
		}
		private void DeSelect()
		{
			if (SelectedMenuItem == this)
				SelectedMenuItem = null;

			selected = false;
			SetBackgroundState();
		}
		private void SetBackgroundState()
		{
			selectedBackground.gameObject.SetActive(selected);
			nonSelectedBackground.gameObject.SetActive(!selected);
		}
	}
}