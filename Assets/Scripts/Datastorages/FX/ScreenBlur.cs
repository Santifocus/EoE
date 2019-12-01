using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class ScreenBlur : FXObject
	{
		[Tooltip("How Intense should the Blur be in the Players sight?")]
		public float Intensity = 0.4f;
		[Tooltip("Higher Distance means a more Soft blur because it reads more pixel from a bigger distance. (Strong impact on performance: Amount of Pixel read = (Distance * 2 + 1)²)")]
		public int BlurDistance = 4;
	}
}