using EoE.Information;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class InventorySlot : CMenuItem
	{
		[SerializeField] private Image iconDisplay = default;
		[SerializeField] private Image itemTypeDisplay = default;
		[SerializeField] private TextMeshProUGUI stackDisplay = default;
		[SerializeField] private Image onSelectBackground = default;
		[SerializeField] private Image onNotSelectBackground = default;
		[SerializeField] private Image equippedIcon = default;

		[Space(10)]
		[Header("Item Type Icons")]
		[SerializeField] private Sprite weaponItemIcon = default;
		[SerializeField] private Sprite armorItemIcon = default;
		[SerializeField] private Sprite spellItemIcon = default;
		[SerializeField] private Sprite useItemIcon = default;

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
				else if (targetInventory[inventoryIndex].data is SpellItem)
				{
					itemTypeDisplay.sprite = spellItemIcon;
				}
				else
				{
					itemTypeDisplay.sprite = useItemIcon;
				}
			}

			iconDisplay.gameObject.SetActive(!empty);
			stackDisplay.gameObject.SetActive(!empty);
			equippedIcon.gameObject.SetActive(!(empty || !targetInventory[inventoryIndex].isEquiped));
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