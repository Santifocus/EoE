using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Utils
{
	public static class BaseUtils
	{
		public static readonly float RootOfTwo = Mathf.Sqrt(2);
		public static bool ChancePercent(float chance) => Chance01(chance / 100);
		public static bool Chance01(float chance)
		{
			return Random.value < chance;
		}
		public static float Polarize01(float value, int iterations = 1, bool smooth = false, bool noClamp = false)
		{
			float pre = value;
			value += 0.5f;
			value *= value;
			if (!noClamp)
				value = Mathf.Clamp01(value - 0.5f);
			else
				value -= 0.5f;

			if (smooth)
				value = (value + pre) / 2;

			iterations--;
			return iterations == 0 ? value : Polarize01(value, iterations);
		}
		public static bool DecreaseCooldown(ref float cooldown)
		{
			cooldown -= Time.deltaTime;
			return cooldown <= 0;
		}
		public static bool DecreaseFixedCooldown(ref float cooldown)
		{
			cooldown -= Time.fixedDeltaTime;
			return cooldown <= 0;
		}
	}
}