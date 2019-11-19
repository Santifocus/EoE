using EoE.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class DialogueInput : FXInstance
	{
		public DialoguePart[] parts;
		[System.Serializable]
		public struct DialoguePart
		{
			public Color textColor;
			public string text;
		}
	}
}