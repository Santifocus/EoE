using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace EoE.UI
{
	public class PlayerStatDisplayController : MonoBehaviour
	{
		[SerializeField] private TargetStat targetStat = TargetStat.Health;
		[SerializeField] private Image statFill = default;
		[SerializeField] private TextMeshProUGUI statAmount = default;
		private Player player => Player.Instance;
		private float curValue
		{
			get
			{
				switch (targetStat)
				{
					case TargetStat.Health:
						return player.curHealth;
					case TargetStat.Mana:
						return player.curMana;
					case TargetStat.Endurance:
						return player.curEndurance;
					default:
						return 1;
				}
			}
		}
		private float maxValue
		{
			get
			{
				switch (targetStat)
				{
					case TargetStat.Health:
						return player.curMaxHealth;
					case TargetStat.Mana:
						return player.curMaxMana;
					case TargetStat.Endurance:
						return player.trueEnduranceAmount;
					default:
						return 1;
				}
			}
		}

		private void Update()
		{
			statFill.fillAmount = curValue / maxValue;
			if (statAmount)
			{
				statAmount.text = Mathf.Ceil(curValue).ToString();
			}
		}
	}
}