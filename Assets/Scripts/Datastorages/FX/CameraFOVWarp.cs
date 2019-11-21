using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class CameraFOVWarp : FXInstance
	{
		[Tooltip("The target FOV that will be reached and kept at TimeStay.")] 
		public float TargetFOV = 75f;
		[Tooltip("The Time it takes for the FOV to warp in.")]
		public float TimeIn = 0.1f;
		[Tooltip("The Time the FOV will be kept.")]
		public float TimeStay = 0.5f;
		[Tooltip("The Time it takes for the FOV to warp out.")]
		public float TimeOut = 0.2f;
		[Tooltip("The Dominance level of this CameraFOVWarp.")]
		public int Dominance = 1;
		public bool OverwriteOtherFOVWarps = false;
	}
}