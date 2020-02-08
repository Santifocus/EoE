using EoE.Entities;
using EoE.Information;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class EquippedItemDisplay : MonoBehaviour
	{
		private const int DISPLAYED_ITEMS = 3;
		private enum TargetItemType { ActivationCompound, Consumable }
		[SerializeField] private TargetItemType targetType = default;

		[SerializeField] private Image flashBorder = default;
		[SerializeField] private float flashTime = 0.5f;

		[SerializeField] private ItemDisplayCombo[] targetDisplays = new ItemDisplayCombo[DISPLAYED_ITEMS];
		private (InventoryItem, float, int)[] targets = new (InventoryItem, float, int)[DISPLAYED_ITEMS];

		private int curSelectIndex => (targetType == TargetItemType.ActivationCompound) ? Player.Instance.SelectedActivationCompoundItemIndex : Player.Instance.SelectedItemIndex;
		private int lastSelectIndex;
		private float flashTimer;
		private float flashMaxFlashAlpha;

		private void OnValidate()
		{
			if (flashTime <= 0)
				flashTime = 0.0001f;
			if (targetDisplays.Length != DISPLAYED_ITEMS)
			{
				System.Array.Resize(ref targetDisplays, DISPLAYED_ITEMS);
			}
		}

		private void Start()
		{
			flashMaxFlashAlpha = flashBorder.color.a;
			flashBorder.color = new Color(flashBorder.color.r, flashBorder.color.g, flashBorder.color.b, 0);

			for (int i = 0; i < targetDisplays.Length; i++)
			{
				targetDisplays[i].OriginalColor = targetDisplays[i].ItemIconDisplay.color;
				targetDisplays[i].ItemCooldownDisplayShadow.color = targetDisplays[i].ItemIconDisplay.color = Color.clear;
			}
		}

		private void Update()
		{
			//Update displays
			for (int i = 0; i < 3; i++)
			{
				UpdateDisplay(i);
			}
			UpdateSwapFlashing();
		}
		private void UpdateDisplay(int index)
		{
			int curIndex = (targetType == TargetItemType.ActivationCompound) ? Player.Instance.SelectedActivationCompoundItemIndex : Player.Instance.SelectedItemIndex;
			int targetIndex = ValidateID(curIndex + index - 1);
			InventoryItem target = (targetType == TargetItemType.ActivationCompound) ? Player.Instance.SelectableActivationCompoundItems[targetIndex] : Player.Instance.SelectableItems[targetIndex];

			if (targets[index].Item1 != target)
			{
				targets[index].Item1 = target;

				targetDisplays[index].ItemIconDisplay.sprite = targetDisplays[index].ItemCooldownDisplayShadow.sprite = (target == null) ? null : target.data.ItemIcon;
				targetDisplays[index].ItemIconDisplay.color = (target == null) ? Color.clear : targetDisplays[index].OriginalColor;
				targetDisplays[index].ItemCooldownDisplayShadow.color = (target == null) ? Color.clear : Color.black;

				if (targetDisplays[index].ItemCountDisplay)
					targetDisplays[index].ItemCountDisplay.gameObject.SetActive(target != null);

				if (targetDisplays[index].ItemCooldownDisplayText)
					targetDisplays[index].ItemCooldownDisplayText.gameObject.SetActive(target != null);

				targets[index].Item2 = GetCurCooldown(target);
			}

			if (target != null)
			{
				//If the cooldown of the item just reached 0:
				//inform the player that the cooldown went to zero via feedback
				float lastCooldown = targets[index].Item2;
				targets[index].Item2 = GetCurCooldown(target);
				if (lastCooldown > 0 && targets[index].Item2 <= 0)
				{
					//Blink
				}

				if (targetDisplays[index].ItemCooldownDisplayText)
				{
					targetDisplays[index].ItemCooldownDisplayText.gameObject.SetActive(targets[index].Item2 > 0);
					targetDisplays[index].ItemCooldownDisplayText.text = (Mathf.Round(targets[index].Item2 * 10) / 10).ToString();
				}

				//Update cooldowndisplay
				float maxCooldown = GetMaxCooldown(target);
				if (maxCooldown > 0)
				{
					targetDisplays[index].ItemCooldownDisplayShadow.fillAmount = Mathf.Clamp01(targets[index].Item2 / maxCooldown);
				}
				else
				{
					targetDisplays[index].ItemCooldownDisplayShadow.fillAmount = 0;
				}

				//Update count
				if ((targetDisplays[index].ItemCountDisplay) && (targets[index].Item3 != target.stackSize))
				{
					targets[index].Item3 = target.stackSize;
					targetDisplays[index].ItemCountDisplay.text = target.stackSize.ToString();
				}
			}
		}
		private void UpdateSwapFlashing()
		{
			int newSelectIndex = curSelectIndex;
			if (newSelectIndex != lastSelectIndex)
			{
				lastSelectIndex = newSelectIndex;
				flashTimer = flashTime;
			}

			if (flashTimer > 0)
			{
				flashTimer -= Time.unscaledDeltaTime;
				flashBorder.color = new Color(flashBorder.color.r, flashBorder.color.g, flashBorder.color.b, (flashTimer / flashTime) * flashMaxFlashAlpha);

				if (flashTimer <= 0)
				{
					flashBorder.color = new Color(flashBorder.color.r, flashBorder.color.g, flashBorder.color.b, 0);
				}
			}
		}
		private int ValidateID(int input)
		{
			int maxIndex = targetType == TargetItemType.ActivationCompound ? Player.Instance.SelectableActivationCompoundItems.Length : Player.Instance.SelectableItems.Length;
			while (input < 0)
				input += maxIndex;

			while (input >= maxIndex)
				input -= maxIndex;

			return input;
		}
		private float GetCurCooldown(InventoryItem target)
		{
			if (target == null)
				return 0;
			else
			{
				if (target.data is ActivationCompoundItem)
				{
					ActivationCompoundItem data = target.data as ActivationCompoundItem;
					if(data.TargetCompound.ActionType == Combatery.ActionType.Attacking)
					{
						return Player.Instance.AttackCooldown;
					}
					else if(data.TargetCompound.ActionType == Combatery.ActionType.Casting)
					{
						return Player.Instance.CastingCooldown;
					}
					else
					{
						return data.curCooldown;
					}
				}
				else
				{
					return target.data.curCooldown;
				}
			}
		}
		private float GetMaxCooldown(InventoryItem target)
		{
			if (target.data is ActivationCompoundItem)
				return (target.data as ActivationCompoundItem).TargetCompound.CausedCooldown;
			else
				return target.data.UseCooldown;
		}

		[System.Serializable]
		private class ItemDisplayCombo
		{
			public Color OriginalColor { get; set; }
			public Image ItemIconDisplay = default;
			public Image ItemCooldownDisplayShadow = default;
			public TextMeshProUGUI ItemCountDisplay = default;
			public TextMeshProUGUI ItemCooldownDisplayText = default;
		}
	}
}