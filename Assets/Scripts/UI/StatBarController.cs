using EoE.Entities;
using TMPro;
using UnityEngine;

namespace EoE.UI
{
	public class StatBarController : MonoBehaviour
	{
		private enum DisplayedStat { Health, Mana }
		[SerializeField] private DisplayedStat displayedStat = default;
		[SerializeField] private TextMeshProUGUI statText = default;
		private int displayedStatAmount;
		private int displayedMaxStatAmount;
		private float nextStatPointUpdate;

		private void Update()
		{
			if (!Player.Alive)
			{
				if (displayedStatAmount != 0)
				{
					displayedStatAmount = 0;
					UpdateStatText();
				}
				return;
			}

			int playerCurrentStatAmount = displayedStat == DisplayedStat.Health ? (int)Player.Instance.curHealth : (int)Player.Instance.curMana;
			int playerMaxStatAmount = displayedStat == DisplayedStat.Health ? (int)Player.Instance.curMaxHealth : (int)Player.Instance.curMaxMana;

			if (displayedStatAmount != playerCurrentStatAmount)
			{
				int dif = displayedStatAmount - playerCurrentStatAmount;
				float updateSpeed = Player.PlayerSettings.StatTextUpdateSpeed;

				nextStatPointUpdate += Time.deltaTime * System.Math.Abs(dif);
				if (nextStatPointUpdate > updateSpeed)
				{
					int difDir = -System.Math.Sign(dif);
					int change = (int)(nextStatPointUpdate / updateSpeed);
					nextStatPointUpdate -= change * updateSpeed;

					displayedStatAmount += change * difDir;
					UpdateStatText();
				}
			}
			else if (playerMaxStatAmount != displayedMaxStatAmount)
			{
				displayedMaxStatAmount = playerMaxStatAmount;
				UpdateStatText();
			}
		}

		private void UpdateStatText()
		{
			string totalStatPoints = displayedStat == DisplayedStat.Health ? ((int)Player.Instance.curMaxHealth).ToString() : ((int)Player.Instance.curMaxMana).ToString();
			string curDisplayedStatPoints = displayedStatAmount.ToString();
			int missingNumbers = totalStatPoints.Length - curDisplayedStatPoints.Length;
			for (int i = 0; i < missingNumbers; i++)
				curDisplayedStatPoints = "0" + curDisplayedStatPoints;

			statText.text = curDisplayedStatPoints + " / " + totalStatPoints;
			statText.color = displayedStat == DisplayedStat.Health ? Player.PlayerSettings.HealthTextColors.Evaluate((float)displayedStatAmount / Player.Instance.curMaxHealth) : Player.PlayerSettings.ManaTextColors.Evaluate((float)displayedStatAmount / Player.Instance.curMaxMana);
		}
	}
}