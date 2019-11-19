using EoE.Sounds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class SoundEffect : FXInstance
	{
		public Sound TargetSound;
		public Vector3 OffsetToTarget;
		public bool Loop;
		public bool LocalEffect;
	}
}