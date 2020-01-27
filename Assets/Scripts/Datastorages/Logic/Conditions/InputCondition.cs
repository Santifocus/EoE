using EoE.Controlls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information.Logic
{
	public class InputCondition : LogicComponent
	{
		public enum InputTarget
		{
			A = 1,
			B = 2,
			X = 3,
			Y = 4,
			UpArrow = 5,
			DownArrow = 6,
			RightArrow = 7,
			LeftArrow = 8,
			RightBumper = 9,
			LeftBumper = 10,
			RightTrigger = 11,
			LeftTrigger = 12,
			Menu = 13,
			Pause = 14,
			CameraReset = 15,
			MovingCamera = 16,
		}
		public enum InputCheckStyle
		{
			Down = 1,
			Active = 2,
			Up = 3,
		}
		protected override bool InternalTrue => InputCheck();
		public InputTarget inputTarget = InputTarget.A;
		public InputCheckStyle inputCheckStyle = InputCheckStyle.Down;
		private bool InputCheck()
		{
			switch (inputTarget)
			{
				case InputTarget.A:
					return InputValidation(InputController.MenuEnter);
				case InputTarget.B:
					return InputValidation(InputController.MenuBack);
				case InputTarget.X:
					return InputValidation(InputController.Attack);
				case InputTarget.Y:
					return InputValidation(InputController.Special);
				case InputTarget.UpArrow:
					return InputValidation(InputController.MenuUp);
				case InputTarget.DownArrow:
					return InputValidation(InputController.MenuDown);
				case InputTarget.RightArrow:
					return InputValidation(InputController.MenuRight);
				case InputTarget.LeftArrow:
					return InputValidation(InputController.MenuLeft);
				case InputTarget.RightBumper:
					return InputValidation(InputController.Cast);
				case InputTarget.LeftBumper:
					return InputValidation(InputController.UseItem);
				case InputTarget.RightTrigger:
					return InputValidation(InputController.Shield);
				case InputTarget.LeftTrigger:
					return InputValidation(InputController.Aim);
				case InputTarget.Menu:
					return InputValidation(InputController.PlayerMenu);
				case InputTarget.Pause:
					return InputValidation(InputController.Pause);
				case InputTarget.CameraReset:
					return InputValidation(InputController.ResetCamera);
				case InputTarget.MovingCamera:
					return InputValidation(InputController.CameraMove);
			}
			return false;
		}
		private bool InputValidation(InputController.Button button)
		{
			switch (inputCheckStyle)
			{
				case InputCheckStyle.Down:
					return button.Down;
				case InputCheckStyle.Active:
					return button.Held;
				case InputCheckStyle.Up:
					return button.Up;
			}
			return false;
		}
		private bool InputValidation(Vector2 inputValue)
		{
			const float valueThreshold = 0.25f;
			return (Mathf.Abs(inputValue.x) > valueThreshold) || (Mathf.Abs(inputValue.x) > valueThreshold);
		}
	}
}