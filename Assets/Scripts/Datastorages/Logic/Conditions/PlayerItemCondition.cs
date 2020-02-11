using EoE.Behaviour.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information.Logic
{
	public class PlayerItemCondition : LogicComponent
	{
		protected override bool InternalTrue => ItemCheck();
		public Item TargetItem = null;
		public int MinStackSize = 1;
		public bool HasMaxStackSize = false;
		public int MaxStackSize = 100;
		public bool HasToBeEquipped = false;

		private bool ItemCheck()
		{
			if (!Player.Instance)
				return false;

			int totalStack = 0;
			bool equipConditionMet = !HasToBeEquipped;

			for(int i = 0; i < Player.Instance.Inventory.Length; i++)
			{
				//Is the item in slot i the one we are looking for?
				if (Player.Instance.Inventory[i] != null && Player.Instance.Inventory[i].data == TargetItem)
				{
					//Add the stacksize of that slot to the totalstack
					totalStack += Player.Instance.Inventory[i].stackSize;
					//OR the equip condition
					equipConditionMet |= Player.Instance.Inventory[i].isEquiped;

					//If we limit how big the stacksize can be at max, then we want to quit the loop if that max is exceeded
					if (HasMaxStackSize)
					{
						if (totalStack > MaxStackSize)
							break;
					}
					//Otherwise we can break the loop if we reached the minimum stacksize as long as the equip condition is true aswell
					//Because the return value will not change to false from here on out
					else if((totalStack >= MinStackSize) && (equipConditionMet))
					{
						break;
					}
				}
			}


			return ((totalStack >= MinStackSize) && (!HasMaxStackSize || totalStack <= MaxStackSize) && (equipConditionMet));
		}
	}
}