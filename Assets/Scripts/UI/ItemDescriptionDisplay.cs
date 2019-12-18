using EoE.Information;
using TMPro;
using UnityEngine;

namespace EoE.UI
{
	public class ItemDescriptionDisplay : MonoBehaviour
	{
		private const string COLOR_CLOSER = "</color>";
		[SerializeField] private TextMeshProUGUI itemNameText = default;
		[SerializeField] private TextMeshProUGUI itemDescriptionText = default;
		public void SetItem(Item target)
		{
			if (target == null)
			{
				itemNameText.text = "";
				itemDescriptionText.text = "";
				return;
			}

			itemNameText.text = ColorToColorOpener(target.ItemName.textColor) + target.ItemName.text + COLOR_CLOSER;

			string descriptionText = "";
			for (int i = 0; i < target.ItemDescription.Length; i++)
			{
				descriptionText += target.ItemDescription[i];
			}
			itemDescriptionText.text = descriptionText;
		}
		private string ColorToColorOpener(Color col)
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(col) + ">";
		}
	}
}