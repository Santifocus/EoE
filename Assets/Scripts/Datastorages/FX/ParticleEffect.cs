using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class ParticleEffect : FXInstance
	{
		[Tooltip("The Object that will be created, all Particle systems attached to it will be played.")]
		public GameObject ParticleMainObject = null;
		[Tooltip("At what point should the particle effect be stopped?")]
		public float DestroyDelay = 0.3f;
		[Tooltip("With which offset should the Effect be spawned?")]
		public Vector3 OffsetToTarget = Vector3.zero;
		[Tooltip("When enabled, this effect will follow the target, otherwise it will stay at the position it was spawned at.")]
		public bool LocalEffect = true;
	}
}