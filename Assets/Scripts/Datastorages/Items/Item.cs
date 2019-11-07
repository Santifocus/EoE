using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class Item : ScriptableObject
	{
		public string ItemName = "Item";
		public ItemFlags GivenFlags;
		public GameObject ItemModel;
		public int MaxStack = 1;
		public Texture2D ItemIcon;

		public ItemDrop[] CreateItemDrop(Vector3 positon, int stackSize, bool stopVelocity)
		{
			ItemDrop[] createdItemDrops = new ItemDrop[stackSize / MaxStack + (stackSize % MaxStack != 0 ? 1 : 0)];

			for(int i = 0; i < createdItemDrops.Length; i++)
			{
				ItemDrop baseObject = Instantiate(GameController.Instance.itemDropPrefab, Storage.ItemStorage);
				InventoryItem item = new InventoryItem(this, (i == createdItemDrops.Length - 1 ? stackSize % MaxStack : MaxStack));
				GameObject model = Instantiate(ItemModel, baseObject.transform);

				baseObject.transform.position = positon;
				baseObject.SetupItemDrop(item, stopVelocity);
			}

			return createdItemDrops;
		}
	}

	[System.Serializable]
	public struct ItemFlags
	{
		public bool Useable;
		public bool Weapon;
		public bool Armor;
	}
}