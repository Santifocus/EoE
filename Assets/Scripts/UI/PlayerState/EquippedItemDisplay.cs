using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EoE.UI
{
	public class EquippedItemDisplay : MonoBehaviour
	{
		private enum TargetItemType { Combat, Use }
		[SerializeField] private TargetItemType targetType = default;
		[SerializeField] private Image targetIconDisplay = default;
		[SerializeField] private Image cooldDownDisplay = default;
		[SerializeField] private TextMeshProUGUI itemCountDisplay = default;

		private InventoryItem lastTarget;
		private InventoryItem curTarget => targetType == TargetItemType.Combat ? (Player.Instance.MagicSelected ? Player.Instance.EquipedSpell : Player.Instance.EquipedWeapon) : Player.Instance.EquipedItem;

		private float lastCooldown;
		private int lastCount;
		private void Update()
		{
			//Update icons
			InventoryItem newTarget = curTarget;
			if(lastTarget != newTarget)
			{
				lastTarget = newTarget;

				targetIconDisplay.sprite = cooldDownDisplay.sprite = (newTarget == null) ? null : newTarget.data.ItemIcon;
				targetIconDisplay.color = (newTarget == null) ? Color.clear : Color.white;
				cooldDownDisplay.color = (newTarget == null) ? Color.clear : Color.black;

				if (itemCountDisplay)
					itemCountDisplay.gameObject.SetActive(newTarget != null);
				UpdateCooldown();
			}

			if(newTarget != null)
			{
				//Update cooldowndisplay
				if(lastCooldown > 0 && newTarget.useCooldown <= 0)
				{
					StartCoroutine(BlinkIcon());
				}
				float maxCooldown = MaxCooldown();
				UpdateCooldown();

				if (maxCooldown > 0)
					cooldDownDisplay.fillAmount = Mathf.Clamp01(lastCooldown / maxCooldown);
				else
					cooldDownDisplay.fillAmount = 0;

				//Update count
				if (itemCountDisplay && newTarget.stackSize != lastCount)
				{
					itemCountDisplay.text = newTarget.stackSize.ToString();
					lastCount = newTarget.stackSize;
				}
			}

		}
		private void UpdateCooldown()
		{
			lastCooldown = (lastTarget == null) ? (0) : ((targetType == TargetItemType.Combat && Player.Instance.MagicSelected ? Mathf.Max(Player.Instance.CastingCooldown, lastTarget.useCooldown) : lastTarget.useCooldown));
		}
		private float MaxCooldown()
		{
			if(targetType == TargetItemType.Combat && Player.Instance.MagicSelected)
			{
				return Mathf.Max(lastTarget.data.UseCooldown, (lastTarget.data as SpellItem).targetSpell.SpellCooldown);
			}
			else
				return lastTarget.data.UseCooldown;
		}

		private IEnumerator BlinkIcon()
		{
			yield return null;
		}
	}
}