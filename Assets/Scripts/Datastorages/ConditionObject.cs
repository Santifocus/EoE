using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class ConditionObject : ScriptableObject
	{
		public enum ConditionType
		{
			Comparison = 1,
			State = 2,
		}
        public enum ComparisonTarget 
		{
			PlayerHealth = 1,
			PlayerMana = 2,
			PlayerEndurance = 3,
			PlayerExperience = 4,
			PlayerLevel = 5,
			PlayerCurrency = 6,
		}
		public enum StateTarget
		{
			PlayerOnGround = 1,
			PlayerRunning = 2,
			PlayerInCombat = 3,
			PlayerHasTarget = 4,
		}

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

		public bool ConditionMet()
		{
			return ((conditionType == ConditionType.Comparison) ? ComparisonCheck() : StateCheck()) != Inverse;
		}
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
			}

			return wasMet;
		}
		private bool StateCheck()
		{
			bool wasMet = false;
			switch (stateTarget)
			{
				case StateTarget.PlayerOnGround:
					wasMet = Player.Instance ? Player.Instance.charController.isGrounded : false;
					break;
				case StateTarget.PlayerRunning:
					wasMet = Player.Instance ? Player.Instance.curStates.Running : false;
					break;
				case StateTarget.PlayerInCombat:
					wasMet = Player.Instance ? Player.Instance.curStates.Fighting : false;
					break;
				case StateTarget.PlayerHasTarget:
					wasMet = Player.Instance ? Player.Instance.TargetedEntitie : false;
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
	}
}