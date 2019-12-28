using EoE.Entities;
using EoE.Events;
using EoE.Information;
using EoE.Controlls;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class ShopController : MonoBehaviour
	{
		private const float NAV_COOLDOWN = 0.2f;
		public enum ShopMode { None = 1, Buy = 2, Sell = 3}
		private enum ShopState { ModeSelection = 1, ItemSelection = 2, SlotAction = 3, ActionAmount = 4 }

		//Constants
		//Inpsector variables
		[SerializeField] private TextMeshProUGUI titleText = default;
		[SerializeField] private Image shopIcon = default;
		[SerializeField] private TextMeshProUGUI actionText = default;
		[SerializeField] private TextMeshProUGUI evaluationText = default;
		[SerializeField] private TextMeshProUGUI playerCurrencyDisplay = default;
		[SerializeField] private TextMeshProUGUI worthEvaluationDisplay = default;

		[Space(5)]
		[Header("Canvases")]
		[SerializeField] private GameObject modeChooseCanvas = default;
		[SerializeField] private GameObject shopCanvas = default;
		[SerializeField] private GameObject amountCanvas = default;

		[Space(5)]
		[Header("Actions")]
		[SerializeField] private ShopModeAction[] shopModeActions = default;
		[SerializeField] private ShopItemAction[] shopItemActions = default;

		[Space(5)]
		[Header("Settings")]
		[SerializeField] private ShopModeInfo buyModeInfo = new ShopModeInfo() { title = "Generate Items", actionName = "Generate", evaluationTitle = "Cost" };
		[SerializeField] private ShopModeInfo sellModeInfo = new ShopModeInfo() { title = "Dismantle Items", actionName = "Dismantle", evaluationTitle = "Worth" };
		[SerializeField] private InventorySlot playerInventorySlotPrefab = default;
		[SerializeField] private ShopSlot shopSlotPrefab = default;

		//Private
		private float navigationCooldown;
		private ShopInventory curInventory;
		private ShopSlot[] shopSlots;
		private InventorySlot[] playerInventorySlots;
		private int[] selectedIndex = new int[4];
		private bool isSetup;
		private ShopMode shopMode = ShopMode.None;
		private ShopState shopState = ShopState.ModeSelection;
		public void BuildShop(ShopInventory inventory)
		{
			curInventory = inventory;

			//If the shop / player invetory is not build yet then we do it now
			if (!isSetup)
			{
				SetupShop();
			}

			//Upate the shop inventory slots
			for(int i = 0; i < shopSlots.Length; i++)
			{
				if (i >= inventory.shopItems.Length)
				{
					shopSlots[i].Setup(null);
					continue;
				}

				if (inventory.shopItems[i].maxPurchases == -1)
					shopSlots[i].Setup(inventory.shopItems[i].item);
				else
					shopSlots[i].Setup(inventory.shopItems[i].item, inventory.shopItems[i].maxPurchases);
			}

			//Set to the base state and then activate the UI
			ResetShop();
			GameController.GameIsPaused = true;
			gameObject.SetActive(true);
		}
		private void Update()
		{
			if (navigationCooldown > 0)
				navigationCooldown -= Time.unscaledDeltaTime;

			switch(shopState)
			{
				case ShopState.ModeSelection:
					ModeChooseNavigation();
					break;
				case ShopState.ItemSelection:
					ItemSelectionNavigation();
					break;
				case ShopState.SlotAction:
					SlotActionNavigation();
					break;
				case ShopState.ActionAmount:
					ActionAmountNavigation();
					break;
			}

			if(InputController.MenuBack.Down && navigationCooldown <= 0)
			{
				switch (shopState)
				{
					case ShopState.ModeSelection:
						CloseShop();
						break;
					case ShopState.ItemSelection:
						SetShopState(ShopState.ModeSelection);
						break;
					case ShopState.SlotAction:
						SetShopState(ShopState.ItemSelection);
						break;
					case ShopState.ActionAmount:
						SetShopState(ShopState.SlotAction);
						break;
				}
			}
		}
		private void ModeChooseNavigation()
		{
			int selfID = (int)ShopState.ModeSelection - 1;

			//Navigation
			int indexChange = InputController.MenuDown.Pressed ? 1 : (InputController.MenuUp.Pressed ? -1 : 0);
			if(indexChange != 0 && navigationCooldown <= 0)
			{
				selectedIndex[selfID] += indexChange;
				PlayFeedback(true);
				navigationCooldown = NAV_COOLDOWN;

				if (selectedIndex[selfID] >= shopModeActions.Length)
					selectedIndex[selfID] -= shopModeActions.Length;
				if (selectedIndex[selfID] < 0)
					selectedIndex[selfID] += shopModeActions.Length;

				shopModeActions[selectedIndex[selfID]].Select();
				selectedIndex[selfID] = selectedIndex[selfID];
			}

			//Choosing option
			if (InputController.MenuEnter.Down && navigationCooldown <= 0)
			{
				switch (shopModeActions[selectedIndex[selfID]].mode)
				{
					case ShopMode.None:
						CloseShop();
						break;
					case ShopMode.Buy:
						SetShopMode(ShopMode.Buy);
						SetShopState(ShopState.ItemSelection);
						break;
					case ShopMode.Sell:
						SetShopMode(ShopMode.Sell);
						SetShopState(ShopState.ItemSelection);
						break;
				}
				PlayFeedback(true);
				navigationCooldown = NAV_COOLDOWN;
			}
		}
		private void ItemSelectionNavigation()
		{
			int selfID = (int)ShopState.ItemSelection - 1;

			if (navigationCooldown <= 0)
			{
				int leftRightChange = InputController.MenuRight.Pressed ? 1 : (InputController.MenuLeft.Pressed ? -1 : 0);
				int upDownChange = InputController.MenuDown.Pressed ? 1 : (InputController.MenuUp.Pressed ? -1 : 0);
				if (leftRightChange != 0 || upDownChange != 0)
				{
					if(leftRightChange != 0)
					{
						selectedIndex[selfID] += leftRightChange;
					}
					else
					{
						selectedIndex[selfID] += upDownChange * SlotsPerRow(shopMode == ShopMode.Buy ? buyModeInfo.itemGrid : sellModeInfo.itemGrid);
					}
					UpdateSelectedSlot();
				}
				else if (InputController.MenuEnter.Down)
				{

				}
			}

			void UpdateSelectedSlot()
			{
				int maxIndex = shopMode == ShopMode.Buy ? shopSlots.Length : playerInventorySlots.Length;
				if (selectedIndex[selfID] >= maxIndex)
					selectedIndex[selfID] -= maxIndex;
				if (selectedIndex[selfID] < 0)
					selectedIndex[selfID] += maxIndex;

				if (shopMode == ShopMode.Buy)
					shopSlots[selectedIndex[selfID]].Select();
				else
					playerInventorySlots[selectedIndex[selfID]].SelectMenuItem();

				PlayFeedback(true);
				navigationCooldown = NAV_COOLDOWN;
			}
			int SlotsPerRow(GridLayoutGroup slotGrid)
			{
				float slotWidht = slotGrid.cellSize.x + slotGrid.spacing.x;
				float gridWidht = (slotGrid.transform as RectTransform).rect.width - slotGrid.padding.horizontal + slotGrid.spacing.x;

				return Mathf.FloorToInt(gridWidht / slotWidht);
			}
		}
		private void SlotActionNavigation()
		{
			int selfID = (int)ShopState.SlotAction - 1;
		}
		private void ActionAmountNavigation()
		{
			int selfID = (int)ShopState.ActionAmount - 1;
		}
		private void ResetShop()
		{
			ResetIndexes();
			SetShopState(ShopState.ModeSelection);
			SetShopMode(ShopMode.None);
		}
		private void SetupShop()
		{
			isSetup = true;

			//Shop inventory
			shopSlots = new ShopSlot[Player.Instance.Inventory.Length];
			for (int i = 0; i < shopSlots.Length; i++)
			{
				shopSlots[i] = Instantiate(shopSlotPrefab, buyModeInfo.itemGrid.transform);
			}

			//Player inventory
			playerInventorySlots = new InventorySlot[Player.Instance.Inventory.Length];
			for (int i = 0; i < playerInventorySlots.Length; i++)
			{
				InventorySlot newSlot = Instantiate(playerInventorySlotPrefab, sellModeInfo.itemGrid.transform);
				newSlot.Setup(Player.Instance.Inventory, i);
				playerInventorySlots[i] = newSlot;
			}

			//Update subscriptions
			Player.Instance.Inventory.InventoryChanged += UpdateInventorySlots;
			EventManager.PlayerCurrencyChangedEvent += UpdateCurrencyDisplay;
			UpdateInventorySlots();
		}
		private void SetShopMode(ShopMode newShopMode)
		{
			if (shopMode != ShopMode.None)
			{
				(shopMode == ShopMode.Buy ? buyModeInfo : sellModeInfo).itemGrid.gameObject.SetActive(false);
			}

			shopMode = newShopMode;
			if(shopMode != ShopMode.None)
			{
				ShopModeInfo targetInfo = shopMode == ShopMode.Buy ? buyModeInfo : sellModeInfo;

				titleText.text = targetInfo.title;
				shopIcon.sprite = targetInfo.icon;
				actionText.text = targetInfo.actionName;
				evaluationText.text = targetInfo.evaluationTitle + ":";
				targetInfo.itemGrid.gameObject.SetActive(true);
			}
		}
		private void SetShopState(ShopState newShopState)
		{
			shopState = newShopState;

			modeChooseCanvas.SetActive(shopState == ShopState.ModeSelection);
			shopCanvas.SetActive(shopState != ShopState.ModeSelection);
			amountCanvas.SetActive(shopState == ShopState.ActionAmount);
		}
		private void UpdateInventorySlots()
		{
			for (int i = 0; i < playerInventorySlots.Length; i++)
			{
				playerInventorySlots[i].UpdateDisplay();
			}
		}
		private void UpdateCurrencyDisplay()
		{
			playerCurrencyDisplay.text = Player.Instance.CurrentCurrencyAmount.ToString();
		}
		private void PlayFeedback(bool succesSound)
		{
			Sounds.SoundManager.SetSoundState(succesSound ? ConstantCollector.MENU_NAV_SOUND : ConstantCollector.FAILED_MENU_NAV_SOUND, true);
		}
		private void CloseShop()
		{
			GameController.GameIsPaused = false;
			gameObject.SetActive(false);
		}
		private void ResetIndexes()
		{
			for(int i = 0; i < selectedIndex.Length; i++)
			{
				selectedIndex[i] = 0;
			}
		}
		private void OnDestroy()
		{
			Player.Instance.Inventory.InventoryChanged -= UpdateInventorySlots;
			EventManager.PlayerCurrencyChangedEvent -= UpdateCurrencyDisplay;
		}
		[System.Serializable]
		private class ShopModeInfo
		{
			public string title;
			public string actionName;
			public string evaluationTitle;
			public Sprite icon;
			public GridLayoutGroup itemGrid;
		}
	}
}