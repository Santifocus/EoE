using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class EnduranceBarController : MonoBehaviour
	{
		[SerializeField] private Image mainBarFill = default;
		[SerializeField] private GridLayoutGroup smallBarsGrid = default;
		[SerializeField] private Image smallBarPrefab = default;
		private Image[] smallBars;

		private Player player => Player.Instance;

		private void Start()
		{
			smallBars = new Image[player.totalEnduranceContainers];
			for(int i = 0; i < smallBars.Length; i++)
			{
				smallBars[i] = Instantiate(smallBarPrefab, smallBarsGrid.transform).transform.GetChild(1).GetComponent<Image>();
			}
		}

		private void Update()
		{
			int t = player.activeEnduranceContainerIndex;
			mainBarFill.fillAmount = Mathf.Lerp(mainBarFill.fillAmount, player.enduranceContainers[t] / Player.PlayerSettings.EndurancePerBar, Player.PlayerSettings.EnduranceBarLerpSpeed * Time.deltaTime);

			for (int i = 0; i < smallBars.Length; i++)
			{
				if (i < t)
				{
					if(smallBars[i].fillAmount != 1 || smallBars[i].color != Player.PlayerSettings.ReserveEnduranceBarColor)
					{
						smallBars[i].fillAmount = 1;
						smallBars[i].color = Player.PlayerSettings.ReserveEnduranceBarColor;
					}
				}
				else if (i == t)
				{
					smallBars[i].fillAmount = Mathf.Lerp(smallBars[i].fillAmount, player.enduranceContainers[t] / Player.PlayerSettings.EndurancePerBar, Player.PlayerSettings.EnduranceBarLerpSpeed * Time.deltaTime);
					if (smallBars[i].color != Player.PlayerSettings.ActiveEnduranceBarColor)
					{
						smallBars[i].color = Player.PlayerSettings.ActiveEnduranceBarColor;
					}
				}
				else if (i == t + 1)
				{
					smallBars[i].fillAmount = Mathf.Lerp(smallBars[i].fillAmount, player.lockedEndurance / Player.PlayerSettings.EndurancePerBar, Player.PlayerSettings.EnduranceBarLerpSpeed * Time.deltaTime);
					if (smallBars[i].color != Player.PlayerSettings.ReloadingEnduranceBarColor)
					{
						smallBars[i].color = Player.PlayerSettings.ReloadingEnduranceBarColor;
					}
				}
				else
					smallBars[i].fillAmount = 0;
			}
		}
	}
}