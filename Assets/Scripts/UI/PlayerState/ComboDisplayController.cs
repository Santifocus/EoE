using EoE.Combatery;
using TMPro;
using UnityEngine;

namespace EoE.UI
{
	public class ComboDisplayController : MonoBehaviour
	{
		public static ComboDisplayController Instance { get; private set; }
		[SerializeField] private TextMeshProUGUI comboDisplay = default;
		private float baseTextSize;

		private int displayedCombo;

		private Gradient curTextColor;
		private float curTextColorScrollSpeed;
		private float curPunchPower;
		private float curPunchResetTime;

		private float punchTime;
		private float textColorScroll;

		private void Start()
		{
			Instance = this;
			baseTextSize = comboDisplay.fontSize;
			displayedCombo = 0;
			HideComboDisplay();
		}
		public void ResetCombo(ComboSet settings)
		{
			curTextColor = settings.StandardTextColor;
			curTextColorScrollSpeed = settings.ColorScrollSpeed;
			curPunchPower = settings.TextPunch;
			curPunchResetTime = settings.PunchResetSpeed;

			displayedCombo = 0;
			HideComboDisplay();
		}
		public void StopCombo()
		{
			displayedCombo = 0;
			HideComboDisplay();
		}
		private void Update()
		{
			if (displayedCombo == 0)
			{
				HideComboDisplay();
				return;
			}

			//Update punch time
			if (punchTime > 0)
			{
				punchTime -= Time.deltaTime;
				if (punchTime < 0)
					punchTime = 0;
			}

			//Update textcolorscroll
			textColorScroll += Time.deltaTime * curTextColorScrollSpeed;
			while (textColorScroll > 1)
				textColorScroll--;

			UpdateText();
		}
		private void HideComboDisplay()
		{
			if (comboDisplay.gameObject.activeInHierarchy)
				comboDisplay.gameObject.SetActive(false);

			if (punchTime > 0)
				punchTime = 0;

			if (textColorScroll > 0)
				textColorScroll = 0;
		}
		private void UpdateText()
		{
			comboDisplay.color = curTextColor.Evaluate(textColorScroll);
			if (curPunchResetTime == 0)
			{
				comboDisplay.fontSize = baseTextSize;
			}
			else
			{
				comboDisplay.fontSize = baseTextSize + Mathf.LerpUnclamped(0, curPunchPower, punchTime / curPunchResetTime);
			}
		}
		public void SetCombo(int amount)
		{
			if (!comboDisplay.gameObject.activeInHierarchy)
				comboDisplay.gameObject.SetActive(true);

			punchTime = curPunchResetTime;
			displayedCombo = amount;
			comboDisplay.text = displayedCombo.ToString()+ "<size=" + (comboDisplay.fontSize * 0.5f) + "> Hit</size>";
		}
		public void OverrideColorSettings(Gradient textColor, float textColorScrollSpeed)
		{
			curTextColor = textColor;
			curTextColorScrollSpeed = textColorScrollSpeed;
		}
		public void OverridePunchSettings(float punchPower, float punchResetTime)
		{
			curPunchPower = punchPower;
			curPunchResetTime = punchResetTime;
		}
		private void OnDestroy()
		{
			Instance = null;
		}
	}
}