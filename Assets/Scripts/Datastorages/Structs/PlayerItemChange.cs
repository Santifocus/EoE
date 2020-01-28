using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	[System.Serializable]
	public class PlayerItemChange
	{
		public Item Item = null;
		public int ItemCount = 0;
		public bool ForceEquip = false;

		public void Activate()
		{
			if (!Item)
				return;
			if(ItemCount > 0)
			{
				GiveItems();
			}
			else if(ItemCount < 0)
			{
				TakeItems();
			}
		}
		private void GiveItems()
		{
			//Add it to the inventory
			List<int> targetSlots = Player.Instance.Inventory.AddItem(new InventoryItem(Item, ItemCount));

			//Force equipp
			if (ForceEquip)
			{
				InventoryItem targetItem = Player.Instance.Inventory[targetSlots[targetSlots.Count - 1]];
				targetItem.isEquiped = true;
				//Find out what slot it belongs to, if it is a spell / normal item we try
				//to put it in a open slot, if all slots are filled we put it in the first
				if (Item is WeaponItem)
				{
					if (Player.Instance.EquipedWeapon != null)
					{
						Player.Instance.EquipedWeapon.isEquiped = false;
						Player.Instance.EquipedWeapon.data.UnEquip(Player.Instance.EquipedWeapon, Player.Instance);
					}

					Player.Instance.EquipedWeapon = targetItem;
					targetItem.isEquiped = true;
					targetItem.data.Equip(targetItem, Player.Instance);
				}
				else if (Item is ArmorItem)
				{
					if (Player.Instance.EquipedArmor != null)
					{
						Player.Instance.EquipedArmor.isEquiped = false;
						Player.Instance.EquipedArmor.data.UnEquip(Player.Instance.EquipedArmor, Player.Instance);
					}

					Player.Instance.EquipedArmor = targetItem;
					targetItem.isEquiped = true;
					targetItem.data.Equip(targetItem, Player.Instance);
				}
				else if (Item is ActivationCompoundItem)
				{
					bool added = false;

					//Try to find a slot that is null and put the item there
					for (int j = 0; j < Player.Instance.SelectableActivationCompoundItems.Length; j++)
					{
						if (Player.Instance.SelectableActivationCompoundItems[j] == null)
						{
							Player.Instance.SelectableActivationCompoundItems[j] = targetItem;
							Player.Instance.SelectableActivationCompoundItems[j].isEquiped = true;
							added = true;
							if (j == Player.Instance.selectedActivationCompoundItemIndex)
							{
								Player.Instance.SelectableActivationCompoundItems[j].data.Equip(targetItem, Player.Instance);
							}
							break;
						}
					}
					//couldnt find a null slot, put it in the first one, (just a fallback)
					if (!added && Player.Instance.SelectableActivationCompoundItems.Length > 0)
					{
						Player.Instance.SelectableActivationCompoundItems[0] = targetItem;
						Player.Instance.SelectableActivationCompoundItems[0].isEquiped = true;
						Player.Instance.SelectableActivationCompoundItems[0].data.Equip(targetItem, Player.Instance);
					}
				}
				else
				{
					bool added = false;

					//Try to find a slot that is null and put the item there
					for (int j = 0; j < Player.Instance.SelectableItems.Length; j++)
					{
						if (Player.Instance.SelectableItems[j] == null)
						{
							Player.Instance.SelectableItems[j] = targetItem;
							Player.Instance.SelectableItems[j].isEquiped = true;
							added = true;
							if (j == Player.Instance.selectedItemIndex)
							{
								Player.Instance.SelectableItems[j].data.Equip(targetItem, Player.Instance);
							}
							break;
						}
					}
					//couldnt find a null slot, put it in the first one, (just a fallback)
					if (!added && Player.Instance.SelectableItems.Length > 0)
					{
						Player.Instance.SelectableItems[0] = targetItem;
						Player.Instance.SelectableItems[0].isEquiped = true;
						Player.Instance.SelectableItems[0].data.Equip(targetItem, Player.Instance);
					}
				}
			}
		}
		private void TakeItems()
		{
			Player.Instance.Inventory.RemoveStackSize(Item, -ItemCount, true);
		}
	}
}