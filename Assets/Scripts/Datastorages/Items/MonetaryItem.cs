using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class MonetaryItem : Item
	{
		public override InUIUses Uses => InUIUses.Drop | InUIUses.Back;
	}
}