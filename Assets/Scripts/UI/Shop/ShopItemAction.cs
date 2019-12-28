using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EoE.UI
{
	public class ShopItemAction : MonoBehaviour
	{
		private static ShopItemAction selectedItemAction;
		public bool isDoAction = false;
		[SerializeField] private GameObject onSelect = default;
		[SerializeField] private GameObject onNotSelect = default;
		[SerializeField] private TextMeshProUGUI[] actionDisplay = default;
		public string displayText
		{
			get => actionDisplay.Length > 0 ? actionDisplay[0].text : "";
			set
			{
				for (int i = 0; i < actionDisplay.Length; i++)
				{
					actionDisplay[i].text = value;
				}
			}
		}
		public void Select()
		{
			if (selectedItemAction)
				selectedItemAction.DeSelect();
			onSelect.SetActive(true);
			onNotSelect.SetActive(false);
		}
		private void DeSelect()
		{
			onSelect.SetActive(false);
			onNotSelect.SetActive(true);
		}
	}
}