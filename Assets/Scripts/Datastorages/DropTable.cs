using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class DropTable : ScriptableObject
	{
		public ItemDrop[] PossibleDrops;
		[System.Serializable]
		public class ItemDrop
		{
			public GameObject Drop = null;
			public int MinDropAmount = 1;
			public int MaxDropAmount = 1;
			public float DropChance = 1;
		}
	}
}