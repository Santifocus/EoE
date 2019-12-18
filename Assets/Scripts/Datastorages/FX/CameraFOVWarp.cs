using UnityEngine;

namespace EoE.Information
{
	public class CameraFOVWarp : FXObject
	{
		[Tooltip("The target FOV that will be reached and kept at TimeStay.")]
		public float TargetFOV = 75f;
		[Tooltip("The Dominance level of this CameraFOVWarp.")]
		public int Dominance = 1;
		public bool OverwriteOtherFOVWarps = false;
	}
}