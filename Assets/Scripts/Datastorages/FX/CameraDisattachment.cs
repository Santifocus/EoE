using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class CameraDisattachment : FXObject
	{
		public bool LookAtPlayer = true;
		public float ReatachTime = 1;
		public bool OverwriteOtherCameraDisattachments = false;
	}
}