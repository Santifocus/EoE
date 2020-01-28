using EoE.Information;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class DialogueBox : MonoBehaviour
	{
		[SerializeField] private RectTransform boxRect = default;
		[SerializeField] private float minHeight = 150;
		[SerializeField] private Image iconDisplay = default;
		[SerializeField] private TextMeshProUGUI textWithIcon = default;
		[SerializeField] private TextMeshProUGUI textWithoutIcon = default;
		public TextMeshProUGUI TextDisplay { get => iconDisplay.sprite ? textWithIcon : textWithoutIcon; }
		public Sprite icon 
		{ 
			get => iconDisplay.sprite;
			set
			{
				iconDisplay.sprite = value;
				iconDisplay.gameObject.SetActive(value);
			}
		}
		public void CalculateBoxSize(ColoredText[] textData)
		{
			string totalText = ColoredText.ToString(textData);
			TextDisplay.text = totalText;
			TextDisplay.ForceMeshUpdate();

			float marginHeight = (TextDisplay.margin.y + TextDisplay.margin.w);
			boxRect.sizeDelta = new Vector2(boxRect.sizeDelta.x, Mathf.Max(minHeight - marginHeight, TextDisplay.textBounds.size.y) + marginHeight);
			TextDisplay.text = "";
		}
	}
}