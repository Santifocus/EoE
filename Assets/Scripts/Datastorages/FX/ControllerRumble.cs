using UnityEngine;

namespace EoE.Information
{
	public class ControllerRumble : FXObject
	{
		[Tooltip("How strong is the shake of the left Motor at the start / end? (LeftMotor is HighFrequency Vibration), (0 - 1)")]
		public float LeftMinIntensity = 0.5f;
		[Tooltip("How strong is the shake of the right Motor at the start / end? (RightMotor is LowFrequency Vibration), (0 - 1)")]
		public float RightMinIntensity = 0.8f;
		[Tooltip("How strong is the shake of the left Motor at the climax? (LeftMotor is HighFrequency Vibration), (0 - 1)")]
		public float LeftMaxIntensity = 0.6f;
		[Tooltip("How strong is the shake of the right Motor at the climax? (RightMotor is LowFrequency Vibration), (0 - 1)")]
		public float RightMaxIntensity = 0.9f;
	}
}