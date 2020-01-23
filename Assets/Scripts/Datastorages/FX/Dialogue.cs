using EoE.UI;
using UnityEngine;

namespace EoE.Information
{
	public class Dialogue : TextBasedFX
	{
		public Sprite dialogueIcon = default;
		public bool pauseTimeWhenDisplaying = false;
		[Tooltip("On which canvas should this Object be spawned on?")]
		public CanvasTarget CanvasTarget = CanvasTarget.Main;
	}
}