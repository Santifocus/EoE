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
		private const int MAX_POSSIBLE_ACTION_AMOUNT = 1000;
		public enum ShopMode { None = 1, Buy = 2, Sell = 3}
		private enum ShopState { ModeSelection = 1, ItemSelection = 2, ActionSelection = 3, ActionAmount = 4 }

		//Constants
		//Inpsector variables
		[SerializeField] private TextMeshProUGUI titleText = default;
		[SerializeField] private Image shopIcon = default;
		[SerializeField] private ShopItemAction actionText = default;
		[SerializeField] private TextMeshProUGUI evaluationText = default;
		[SerializeField] private TextMeshProUGUI playerCurrencyDisplay = default;
		[SerializeField] private TextMeshProUGUI worthEvaluationDisplay = default;
		[SerializeField] private TextMeshProUGUI itemDescriptionDisplay = default;

		[Space(5)]
		[Header("Canvases")]
		[SerializeField] private GameObject modeChooseCanvas = default;
		[SerializeField] private GameObject shopCanvas = default;
		[SerializeField] private GameObject amountCanvas = default;

		[Space(5)]
		[Header("Actions")]
		[SerializeField] private ShopModeAction[] shopModeActions = default;
		[SerializeField] private ShopItemAction[] shopItemActions = default;
		[SerializeField] private ActionAmountController amountController = default;

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
		private int[] selectedIndex = new int[3];
		private bool isSetup;
		private ShopMode shopMode = ShopMode.None;
		private ShopState shopState = 0;

		private void Start()
		{
			(transform as RectTransform).anchoredPosition = Vector2.zero;
			gameObject.SetActive(false);
			EventManager.PlayerCurrencyChangedEvent += UpdateCurrencyDisplay;
		}
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
				if (i >= inventory.ShopItems.Length)
				{
					shopSlots[i].Setup(null);
					continue;
				}

				if (inventory.ShopItems[i].InfinitePurchases)
					shopSlots[i].Setup(inventory.ShopItems[i].Item);
				else
					shopSlots[i].Setup(inventory.ShopItems[i].Item, inventory.ShopItems[i].MaxPurchases);
			}

			//Set to the base state and then activate the UI
			ResetShop();
			GameController.ActivePauses++;
			gameObject.SetActive(true);
		}
		private void Update()
		{
			if(InputController.PlayerMenu.Down && navigationCooldown <= 0)
			{
				CloseShop();
				return;
			}

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
				case ShopState.ActionSelection:
					ActionSelectionNavigation();
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
					case ShopState.ActionSelection:
						SetShopState(ShopState.ItemSelection);
						break;
					case ShopState.ActionAmount:
						SetShopState(ShopState.ItemSelection);
						shopItemActions[selectedIndex[(int)ShopState.ActionSelection - 1]].DeSelect();
						break;
				}
				PlayFeedback(true);
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
				return;
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
				selectedIndex[(int)ShopState.ItemSelection - 1] = 0;
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
					if (leftRightChange != 0)
					{
						selectedIndex[selfID] += leftRightChange;
					}
					else
					{
						selectedIndex[selfID] += upDownChange * SlotsPerRow(shopMode == ShopMode.Buy ? buyModeInfo.itemGrid : sellModeInfo.itemGrid);
					}
					UpdateSelectedItemSlot();
				}
				else if (InputController.MenuEnter.Down)
				{
					int targetIndex = selectedIndex[selfID];

					bool allowedToOpen;
					if (shopMode == ShopMode.Buy)
						allowedToOpen = curInventory.ShopItems.Length > targetIndex && shopSlots[targetIndex].containedItem != null;
					else
						allowedToOpen = Player.Instance.Inventory[targetIndex] != null && Player.Instance.Inventory[targetIndex].data.ItemFlags != ItemSpecialFlag.NonRemoveable;

					if (allowedToOpen)
					{
						SetShopState(ShopState.ActionSelection);
						selectedIndex[(int)ShopState.ActionSelection - 1] = 0;
						shopItemActions[0].Select();
					}

					PlayFeedback(allowedToOpen);
					navigationCooldown = NAV_COOLDOWN;
				}
			}


			int SlotsPerRow(GridLayoutGroup slotGrid)
			{
				float slotWidht = slotGrid.cellSize.x + slotGrid.spacing.x;
				float gridWidht = (slotGrid.transform as RectTransform).rect.width - slotGrid.padding.horizontal + slotGrid.spacing.x;

				return Mathf.FloorToInt(gridWidht / slotWidht);
			}
		}
		private void UpdateSelectedItemSlot()
		{
			int itemSelectionID = (int)ShopState.ItemSelection - 1;
			int maxIndex = shopMode == ShopMode.Buy ? shopSlots.Length : playerInventorySlots.Length;
			if (selectedIndex[itemSelectionID] >= maxIndex)
				selectedIndex[itemSelectionID] -= maxIndex;
			if (selectedIndex[itemSelectionID] < 0)
				selectedIndex[itemSelectionID] += maxIndex;

			if (shopMode == ShopMode.Buy)
			{
				int newIndex = selectedIndex[itemSelectionID];
				shopSlots[newIndex].Select();
				if(shopSlots[newIndex].containedItem != null)
				{
					worthEvaluationDisplay.text = shopSlots[newIndex].containedItem.ItemWorth.ToString();
					itemDescriptionDisplay.text = ColoredText.ToString(shopSlots[newIndex].containedItem.ItemDescription);
				}
				else
				{
					worthEvaluationDisplay.text = "0";
					itemDescriptionDisplay.text = "";
				}
			}
			else
			{
				int newIndex = selectedIndex[itemSelectionID];
				playerInventorySlots[newIndex].Select();
				if(Player.Instance.Inventory[newIndex] != null)
				{
					worthEvaluationDisplay.text = (Player.Instance.Inventory[newIndex].data.ItemWorth * GameController.CurrentGameSettings.ItemSellMultiplier).ToString();
					itemDescriptionDisplay.text = ColoredText.ToString(Player.Instance.Inventory[newIndex].data.ItemDescription);
				}
				else
				{
					worthEvaluationDisplay.text = "0";
					itemDescriptionDisplay.text = "";
				}
			}

			PlayFeedback(true);
			navigationCooldown = NAV_COOLDOWN;
		}
		private void ActionSelectionNavigation()
		{
			int selfID = (int)ShopState.ActionSelection - 1;

			//Navigation
			int indexChange = InputController.MenuDown.Pressed ? 1 : (InputController.MenuUp.Pressed ? -1 : 0);
			if (indexChange != 0 && navigationCooldown <= 0)
			{
				selectedIndex[selfID] += indexChange;
				PlayFeedback(true);
				navigationCooldown = NAV_COOLDOWN;

				int maxIndex = shopItemActions.Length;

				if (selectedIndex[selfID] >= maxIndex)
					selectedIndex[selfID] -= maxIndex;
				if (selectedIndex[selfID] < 0)
					selectedIndex[selfID] += maxIndex;

				shopItemActions[selectedIndex[selfID]].Select();
			}

			//Choosing option
			if (InputController.MenuEnter.Down && navigationCooldown <= 0)
			{
				if (shopItemActions[selectedIndex[selfID]].isDoAction)
				{
					if (BuildAmountController())
					{
						SetShopState(ShopState.ActionAmount);
						PlayFeedback(true);
					}
					else
					{
						PlayFeedback(false);
					}
				}
				else 
				{
					SetShopState(ShopState.ItemSelection);
					shopItemActions[selectedIndex[selfID]].DeSelect();
					PlayFeedback(true);
				}

				navigationCooldown = NAV_COOLDOWN;
			}
		}
		private bool BuildAmountController()
		{
			int actionWorth;
			int maxAction;
			int selectedItemIndex = selectedIndex[(int)ShopState.ItemSelection - 1];

			//Get the action worth, based on the item that is currently selected (If the player is selling then the item worth will be mutliplied by a set value)
			//And
			//Find out what the possible max action amount is
			//Buying => As much as the player can pay for; The amount of space he has in his inventory; How much the shop offers; Max action amount
			//Selling => How much the player has; Max action amount
			if (shopMode == ShopMode.Buy)
			{
				actionWorth = curInventory.ShopItems[selectedItemIndex].Item.ItemWorth;

				int maxBuys = actionWorth == 0 ? MAX_POSSIBLE_ACTION_AMOUNT : (Player.Instance.CurrentCurrencyAmount / actionWorth);
				maxBuys = maxBuys - Player.Instance.Inventory.CheckAddablity(curInventory.ShopItems[selectedItemIndex].Item, maxBuys);
				maxAction = Mathf.Min(maxBuys, shopSlots[selectedItemIndex].Stacksize, MAX_POSSIBLE_ACTION_AMOUNT);
			}
			else
			{
				actionWorth = Mathf.RoundToInt(Player.Instance.Inventory[selectedItemIndex].data.ItemWorth * GameController.CurrentGameSettings.ItemSellMultiplier);
				maxAction = Mathf.Min(Player.Instance.Inventory[selectedItemIndex].stackSize, MAX_POSSIBLE_ACTION_AMOUNT);
			}

			if (maxAction <= 0)
				return false;

			amountController.SetupController(actionWorth, maxAction);
			return true;
		}
		private void ActionAmountNavigation()
		{
			AmountAction action = amountController.NavigationUpdate();
			if(action == AmountAction.Executed)
			{
				(int actionAmount, int actionWorth) = amountController.GetChange();

				int selectedSlotIndex = selectedIndex[(int)ShopState.ItemSelection - 1];
				if (shopMode == ShopMode.Buy)
				{
					Player.Instance.ChangeCurrency(-actionWorth);
					curInventory.ShopItems[selectedSlotIndex].MaxPurchases -= actionAmount;
					shopSlots[selectedSlotIndex].Stacksize -= actionAmount;
					Player.Instance.Inventory.AddItem(new InventoryItem(curInventory.ShopItems[selectedSlotIndex].Item, actionAmount));
				}
				else
				{
					Player.Instance.ChangeCurrency(actionWorth);
					Player.Instance.Inventory.RemoveStackSize(selectedSlotIndex, actionAmount);
				}

				SetShopState(ShopState.ItemSelection);
				UpdateSelectedItemSlot();
				shopItemActions[selectedIndex[(int)ShopState.ActionSelection - 1]].DeSelect();
			}
		}
		private void ResetShop()
		{
			ResetIndexes();
			SetShopMode(ShopMode.None);
			SetShopState(ShopState.ModeSelection);
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
				actionText.displayText = targetInfo.actionName;
				amountController.ExecutionText = targetInfo.actionName;
				evaluationText.text = targetInfo.evaluationTitle + ":";
				targetInfo.itemGrid.gameObject.SetActive(true);
			}
		}
		private void SetShopState(ShopState newShopState)
		{
			if((int)shopState < (int)newShopState)
			{
				switch (newShopState)
				{
					case ShopState.ModeSelection:
						selectedIndex[(int)ShopState.ModeSelection - 1] = 0;
						shopModeActions[0].Select();
						break;
					case ShopState.ItemSelection:
						selectedIndex[(int)ShopState.ItemSelection - 1] = 0;
						UpdateSelectedItemSlot();
						break;
					case ShopState.ActionSelection:
						selectedIndex[(int)ShopState.ActionSelection - 1] = 0;
						shopItemActions[0].Select();
						break;
				}
			}
			else if((int)shopState > (int)newShopState)
			{
				if(shopState == ShopState.ActionSelection)
				{
					shopItemActions[selectedIndex[(int)ShopState.ActionSelection - 1]].DeSelect();
				}
			}
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
			GameController.ActivePauses--;
			gameObject.SetActive(false);
		}
		private void ResetIndexes()
		{
			shopState = 0;
			for (int i = 0; i < selectedIndex.Length; i++)
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

			public ShopModeInfo() { }
			public ShopModeInfo(string title, string actionName, string evaluationTitle, Sprite icon, GridLayoutGroup itemGrid)
			{
				this.title = title;
				this.actionName = actionName;
				this.evaluationTitle = evaluationTitle;
				this.icon = icon;
				this.itemGrid = itemGrid;
			}
		}
	}
}