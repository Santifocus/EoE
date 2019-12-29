using EoE.Information;
using TMPro;
using UnityEngine;

namespace EoE.UI
{
	public class ItemDescriptionDisplay : MonoBehaviour
	{
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

			itemNameText.text = target.ItemName.ToString();
			itemDescriptionText.text = ColoredText.ToString(target.ItemDescription);
		}
	}
}