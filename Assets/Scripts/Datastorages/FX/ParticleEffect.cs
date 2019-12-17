using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class ParticleEffect : FXObject
	{
		[Tooltip("The Object that will be created, all Particle systems attached to it will be played.")]
		public GameObject ParticleMainObject = null;
		[Tooltip("With which offset should the Effect be spawned?")]
		public Vector3 OffsetToTarget = Vector3.zero;
		[Tooltip("With which rotation offset should the Effect be spawned?")]
		public Vector3 RotationOffset = Vector3.zero;
		[Tooltip("When enabled, this effect will follow the target, otherwise it will stay at the position it was spawned at.")]
		public bool FollowTarget = false;
		[Tooltip("When enabled, this effect will inherit the rotation of the target it is following, otherwise the rotation stays as it was when it spawned.")]
		public bool InheritRotationOfTarget = false;
		[Tooltip("If this Effect should inherit rotation, then based on this value the rotation will be lerped (Higher == faster).")]
		public float RotationInheritLerpSpeed = 4;
	}
}