using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Utils
{
	public static class BaseUtils
	{
		public static bool ChancePercent(float chance) => Chance01(chance / 100);
		public static bool Chance01(float chance)
		{
			return Random.value < chance;
		}
	}
}