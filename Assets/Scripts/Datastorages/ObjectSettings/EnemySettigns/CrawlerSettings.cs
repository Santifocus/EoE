using UnityEngine;

namespace EoE.Information
{
	public class CrawlerSettings : EnemySettings
	{
		//Combat
		public float AttackSpeed = 1;
		public float BashSpeed = 5;
		public float BashDistance = 4;
		public float ForceTranslationMultiplier = 2;

		//VFX
		public float BashAnnouncementDelay = -0.2f;
		public FXObject[] BashAnnouncement = new FXObject[0];
	}
}