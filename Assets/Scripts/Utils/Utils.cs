﻿using UnityEngine;
using UnityEngine.UI;

namespace EoE
{
	public static class Utils
	{
		public static readonly float RootOfTwo = Mathf.Sqrt(2);
		public static bool ChancePercent(float chance) => Chance01(chance / 100);
		public static bool Chance01(float chance)
		{
			return Random.value <= chance;
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
		public static float SpringLerp(float curValue, float targetValue, ref float acceleration, float springStiffness, float lerpStep)
		{
			acceleration = Mathf.Lerp(acceleration, (targetValue - curValue) * springStiffness, lerpStep);
			return curValue + acceleration * lerpStep;
		}
		public static float[] FetchAlphaValues(Graphic[] graphics, bool? setState = null)
		{
			float[] alphaValues = new float[graphics.Length];
			for (int i = 0; i < graphics.Length; i++)
			{
				alphaValues[i] = graphics[i].color.a;
				graphics[i].color = new Color(graphics[i].color.r, graphics[i].color.g, graphics[i].color.b, 0);
				if(setState.HasValue)
					graphics[i].gameObject.SetActive(setState.Value);
			}
			return alphaValues;
		}
	}
}