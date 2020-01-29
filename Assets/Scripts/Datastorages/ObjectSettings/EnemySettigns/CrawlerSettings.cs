using EoE.Combatery;

namespace EoE.Information
{
	public class CrawlerSettings : EnemySettings
	{
		public float ForceTranslationMultiplier = 2;

		//Combat
		public float BashChargeSpeed = 1;
		public float BashSpeed = 5;
		public float BashDistance = 4;

		//TrickBash
		public float ChanceForTrickBash = 0.3f;
		public float TrickBashSpeed = 5;
		public float TrickBashDistance = 4;

		//Effects
		public EffectSingle DirectHitEntitieEffect = default;
		public ActivationEffect[] BashChargeStartEffects = new ActivationEffect[0];
		public ActivationEffect[] TrickBashChargeStartEffects = new ActivationEffect[0];
		public ActivationEffect[] BashStartEffects = new ActivationEffect[0];
		public ActivationEffect[] BashHitTerrainEffects = new ActivationEffect[0];
		public ActivationEffect[] BashHitEntitieEffects = new ActivationEffect[0];

		//Animation
		public float AnimationWalkSpeedDivider = 6;
	}
}