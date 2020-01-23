using EoE.Controlls;
using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class ConditionObject : ScriptableObject
	{
		#region Enums
		public enum ConditionType
		{
			Comparison = 1,
			State = 2,
			Input = 3,
		}
        public enum ComparisonTarget 
		{
			PlayerHealth = 1,
			PlayerMana = 2,
			PlayerEndurance = 3,
			PlayerExperience = 4,
			PlayerLevel = 5,
			PlayerCurrency = 6,
			PlayerVelocity = 7,
		}
		public enum StateTarget
		{
			PlayerIsMoving = 1,
			PlayerIsRunning = 2,
			PlayerOnGround = 3,
			PlayerInCombat = 4,
			PlayerIsTurning = 5,
			PlayerIsFalling = 6,
			PlayerHasTarget = 7,
			PlayerIsStunned = 8,
			PlayerIsMovementStopped = 9,
			PlayerIsRotationStopped = 10,
			PlayerIsInvincible = 11,
		}
		public enum InputTarget
		{
			Enter = 1,
			Back = 2,
			Action = 3,
			Special = 4,
			Up = 5,
			Down = 6,
			Right = 7,
			Left = 8,
			RightBumper = 9,
			LeftBumper = 10,
			RightTrigger = 11,
			LeftTrigger = 12,
			Menu = 13,
			Pause = 14,
		}
		public enum InputCheckStyle
		{
			Down = 1,
			Active = 2,
			Up = 3,
		}
		#endregion
		#region Fields
		//Base settings
		public bool Inverse = false;
		public ConditionType conditionType = ConditionType.Comparison;

		//Comparison
		public ComparisonTarget comparisonTarget = ComparisonTarget.PlayerHealth;
		public ValueComparer firstComparer = new ValueComparer();
		public bool useSecondComparer = false;
		public ValueComparer secondComparer = new ValueComparer();

		//State
		public StateTarget stateTarget = StateTarget.PlayerOnGround;

		//Input
		public InputTarget inputTarget = InputTarget.Enter;
		public InputCheckStyle inputCheckStyle = InputCheckStyle.Down;
		#endregion
		#region ConditionMetRequest
		public bool ConditionMet()
		{
			switch (conditionType)
			{
				case ConditionType.Comparison:
					return ComparisonCheck() != Inverse;
				case ConditionType.State:
					return StateCheck() != Inverse;
				case ConditionType.Input:
					return InputCheck() != Inverse;
			}
			return false;
		}
		#endregion
		#region Comparison
		private bool ComparisonCheck()
		{
			bool wasMet = false;
			switch (comparisonTarget)
			{
				case ComparisonTarget.PlayerHealth:
					wasMet = Compare(Player.Instance ? Player.Instance.curHealth : 0);
					break;
				case ComparisonTarget.PlayerMana:
					wasMet = Compare(Player.Instance ? Player.Instance.curMana : 0);
					break;
				case ComparisonTarget.PlayerEndurance:
					wasMet = Compare(Player.Instance ? Player.Instance.curEndurance : 0);
					break;
				case ComparisonTarget.PlayerExperience:
					wasMet = Compare(Player.Instance ? Player.Instance.TotalExperience : 0);
					break;
				case ComparisonTarget.PlayerLevel:
					wasMet = Compare(Player.Instance ? Player.Instance.EntitieLevel : 0);
					break;
				case ComparisonTarget.PlayerCurrency:
					wasMet = Compare(Player.Instance ? Player.Instance.CurrentCurrencyAmount : 0);
					break;
				case ComparisonTarget.PlayerVelocity:
					wasMet = Compare(Player.Instance ? Player.Instance.CurVelocity.magnitude : 0);
					break;
			}

			return wasMet;
		}
		private bool Compare(float value)
		{
			return firstComparer.ComparisonMet(value) && (!useSecondComparer || secondComparer.ComparisonMet(value));
		}
		[System.Serializable]
		public class ValueComparer
		{
			public enum ComparisonStyle
			{
				LowerEquals = 1,    //val <= input
				Lower = 2,          //val < input
				Equals = 3,         //val == input
				Higher = 4,         //val > input
				HigherEquals = 5,   //val >= input
			}
			public ComparisonStyle compareStyle = ComparisonStyle.HigherEquals;
			public float compareValue = 1;

			public bool ComparisonMet(float value)
			{
				switch (compareStyle)
				{
					case ComparisonStyle.LowerEquals:
						return compareValue <= value;
					case ComparisonStyle.Lower:
						return compareValue < value;
					case ComparisonStyle.Equals:
						return compareValue == value;
					case ComparisonStyle.Higher:
						return compareValue > value;
					case ComparisonStyle.HigherEquals:
						return compareValue >= value;
				}
				return false;
			}
		}
		#endregion
		#region State
		private bool StateCheck()
		{
			bool wasMet = false;
			switch (stateTarget)
			{
				case StateTarget.PlayerIsMoving:
					wasMet = Player.Instance ? Player.Instance.curStates.Moving : false;
					break;
				case StateTarget.PlayerIsRunning:
					wasMet = Player.Instance ? Player.Instance.curStates.Running : false;
					break;
				case StateTarget.PlayerOnGround:
					wasMet = Player.Instance ? Player.Instance.charController.isGrounded : false;
					break;
				case StateTarget.PlayerInCombat:
					wasMet = Player.Instance ? Player.Instance.curStates.Fighting : false;
					break;
				case StateTarget.PlayerIsTurning:
					wasMet = Player.Instance ? Player.Instance.curStates.Turning : false;
					break;
				case StateTarget.PlayerIsFalling:
					wasMet = Player.Instance ? Player.Instance.curStates.Falling : false;
					break;
				case StateTarget.PlayerHasTarget:
					wasMet = Player.Instance ? Player.Instance.TargetedEntitie : false;
					break;
				case StateTarget.PlayerIsStunned:
					wasMet = Player.Instance ? Player.Instance.IsStunned : false;
					break;
				case StateTarget.PlayerIsMovementStopped:
					wasMet = Player.Instance ? Player.Instance.IsMovementStopped : false;
					break;
				case StateTarget.PlayerIsRotationStopped:
					wasMet = Player.Instance ? Player.Instance.IsTurnStopped : false;
					break;
				case StateTarget.PlayerIsInvincible:
					wasMet = Player.Instance ? Player.Instance.IsInvincible : false;
					break;
			}
			return wasMet;
		}
		#endregion
		#region Input
		private bool InputCheck()
		{
			switch (inputTarget)
			{
				case InputTarget.Enter:
					return InputValidation(InputController.MenuEnter);
				case InputTarget.Back:
					return InputValidation(InputController.MenuBack);
				case InputTarget.Action:
					return InputValidation(InputController.Attack);
				case InputTarget.Special:
					return InputValidation(InputController.HeavyAttack);
				case InputTarget.Up:
					return InputValidation(InputController.MenuUp);
				case InputTarget.Down:
					return InputValidation(InputController.MenuDown);
				case InputTarget.Right:
					return InputValidation(InputController.MenuRight);
				case InputTarget.Left:
					return InputValidation(InputController.MenuLeft);
				case InputTarget.RightBumper:
					return InputValidation(InputController.MagicCast);
				case InputTarget.LeftBumper:
					return InputValidation(InputController.UseItem);
				case InputTarget.RightTrigger:
					return InputValidation(InputController.Block);
				case InputTarget.LeftTrigger:
					return InputValidation(InputController.Aim);
				case InputTarget.Menu:
					return InputValidation(InputController.PlayerMenu);
				case InputTarget.Pause:
					return InputValidation(InputController.Pause);
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
		#endregion
	}
}