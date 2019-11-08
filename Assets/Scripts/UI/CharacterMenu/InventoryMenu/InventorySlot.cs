using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.UI
{
	public class InventorySlot : CMenuItem
	{
		[SerializeField] private RectTransform slotMenuAnchor = default;
		[SerializeField] private InventorySlotMenuItem slotMenuItemPrefab = default;
		public static bool InventorySlotMenuOpen { get; private set; }
		private int selectedSlotMenuIndex;

		private int inventoryIndex;
		private bool requiresUpdate;

		private InventorySlotMenuItem[] menuItems;

		public void Setup(int inventoryIndex)
		{
			this.inventoryIndex = inventoryIndex;
			requiresUpdate = true;
			Player.PlayerInventory.InventoryChanged += () => requiresUpdate = true;
		}
		protected override void OnPress()
		{
			if (!InventorySlotMenuOpen)
			{ 
				InventorySlotMenuOpen = true;
				ShowSlotMenu();
			}
			else
			{

			}
		}
		protected override void OnBack()
		{
			if (InventorySlotMenuOpen)
			{
				InventorySlotMenuOpen = false;
				HideSlotMenu();
			}
		}
		private void ShowSlotMenu()
		{
			if (requiresUpdate)
				BuildSlotMenu();

			selectedSlotMenuIndex = 0;
			if(menuItems.Length > 0)
			{
				for (int i = 0; i < menuItems.Length; i++)
				{
					menuItems[i].gameObject.SetActive(true);
				}
				menuItems[selectedSlotMenuIndex].Select();
			}
		}
		private void HideSlotMenu()
		{
			for(int i = 0; i < menuItems.Length; i++)
			{
				menuItems[i].gameObject.SetActive(false);
			}
		}
		private void BuildSlotMenu()
		{
			requiresUpdate = false;
			if(menuItems != null)
			{
				for(int i = 0; i < menuItems.Length; i++)
				{
					Destroy(menuItems[i].gameObject);
				}
			}

			List<InventorySlotMenuItem> newMenuItems = new List<InventorySlotMenuItem>();
			if (Player.PlayerInventory[inventoryIndex].data.GivenFlags.Useable)
			{
				InventorySlotMenuItem useMenuItem = Instantiate(slotMenuItemPrefab, slotMenuAnchor);
				useMenuItem.Setup(InventoryMenuFunction.UseItem);
				newMenuItems.Add(useMenuItem);

				InventorySlotMenuItem equipItemMenuItem = Instantiate(slotMenuItemPrefab, slotMenuAnchor);
				equipItemMenuItem.Setup(InventoryMenuFunction.EquipItem);
				newMenuItems.Add(equipItemMenuItem);
			}

			if (Player.PlayerInventory[inventoryIndex].data.GivenFlags.Weapon)
			{
				InventorySlotMenuItem weaponMenuItem = Instantiate(slotMenuItemPrefab, slotMenuAnchor);
				weaponMenuItem.Setup(InventoryMenuFunction.EquipWeapon);
				newMenuItems.Add(weaponMenuItem);
			}

			if (Player.PlayerInventory[inventoryIndex].data.GivenFlags.Armor)
			{
				InventorySlotMenuItem armorMenuItem = Instantiate(slotMenuItemPrefab, slotMenuAnchor);
				armorMenuItem.Setup(InventoryMenuFunction.EquipArmor);
				newMenuItems.Add(armorMenuItem);
			}

			if (Player.PlayerInventory[inventoryIndex].data.GivenFlags.Deletable)
			{
				InventorySlotMenuItem deleteMenuItem = Instantiate(slotMenuItemPrefab, slotMenuAnchor);
				deleteMenuItem.Setup(InventoryMenuFunction.Delete);
				newMenuItems.Add(deleteMenuItem);
			}

			for(int i = 0; i < newMenuItems.Count; i++)
			{
				RectTransform r = (newMenuItems[i].transform as RectTransform);
				r.anchoredPosition = new Vector2(0, r.rect.height * i);
			}

			menuItems = newMenuItems.ToArray();
		}
		private void OnDestroy()
		{
			Player.PlayerInventory.InventoryChanged -= () => requiresUpdate = true;
		}
	}
}