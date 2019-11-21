using EoE.Sounds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class SoundEffect : FXInstance
	{
		public Sound TargetSound = null;
		public Vector3 OffsetToTarget = Vector3.zero;
		public bool Loop = false;
		public bool FollowTarget = false;
		public BehaiviorOnParentDeath OnParentDeathBehaivior = BehaiviorOnParentDeath.Remove;
		public float fadeOutTime = 0;
	}
}