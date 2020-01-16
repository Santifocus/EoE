using EoE.Combatery;

namespace EoE.Information
{
	public class CasterSettings : EnemySettings
	{
		public CasterBehaiviorPattern[] BehaiviorPatterns = new CasterBehaiviorPattern[0];
		[System.Serializable]
		public class CasterBehaiviorPattern
		{
			public Spell TargetSpell = default;
			public float MinRange = default;
			public float MaxRange = default;
		}
	}
}