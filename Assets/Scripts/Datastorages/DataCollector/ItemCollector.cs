using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class ItemCollector : DataCollector<Item>
	{
		protected override List<Item> SortData(List<Item> input)
		{
			input.Sort((x, y) => x.SortingID.CompareTo(y.SortingID));
			return base.SortData(input);
		}
	}
}