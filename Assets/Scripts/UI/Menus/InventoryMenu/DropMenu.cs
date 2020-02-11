using EoE.Controlls;
using EoE.Behaviour.Entities;
using EoE.Information;
using TMPro;
using UnityEngine;

namespace EoE.UI
{
	public class DropMenu : MonoBehaviour
	{
		private const float NAV_COOLDOWN = 0.15f;
		private enum SelectedPart { Drop, Cancel, Count }

		[SerializeField] private TextMeshProUGUI dropCount = default;
		[SerializeField] private GameObject dropCountSelected = default;
		[SerializeField] private GameObject dropCountNotSelected = default;

		[SerializeField] private GameObject dropOptionSelected = default;
		[SerializeField] private GameObject dropOptionNotSelected = default;

		[SerializeField] private GameObject cancelOptionSelected = default;
		[SerializeField] private GameObject cancelOptionNotSelected = default;

		private InventoryMenu parent;
		private SelectedPart selectedPart;
		private int curDropCount;
		private int allowedMax => Player.Instance.Inventory[parent.CurSlotIndex].stackSize;
		private float navigationCooldown;

		public void Setup(InventoryMenu parent)
		{
			this.parent = parent;
		}
		public void Show()
		{
			selectedPart = SelectedPart.Count;
			curDropCount = 1;
			dropCount.text = curDropCount.ToString();

			gameObject.SetActive(true);
			UpdateSelectedState();
			navigationCooldown = NAV_COOLDOWN * 2;
		}
		private void Update()
		{
			if (navigationCooldown > 0)
				navigationCooldown -= Time.unscaledDeltaTime;

			if (navigationCooldown <= 0)
			{
				bool noInput = false;
				int sideInput = InputController.MenuRight.Down ? 1 : (InputController.MenuLeft.Down ? -1 : 0);
				if (sideInput != 0)
				{
					if (selectedPart == SelectedPart.Count)
					{
						curDropCount += sideInput;

						if (curDropCount > allowedMax)
							curDropCount = 1;
						else if (curDropCount == 0)
							curDropCount = allowedMax;

						dropCount.text = curDropCount.ToString();
					}
					else
					{
						if (selectedPart == SelectedPart.Cancel)
							selectedPart = SelectedPart.Drop;
						else
							selectedPart = SelectedPart.Cancel;

						UpdateSelectedState();
					}
					navigationCooldown = NAV_COOLDOWN;
				}
				else if (InputController.MenuUp.Down || InputController.MenuDown.Down)
				{
					if (selectedPart == SelectedPart.Count)
						selectedPart = SelectedPart.Drop;
					else
						selectedPart = SelectedPart.Count;

					UpdateSelectedState();
					navigationCooldown = NAV_COOLDOWN;
				}
				else if (InputController.MenuEnter.Down)
				{
					if (selectedPart != SelectedPart.Cancel)
					{
						InventoryItem targetItem = Player.Instance.Inventory[parent.CurSlotIndex];

						targetItem.data.CreateItemDrop(Player.Instance.ActuallWorldPosition, curDropCount, true);
						Player.Instance.Inventory.RemoveStackSize(targetItem.data, curDropCount);

						if (targetItem.stackSize <= 0)
						{
							parent.MenuBack();
						}
					}
					parent.HideDropMenu();
				}
				else if (InputController.MenuBack.Down)
				{
					parent.HideDropMenu();
				}
				else
				{
					noInput = true;
				}
				if (!noInput)
				{
					PlayFeedback(true);
				}
			}
		}
		private void PlayFeedback(bool succesSound)
		{
			Sounds.SoundManager.SetSoundState(succesSound ? ConstantCollector.MENU_NAV_SOUND : ConstantCollector.FAILED_MENU_NAV_SOUND, true);
		}
		private void UpdateSelectedState()
		{
			dropOptionSelected.SetActive(selectedPart == SelectedPart.Drop);
			dropOptionNotSelected.SetActive(selectedPart != SelectedPart.Drop);

			dropCountSelected.SetActive(selectedPart == SelectedPart.Count);
			dropCountNotSelected.SetActive(selectedPart != SelectedPart.Count);

			cancelOptionSelected.SetActive(selectedPart == SelectedPart.Cancel);
			cancelOptionNotSelected.SetActive(selectedPart != SelectedPart.Cancel);
		}
		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}