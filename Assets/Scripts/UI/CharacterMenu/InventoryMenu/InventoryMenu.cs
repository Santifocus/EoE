using EoE.Controlls;
using EoE.Entities;
using EoE.Information;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class InventoryMenu : CharacterMenuPage
	{
		private const float NAV_COOLDOWN = 0.15f;
		[SerializeField] private InventoryTarget target = default;
		[SerializeField] private InventorySlot slotPrefab = default;
		[SerializeField] private GridLayoutGroup slotGrid = default;
		[SerializeField] private ItemAction[] itemActions = default;
		[SerializeField] private DropMenu dropMenu = default;

		private enum InventoryTarget { Item, Weapons, Armor, Spells }
		private InventorySlot[] slots;
		[HideInInspector] public Inventory targetInventory;
		private float navigationCooldown;

		private bool actionMenuOpen;
		public int curSlotIndex { get; private set; }
		private int itemActionIndex;
		private bool dropMenuOpen;

		protected override void Start()
		{
			base.Start();
			switch (target)
			{
				case InventoryTarget.Item:
					targetInventory = Player.ItemInventory;
					break;
				case InventoryTarget.Weapons:
					targetInventory = Player.WeaponInventory;
					break;
				case InventoryTarget.Armor:
					targetInventory = Player.ArmorInventory;
					break;
				case InventoryTarget.Spells:
					targetInventory = Player.SpellInventory;
					break;
			}

			slots = new InventorySlot[targetInventory.Lenght];
			for (int i = 0; i < targetInventory.Lenght; i++)
			{
				InventorySlot newSlot = Instantiate(slotPrefab, slotGrid.transform);
				newSlot.Setup(targetInventory, i);
				slots[i] = newSlot;
			}

			dropMenu.Setup(this);
			targetInventory.InventoryChanged += UpdateAllSlots;
			UpdateAllSlots();
		}
		private void UpdateAllSlots()
		{
			for (int i = 0; i < slots.Length; i++)
			{
				slots[i].UpdateDisplay();
			}
		}
		protected override void ResetPage()
		{
			curSlotIndex = 0;
			slots[0].SelectMenuItem();
			actionMenuOpen = false;

			if (dropMenuOpen)
				HideDropMenu();
		}
		private void Update()
		{
			if (!ActivePage)
				return;
			SelectControll();
		}
		private void SelectControll()
		{
			if (dropMenuOpen)
				return;

			if (InputController.MenuEnter.Down && targetInventory[curSlotIndex] != null)
				MenuEnter();
			if (InputController.MenuBack.Down)
				MenuBack();

			if (navigationCooldown > 0)
			{
				navigationCooldown -= Time.unscaledDeltaTime;
				return;
			}

			int preSlotIndex = curSlotIndex;
			int preActionIndex = itemActionIndex;
			if (InputController.MenuRight.Active)
			{
				if (!actionMenuOpen)
					curSlotIndex++;
			}
			else if (InputController.MenuLeft.Active)
			{
				if (!actionMenuOpen)
					curSlotIndex--;
			}
			else if (InputController.MenuDown.Active)
			{
				if (!actionMenuOpen)
					curSlotIndex += SlotsPerRow();
				else
					itemActionIndex++;
			}
			else if (InputController.MenuUp.Active)
			{
				if (!actionMenuOpen)
					curSlotIndex -= SlotsPerRow();
				else
					itemActionIndex--;
			}

			if (preSlotIndex != curSlotIndex)
			{
				if (curSlotIndex < 0)
					curSlotIndex += slots.Length;
				else if (curSlotIndex >= slots.Length)
					curSlotIndex -= slots.Length;

				//Check again in case we scrolled down on a one-line inventory (eg.: Size:6, Index: 4 -> 10 -> 4)
				if (preSlotIndex != curSlotIndex)
				{
					navigationCooldown = NAV_COOLDOWN;
					slots[curSlotIndex].SelectMenuItem();
				}
			}

			if (preActionIndex != itemActionIndex)
			{
				if (itemActionIndex < 0)
					itemActionIndex += itemActions.Length;
				else if (itemActionIndex >= itemActions.Length)
					itemActionIndex -= itemActions.Length;

				navigationCooldown = NAV_COOLDOWN;
				itemActions[itemActionIndex].SelectMenuItem();
			}
		}
		private int SlotsPerRow()
		{
			float slotWidht = slotGrid.cellSize.x + slotGrid.spacing.x;
			float gridWidht = (slotGrid.transform as RectTransform).rect.width - slotGrid.padding.horizontal + slotGrid.spacing.x; //Have to add cellspacing once
			return Mathf.FloorToInt(gridWidht / slotWidht);
		}

		private void MenuEnter()
		{
			if (actionMenuOpen)
			{
				ItemAction.ItemActionType action = itemActions[itemActionIndex].actionType;
				if (action == ItemAction.ItemActionType.Back)
				{
					MenuBack();
					return;
				}

				InventoryItem item = targetInventory[curSlotIndex];
				if (item == null)
					return;

				Item target = item.data;
				if (action == ItemAction.ItemActionType.Use)
				{
					if (target)
						target.Use(Player.Instance, targetInventory);
				}
				else if (action == ItemAction.ItemActionType.Equip)
				{
					if (target)
						target.Equip(Player.Instance);
				}
				else if (action == ItemAction.ItemActionType.Drop)
				{
					if (target)
					{
						ShowDropMenu();
					}
				}

				//If from any of the action the stacksize was changed to 0 (which means null after inventory update) then we want to close the action menu
				if (targetInventory[curSlotIndex] == null)
					MenuBack();
			}
			else
			{
				actionMenuOpen = true;
				itemActionIndex = 0;
				itemActions[0].SelectMenuItem();
			}
		}
		private void ShowDropMenu()
		{
			dropMenuOpen = true;
			dropMenu.Show();
		}
		public void HideDropMenu()
		{
			dropMenuOpen = false;
			dropMenu.Hide();
		}
		public void MenuBack()
		{
			slots[curSlotIndex].SelectMenuItem();
			actionMenuOpen = false;
		}

		protected override void DeactivatePage()
		{
			gameObject.SetActive(false);
		}
	}
}