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
		[SerializeField] private TextMeshProUGUI curStatValue = default;
		[SerializeField] private TextMeshProUGUI totalPoints = default;
		[SerializeField] private TextMeshProUGUI newStatValue = default;

		[Space(10)]
		private int lastAppliedPoints;
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
			else if (InputController.MenuRight.Active && PointScrollingCooldown <= 0)
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
			float curValue = Player.Instance.GetLeveledValue(targetStat);
			curStatValue.text = curValue.ToString();

			int notConfirmedPoints = LevelingMenuController.Instance[targetStat];
			int curAppliedPoints = Mathf.RoundToInt(Player.LevelingPointsBuff.Effects[(int)targetStat].Amount / Player.PlayerSettings.LevelSettings[targetStat]);
			totalPoints.text = (curAppliedPoints + notConfirmedPoints).ToString();
			if(curAppliedPoints != lastAppliedPoints)
			{
				lastAppliedPoints = curAppliedPoints;
				StartCoroutine(ChangedCurrent());
			}

			newStatValue.text = (curValue + notConfirmedPoints * Player.PlayerSettings.LevelSettings[targetStat]).ToString();

			bool changed = notConfirmedPoints != 0;
			totalPoints.color = changed ? changedColor : standardColor;
			newStatValue.color = changed ? changedColor : standardColor;
		}

		private void OnDisable()
		{
			StopAllCoroutines();
			totalPoints.color = standardColor;
			curStatValue.color = standardColor;
		}

		private IEnumerator ChangedCurrent()
		{
			for(int i = 0; i < recentlyChangedFlashCount; i++)
			{
				yield return new WaitForSecondsRealtime(recentlyChangedFlashTime);
				totalPoints.color = recentlyChangedCurColor;
				curStatValue.color = recentlyChangedCurColor;
				yield return new WaitForSecondsRealtime(recentlyChangedFlashTime);
				totalPoints.color = standardColor;
				curStatValue.color = standardColor;
			}
		}
	}
}