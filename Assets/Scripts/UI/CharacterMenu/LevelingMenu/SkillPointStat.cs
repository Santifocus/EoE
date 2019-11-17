using EoE.Controlls;
using EoE.Entities;
using EoE.Information;
using System.Collections;
using TMPro;
using UnityEngine;

namespace EoE.UI
{
	public class SkillPointStat : CMenuItem
	{
		private const float SCROLLING_COOLDOWN = 0.125f;
		private const float SCROLLING_COOLDOWN_ON_FIRST = SCROLLING_COOLDOWN * 3f;
		private static float PointScrollingCooldown;

		public TargetBaseStat targetStat = TargetBaseStat.Health;
		[SerializeField] private Color standardColor = Color.white;
		[SerializeField] private Color changedColor = Color.green;

		[Space(10)]
		[SerializeField] private TextMeshProUGUI newPoints = default;
		[SerializeField] private TextMeshProUGUI curPoints = default;
		[SerializeField] private TextMeshProUGUI curStatValue = default;

		[Space(10)]
		[SerializeField] private Color recentlyChangedCurColor = Color.red;
		[SerializeField] private float recentlyChangedFlashTime = 0.2f;
		[SerializeField] private int recentlyChangedFlashCount = 3;
		protected override void Update()
		{
			if (!selected)
				return;
			if (PointScrollingCooldown > 0)
				PointScrollingCooldown -= Time.unscaledDeltaTime;

			if (InputController.MenuRight.Down)
			{
				LevelingMenuController.Instance.ModifyAssignedSkillPoint(true, targetStat);
				PointScrollingCooldown = SCROLLING_COOLDOWN_ON_FIRST;
			}
			else if(InputController.MenuRight.Active && PointScrollingCooldown <= 0)
			{
				LevelingMenuController.Instance.ModifyAssignedSkillPoint(true, targetStat);
				PointScrollingCooldown = SCROLLING_COOLDOWN;
			}
			else if (InputController.MenuLeft.Down)
			{
				LevelingMenuController.Instance.ModifyAssignedSkillPoint(false, targetStat);
				PointScrollingCooldown = SCROLLING_COOLDOWN_ON_FIRST;
			}
			else if (InputController.MenuLeft.Active && PointScrollingCooldown <= 0)
			{
				LevelingMenuController.Instance.ModifyAssignedSkillPoint(false, targetStat);
				PointScrollingCooldown = SCROLLING_COOLDOWN;
			}
			else if (InputController.MenuEnter.Down)
			{
				LevelingMenuController.Instance.GotoConfirmation();
			}
		}

		public void UpdateNumbers()
		{
			float curValue = Player.LevelingPointsBuff.Effects[(int)targetStat].Amount;
			int curPointsCount = Mathf.RoundToInt(curValue / Player.PlayerSettings.LevelSettings[targetStat]);
			if(!int.TryParse(curPoints.text, out int oldPoints) || oldPoints != curPointsCount)
			{
				curPoints.color = standardColor;
				StopAllCoroutines();
				StartCoroutine(ChangedCurrent());
			}
			curPoints.text = curPointsCount.ToString();

			int newPointsCount = LevelingMenuController.Instance[targetStat];
			newPoints.text = newPointsCount.ToString();

			curStatValue.text = GetCurValue(newPointsCount).ToString();

			bool changed = newPointsCount != 0;
			curStatValue.color = changed ? changedColor : standardColor;
			newPoints.color = changed ? changedColor : standardColor;
		}

		private IEnumerator ChangedCurrent()
		{
			for(int i = 0; i < recentlyChangedFlashCount; i++)
			{
				yield return new WaitForSecondsRealtime(recentlyChangedFlashTime);
				curPoints.color = recentlyChangedCurColor;
				yield return new WaitForSecondsRealtime(recentlyChangedFlashTime);
				curPoints.color = standardColor;
			}
		}

		private float GetCurValue(int newPointsCount)
		{
			return Player.Instance.GetLeveledValue(targetStat) + newPointsCount * Player.PlayerSettings.LevelSettings[targetStat];
		}
	}
}