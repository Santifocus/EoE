using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EoE.UI
{
	public class ShopSlot : MonoBehaviour
	{
		private static ShopSlot selectedShopSlot;

		//Inspector variables
		[SerializeField] private Image iconDisplay = default;
		[SerializeField] private Image itemTypeDisplay = default;
		[SerializeField] private TextMeshProUGUI stackDisplay = default;
		[SerializeField] private Image onSelectBackground = default;
		[SerializeField] private Image onNotSelectBackground = default;

		[Space(10)]
		[Header("Item Type Icons")]
		[SerializeField] private Sprite weaponItemIcon = default;
		[SerializeField] private Sprite armorItemIcon = default;
		[SerializeField] private Sprite spellItemIcon = default;
		[SerializeField] private Sprite useItemIcon = default;

		public Item containedItem { get; private set; }
		private int? containedStacksize;
		public int Stacksize 
		{ 
			get => containedStacksize.HasValue ? containedStacksize.Value : int.MaxValue;
			set
			{
				if (containedStacksize.HasValue)
				{
					containedStacksize = value;
					stackDisplay.text = value.ToString();
					if (value <= 0)
						HideSlot();
				}
			}
		}

		public void Setup(Item containedItem, int? containedStacksize = null)
		{
			if (containedItem != null && (!containedStacksize.HasValue || containedStacksize.Value > 0))
			{
				this.containedItem = containedItem;
				iconDisplay.sprite = containedItem.ItemIcon;
				iconDisplay.gameObject.SetActive(true);
				itemTypeDisplay.gameObject.SetActive(true);
				SetItemTypeIcon();

				this.containedStacksize = containedStacksize;
				stackDisplay?.gameObject.SetActive(containedStacksize.HasValue);
				if (containedStacksize.HasValue && stackDisplay)
				{
					stackDisplay.text = containedStacksize.Value.ToString();
				}
			}
			else
			{
				HideSlot();
			}
		}
		private void HideSlot()
		{
			this.containedItem = null;
			this.containedStacksize = null;

			iconDisplay.gameObject.SetActive(false);
			itemTypeDisplay.gameObject.SetActive(false);
			stackDisplay?.gameObject.SetActive(false);
		}
		private void SetItemTypeIcon()
		{
			if (containedItem is WeaponItem)
			{
				itemTypeDisplay.sprite = weaponItemIcon;
			}
			else if (containedItem is ArmorItem)
			{
				itemTypeDisplay.sprite = armorItemIcon;
			}
			else if (containedItem is ActivationCompoundItem)
			{
				itemTypeDisplay.sprite = spellItemIcon;
			}
			else
			{
				itemTypeDisplay.sprite = useItemIcon;
			}
		}
		public void Select()
		{
			if (selectedShopSlot)
				selectedShopSlot.DeSelect();
			selectedShopSlot = this;

			onSelectBackground.gameObject.SetActive(true);
			onNotSelectBackground.gameObject.SetActive(false);
		}
		private void DeSelect()
		{
			if (selectedShopSlot == this)
				selectedShopSlot = null;

			onSelectBackground.gameObject.SetActive(false);
			onNotSelectBackground.gameObject.SetActive(true);
		}
	}
}