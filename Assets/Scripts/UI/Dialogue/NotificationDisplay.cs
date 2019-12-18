using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EoE.Information;
using UnityEngine.UI;

namespace EoE.UI
{
	public class NotificationDisplay : MonoBehaviour, IPoolableObject<NotificationDisplay>
	{ 
		[SerializeField] private Image notificationIconDisplay = default;
		[SerializeField] private TextMeshProUGUI textDisplay = default;

		private float curAlphaMultiplier;
		public float AlphaMultiplier 
		{ 
			get => curAlphaMultiplier; 
			set 
			{ 
				curAlphaMultiplier = value;
				textDisplay.color = new Color(textDisplay.color.r, textDisplay.color.g, textDisplay.color.b, curAlphaMultiplier * textBaseAlpha);
				notificationIconDisplay.color = new Color(notificationIconDisplay.color.r, notificationIconDisplay.color.g, notificationIconDisplay.color.b, notificationIconDisplay.sprite ? (curAlphaMultiplier * iconBaseAlpha) : 0);
			} 
		}
		public RectTransform rectTransform { get => transform as RectTransform; }
		public PoolableObject<NotificationDisplay> SelfPool { get; set; }

		private float textBaseAlpha;
		private float iconBaseAlpha;
		private bool textIsOffset;

		private Notification info;

		private bool doneAddingLetters;
		private int insertIndex => textDisplay.text.Length - ColoredText.COLOR_CLOSER.Length;
		private int curPartIndex;
		private int curLetterIndex;

		private float timePerLetter;
		private float letterTimer;

		public void Created()
		{
			textBaseAlpha = textDisplay.color.a;
			iconBaseAlpha = notificationIconDisplay.color.a;
		}

		public void Setup(Notification info)
		{
			this.info = info;

			//Setup starttext
			if (info.parts.Length > 0)
			{
				textDisplay.text = ColoredText.ColorToColorOpener(info.parts[0].textColor) + ColoredText.COLOR_CLOSER;
			}
			else
			{
				textDisplay.text = "";
				doneAddingLetters = true;
			}

			//Setup icon
			if (info.notificationIcon)
			{
				notificationIconDisplay.sprite = info.notificationIcon;
				notificationIconDisplay.color = info.iconColor;
				if (textIsOffset)
				{
					textIsOffset = false;
					textDisplay.rectTransform.anchoredPosition = new Vector2(	textDisplay.rectTransform.anchoredPosition.x + notificationIconDisplay.rectTransform.rect.width,
																				textDisplay.rectTransform.anchoredPosition.y);
				}
			}
			else
			{
				notificationIconDisplay.sprite = null;
				if (!textIsOffset)
				{
					textIsOffset = true;
					textDisplay.rectTransform.anchoredPosition = new Vector2(	textDisplay.rectTransform.anchoredPosition.x - notificationIconDisplay.rectTransform.rect.width,
																				textDisplay.rectTransform.anchoredPosition.y);
				}
			}

			//Reset info
			doneAddingLetters = false;
			curPartIndex = curLetterIndex = 0;
			letterTimer = 0;
			AlphaMultiplier = 1;

			//Calculate how long it takes for one letter to appear
			int letterCount = 0;
			for(int i = 0; i < info.parts.Length; i++)
			{
				letterCount += info.parts[i].text.Length;
			}
			timePerLetter = info.timeToExpand / letterCount;
		}

		private void Update()
		{
			if (!doneAddingLetters)
			{
				letterTimer += Time.unscaledDeltaTime;
				while((letterTimer > timePerLetter) && (!doneAddingLetters))
				{
					letterTimer -= timePerLetter;
					AddLetter();
				}
			}
		}

		private void AddLetter()
		{
			while(curLetterIndex == info.parts[curPartIndex].text.Length)
			{
				curLetterIndex -= info.parts[curPartIndex].text.Length;
				curPartIndex++;

				if(curPartIndex == info.parts.Length)
				{
					doneAddingLetters = true;
					return;
				}

				textDisplay.text += ColoredText.ColorToColorOpener(info.parts[curPartIndex].textColor) + ColoredText.COLOR_CLOSER;
			}
			textDisplay.text = textDisplay.text.Insert(insertIndex, info.parts[curPartIndex].text[curLetterIndex].ToString());
			curLetterIndex++;
		}

		public void ReturnToPool()
		{
			SelfPool.ReturnPoolObject(this);
		}
	}
}