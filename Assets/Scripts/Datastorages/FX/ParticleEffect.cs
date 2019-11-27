using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public enum BehaiviorOnParentDeath { Remove, Stay }
	public enum EffectRemoveStyle { Fade, Destroy }
	public class ParticleEffect : FXObject
	{
		[Tooltip("The Object that will be created, all Particle systems attached to it will be played.")]
		public GameObject ParticleMainObject = null;
		[Tooltip("Should the particle system be stopped after a set delay?")]
		public bool DestroyAfterDelay = true;
		[Tooltip("With what delay should the effect be stopped?")]
		public float DestroyDelay = 0.3f;
		[Tooltip("With which offset should the Effect be spawned?")]
		public Vector3 OffsetToTarget = Vector3.zero;
		[Tooltip("When enabled, this effect will follow the target, otherwise it will stay at the position it was spawned at.")]
		public bool FollowTarget = false;
		[Tooltip("When enabled, this effect will inherit the rotation of the target it is following, otherwise the rotation stays as it was when it spawned.")]
		public bool InheritRotationOfTarget = false;
		[Tooltip("What should happen when the target the effect was following was destroyed?")]
		public BehaiviorOnParentDeath OnParentDeathBehaivior = BehaiviorOnParentDeath.Remove;
		[Tooltip("When this particle system should be removed, how should that happen? Fade = StopEmitting Particles, Destroy = Instant remove")]
		public EffectRemoveStyle DestroyStyle = EffectRemoveStyle.Fade;
	}
}