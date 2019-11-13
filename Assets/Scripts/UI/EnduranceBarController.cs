using EoE.Entities;
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
			SetupSmallBars();
		}

		private void SetupSmallBars()
		{
			if(smallBars != null)
			{
				for(int i = 0; i < smallBars.Length; i++)
				{
					Destroy(smallBars[i].transform.parent.gameObject);
				}
			}

			smallBars = new Image[player.totalEnduranceContainers];
			for (int i = 0; i < smallBars.Length; i++)
			{
				smallBars[i] = Instantiate(smallBarPrefab, smallBarsGrid.transform).transform.GetChild(1).GetComponent<Image>();
			}
		}

		private void Update()
		{
			if(smallBars.Length != player.totalEnduranceContainers)
			{
				//Rebuild the smallbar grid
				SetupSmallBars();
			}

			float enduranceToDistribute = player.curEndurance;
			for (int i = 0; i < smallBars.Length; i++)
			{
				enduranceToDistribute -= Player.PlayerSettings.EndurancePerBar;
				if (enduranceToDistribute > 0)
				{
					smallBars[i].fillAmount = Mathf.Lerp(smallBars[i].fillAmount, 1, Time.deltaTime * Player.PlayerSettings.EnduranceBarLerpSpeed);
					smallBars[i].color = Player.PlayerSettings.ReserveEnduranceBarColor;
				}
				else
				{
					float dif = Mathf.Abs(enduranceToDistribute);
					if(dif < Player.PlayerSettings.EndurancePerBar)
					{
						float fill = (Player.PlayerSettings.EndurancePerBar - dif) / Player.PlayerSettings.EndurancePerBar;
						smallBars[i].fillAmount = Mathf.Lerp(smallBars[i].fillAmount, fill, Time.deltaTime * Player.PlayerSettings.EnduranceBarLerpSpeed);
						mainBarFill.fillAmount = Mathf.Lerp(mainBarFill.fillAmount, fill, Time.deltaTime * Player.PlayerSettings.EnduranceBarLerpSpeed);

						smallBars[i].color = Player.PlayerSettings.ActiveEnduranceBarColor;
					}
					else if(dif < Player.PlayerSettings.EndurancePerBar * 2)
					{
						smallBars[i].fillAmount = Mathf.Lerp(smallBars[i].fillAmount, player.lockedEndurance / Player.PlayerSettings.EndurancePerBar, Time.deltaTime * Player.PlayerSettings.EnduranceBarLerpSpeed);
						smallBars[i].color = Player.PlayerSettings.ReloadingEnduranceBarColor;
					}
					else
					{
						smallBars[i].fillAmount = 0;
					}
				}
			}
		}
	}
}