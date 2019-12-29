using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class ShopInventory : ScriptableObject
	{
		public ShopItem[] ShopItems;
	}

	[System.Serializable]
    public class ShopItem
	{
		public Item Item;
		public bool InfinitePurchases = true;
		public int MaxPurchases;
	}
}