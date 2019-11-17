namespace EoE.Information
{
	public class LevelingSettings : ObjectSettings
	{
		public LevelingCurve curve;
		public int AttributePointsPerLevel;
		public int SkillPointsPerLevel;

		public float PerTenLevelsBasePoints;
		public float RotationExtraPoints;

		public float this[TargetBaseStat stat]
		{
			get
			{
				switch (stat)
				{
					case TargetBaseStat.Health:
						return HealthPerSkillPoint;
					case TargetBaseStat.Mana:
						return ManaPerSkillPoint;
					case TargetBaseStat.Endurance:
						return EndurancePerSkillPoint;
					case TargetBaseStat.PhysicalDamage:
						return PhysicalDamagePerSkillPoint;
					case TargetBaseStat.MagicalDamage:
						return MagicDamagePerSkillPoint;
					case TargetBaseStat.Defense:
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