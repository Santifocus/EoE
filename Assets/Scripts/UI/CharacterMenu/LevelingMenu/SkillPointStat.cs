using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Controlls;
using EoE.Information;
using TMPro;
using EoE.Entities;

namespace EoE.UI
{
	public class SkillPointStat : CMenuItem
	{
		public TargetStat targetStat = TargetStat.Health;
		[SerializeField] private TextMeshProUGUI newPoints = default;
		[SerializeField] private TextMeshProUGUI curPoints = default;
		protected override void Update()
		{
			if (!selected)
				return;

			byte changedStat = (byte)(InputController.MenuRight.Down ? 2 : (InputController.MenuLeft.Down ? 1 : 0));
			if (changedStat > 0)
			{
				LevelingMenuController.Instance.ModifyAssignedSkillPoint(changedStat == 2, targetStat);
			}
			else if (InputController.MenuEnter.Down)
			{
				LevelingMenuController.Instance.GotoConfirmation();
			}
		}

		public void UpdateNumbers()
		{
			int curPointsCount = Mathf.RoundToInt(Player.LevelingPointsBuff.Effects[(int)targetStat].Amount / Player.PlayerSettings.LevelSettings[targetStat]);
			curPoints.text = curPointsCount.ToString();

			int newPointsCount = LevelingMenuController.Instance[targetStat];
			newPoints.text = newPointsCount.ToString();
		}
	}
}