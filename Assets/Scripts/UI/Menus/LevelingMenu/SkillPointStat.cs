using EoE.Controlls;
using EoE.Entities;
using EoE.Information;
using System.Collections;
using TMPro;
using UnityEngine;
namespace EoE.UI
{
	public class SkillPointStat : ControllerMenuItem
	{
		public TargetBaseStat targetStat = TargetBaseStat.Health;

		[Space(10)]
		[SerializeField] private TextMeshProUGUI curStatValue = default;
		[SerializeField] private TextMeshProUGUI totalPoints = default;
		[SerializeField] private TextMeshProUGUI newStatValue = default;

		private int lastAppliedPoints;

		public void UpdateNumbers()
		{
			float curValue = Player.Instance.GetLeveledValue(targetStat);
			curStatValue.text = curValue.ToString();

			int notConfirmedPoints = LevelingMenuController.Instance[targetStat];
			int curAppliedPoints = Mathf.RoundToInt(Player.Instance.LevelingPointsBuff.Effects[(int)targetStat].Amount / Player.PlayerSettings.LevelSettings[targetStat]);
			totalPoints.text = (curAppliedPoints + notConfirmedPoints).ToString();
			if (curAppliedPoints != lastAppliedPoints)
			{
				lastAppliedPoints = curAppliedPoints;
				StartCoroutine(ChangedCurrent());
			}

			newStatValue.text = (curValue + notConfirmedPoints * Player.PlayerSettings.LevelSettings[targetStat]).ToString();

			bool changed = notConfirmedPoints != 0;
			totalPoints.color = changed ? LevelingMenuController.Instance.notConfirmedChangeColor : LevelingMenuController.Instance.standardColor;
			newStatValue.color = changed ? LevelingMenuController.Instance.notConfirmedChangeColor : LevelingMenuController.Instance.standardColor;
		}

		private void OnDisable()
		{
			StopAllCoroutines();
			totalPoints.color = LevelingMenuController.Instance.standardColor;
			curStatValue.color = LevelingMenuController.Instance.standardColor;
		}

		private IEnumerator ChangedCurrent()
		{
			for (int i = 0; i < LevelingMenuController.Instance.recentlyChangedFlashCount; i++)
			{
				yield return new WaitForSecondsRealtime(LevelingMenuController.Instance.recentlyChangedFlashTime);
				totalPoints.color = LevelingMenuController.Instance.recentlyChangedFlashColor;
				curStatValue.color = LevelingMenuController.Instance.recentlyChangedFlashColor;
				yield return new WaitForSecondsRealtime(LevelingMenuController.Instance.recentlyChangedFlashTime);
				totalPoints.color = LevelingMenuController.Instance.standardColor;
				curStatValue.color = LevelingMenuController.Instance.standardColor;
			}
		}
	}
}