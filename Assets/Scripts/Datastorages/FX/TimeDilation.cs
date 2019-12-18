using UnityEngine;

namespace EoE.Information
{
	public class TimeDilation : FXObject
	{
		[Tooltip("The target TimeScale that will be reached and kept at TimeStay.")]
		public float Scale = 0.25f;

		[Tooltip("If some other Time dilation is active their dominance levels will be compared, the bigger one will cause the TimeDilation and the weaker one will be ignored.")] public uint Dominance = 0;
		[Tooltip("If this is enabled other TimeDilations that are currently active will be deleted and only this one will executed")]
		public bool OverwriteOtherTimeDilations = false;
	}
}