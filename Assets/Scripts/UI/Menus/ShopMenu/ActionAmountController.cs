using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EoE.Controlls;

namespace EoE.UI
{
	public enum AmountAction { None, Back, Executed }
	public class ActionAmountController : MonoBehaviour
	{
		private const float NAV_COOLDOWN = 0.2f;
		[Header("Amount Selection")]
		[SerializeField] private TextMeshProUGUI amountDisplay = default;
		[SerializeField] private TextMeshProUGUI amountWorthDisplay = default;
		[SerializeField] private GameObject amountArrowOn = default;
		[SerializeField] private GameObject amountArrowOff = default;

		[Header("Action Selection")]
		[SerializeField] private TextMeshProUGUI executeActionOn = default;
		[SerializeField] private TextMeshProUGUI executeActionOff = default;
		[SerializeField] private GameObject cancelActionOn = default;
		[SerializeField] private GameObject cancelActionOff = default;

		public string ExecutionText
		{
			get => executeActionOn.text;
			set
			{
				executeActionOn.text = value;
				executeActionOff.text = value;
			}
		}

		private int curActionAmount;
		private int curActionWorth;
		private int curMaxActionAmount;

		private float navigationCooldown;
		private bool choiceSelected;
		private bool executeActionSelected;

		public AmountAction NavigationUpdate()
		{
			if(navigationCooldown > 0)
			{
				navigationCooldown -= Time.unscaledDeltaTime;
				return AmountAction.None;
			}

			if (InputController.MenuBack.Down)
			{
				PlayFeedback(true);
				return AmountAction.Back;
			}

			if (InputController.MenuDown.Pressed || InputController.MenuUp.Pressed)
			{
				SelectChoice(!choiceSelected);
				PlayFeedback(true);
				navigationCooldown = NAV_COOLDOWN;
			}
			else if (InputController.MenuEnter.Down)
			{
				if (choiceSelected)
				{
					return executeActionSelected ? AmountAction.Executed : AmountAction.Back;
				}
				else
					SelectChoice(true);

				PlayFeedback(true);
				navigationCooldown = NAV_COOLDOWN;
			}
			else
			{
				int rightLeftChange = InputController.MenuRight.Pressed ? 1 : (InputController.MenuLeft.Pressed ? -1 : 0);
				if(rightLeftChange != 0)
				{
					if (choiceSelected)
					{
						SelectExecuteAction(!executeActionSelected);
						PlayFeedback(true);
						navigationCooldown = NAV_COOLDOWN;
					}
					else
					{
						bool maxMorethenOne = curMaxActionAmount > 1;
						if (maxMorethenOne)
						{
							SetActionAmount(curActionAmount + rightLeftChange);
						}
						PlayFeedback(maxMorethenOne);
						navigationCooldown = NAV_COOLDOWN;
					}
				}
			}

			return AmountAction.None;
		}
		public (int, int) GetChange()
		{
			return (curActionAmount, curActionAmount * curActionWorth);
		}
		public void SetupController(int actionWorth, int maxAction)
		{
			SelectChoice(false);
			SetActionAmount(1);
			SetActionWorth(actionWorth);
			curMaxActionAmount = maxAction;
			navigationCooldown = NAV_COOLDOWN;
		}
		private void SelectChoice(bool state)
		{
			amountArrowOn.SetActive(!state);
			amountArrowOff.SetActive(state);
			if (state)
			{
				if (choiceSelected != state)
				{
					SelectExecuteAction(false);
				}
			}
			else
			{
				executeActionOn.gameObject.SetActive(false);
				executeActionOff.gameObject.SetActive(true);
				cancelActionOn.SetActive(false);
				cancelActionOff.SetActive(true);
			}

			choiceSelected = state;
		}
		private void SelectExecuteAction(bool state)
		{
			executeActionSelected = state;

			executeActionOn.gameObject.SetActive(state);
			executeActionOff.gameObject.SetActive(!state);
			cancelActionOn.SetActive(!state);
			cancelActionOff.SetActive(state);
		}
		public void SetActionAmount(int amount)
		{
			curActionAmount = amount;

			if (curActionAmount < 1)
				curActionAmount += curMaxActionAmount;
			if (curActionAmount > curMaxActionAmount)
				curActionAmount = 1;

			amountDisplay.text = curActionAmount.ToString();
			amountWorthDisplay.text = (curActionAmount * curActionWorth).ToString();
		}
		public void SetActionWorth(int worth)
		{
			curActionWorth = worth;
			amountWorthDisplay.text = (curActionAmount * curActionWorth).ToString();
		}
		private void PlayFeedback(bool succesSound)
		{
			Sounds.SoundManager.SetSoundState(succesSound ? ConstantCollector.MENU_NAV_SOUND : ConstantCollector.FAILED_MENU_NAV_SOUND, true);
		}
	}
}