using EoE.Information;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class InventorySlot : MonoBehaviour
	{
		public static InventorySlot SelectedInventorySlot { get; private set; }

		[SerializeField] private Image iconDisplay = default;
		[SerializeField] private Image itemTypeDisplay = default;
		[SerializeField] private TextMeshProUGUI stackDisplay = default;
		[SerializeField] private GameObject onSelectBackground = default;
		[SerializeField] private GameObject onNotSelectBackground = default;
		[SerializeField] private Image equippedIcon = default;

		[Space(10)]
		[Header("Item Type Icons")]
		[SerializeField] private Sprite weaponItemIcon = default;
		[SerializeField] private Sprite armorItemIcon = default;
		[SerializeField] private Sprite spellItemIcon = default;
		[SerializeField] private Sprite useItemIcon = default;
		[SerializeField] private Sprite monetaryItemIcon = default;

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

				if (targetInventory[inventoryIndex].data is WeaponItem)
				{
					itemTypeDisplay.sprite = weaponItemIcon;
				}
				else if (targetInventory[inventoryIndex].data is ArmorItem)
				{
					itemTypeDisplay.sprite = armorItemIcon;
				}
				else if (targetInventory[inventoryIndex].data is ActivationCompoundItem)
				{
					itemTypeDisplay.sprite = spellItemIcon;
				}
				else if(targetInventory[inventoryIndex].data is ConsumableItem)
				{
					itemTypeDisplay.sprite = useItemIcon;
				}
				else
				{
					itemTypeDisplay.sprite = monetaryItemIcon;
				}
			}

			iconDisplay.gameObject.SetActive(!empty);
			stackDisplay.gameObject.SetActive(!empty);
			equippedIcon.gameObject.SetActive(!(empty || !targetInventory[inventoryIndex].isEquiped));
		}

		public void Select()
		{
			if (SelectedInventorySlot != null)
				SelectedInventorySlot.DeSelect();

			SelectedInventorySlot = this;
			onSelectBackground.SetActive(true);
			onNotSelectBackground.SetActive(false);
		}
		public void DeSelect()
		{
			if (SelectedInventorySlot == this)
				SelectedInventorySlot = null;

			onSelectBackground.SetActive(false);
			onNotSelectBackground.SetActive(true);
		}
	}
}