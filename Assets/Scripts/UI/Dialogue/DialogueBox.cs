using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class DialogueBox : MonoBehaviour
	{
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
	}
}