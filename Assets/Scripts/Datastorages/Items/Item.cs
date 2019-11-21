using EoE.Entities;
using EoE.Utils;
using UnityEngine;

namespace EoE.Information
{
	public abstract class Item : ScriptableObject
	{
		public string ItemName = "Item";
		public GameObject ItemModel;
		public int MaxStack = 1;
		public Sprite ItemIcon;
		public bool RemoveOnUse;
		public float UseCooldown;
		public FXInstance[] VFXEffectsOnUse;

		public ItemDrop[] CreateItemDrop(Vector3 positon, int stackSize, bool stopVelocity)
		{
			ItemDrop[] createdItemDrops = new ItemDrop[stackSize / MaxStack + (stackSize % MaxStack != 0 ? 1 : 0)];

			for (int i = 0; i < createdItemDrops.Length; i++)
			{
				ItemDrop baseObject = Instantiate(GameController.Instance.itemDropPrefab, Storage.DropStorage);
				InventoryItem item = new InventoryItem(this, ((i == createdItemDrops.Length - 1) && (stackSize % MaxStack != 0) ? stackSize % MaxStack : MaxStack));
				GameObject model = Instantiate(ItemModel, baseObject.transform);

				baseObject.transform.position = positon;
				baseObject.SetupItemDrop(item, stopVelocity);

				createdItemDrops[i] = baseObject;
			}

			return createdItemDrops;
		}

		public void Equip(Entitie user)
		{
			OnEquip(user);
		}
		public void Use(InventoryItem originStack, Entitie user, Inventory origin)
		{
			originStack.useCooldown = UseCooldown;
			if (RemoveOnUse)
			{
				origin.RemoveInventoryItem(originStack, 1);
			}
			PlayEffects(user);
			OnUse(user);
		}
		private void PlayEffects(Entitie user)
		{
			for (int i = 0; i < VFXEffectsOnUse.Length; i++)
			{
				FXManager.PlayFX(VFXEffectsOnUse[i], user.transform);
			}
		}
		protected virtual void OnEquip(Entitie user) { }
		protected abstract void OnUse(Entitie user);
	}
}