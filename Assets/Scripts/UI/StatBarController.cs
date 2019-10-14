using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EoE.Entities;

namespace EoE.UI
{
	public class StatBarController : MonoBehaviour
	{
		private enum DisplayedStat { Health, Mana }
		[SerializeField] private DisplayedStat displayedStat = default;
		[SerializeField] private TextMeshProUGUI statText = default;
		private int displayedStatAmount;
		private float nextStatPointUpdate;
		private bool playerDead;
		private void Start()
		{
			Events.EventManager.PlayerDiedEvent += PlayerDied;
		}

		private void PlayerDied(Entitie killer)
		{
			playerDead = true;
		}

		private void OnDestroy()
		{
			Events.EventManager.PlayerDiedEvent -= PlayerDied;
		}

		private void Update()
		{
			if (playerDead)
				return;

			int playerCurrentStatAmount = displayedStat == DisplayedStat.Health ? (int)Player.Instance.CurHealth : (int)Player.Instance.CurMana;

			if (displayedStatAmount != playerCurrentStatAmount)
			{
				int dif = displayedStatAmount - playerCurrentStatAmount;
				float updateSpeed = Player.PlayerSettings.StatTextUpdateSpeed;

				nextStatPointUpdate += Time.deltaTime * System.Math.Abs(dif);
				if(nextStatPointUpdate > updateSpeed)
				{
					int difDir = -System.Math.Sign(dif);
					int change = (int)(nextStatPointUpdate / updateSpeed);
					nextStatPointUpdate -= change * updateSpeed;

					displayedStatAmount += change * difDir;
					UpdateStatText();
				}
			}
		}

		private void UpdateStatText()
		{
			string totalStatPoints = displayedStat == DisplayedStat.Health ? ((int)Player.PlayerSettings.Health).ToString() : ((int)Player.PlayerSettings.Mana).ToString();
			string curDisplayedStatPoints = displayedStatAmount.ToString();
			int missingNumbers = totalStatPoints.Length - curDisplayedStatPoints.Length;
			for (int i = 0; i < missingNumbers; i++)
				curDisplayedStatPoints = "0" + curDisplayedStatPoints;

			statText.text = curDisplayedStatPoints + " / " + totalStatPoints;
			statText.color = displayedStat == DisplayedStat.Health ? Player.PlayerSettings.HealthTextColors.Evaluate((float)displayedStatAmount / Player.PlayerSettings.Health) : Player.PlayerSettings.ManaTextColors.Evaluate((float)displayedStatAmount / Player.PlayerSettings.Mana);
		}
	}
}