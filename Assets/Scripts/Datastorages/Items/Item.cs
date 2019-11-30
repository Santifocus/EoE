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
		public FXObject[] VFXEffectsOnUse;

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
			//We have basic item mechanics that we always want to execute, however in some cases the override OnUse
			//give us the info that the item cannot be used if that is the case we dont do any of the base mechanics
			if (OnUse(user))
			{
				PlayEffects(user);
				originStack.useCooldown = UseCooldown;
				if (RemoveOnUse)
				{
					origin.RemoveInventoryItem(originStack, 1);
				}
			}
		}
		private void PlayEffects(Entitie user)
		{
			for (int i = 0; i < VFXEffectsOnUse.Length; i++)
			{
				FXManager.PlayFX(VFXEffectsOnUse[i], user.transform, user is Player);
			}
		}
		protected virtual void OnEquip(Entitie user) { }
		protected abstract bool OnUse(Entitie user);
	}
}