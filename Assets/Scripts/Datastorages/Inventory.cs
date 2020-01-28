using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public enum InventoryAddability { Full, Capped, Nothing }
	public class Inventory
	{
		private InventoryItem[] containedItems;
		public InventoryItem this[int index] { get { if (index < 0 || index > Length) return null; else return containedItems[index]; } }
		private int inventorySize;
		public int Length { get => inventorySize; set => ChangeSize(value); }

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
		/// <summary>
		/// Returns how many items could not be added, returns 0 if the the whole stacksize can be fitted into the inventory.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="stackSize"></param>
		/// <returns></returns>
		public int CheckAddablity(Item item, int stackSize)
		{
			for (int i = 0; i < Length; i++)
			{
				if (containedItems[i] == null)
				{
					stackSize -= item.MaxStack;
					if (stackSize <= 0)
						break;
				}
				else if (containedItems[i].data == item)
				{
					stackSize -= item.MaxStack - containedItems[i].stackSize;
					if (stackSize <= 0)
						break;
				}
			}

			return System.Math.Max(0, stackSize);
		}
		public List<int> AddItem(InventoryItem toAdd)
		{
			int remainingStack = toAdd.stackSize;
			bool changed = false;
			List<int> targetSlots = new List<int>();
			//Try to add item to incomplete stacks
			for (int i = 0; i < Length; i++)
			{
				//Is there a item in this slot? If so is it the same type as the one we are trying to add? If either is false goto the next slot
				if (containedItems[i] == null || containedItems[i].data != toAdd.data)
					continue;

				int openStack = containedItems[i].data.MaxStack - containedItems[i].stackSize;

				if (openStack > 0)
				{
					changed = true;
					targetSlots.Add(i);
				}

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
			for (int i = 0; i < Length; i++)
			{
				if (containedItems[i] == null)
				{
					int size = Mathf.Min(remainingStack, toAdd.data.MaxStack);
					InventoryItem newItem = new InventoryItem(toAdd.data, size);
					remainingStack -= size;
					changed = true;
					targetSlots.Add(i);

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

			return targetSlots;
		}
		public void RemoveStackSize(Item data, int stackSize)
		{
			bool changed = false;
			for (int i = Length - 1; i >= 0; i--)
			{
				if (containedItems[i] != null && containedItems[i].data == data)
				{
					if (stackSize >= containedItems[i].stackSize)
					{
						stackSize -= containedItems[i].stackSize;
						containedItems[i].stackSize = 0;
						containedItems[i].OnRemove();
						containedItems[i] = null;
						changed = true;

						if (stackSize == 0)
							goto FullyRemovedStack;
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

		//We make a goto jump point here in case we succesfully removed the complete stack
		FullyRemovedStack:;

			if (changed)
				InventoryChanged?.Invoke();
		}
		public void RemoveStackSize(int index, int stackSize)
		{
			if (index < 0 || index >= Length)
				return;

			if (containedItems[index] != null)
			{
				containedItems[index].stackSize -= stackSize;
				if(containedItems[index].stackSize <= 0)
				{
					containedItems[index].OnRemove();
					containedItems[index] = null;
				}

				if(stackSize > 0)
					InventoryChanged?.Invoke();
			}
		}
		public void RemoveInventoryItem(InventoryItem item)
		{
			for (int i = 0; i < Length; i++)
			{
				if (containedItems[i] == item)
				{
					containedItems[i].OnRemove();
					containedItems[i] = null;
					InventoryChanged?.Invoke();
					break;
				}
			}
		}
		public void RemoveInventoryItem(InventoryItem item, int stackSize)
		{
			for (int i = 0; i < Length; i++)
			{
				if (containedItems[i] == item)
				{
					containedItems[i].stackSize -= stackSize;
					if (containedItems[i].stackSize <= 0)
					{
						containedItems[i].OnRemove();
						containedItems[i] = null;
					}
					InventoryChanged?.Invoke();
					break;
				}
			}
		}
		public bool Contains(Item type, int stack = 1)
		{
			for (int i = 0; i < Length; i++)
			{
				if (containedItems[i] != null && containedItems[i].data == type)
				{
					stack -= containedItems[i].stackSize;
					if (stack <= 0)
						return true;
				}
			}

			return false;
		}
		public void ClearSlot(int index)
		{
			if (index < 0 || index >= Length)
				return;

			if (containedItems[index] != null)
			{
				containedItems[index].OnRemove();
				containedItems[index] = null;
				InventoryChanged?.Invoke();
			}
		}
		public void Empty()
		{
			for (int i = 0; i < Length; i++)
			{
				if (containedItems[i] != null)
				{
					containedItems[i].OnRemove();
					containedItems[i] = null;
				}
			}
			InventoryChanged?.Invoke();
		}
		public void ForceUpdate()
		{
			//Make sure that all items that have 0 in stacksize will be deleted
			for (int i = 0; i < Length; i++)
			{
				if (containedItems[i] != null && containedItems[i].stackSize <= 0)
				{
					containedItems[i].OnRemove();
					containedItems[i] = null;
				}
			}
			InventoryChanged?.Invoke();
		}
		public override string ToString()
		{
			string fullString = "";

			for (int i = 0; i < Length; i++)
			{
				if (containedItems[i] != null)
				{
					fullString += i + ": " + containedItems[i].stackSize + "x " + containedItems[i].data.ItemName;
					if (i != Length - 1)
						fullString += ", ";
				}
				else
				{
					fullString += i + ": Empty";
					if (i != Length - 1)
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
		public FXInstance[] BoundFXInstances;

		public InventoryItem(Item data, int stackSize = 1)
		{
			this.data = data;
			this.stackSize = stackSize;
		}
		public void OnRemove()
		{
			StopBoundEffects();
			if (isEquiped)
				data.UnEquip(this, Entities.Player.Instance);
		}
		public void StopBoundEffects()
		{
			FXManager.FinishFX(ref BoundFXInstances);
		}
	}
}