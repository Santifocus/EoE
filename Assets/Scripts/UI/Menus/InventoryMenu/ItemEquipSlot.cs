using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class ItemEquipSlot : MonoBehaviour
	{
		[SerializeField] private Image selectedBG = default;
		[SerializeField] private Image notSelectedBG = default;
		[SerializeField] private Image selectedItemDisplay = default;
		[SerializeField] private Image notSelectedItemDisplay = default;

		public Sprite sprite
		{
			get => selectedItemDisplay.sprite;
			set
			{
				selectedItemDisplay.sprite = notSelectedItemDisplay.sprite = value;
			}
		}
		public Color color
		{
			get => selectedItemDisplay.color;
			set
			{
				selectedItemDisplay.color = notSelectedItemDisplay.color = value;
			}
		}
		public void Select()
		{
			selectedBG.gameObject.SetActive(true);
			notSelectedBG.gameObject.SetActive(false);
		}
		public void DeSelect()
		{
			selectedBG.gameObject.SetActive(false);
			notSelectedBG.gameObject.SetActive(true);
		}
	}
}