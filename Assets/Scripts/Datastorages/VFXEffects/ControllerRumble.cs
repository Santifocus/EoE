using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class ControllerRumble : VFXEffect
	{
		[Tooltip("How long should the ControllerRumble last?")]
		public float Time = 0.1f;
		[Tooltip("How strong is the shake of the left Motor at the start? (LeftMotor is HighFrequency Vibration), (0 - 1)")]
		public float LeftStartIntensity = 0.6f;
		[Tooltip("How strong is the shake of the right Motor at the start? (RightMotor is LowFrequency Vibration), (0 - 1)")]
		public float RightStartIntensity = 0.9f;
		[Tooltip("How strong is the shake of the left Motor at the end? (LeftMotor is HighFrequency Vibration), (0 - 1)")]
		public float LeftEndIntensity = 0.5f;
		[Tooltip("How strong is the shake of the right Motor at the end? (RightMotor is LowFrequency Vibration), (0 - 1)")]
		public float RightEndIntensity = 0.8f;
	}
}