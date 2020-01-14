using EoE.Combatery;

namespace EoE.Information
{
	public class CrawlerSettings : EnemySettings
	{
		//Combat
		[UnityEngine.Serialization.FormerlySerializedAs("AttackSpeed")]public float BashChargeSpeed = 1;
		public float BashSpeed = 5;
		public float BashDistance = 4;
		public float ForceTranslationMultiplier = 2;

		//VFX
		public float BashAnnouncementDelay = -0.2f;
		public EffectSingle DirectHitEntitieEffect = default;
		public ActivationEffect[] BashStartEffects = new ActivationEffect[0];
		public ActivationEffect[] BashHitWallEffects = new ActivationEffect[0];
		public ActivationEffect[] BashHitEntitieEffects = new ActivationEffect[0];

		public FXObject[] BashAnnouncement = new FXObject[0];
	}
}