using EoE.Information;
using TMPro;
using UnityEngine;

namespace EoE.UI
{
	public class ItemAction : MonoBehaviour
	{
		public InUIUses actionType;
		[SerializeField] private GameObject onSelect = default;
		[SerializeField] private GameObject onNotSelect = default;
		[SerializeField] private GameObject inActive = default;
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
			onSelect.SetActive(true);
			onNotSelect.SetActive(false);
			inActive.SetActive(false);
		}
		public void DeSelect()
		{
			onSelect.SetActive(false);
			onNotSelect.SetActive(true);
			inActive.SetActive(false);
		}
		public void SetAllowed(bool allowed)
		{
			onSelect.SetActive(false);
			onNotSelect.SetActive(allowed);
			inActive.SetActive(!allowed);
		}
	}
}