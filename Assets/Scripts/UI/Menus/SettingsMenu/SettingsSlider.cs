using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EoE.UI
{
	public class SettingsSlider : MonoBehaviour
	{
		[SerializeField] private RectTransform handle = default;
		[SerializeField] private TextMeshProUGUI valueDisplay = default;
		[SerializeField] private float startX = default;
		[SerializeField] private float endX = default;

		private float value;
		private float curTargetX;
		public float Value
		{
			get => this.value;
			set
			{
				this.value = Mathf.Clamp01(value);
				float xPos = (endX - startX) * this.value;
				curTargetX = startX + xPos;
				if (valueDisplay)
					valueDisplay.text = (this.value).ToString();
			}
		}
		private void Update()
		{
			handle.anchoredPosition = new Vector2(Mathf.Lerp(handle.anchoredPosition.x, curTargetX, Time.unscaledDeltaTime * 10), handle.anchoredPosition.y);
		}
		public void SetValueNoLerp(float value)
		{
			Value = value;
			handle.anchoredPosition = new Vector2(curTargetX, handle.anchoredPosition.y);
		}
	}
}