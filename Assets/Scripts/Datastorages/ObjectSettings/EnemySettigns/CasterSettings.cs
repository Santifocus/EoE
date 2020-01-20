using EoE.Combatery;

namespace EoE.Information
{
	public enum CastCooldownBehaivior { WaitHere = 1, StayAtDistance = 2, GotoTarget = 3, FleeToAlly = 4 }
	public class CasterSettings : EnemySettings
	{
		public CasterBehaiviorPattern[] BehaiviorPatterns = new CasterBehaiviorPattern[0];
		public CastCooldownBehaivior CooldownBehaivior = CastCooldownBehaivior.WaitHere;
		public float TargetDistance;

		public float PanicModeThreshold = 0.2f;
		public float PanicModeAlliedSearchRange = 50;
		public ActivationEffect[] PanicModeEffects = new ActivationEffect[0];
		public ActivationEffect[] PanicModeFinishedEffects = new ActivationEffect[0];

		[System.Serializable]
		public class CasterBehaiviorPattern
		{
			public ActivationCompound TargetCompound = default;
			public float MinRange = default;
			public float MaxRange = default;
		}
	}
}