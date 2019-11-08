using EoE.Controlls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.UI
{
	public class AcceptResetButton : CMenuItem
	{
		[SerializeField] private Vector2 pointerAcceptPosition = default;
		[SerializeField] private Vector2 pointerResetPosition = default;
		public PointTarget targetPoints = default;

		public enum PointTarget { SkillPoints, AttributePoints }

		private bool onAcceptPosition;
		protected override void Select()
		{
			onAcceptPosition = true;
			pointer.rectTransform.anchoredPosition = pointerAcceptPosition;
		}
		protected override void Update()
		{
			base.Update();
			
			if (!selected)
				return;

			if(InputController.MenuRight.Down || InputController.MenuLeft.Down)
			{
				onAcceptPosition = !onAcceptPosition;
				pointer.rectTransform.anchoredPosition = onAcceptPosition ? pointerAcceptPosition : pointerResetPosition;
			}

			if (InputController.MenuEnter.Down)
			{
				if (onAcceptPosition)
					LevelingMenuController.Instance.ApplyAssignedPoints(targetPoints == PointTarget.AttributePoints);
				else
					LevelingMenuController.Instance.ResetAssignedSkillPoints(targetPoints == PointTarget.AttributePoints);
			}
		}
	}
}