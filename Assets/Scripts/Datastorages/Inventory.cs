using UnityEngine;

namespace EoE.Information
{
	public enum InventoryAddability { Full, Capped, Nothing }
	public class Inventory
	{
		private InventoryItem[] containedItems;
		public InventoryItem this[int index] { get { if (index < 0 || index > Lenght) return null; else return containedItems[index]; } }
		public int inventorySize;
		public int Lenght { get => inventorySize; set => ChangeSize(value); }

		public delegate void InventoryUpdate();
		public InventoryUpdate InventoryChanged;

		public Inventory(int size)
		{
			inventorySize = size;
			containedItems = new InventoryItem[size];
		}
		private void ChangeSize(int size)
		{
			InventoryItem[] newArray = new InventoryItem[size];

			for (int i = 0; i < size; i++)
			{
				if (i < inventorySize)
					newArray[i] = containedItems[i];
				else //Done filling
					break;
			}
			containedItems = newArray;
			inventorySize = size;

			InventoryChanged?.Invoke();
		}
		public InventoryAddability CheckAddablity(InventoryItem item)
		{
			int remainingStack = item.stackSize;
			for (int i = 0; i < Lenght; i++)
			{
				if (containedItems[i] == null)
				{
					remainingStack -= item.data.MaxStack;
					if (remainingStack <= 0)
						break;
				}
				else if (containedItems[i].data == item.data)
				{
					remainingStack -= item.data.MaxStack - containedItems[i].stackSize;
					if (remainingStack <= 0)
						break;
				}
			}

			//If the remaining stack = 0 we can completly put the item into the inventory, if the stack changed the answer is capped, if it stayed the same the answer is Nothing
			return remainingStack <= 0 ? InventoryAddability.Full : (remainingStack < item.stackSize ? InventoryAddability.Capped : InventoryAddability.Nothing);
		}
		public void AddItem(InventoryItem toAdd)
		{
			int remainingStack = toAdd.stackSize;
			bool changed = false;
			//Try to add item to incomplete stacks
			for (int i = 0; i < Lenght; i++)
			{
				//Is there a item in this slot? If so is it the same type as the one we are trying to add? If either is false goto the next slot
				if (containedItems[i] == null || containedItems[i].data != toAdd.data)
					continue;

				int openStack = containedItems[i].data.MaxStack - containedItems[i].stackSize;
				changed |= openStack > 0;

				if (openStack >= remainingStack)
				{
					containedItems[i].stackSize += remainingStack;
					remainingStack = 0;
					goto FullyAddedStack;
				}
				else
				{
					containedItems[i].stackSize = containedItems[i].data.MaxStack;
					remainingStack -= openStack;
				}

			}

			//Still something of the stack left? Try to add it to open slots
			for (int i = 0; i < Lenght; i++)
			{
				if (containedItems[i] == null)
				{
					int size = Mathf.Min(remainingStack, toAdd.data.MaxStack);
					InventoryItem newItem = new InventoryItem(toAdd.data, size);
					remainingStack -= size;
					changed = true;

					containedItems[i] = newItem;
					if (remainingStack == 0)
						goto FullyAddedStack;
				}
			}

		//We make a goto jump point here in case we succesfully add the fullstack
		FullyAddedStack:;

			//Update the stacksize of the item that we tried to add
			toAdd.stackSize = remainingStack;
			if (changed)
				InventoryChanged?.Invoke();
		}
		public void RemoveStackSize(Item data, int stackSize)
		{
			bool changed = false;
			for (int i = Lenght -1; i >= 0; i--)
			{
				if (containedItems[i] != null && containedItems[i].data == data)
				{
					if (stackSize >= containedItems[i].stackSize)
					{
						stackSize -= containedItems[i].stackSize;
						containedItems[i] = null;
						changed = true;
					}
					else
					{
						containedItems[i].stackSize -= stackSize;
						stackSize = 0;
						changed = true;
						goto FullyRemovedStack;
					}
				}
			}

			//We make a goto jump point here in case we succesfully remove the fullstack
			FullyRemovedStack:;

			if (changed)
				InventoryChanged?.Invoke();
		}
		public void RemoveInventoryItem(InventoryItem item)
		{
			for (int i = 0; i < Lenght; i++)
			{
				if (containedItems[i] == item)
				{
					containedItems[i] = null;
					InventoryChanged?.Invoke();
					break;
				}
			}
		}
		public void RemoveInventoryItem(InventoryItem item, int stackSize)
		{
			for (int i = 0; i < Lenght; i++)
			{
				if (containedItems[i] == item)
				{
					containedItems[i].stackSize -= stackSize;
					if (containedItems[i].stackSize <= 0)
						containedItems[i] = null;
					InventoryChanged?.Invoke();
					break;
				}
			}
		}
		public void ClearSlot(int index)
		{
			if (index < 0 || index >= Lenght)
				return;

			if (containedItems[index] != null)
			{
				containedItems[index] = null;
				InventoryChanged?.Invoke();
			}
		}
		public void Empty()
		{
			for (int i = 0; i < Lenght; i++)
			{
				if (containedItems[i] != null)
					containedItems[i] = null;
			}
			InventoryChanged?.Invoke();
		}
		public void ForceUpdate()
		{
			//Make sure that all items that have 0 in stacksize will be deleted
			for (int i = 0; i < Lenght; i++)
			{
				if (containedItems[i] != null && containedItems[i].stackSize <= 0)
				{
					containedItems[i] = null;
				}
			}
			InventoryChanged?.Invoke();
		}
		public override string ToString()
		{
			string fullString = "";

			for (int i = 0; i < Lenght; i++)
			{
				if (containedItems[i] != null)
				{
					fullString += i + ": " + containedItems[i].stackSize + "x " + containedItems[i].data.ItemName;
					if (i != Lenght - 1)
						fullString += ", ";
				}
				else
				{
					fullString += i + ": Empty";
					if (i != Lenght - 1)
						fullString += ", ";
				}
			}
			return fullString;
		}
	}
	public class InventoryItem
	{
		public readonly Item data;
		public int stackSize;
		public bool isEquiped;

		public InventoryItem(Item data, int stackSize = 1)
		{
			this.data = data;
			this.stackSize = stackSize;
		}
	}
}