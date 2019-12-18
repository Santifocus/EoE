using EoE.UI;
using UnityEngine;

namespace EoE.Information
{
	public class Notification : FXObject
	{
		public Sprite notificationIcon;
		public Color iconColor = Color.white;
		public ColoredText[] parts;
		public float timeToExpand = 0.5f;

		public Notification CreateInstructedNotification((string, string)[] replaceInstrctions)
		{
			Notification newNotification = Instantiate(this);

			for (int i = 0; i < newNotification.parts.Length; i++) 
			{
				string curText = parts[i].text;
				for (int j = 0; j < replaceInstrctions.Length; j++)
				{
					curText = curText.Replace(replaceInstrctions[j].Item1, replaceInstrctions[j].Item2);
				}

				newNotification.parts[i].text = curText;
				newNotification.parts[i].textColor = parts[i].textColor;
			}

			return newNotification;
		}
	}
}