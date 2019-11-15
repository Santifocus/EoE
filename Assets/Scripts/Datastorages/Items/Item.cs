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
		public VFXEffect[] VFXEffectsOnUse;

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
			if (this is WeaponItem)
			{
				Player.EquipedWeapon = this as WeaponItem;
			}
			else if (this is SpellItem)
			{
				Player.EquipedSpell = this as SpellItem;
			}
			else if (this is ArmorItem)
			{
				Player.EquipedArmor = this as ArmorItem;
			}
			else
			{
				Player.EquipedItem = this;
			}

			OnEquip(user);
		}
		public void Use(Entitie user, Inventory origin)
		{
			if (RemoveOnUse)
			{
				origin.RemoveStackSize(this, 1);
			}
			PlayEffects(user);
			OnUse(user);
		}
		private void PlayEffects(Entitie user)
		{
			if (GameController.GameIsPaused)
				return;

			for (int i = 0; i < VFXEffectsOnUse.Length; i++)
			{
				if(VFXEffectsOnUse[i] is ParticleEffect)
				{
					EffectUtils.PlayParticleEffect(VFXEffectsOnUse[i] as ParticleEffect, user);
				}
				else if(user is Player)
				{
					PlayerVFXManager.PlayVFX(VFXEffectsOnUse[i]);
				}
			}
		}
		protected virtual void OnEquip(Entitie user) { }
		protected abstract void OnUse(Entitie user);
	}
}