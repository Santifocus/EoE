using EoE.Combatery;
using EoE.Behaviour.Entities;
using EoE.Information;
using TMPro;
using UnityEngine;
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
						return player.CurHealth;
					case TargetStat.Mana:
						return player.CurMana;
					case TargetStat.Stamina:
						return player.CurStamina;
					case TargetStat.UltimateCharge:
						return (WeaponController.Instance ? WeaponController.Instance.ultimateCharge : 0);
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
						return player.CurMaxHealth;
					case TargetStat.Mana:
						return player.CurMaxMana;
					case TargetStat.Stamina:
						return player.CurMaxStamina;
					case TargetStat.UltimateCharge:
						return ((WeaponController.Instance && WeaponController.Instance.weaponInfo.HasUltimate) ? WeaponController.Instance.weaponInfo.UltimateSettings.TotalRequiredCharge : 0);
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
				if(targetStat == TargetStat.Health)
					statAmount.text = Mathf.Ceil(curValue).ToString();
				else
					statAmount.text = Mathf.Floor(curValue).ToString();
			}
		}
	}
}