using UnityEngine;

namespace EoE.Information
{
	public class ScreenTint : FXObject
	{
		[Tooltip("The target Tint that will be reached and kept at TimeStay.")]
		public Color TintColor = Color.blue / 2;

		[Tooltip("If there are other Screentints active then their colors will be mixed, higher dominance means more impact in the mix result. (Alpha * Value / TotalDominanceAmount).")] public uint Dominance = 1;
	}
}