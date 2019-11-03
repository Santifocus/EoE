using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class LevelingSettings : ObjectSettings
	{
		public LevelingCurve curve;
		public float[] baseIncrementPerLevel;
		public float[] incrementsForSkillpoint;

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