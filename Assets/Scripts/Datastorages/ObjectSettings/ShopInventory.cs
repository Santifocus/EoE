using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class ShopInventory : ScriptableObject
	{
		public ShopItem[] shopItems;

		[System.Serializable]
        public class ShopItem
		{
			public Item item;
			[Tooltip("-1 = Infinite purchases.")]
			public int maxPurchases = -1;
		}
	}
}