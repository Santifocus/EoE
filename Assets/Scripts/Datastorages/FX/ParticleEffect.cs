using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public enum BehaiviorOnParentDeath { Fade, Remove, Stay }
	public class ParticleEffect : FXInstance
	{
		[Tooltip("The Object that will be created, all Particle systems attached to it will be played.")]
		public GameObject ParticleMainObject = null;
		[Tooltip("At what point should the particle effect be stopped?")]
		public float DestroyDelay = 0.3f;
		[Tooltip("With which offset should the Effect be spawned?")]
		public Vector3 OffsetToTarget = Vector3.zero;
		[Tooltip("When enabled, this effect will follow the target, otherwise it will stay at the position it was spawned at.")]
		public bool FollowTarget = false;
		[Tooltip("When enabled, this effect will inherit the rotation of the target it is following, otherwise the rotation stays as it was when it spawned.")]
		public bool InheritRotationOfTarget = false;
		[Tooltip("What should happen when the target the effect was following was destroyed?")]
		public BehaiviorOnParentDeath OnTargetDeathBehaivior = BehaiviorOnParentDeath.Fade;
	}
}