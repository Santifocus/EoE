﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class LevelingSettings : ObjectSettings
	{
		public LevelingCurve curve;
		public int AttributePointsPerLevel;
		public int SkillPointsPerLevel;

		public float PerTenLevelsBasePoints;
		public float RotationExtraPoints;

		public float this[TargetStat stat]
		{
			get
			{
				switch (stat)
				{
					case TargetStat.Health:
						return HealthPerSkillPoint;
					case TargetStat.Mana:
						return ManaPerSkillPoint;
					case TargetStat.Endurance:
						return EndurancePerSkillPoint;
					case TargetStat.PhysicalDamage:
						return PhysicalDamagePerSkillPoint;
					case TargetStat.MagicalDamage:
						return MagicDamagePerSkillPoint;
					case TargetStat.Defense:
						return DefensePerSkillPoint;
					default:
						return 0;
				}
			}
		}

		public float HealthPerSkillPoint;
		public float ManaPerSkillPoint;
		public float EndurancePerSkillPoint;
		public float PhysicalDamagePerSkillPoint;
		public float MagicDamagePerSkillPoint;
		public float DefensePerSkillPoint;

		[System.Serializable]
		public class LevelingCurve
		{
			public double a = 1;
			public double b = 1;
			public double c = 1;

			public int GetRequiredSouls(int x)
			{
				double value = (x * x) * a + x * b + c;
				return (int)value;
			}
		}
	}
}