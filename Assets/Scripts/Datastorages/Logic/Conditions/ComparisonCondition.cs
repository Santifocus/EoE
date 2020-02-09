using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information.Logic
{
	public class ComparisonCondition : LogicComponent
	{
		public enum ComparisonTarget
		{
			PlayerHealth = 1,
			PlayerMana = 2,
			PlayerStamina = 3,
			PlayerExperience = 4,
			PlayerLevel = 5,
			PlayerCurrency = 6,
			PlayerVelocity = 7,
		}
		public ComparisonTarget comparisonTarget = ComparisonTarget.PlayerHealth;
		public ValueComparer firstComparer = new ValueComparer();
		public bool useSecondComparer = false;
		public ValueComparer secondComparer = new ValueComparer();

		protected override bool InternalTrue => ComparisonCheck();

		private bool ComparisonCheck()
		{
			bool wasMet = false;
			switch (comparisonTarget)
			{
				case ComparisonTarget.PlayerHealth:
					wasMet = Compare(Player.Instance ? Player.Instance.CurHealth : 0);
					break;
				case ComparisonTarget.PlayerMana:
					wasMet = Compare(Player.Instance ? Player.Instance.CurMana : 0);
					break;
				case ComparisonTarget.PlayerStamina:
					wasMet = Compare(Player.Instance ? Player.Instance.CurStamina : 0);
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
	}
}