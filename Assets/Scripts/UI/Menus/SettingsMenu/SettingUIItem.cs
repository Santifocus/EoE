using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.UI
{
	public enum SettingType { MusicVolume = 1, SoundVolume = 2, Gamma = 3, XInvert = 4, YInvert = 5, Apply = 6, Cancel = 7, Reset = 8 }
	public class SettingUIItem : MonoBehaviour
	{
		private SettingUIItem SelectedItem;
		[SerializeField] private GameObject onNotSelect = default;
		[SerializeField] private GameObject onSelect = default;
		public SettingType Setting = SettingType.MusicVolume;
		public bool isAction => ((int)Setting >= (int)SettingType.Apply);
		public bool isSlider => (((int)Setting >= (int)SettingType.MusicVolume) && ((int)Setting <= (int)SettingType.Gamma));
		public void Select()
		{
			if (SelectedItem)
				SelectedItem.DeSelect();
			SelectedItem = this;

			onSelect.SetActive(true);
			onNotSelect.SetActive(false);
		}
		public void DeSelect()
		{
			if (SelectedItem == this)
				SelectedItem = null;

			onSelect.SetActive(false);
			onNotSelect.SetActive(true);
		}
	}
}