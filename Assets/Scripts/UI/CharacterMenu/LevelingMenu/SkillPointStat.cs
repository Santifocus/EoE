using EoE.Controlls;
using EoE.Entities;
using EoE.Information;
using TMPro;
using UnityEngine;

namespace EoE.UI
{
	public class SkillPointStat : CMenuItem
	{
		private const float SCROLLING_COOLDOWN = 0.125f;
		private const float SCROLLING_COOLDOWN_ON_FIRST = SCROLLING_COOLDOWN * 3f;
		private static float PointScrollingCooldown;

		public TargetStat targetStat = TargetStat.Health;
		[SerializeField] private TextMeshProUGUI newPoints = default;
		[SerializeField] private TextMeshProUGUI curPoints = default;
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
			int curPointsCount = Mathf.RoundToInt(Player.LevelingPointsBuff.Effects[(int)targetStat].Amount / Player.PlayerSettings.LevelSettings[targetStat]);
			curPoints.text = curPointsCount.ToString();

			int newPointsCount = LevelingMenuController.Instance[targetStat];
			newPoints.text = newPointsCount.ToString();
		}
	}
}