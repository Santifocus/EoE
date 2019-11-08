using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class InventoryMenu : CharacterMenuPage
	{
		[SerializeField] private InventorySlot[] inventorySlots = default;
		protected override void ResetPage()
		{

		}
	}
}