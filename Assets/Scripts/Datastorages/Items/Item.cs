using EoE.Behaviour;
using EoE.Behaviour.Entities;
using EoE.UI;
using UnityEngine;

namespace EoE.Information
{
	[System.Flags] public enum InUIUses { Use = 1, Equip = 2, Drop = 4, Back = 8 }
	public enum ItemSpecialFlag { None = 1, NonRemoveable = 2, OnlySellable = 3 }
	public abstract class Item : ScriptableObject
	{
		[Header("UI Data")]
		public ColoredText ItemName = new ColoredText() { text = "Item", textColor = Color.white };
		public ColoredText[] ItemDescription = new ColoredText[0];
		public int SortingID;
		public Sprite ItemIcon;

		[Space(5)]
		[Header("Effects")]
		public FXObject[] FXEffectsOnUse;
		public FXObject[] FXEffectsWhenEquipped;

		[Space(5)]
		[Header("Other")]
		public bool AllowHoldUse;
		public float UseCooldown;
		public bool RemoveOnUse;
		public GameObject ItemModel;
		public int MaxStack = 1;
		public int ItemWorth = 10;
		public bool NonRemoveable = false;

		//Internal
		public float curCooldown { get; set; }
		public abstract InUIUses Uses { get; }

		public ItemDrop[] CreateItemDrop(Vector3 positon, int stackSize, bool stopVelocity)
		{
			ItemDrop[] createdItemDrops = new ItemDrop[stackSize / MaxStack + (stackSize % MaxStack != 0 ? 1 : 0)];

			for (int i = 0; i < createdItemDrops.Length; i++)
			{
				ItemDrop baseObject = Instantiate(GameController.Instance.itemDropPrefab, Storage.DropStorage);
				InventoryItem item = new InventoryItem(this, ((i == createdItemDrops.Length - 1) && (stackSize % MaxStack != 0) ? stackSize % MaxStack : MaxStack));
				if(ItemModel)
					Instantiate(ItemModel, baseObject.transform);

				baseObject.transform.position = positon;
				baseObject.SetupItemDrop(item, stopVelocity);

				createdItemDrops[i] = baseObject;
			}

			return createdItemDrops;
		}

		public void Use(InventoryItem originStack, Entity user, Inventory origin)
		{
			//We have basic item mechanics that we always want to execute, however in some cases the override OnUse
			//give us the info that the item cannot be used if that is the case we dont do any of the base mechanics
			if (OnUse(user))
			{
				FXManager.ExecuteFX(FXEffectsOnUse, user.transform, user is Player);
				curCooldown = UseCooldown;
				if (RemoveOnUse)
				{
					origin.RemoveInventoryItem(originStack, 1);
				}
			}
		}
		public void Equip(InventoryItem originStack, Entity user)
		{
			if (OnEquip(user))
			{
				originStack.StopBoundEffects();
				FXManager.ExecuteFX(FXEffectsWhenEquipped, user.transform, user is Player, out originStack.BoundFXInstances);
			}
		}
		public void UnEquip(InventoryItem originStack, Entity user)
		{
			if (OnUnEquip(user))
			{
				originStack.StopBoundEffects();
			}
		}
		protected virtual bool OnEquip(Entity user) => false;
		protected virtual bool OnUnEquip(Entity user) => true;
		protected virtual bool OnUse(Entity user) => false;
	}
}