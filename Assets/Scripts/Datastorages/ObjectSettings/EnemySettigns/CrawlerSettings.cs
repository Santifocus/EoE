using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class CrawlerSettings : EnemySettings
	{
		//Combat
		public float AttackSpeed = 1;
		public float InRangeWaitTime = 0.3f;
		public float BashSpeed = 5;
		public float BashDistance = 4;
		public float ForceTranslationMultiplier = 2;
	}
}