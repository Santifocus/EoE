using EoE.Controlls;
using EoE.Entities;
using EoE.Information;
using EoE.Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class InventoryMenu : CharacterMenuPage
	{
		private const float NAV_COOLDOWN = 0.15f;
		[SerializeField] private InventorySlot slotPrefab = default;
		[SerializeField] private GridLayoutGroup slotGrid = default;
		[SerializeField] private ItemAction[] itemActions = default;
		[SerializeField] private DropMenu dropMenu = default;

		[SerializeField] private Image equippedWeaponDisplay = default;
		[SerializeField] private Image equippedSpellDisplay = default;
		[SerializeField] private Image equippedArmorDisplay = default;
		[SerializeField] private Image equippedItemDisplay = default;

		private InventorySlot[] slots;
		private float navigationCooldown;

		private bool actionMenuOpen;
		public int curSlotIndex { get; private set; }
		private int itemActionIndex;
		private bool dropMenuOpen;

		protected override void Start()
		{
			base.Start();
			slots = new InventorySlot[Player.Instance.Inventory.Lenght];
			for (int i = 0; i < Player.Instance.Inventory.Lenght; i++)
			{
				InventorySlot newSlot = Instantiate(slotPrefab, slotGrid.transform);
				newSlot.Setup(Player.Instance.Inventory, i);
				slots[i] = newSlot;
			}

			dropMenu.Setup(this);
			Player.Instance.Inventory.InventoryChanged += UpdateAllSlots;
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
			UpdateEquippedSlots();
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

			if (InputController.MenuEnter.Down && Player.Instance.Inventory[curSlotIndex] != null)
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
					PlayFeedback(true);
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
				PlayFeedback(true);
			}
		}
		private void PlayFeedback(bool succesSound)
		{
			SoundManager.SetSoundState(succesSound ? ConstantCollector.MENU_NAV_SOUND : ConstantCollector.FAILED_MENU_NAV_SOUND, true);
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

				InventoryItem item = Player.Instance.Inventory[curSlotIndex];
				if (item == null)
					return;

				Item target = item.data;
				if (action == ItemAction.ItemActionType.Use)
				{
					if (target && item.useCooldown <= 0)
						target.Use(item, Player.Instance, Player.Instance.Inventory);
					UpdateEquippedSlots();
				}
				else if (action == ItemAction.ItemActionType.Equip)
				{
					if (target)
					{
						bool shouldBeEquipped = true;
						if (target is WeaponItem)
						{
							if (Player.Instance.EquipedWeapon != null)
								Player.Instance.EquipedWeapon.data.UnEquip(Player.Instance.EquipedWeapon, Player.Instance);
							Player.Instance.EquipedWeapon = item;
							shouldBeEquipped = !Player.Instance.MagicSelected;
						}
						else if (target is SpellItem)
						{
							if (Player.Instance.EquipedSpell != null)
								Player.Instance.EquipedSpell.data.UnEquip(Player.Instance.EquipedSpell, Player.Instance);
							Player.Instance.EquipedSpell = item;
							shouldBeEquipped = Player.Instance.MagicSelected;
						}
						else if (target is ArmorItem)
						{
							if (Player.Instance.EquipedArmor != null)
								Player.Instance.EquipedArmor.data.UnEquip(Player.Instance.EquipedArmor, Player.Instance);
							Player.Instance.EquipedArmor = item;
						}
						else
						{
							if (Player.Instance.EquipedItem != null)
								Player.Instance.EquipedItem.data.UnEquip(Player.Instance.EquipedItem, Player.Instance);
							Player.Instance.EquipedItem = item;
						}

						if(shouldBeEquipped)
							target.Equip(item, Player.Instance);
						UpdateEquippedSlots();
					}
				}
				else if (action == ItemAction.ItemActionType.Drop)
				{
					if (target)
					{
						ShowDropMenu();
					}
				}

				//If from any of the action the stacksize was changed to 0 (which means null after inventory update) then we want to close the action menu
				if (Player.Instance.Inventory[curSlotIndex] == null)
					MenuBack();
			}
			else
			{
				actionMenuOpen = true;
				itemActionIndex = 0;
				itemActions[0].SelectMenuItem();
			}
		}
		private void UpdateEquippedSlots()
		{
			if (Player.Instance.EquipedWeapon != null)
			{
				equippedWeaponDisplay.sprite = Player.Instance.EquipedWeapon.data.ItemIcon;
				equippedWeaponDisplay.color = Color.white;
			}
			else
			{
				equippedWeaponDisplay.sprite = null;
				equippedWeaponDisplay.color = Color.clear;
			}

			if (Player.Instance.EquipedSpell != null)
			{
				equippedSpellDisplay.sprite = Player.Instance.EquipedSpell.data.ItemIcon;
				equippedSpellDisplay.color = Color.white;
			}
			else
			{
				equippedSpellDisplay.sprite = null;
				equippedSpellDisplay.color = Color.clear;
			}

			if (Player.Instance.EquipedArmor != null)
			{
				equippedArmorDisplay.sprite = Player.Instance.EquipedArmor.data.ItemIcon;
				equippedArmorDisplay.color = Color.white;
			}
			else
			{
				equippedArmorDisplay.sprite = null;
				equippedArmorDisplay.color = Color.clear;
			}

			if (Player.Instance.EquipedItem != null)
			{
				equippedItemDisplay.sprite = Player.Instance.EquipedItem.data.ItemIcon;
				equippedItemDisplay.color = Color.white;
			}
			else
			{
				equippedItemDisplay.sprite = null;
				equippedItemDisplay.color = Color.clear;
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
			UpdateEquippedSlots();
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