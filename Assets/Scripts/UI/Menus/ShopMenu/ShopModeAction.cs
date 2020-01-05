using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.UI
{
	public class ShopModeAction : MonoBehaviour
	{
		private static ShopModeAction selectedMode;
		public ShopController.ShopMode mode = ShopController.ShopMode.None;
		[SerializeField] private GameObject onSelect = default;
		[SerializeField] private GameObject onNotSelect = default;
		[SerializeField] private GameObject description = default;
		public void Select()
		{
			if (selectedMode)
				selectedMode.DeSelect();

			selectedMode = this;
			onSelect.SetActive(true);
			onNotSelect.SetActive(false);
			description.SetActive(true);
		}
		public void DeSelect()
		{
			if (selectedMode == this)
				selectedMode = null;

			onSelect.SetActive(false);
			onNotSelect.SetActive(true);
			description.SetActive(false);
		}
	}
}