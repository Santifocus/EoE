using EoE.Sounds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class SoundEffect : FXObject
	{
		public Sound TargetSound = null;
		public bool FollowTarget = false;
		public Vector3 OffsetToTarget = Vector3.zero;
		[Tooltip("Should the sound be stopped after a set delay?")]
		public bool DestroyAfterDelay = false;
		[Tooltip("With what delay should the effect be stopped?")]
		public float DestroyDelay = 0.3f;
		public BehaiviorOnParentDeath OnParentDeathBehaivior = BehaiviorOnParentDeath.Remove;
		public EffectRemoveStyle RemoveStyle = EffectRemoveStyle.Destroy;
		public float FadeOutTime = 0;
	}
}