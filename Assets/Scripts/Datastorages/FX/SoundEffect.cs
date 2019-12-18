using EoE.Sounds;
using UnityEngine;

namespace EoE.Information
{
	public class SoundEffect : FXObject
	{
		public Sound TargetSound = null;
		public bool FollowTarget = false;
		public Vector3 OffsetToTarget = Vector3.zero;
	}
}