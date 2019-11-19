using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class TimeDilation : FXInstance
	{
		[Tooltip("The target TimeScale that will be reached and kept at TimeStay.")] 
		public float Scale = 0.25f;
		[Tooltip("The Time it takes for the TimeScale to fade in.")]
		public float TimeIn = 0.1f;
		[Tooltip("The Time the TimeScale will be kept.")]
		public float TimeStay = 0.5f;
		[Tooltip("The Time it takes for the TimeScale to fade out.")] 
		public float TimeOut = 0.2f;

		[Tooltip("If some other Time dilation is active their dominance levels will be compared, the bigger one will cause the TimeDilation and the weaker one will be ignored.")] public uint Dominance = 0;
		[Tooltip("If this is enabled other TimeDilations that are currently active will be deleted and only this one will executed")]
		public bool OverwriteOtherTimeDilations = false;
	}
}