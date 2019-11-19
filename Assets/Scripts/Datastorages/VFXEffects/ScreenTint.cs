using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class ScreenTint : FXInstance
	{
		[Tooltip("The target Tint that will be reached and kept at TimeStay.")] 
		public Color TintColor = Color.blue / 2;
		[Tooltip("The Time it takes for the Tint to fade in.")]
		public float TimeIn = 0.05f;
		[Tooltip("The Time the Tint will be kept.")]
		public float TimeStay = 0.2f;
		[Tooltip("The Time it takes for the Tint to fade out.")] 
		public float TimeOut = 0.1f;

		[Tooltip("If there are other Screentints active then their colors will be mixed, higher dominance means more impact in the mix result. (Alpha * Value / TotalDominanceAmount).")] public uint Dominance = 1;
	}
}