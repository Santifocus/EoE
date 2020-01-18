using EoE.Entities;
using EoE.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class SoulDisplay : MonoBehaviour
	{
		[SerializeField] private float showTime = default;
		[SerializeField] private float updateExperienceAmountSpeed = 5;

		[SerializeField] private Image soulIcon = default;
		[SerializeField] private float soulIconColorTransitionSpeed = 2;
		[SerializeField] private Color soulIconActiveColor = Color.white;
		[SerializeField] private Color soulIconInActiveColor = Color.white / 3;

		[SerializeField] private TransitionUI textTransition = default;
		[SerializeField] private TextMeshProUGUI textAmount = default;

		private float delayTillHide;
		private float iconTransitionPoint;
		private float shownExperianceAmount;
		private int experienceToLevelup;

		private void Start()
		{
			EventManager.PlayerExperienceChangedEvent += PlayerSoulCountChanged;
			experienceToLevelup = Player.Instance.RequiredExperienceForLevel;
			shownExperianceAmount = Player.Instance.TotalExperience;
			textAmount.text = Mathf.CeilToInt(shownExperianceAmount) + " / " + experienceToLevelup;
		}
		private void PlayerSoulCountChanged()
		{
			delayTillHide = showTime;
			textTransition.ChangeTransitionState(true);
		}
		private void Update()
		{
			if (delayTillHide > 0)
			{
				delayTillHide -= Time.unscaledDeltaTime;
				if (delayTillHide <= 0)
				{
					textTransition.ChangeTransitionState(false);
				}

				if (textTransition.trueState)
				{
					shownExperianceAmount = Mathf.Lerp(shownExperianceAmount, Player.Instance.TotalExperience, Time.unscaledDeltaTime * updateExperienceAmountSpeed);
					if (Mathf.CeilToInt(shownExperianceAmount) >= experienceToLevelup)
					{
						experienceToLevelup = Player.Instance.RequiredExperienceForLevel;
					}
					textAmount.text = Mathf.CeilToInt(shownExperianceAmount) + " / " + experienceToLevelup;
				}
			}
			else if (!textTransition.trueState)
			{
				experienceToLevelup = Player.Instance.RequiredExperienceForLevel;
				shownExperianceAmount = Player.Instance.TotalExperience;
				textAmount.text = Mathf.CeilToInt(shownExperianceAmount) + " / " + experienceToLevelup;
			}
			UpdateIconTransition();
		}
		private void UpdateIconTransition()
		{
			if (delayTillHide > 0)
			{
				if (iconTransitionPoint < 1)
				{
					iconTransitionPoint += Time.unscaledDeltaTime * soulIconColorTransitionSpeed;
					if (iconTransitionPoint > 1)
						iconTransitionPoint = 1;
				}
			}
			else
			{
				if (iconTransitionPoint > 0)
				{
					iconTransitionPoint -= Time.unscaledDeltaTime * soulIconColorTransitionSpeed;
					if (iconTransitionPoint < 0)
						iconTransitionPoint = 0;
				}
			}

			soulIcon.color = Color.Lerp(soulIconInActiveColor, soulIconActiveColor, iconTransitionPoint);
		}
		private void OnDestroy()
		{
			EventManager.PlayerExperienceChangedEvent -= PlayerSoulCountChanged;
		}
	}
}