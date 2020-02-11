using EoE.Controlls;
using EoE.Behaviour.Entities;
using EoE.Information;
using EoE.Sounds;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EoE.Events;

namespace EoE.UI
{
	public class InventoryMenu : CharacterMenuPage
	{
		[SerializeField] private InventorySlot slotPrefab = default;
		[SerializeField] private GridLayoutGroup slotGrid = default;

		[SerializeField] private DropMenu dropMenu = default;
		[SerializeField] private ItemDescriptionDisplay itemDescriptionDisplay = default;
		[SerializeField] private TextMeshProUGUI currencyDisplay = default;

		[Space(5)]
		[Header("Displays")]
		[SerializeField] private Image equippedWeaponDisplay = default;
		[SerializeField] private Image equippedArmorDisplay = default;
		[SerializeField] private ItemEquipSlot[] equippedSpellDisplays = default;
		[SerializeField] private ItemEquipSlot[] equippedItemDisplays = default;

		[Space(5)]
		[Header("Options")]
		[SerializeField] private ItemAction useOption = default;
		[SerializeField] private ItemAction equipOption = default;
		[SerializeField] private ItemAction unEquipOption = default;
		[SerializeField] private ItemAction dropOption = default;
		[SerializeField] private ItemAction backOption = default;

		private InventorySlot[] slots;
		private float navigationCooldown;

		private bool actionMenuOpen;
		private bool equipSlotsOpen;
		private bool equippedItemsSelected;
		private bool dropMenuOpen;
		private bool isSetup;

		public int CurSlotIndex { get; private set; }
		private ItemAction this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return useOption;
					case 1:
						InventoryItem item = Player.Instance.Inventory[CurSlotIndex];
						bool isEquipped = ((item != null) && item.isEquiped);
						return isEquipped ? unEquipOption : equipOption;
					case 2:
						return dropOption;
					case 3:
						return backOption;
				}
				return null;
			}
		}


		private int itemActionIndex;
		private int equipedSlotIndex;
		private List<ItemAction> allowedActions = new List<ItemAction>();

		protected override void ResetPage()
		{
			if (!isSetup)
				Setup();

			CurSlotIndex = 0;
			SelectSlot(false);

			actionMenuOpen = dropMenuOpen = equipSlotsOpen = false;

			if (dropMenuOpen)
				HideDropMenu();
			UpdateEquippedSlots();
			UpdateCurrencyDisplay();
		}
		private void Setup()
		{
			isSetup = true;

			slots = new InventorySlot[Player.Instance.Inventory.Length];
			for (int i = 0; i < Player.Instance.Inventory.Length; i++)
			{
				InventorySlot newSlot = Instantiate(slotPrefab, slotGrid.transform);
				newSlot.Setup(Player.Instance.Inventory, i);
				slots[i] = newSlot;
			}

			dropMenu.Setup(this);
			Player.Instance.Inventory.InventoryChanged += UpdateAllSlots;
			EventManager.PlayerCurrencyChangedEvent += UpdateCurrencyDisplay;
			UpdateAllSlots();
		}
		private void Update()
		{
			if (!ActivePage)
				return;
			SelectControll();
		}
		private void UpdateAllSlots()
		{
			for (int i = 0; i < slots.Length; i++)
			{
				slots[i].UpdateDisplay();
			}
		}
		private void SelectControll()
		{
			if (dropMenuOpen)
				return;

			if (InputController.MenuEnter.Down && Player.Instance.Inventory[CurSlotIndex] != null)
				MenuEnter();
			if (InputController.MenuBack.Down)
				MenuBack();

			if (navigationCooldown > 0)
			{
				navigationCooldown -= Time.unscaledDeltaTime;
				return;
			}

			int preSlotIndex = CurSlotIndex;
			int preActionIndex = itemActionIndex;
			int preEquipIndex = equipedSlotIndex;

			if (InputController.MenuRight.Held)
			{
				if (!actionMenuOpen)
				{
					if (!equipSlotsOpen)
						CurSlotIndex++;
					else
						equipedSlotIndex++;
				}
			}
			else if (InputController.MenuLeft.Held)
			{
				if (!actionMenuOpen)
				{
					if (!equipSlotsOpen)
						CurSlotIndex--;
					else
						equipedSlotIndex--;

				}
			}
			else if (InputController.MenuDown.Held)
			{
				if (!equipSlotsOpen)
				{
					if (!actionMenuOpen)
						CurSlotIndex += SlotsPerRow();
					else
						itemActionIndex++;

				}
			}
			else if (InputController.MenuUp.Held)
			{
				if (!equipSlotsOpen)
				{
					if (!actionMenuOpen)
						CurSlotIndex -= SlotsPerRow();
					else
						itemActionIndex--;
				}
			}

			if (preSlotIndex != CurSlotIndex)
			{
				if (CurSlotIndex < 0)
					CurSlotIndex += slots.Length;
				else if (CurSlotIndex >= slots.Length)
					CurSlotIndex -= slots.Length;

				//Check again in case we scrolled down on a one-line inventory (eg.: Size:6, Index: 4 -> 10 -> 4)

				if (preSlotIndex != CurSlotIndex)
				{
					SelectSlot(true);
				}
			}

			if (preActionIndex != itemActionIndex)
			{
				int maxIndex = allowedActions.Count;
				if (itemActionIndex < 0)
					itemActionIndex += maxIndex;
				else if (itemActionIndex >= maxIndex)
					itemActionIndex -= maxIndex;

				navigationCooldown = NAV_COOLDOWN;

				allowedActions[preActionIndex].DeSelect();
				allowedActions[itemActionIndex].Select();

				PlayFeedback(true);
			}

			if (preEquipIndex != equipedSlotIndex)
			{
				int maxIndex = equippedItemsSelected ? equippedItemDisplays.Length : equippedSpellDisplays.Length;
				if (equipedSlotIndex < 0)
					equipedSlotIndex += maxIndex;
				else if (equipedSlotIndex >= maxIndex)
					equipedSlotIndex -= maxIndex;

				navigationCooldown = NAV_COOLDOWN;

				(equippedItemsSelected ? equippedItemDisplays : equippedSpellDisplays)[preEquipIndex].DeSelect();
				(equippedItemsSelected ? equippedItemDisplays : equippedSpellDisplays)[equipedSlotIndex].Select();

				PlayFeedback(true);
			}
		}
		private void SelectSlot(bool feedBack)
		{
			if (feedBack)
				PlayFeedback(true);

			navigationCooldown = NAV_COOLDOWN;
			slots[CurSlotIndex].Select();
			UpdateActionMenu();
			itemDescriptionDisplay.SetItem(Player.Instance.Inventory[CurSlotIndex]?.data);
		}
		private void UpdateActionMenu()
		{
			allowedActions = new List<ItemAction>(Player.SELECTABLE_ITEMS_AMOUNT);
			InventoryItem item = Player.Instance.Inventory[CurSlotIndex];

			bool isEquipped = ((item != null) && item.isEquiped);
			equipOption.gameObject.SetActive(!isEquipped);
			unEquipOption.gameObject.SetActive(isEquipped);

			for (int i = 0; i < 4; i++)
			{
				if (item != null)
				{
					bool allowed = (item.data.Uses | this[i].actionType) == item.data.Uses;

					if (this[i].actionType == InUIUses.Drop && item.data.NonRemoveable)
						allowed = false;

					if (allowed)
					{
						allowedActions.Add(this[i]);
					}
					this[i].SetAllowed(allowed);
				}
				else
				{
					this[i].SetAllowed(false);
				}
			}
		}
		private void PlayFeedback(bool succesSound)
		{
			SoundManager.SetSoundState(succesSound ? ConstantCollector.MENU_NAV_SOUND : ConstantCollector.FAILED_MENU_NAV_SOUND, true);
		}
		private int SlotsPerRow()
		{
			float slotWidht = slotGrid.cellSize.x + slotGrid.spacing.x;
			float gridWidht = (slotGrid.transform as RectTransform).rect.width - slotGrid.padding.horizontal + slotGrid.spacing.x;

			return Mathf.FloorToInt(gridWidht / slotWidht);
		}
		private void MenuEnter()
		{
			if (actionMenuOpen)
			{
				InUIUses action = allowedActions[itemActionIndex].actionType;
				if (action == InUIUses.Back)
				{
					MenuBack();
					return;
				}

				InventoryItem item = Player.Instance.Inventory[CurSlotIndex];
				if (item == null)
					return;

				Item target = item.data;
				if (action == InUIUses.Use)
				{
					if (target && item.data.curCooldown <= 0)
						target.Use(item, Player.Instance, Player.Instance.Inventory);

					UpdateEquippedSlots();
				}
				else if (action == InUIUses.Equip)
				{
					if (target)
					{
						bool unequip = item.isEquiped;
						if (target is WeaponItem || target is ArmorItem)
						{
							if (target is WeaponItem)
							{
								if (Player.Instance.EquipedWeapon != null)
								{
									Player.Instance.EquipedWeapon.isEquiped = false;
									Player.Instance.EquipedWeapon.data.UnEquip(Player.Instance.EquipedWeapon, Player.Instance);
									Player.Instance.EquipedWeapon = null;
								}

								if (!unequip)
								{
									Player.Instance.EquipedWeapon = item;
									Player.Instance.EquipedWeapon.isEquiped = true;
									target.Equip(Player.Instance.EquipedWeapon, Player.Instance);
								}
							}
							else //target is ArmorItem
							{
								if (Player.Instance.EquipedArmor != null)
								{
									Player.Instance.EquipedArmor.isEquiped = false;
									Player.Instance.EquipedArmor.data.UnEquip(Player.Instance.EquipedArmor, Player.Instance);
									Player.Instance.EquipedArmor = null;
								}

								if (!unequip)
								{
									Player.Instance.EquipedArmor = item;
									Player.Instance.EquipedArmor.isEquiped = true;
									target.Equip(Player.Instance.EquipedArmor, Player.Instance);
								}
							}

							equipSlotsOpen = actionMenuOpen = false;
							SelectSlot(true);
						}
						else //(target is (ActivationCompoundItem / ConsumableItem / any other not mentioned type))
						{
							if (unequip)
							{
								InventoryItem[] targetArray = (target is ActivationCompoundItem) ? Player.Instance.SelectableActivationCompoundItems : Player.Instance.SelectableItems;

								for (int i = 0; i < targetArray.Length; i++)
								{
									if (targetArray[i] == item)
									{
										targetArray[i].isEquiped = false;
										targetArray[i].data.UnEquip(targetArray[i], Player.Instance);
										targetArray[i] = null;
										Player.Instance.UpdateEquipedItems();
										equipSlotsOpen = actionMenuOpen = false;
										SelectSlot(true);
										break;
									}
								}
							}
							else
							{
								actionMenuOpen = false;
								equipSlotsOpen = true;
								equippedItemsSelected = !(target is ActivationCompoundItem);
								(equippedItemsSelected ? equippedItemDisplays : equippedSpellDisplays)[equipedSlotIndex = 0].Select();
							}
						}

						UpdateEquippedSlots();
					}
				}
				else if (action == InUIUses.Drop)
				{
					if (target)
					{
						ShowDropMenu();
					}
				}

				//If from any of the action the stacksize was changed to 0 (which means null after inventory update) then we want to close the action menu
				if (Player.Instance.Inventory[CurSlotIndex] == null)
					MenuBack();
			}
			else if (equipSlotsOpen)
			{
				InventoryItem[] targetArray = equippedItemsSelected ? Player.Instance.SelectableItems : Player.Instance.SelectableActivationCompoundItems;

				if (targetArray[equipedSlotIndex] != null)
				{
					targetArray[equipedSlotIndex].isEquiped = false;
					targetArray[equipedSlotIndex].data.UnEquip(targetArray[equipedSlotIndex], Player.Instance);
				}

				InventoryItem item = Player.Instance.Inventory[CurSlotIndex];
				targetArray[equipedSlotIndex] = item;
				targetArray[equipedSlotIndex].isEquiped = true;

				if (item == (equippedItemsSelected ? Player.Instance.EquipedItem : Player.Instance.EquipedSpell))
				{
					item.data.Equip(item, Player.Instance);
				}

				(equippedItemsSelected ? equippedItemDisplays : equippedSpellDisplays)[equipedSlotIndex].DeSelect();
				equipSlotsOpen = actionMenuOpen = false;
				SelectSlot(true);
				UpdateEquippedSlots();
				Player.Instance.SelectectableItemsChanged();
			}
			else
			{
				if (allowedActions.Count > 0)
				{
					actionMenuOpen = true;
					allowedActions[itemActionIndex = 0].Select();
				}
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

			for (int i = 0; i < equippedSpellDisplays.Length; i++)
			{
				if (Player.Instance.SelectableActivationCompoundItems[i] != null)
				{
					equippedSpellDisplays[i].sprite = Player.Instance.SelectableActivationCompoundItems[i].data.ItemIcon;
					equippedSpellDisplays[i].color = Color.white;
				}
				else
				{
					equippedSpellDisplays[i].sprite = null;
					equippedSpellDisplays[i].color = Color.clear;
				}
			}

			for (int i = 0; i < equippedItemDisplays.Length; i++)
			{
				if (Player.Instance.SelectableItems[i] != null)
				{
					equippedItemDisplays[i].sprite = Player.Instance.SelectableItems[i].data.ItemIcon;
					equippedItemDisplays[i].color = Color.white;
				}
				else
				{
					equippedItemDisplays[i].sprite = null;
					equippedItemDisplays[i].color = Color.clear;
				}
			}

			UpdateAllSlots();
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
			if (equipSlotsOpen)
			{
				(equippedItemsSelected ? equippedItemDisplays : equippedSpellDisplays)[equipedSlotIndex].DeSelect();
				equipSlotsOpen = false;
				actionMenuOpen = true;
			}
			else
			{
				SelectSlot(false);
				actionMenuOpen = false;
			}
		}
		private void UpdateCurrencyDisplay()
		{
			currencyDisplay.text = Player.Instance.CurrentCurrencyAmount.ToString();
		}
		protected override void DeactivatePage()
		{
			if (equipSlotsOpen)
				(equippedItemsSelected ? equippedItemDisplays : equippedSpellDisplays)[equipedSlotIndex].DeSelect();
			gameObject.SetActive(false);
			dropMenu.Hide();
		}
		private void OnDestroy()
		{
			if(Player.Instance)
				Player.Instance.Inventory.InventoryChanged -= UpdateAllSlots;
			EventManager.PlayerCurrencyChangedEvent -= UpdateCurrencyDisplay;
		}
	}
}