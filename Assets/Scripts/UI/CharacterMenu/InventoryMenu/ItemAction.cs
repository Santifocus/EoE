using UnityEngine;

namespace EoE.UI
{
	public class ItemAction : CMenuItem
	{
		public enum ItemActionType { Use, Equip, Drop, Back }
		public ItemActionType actionType;
		[SerializeField] private GameObject onSelect = default;
		[SerializeField] private GameObject onNotSelect = default;
		protected override void Select()
		{
			onSelect.SetActive(true);
			onNotSelect.SetActive(false);
		}
		protected override void DeSelect()
		{
			onSelect.SetActive(false);
			onNotSelect.SetActive(true);
		}
	}
}