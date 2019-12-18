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
		[SerializeField] private float updateSoulCountSpeed = 5;

		[SerializeField] private Image soulIcon = default;
		[SerializeField] private float soulIconColorTransitionSpeed = 2;
		[SerializeField] private Color soulIconActiveColor = Color.white;
		[SerializeField] private Color soulIconInActiveColor = Color.white / 3;

		[SerializeField] private TransitionUI textTransition = default;
		[SerializeField] private TextMeshProUGUI textAmount = default;

		private float delayTillHide;
		private float iconTransitionPoint;
		private float shownSoulCount;
		private int soulsToLevelup;

		private void Start()
		{
			EventManager.PlayerSoulCountChangedEvent += PlayerSoulCountChanged;
			soulsToLevelup = Player.Instance.RequiredSoulsForLevel;
			shownSoulCount = Player.Instance.TotalSoulCount;
			textAmount.text = Mathf.CeilToInt(shownSoulCount) + " / " + soulsToLevelup;
		}
		private void OnDestroy()
		{
			EventManager.PlayerSoulCountChangedEvent -= PlayerSoulCountChanged;
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
					shownSoulCount = Mathf.Lerp(shownSoulCount, Player.Instance.TotalSoulCount, Time.unscaledDeltaTime * updateSoulCountSpeed);
					if (Mathf.CeilToInt(shownSoulCount) >= soulsToLevelup)
					{
						soulsToLevelup = Player.Instance.RequiredSoulsForLevel;
					}
					textAmount.text = Mathf.CeilToInt(shownSoulCount) + " / " + soulsToLevelup;
				}
			}
			else if (!textTransition.trueState)
			{
				soulsToLevelup = Player.Instance.RequiredSoulsForLevel;
				shownSoulCount = Player.Instance.TotalSoulCount;
				textAmount.text = Mathf.CeilToInt(shownSoulCount) + " / " + soulsToLevelup;
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
	}
}