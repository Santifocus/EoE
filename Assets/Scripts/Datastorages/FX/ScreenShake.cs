using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class ScreenShake : FXObject
	{
		[Tooltip("How strong is the intensity on the XYZ Axis")]
		public float AxisIntensity = 0.125f;
		[Tooltip("How strong is the intensity on the XYZ Angles")]
		public float AngleIntensity = 0.05f;
		[Tooltip("If you want different amount of shake intensitys for the Axis you can use this Vector, it will multiply the corrosponding Axis")]
		public Vector3 CustomAxisMultiplier = Vector3.one;
		[Tooltip("If you want different amount of shake intensitys for the Angles you can use this Vector, it will multiply the corrosponding Angles")]
		public Vector3 CustomAngleMultiplier = Vector3.one;
	}
}