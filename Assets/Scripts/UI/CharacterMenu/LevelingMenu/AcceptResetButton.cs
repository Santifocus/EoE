using EoE.Controlls;
using UnityEngine;

namespace EoE.UI
{
	public class AcceptResetButton : CMenuItem
	{
		[SerializeField] private Vector2 pointerAcceptPosition = default;
		[SerializeField] private Vector2 pointerResetPosition = default;

		private bool onAcceptPosition;
		private bool justSelected;
		protected override void Select()
		{
			justSelected = true;
			onAcceptPosition = true;
			pointer.rectTransform.anchoredPosition = pointerAcceptPosition;
		}
		protected override void Update()
		{
			base.Update();

			if (justSelected)
			{
				justSelected = false;
				return;
			}

			if (InputController.MenuRight.Down || InputController.MenuLeft.Down)
			{
				onAcceptPosition = !onAcceptPosition;
				pointer.rectTransform.anchoredPosition = onAcceptPosition ? pointerAcceptPosition : pointerResetPosition;
			}

			if (InputController.MenuEnter.Down)
			{
				if (onAcceptPosition)
					LevelingMenuController.Instance.ApplyAssignedPoints();
				else
					LevelingMenuController.Instance.ResetAssignedSkillPoints();
			}
		}
	}
}