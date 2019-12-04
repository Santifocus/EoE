using EoE.Entities;
using EoE.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	[System.Flags] public enum InUIUses { Use = 1, Equip = 2, Drop = 4, Back = 8 }
	public abstract class Item : ScriptableObject
	{
		public abstract InUIUses Uses { get; }
		public string ItemName = "Item";
		public int SortingID;
		public GameObject ItemModel;
		public int MaxStack = 1;
		public Sprite ItemIcon;
		public bool RemoveOnUse;
		public FXObject[] FXEffectsOnUse;
		public FXObject[] FXEffectsWhenEquipped;

		public float curCooldown { get; set; }
		public float UseCooldown;

		public ItemDrop[] CreateItemDrop(Vector3 positon, int stackSize, bool stopVelocity)
		{
			ItemDrop[] createdItemDrops = new ItemDrop[stackSize / MaxStack + (stackSize % MaxStack != 0 ? 1 : 0)];

			for (int i = 0; i < createdItemDrops.Length; i++)
			{
				ItemDrop baseObject = Instantiate(GameController.Instance.itemDropPrefab, Storage.DropStorage);
				InventoryItem item = new InventoryItem(this, ((i == createdItemDrops.Length - 1) && (stackSize % MaxStack != 0) ? stackSize % MaxStack : MaxStack));
				Instantiate(ItemModel, baseObject.transform);

				baseObject.transform.position = positon;
				baseObject.SetupItemDrop(item, stopVelocity);

				createdItemDrops[i] = baseObject;
			}

			return createdItemDrops;
		}

		public void Use(InventoryItem originStack, Entitie user, Inventory origin)
		{
			//We have basic item mechanics that we always want to execute, however in some cases the override OnUse
			//give us the info that the item cannot be used if that is the case we dont do any of the base mechanics
			if (OnUse(user))
			{
				PlayEffects(user, FXEffectsOnUse);
				curCooldown = UseCooldown;
				if (RemoveOnUse)
				{
					origin.RemoveInventoryItem(originStack, 1);
				}
			}
		}
		public void Equip(InventoryItem originStack, Entitie user)
		{
			if (OnEquip(user))
			{
				originStack.StopBoundEffects();
				originStack.BoundFXInstances = PlayEffects(user, FXEffectsWhenEquipped);
			}
		}
		public void UnEquip(InventoryItem originStack, Entitie user)
		{
			if (OnUnEquip(user))
			{
				originStack.StopBoundEffects();
			}
			OnUnEquip(user);
		}
		private FXInstance[] PlayEffects(Entitie user, FXObject[] effects)
		{
			FXInstance[] playedEffects = new FXInstance[effects.Length];
			for (int i = 0; i < effects.Length; i++)
			{
				playedEffects[i] = FXManager.PlayFX(effects[i], user.transform, user is Player);
			}
			return playedEffects;
		}
		protected virtual bool OnEquip(Entitie user) => false;
		protected virtual bool OnUnEquip(Entitie user) => true;
		protected virtual bool OnUse(Entitie user) => false;
	}
}