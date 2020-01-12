using EoE.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class TextBasedFX : FXObject
	{
		public ColoredText[] parts;
		public TextBasedFX CreateInstructedNotification((string, string)[] replaceInstructions)
		{
			TextBasedFX newTextBasedFX = Instantiate(this);

			for (int i = 0; i < newTextBasedFX.parts.Length; i++)
			{
				string curText = parts[i].text;
				for (int j = 0; j < replaceInstructions.Length; j++)
				{
					curText = curText.Replace(replaceInstructions[j].Item1, replaceInstructions[j].Item2);
				}

				newTextBasedFX.parts[i].text = curText;
				newTextBasedFX.parts[i].textColor = parts[i].textColor;
			}

			return newTextBasedFX;
		}
	}
	[System.Serializable]
	public struct ColoredText
	{
		public const string COLOR_CLOSER = "</color>";
		public Color textColor;
		public string text;

		public override string ToString()
		{
			return ColorToColorOpener(textColor) + text + COLOR_CLOSER;
		}
		public static string ToString(ColoredText[] coloredTexts)
		{
			if (coloredTexts == null)
				return "";

			string fullText = "";
			for (int i = 0; i < coloredTexts.Length; i++)
			{
				fullText += coloredTexts[i];
			}
			return fullText;
		}
		public static string ColorToColorOpener(Color col)
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(col) + ">";
		}
	}
}