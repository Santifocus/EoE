using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class ScreenBorderColor : FXObject
	{
		[Tooltip("How long should the ScreenBorderColor last?")]
		public float Time = 0.2f;
		public Color Color = Color.red;
		public float Depth = 0.03f;
	}
}