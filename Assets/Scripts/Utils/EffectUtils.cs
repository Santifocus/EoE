using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.Utils
{
	public class EffectUtils : MonoBehaviour
	{
		private static EffectUtils instance;
		[SerializeField] private RawImage screenEffectTarget = default;
		[SerializeField] private Transform cameraShakeCore = default;
		[SerializeField] private DamageNumber damageNumberPrefab = default;
		private PoolableObject<DamageNumber> damageNumberPool;
		private void Start()
		{
			if (instance)
				Destroy(instance);

			BaseFixedDeltaTime = Time.fixedDeltaTime;
			ResetBlurEffect();
			damageNumberPool = new PoolableObject<DamageNumber>(50, true, damageNumberPrefab, Storage.ParticleStorage);
			instance = this;
		}
		#region ScreenShake
		private const float DELAY_PER_SHAKE = 0.02f;
		private const float UN_SHAKE_SPEED = 20;
		private static Coroutine ShakeScreenCoroutine = null;
		private static List<ScreenShakeInfo> AllScreenShakes = new List<ScreenShakeInfo>();
		public static void ShakeScreen(float lenght, float axisIntensity, float angleIntensity)
		{
			AllScreenShakes.Add(new ScreenShakeInfo(lenght, axisIntensity, angleIntensity));
			if (ShakeScreenCoroutine == null)
			{
				ShakeScreenCoroutine = instance.StartCoroutine(instance.ShakeScreenC());
			}
		}
		private IEnumerator ShakeScreenC()
		{
			float timeTillNextShake = 0;
			while (true)
			{
				yield return new WaitForEndOfFrame();
				if(AllScreenShakes.Count > 0) //SHAKE
				{
					float strongestAxisIntensity = 0;
					float strongestAngleIntensity = 0;
					for(int i = 0; i < AllScreenShakes.Count; i++)
					{
						AllScreenShakes[i].remainingTime -= Time.deltaTime;
						if(AllScreenShakes[i].remainingTime <= 0)
						{
							AllScreenShakes.RemoveAt(i);
							i--;
						}
						else
						{
							if (strongestAxisIntensity < AllScreenShakes[i].axisIntensity)
								strongestAxisIntensity = AllScreenShakes[i].axisIntensity;

							if (strongestAngleIntensity < AllScreenShakes[i].angleIntensity)
								strongestAngleIntensity = AllScreenShakes[i].angleIntensity;
						}
					}

					timeTillNextShake -= Time.deltaTime;
					if(timeTillNextShake <= 0)
					{
						Shake(strongestAxisIntensity, strongestAngleIntensity);
						timeTillNextShake += DELAY_PER_SHAKE;
					}
				}
				else //UNSHAKE
				{
					cameraShakeCore.localPosition = Vector3.Lerp(cameraShakeCore.localPosition, Vector3.zero, Time.deltaTime * UN_SHAKE_SPEED);
					cameraShakeCore.localEulerAngles = Vector3.Lerp(cameraShakeCore.localEulerAngles, Vector3.zero, Time.deltaTime * UN_SHAKE_SPEED / 4);
					if (cameraShakeCore.localPosition.sqrMagnitude < 0.001f && cameraShakeCore.localEulerAngles.sqrMagnitude < 0.001f)
					{
						cameraShakeCore.localPosition = Vector3.zero;
						cameraShakeCore.localEulerAngles = Vector3.zero;
						break;
					}
				}
			}

			ShakeScreenCoroutine = null;
			void Shake(float axisIntensity, float angleIntensity)
			{
				cameraShakeCore.localPosition = ((Random.value - 0.5f) * cameraShakeCore.transform.right + (Random.value - 0.5f) * cameraShakeCore.transform.up) * axisIntensity;
				cameraShakeCore.localEulerAngles = new Vector3((Random.value - 0.5f) * angleIntensity, (Random.value - 0.5f) * angleIntensity, (Random.value - 0.5f) * angleIntensity);
			}
		}
		private class ScreenShakeInfo
		{
			public float remainingTime;
			public float axisIntensity;
			public float angleIntensity;
			public ScreenShakeInfo(float remainingTime, float axisIntensity, float angleIntensity)
			{
				this.remainingTime = remainingTime;
				this.axisIntensity = axisIntensity;
				this.angleIntensity = angleIntensity;
			}
		}
		#endregion
		#region ColorScreen
		private static List<ColorScreenEffect> AllScreenColorEffects = new List<ColorScreenEffect>();
		private static Coroutine ColorScreenCoroutine;
		public static void ColorScreen(Color col, float lenght, float depth = 0.2f)
		{
			AllScreenColorEffects.Add(new ColorScreenEffect(col, lenght, Mathf.Clamp01(depth * 2) / 2));
			if (ColorScreenCoroutine == null)
			{
				ColorScreenCoroutine = instance.StartCoroutine(instance.ColorScreenC());
			}
		}
		private IEnumerator ColorScreenC()
		{
			while(AllScreenColorEffects.Count > 0)
			{
				yield return new WaitForEndOfFrame();
				float averageDepth = 0;
				Color averageColor = Color.clear;
				for(int i = 0; i < AllScreenColorEffects.Count; i++)
				{
					AllScreenColorEffects[i].passedTime += Time.deltaTime;
					if (AllScreenColorEffects[i].passedTime >= AllScreenColorEffects[i].totalTime)
					{
						AllScreenColorEffects.RemoveAt(i);
						i--;
					}
					else
					{
						averageDepth += AllScreenColorEffects[i].GetDepth();
						averageColor += AllScreenColorEffects[i].col;
					}
				}
				averageDepth /= AllScreenColorEffects.Count;
				averageColor /= AllScreenColorEffects.Count;

				screenEffectTarget.material.SetFloat("_Depth", averageDepth);
				screenEffectTarget.color = averageColor;
			}
			ColorScreenCoroutine = null;
		}
		private class ColorScreenEffect
		{
			public Color col;
			public float totalTime;
			public float depth;
			public float passedTime;
			public ColorScreenEffect(Color col, float totalTime, float depth)
			{
				this.col = col;
				this.totalTime = totalTime;
				this.depth = depth;
			}
			public float GetDepth()
			{
				return Mathf.Max(0, Mathf.Sin((passedTime / totalTime * Mathf.PI)) * depth);
			}
		}
		#endregion
		#region BlurScreen
		private const float BLUR_LERP_SPEED = 8;
		private static List<BlurScreenEffect> AllBlurScreenEffects = new List<BlurScreenEffect>();
		private static Coroutine BlurScreenCoroutine = null;
		public static void BlurScreen(float intensity, float lenght, int blurDistance = 4)
		{
			AllBlurScreenEffects.Add(new BlurScreenEffect(intensity, lenght, blurDistance));
			if(BlurScreenCoroutine == null)
			{
				BlurScreenCoroutine = instance.StartCoroutine(instance.BlurScreenC());
			}
		}
		private IEnumerator BlurScreenC()
		{
			float curBlurIntensity = 0;
			float curBlurDistance = 0;

			while (true)
			{
				yield return new WaitForEndOfFrame();
				if (AllBlurScreenEffects.Count > 0) //BLUR
				{
					float strongestIntensity = 0;
					int strongestBlurDistance = 0;
					for (int i = 0; i < AllBlurScreenEffects.Count; i++)
					{
						AllBlurScreenEffects[i].remainingTime -= Time.deltaTime;
						if (AllBlurScreenEffects[i].remainingTime <= 0)
						{
							AllBlurScreenEffects.RemoveAt(i);
							i--;
						}
						else
						{
							if (AllBlurScreenEffects[i].intensity > strongestIntensity)
								strongestIntensity = AllBlurScreenEffects[i].intensity;

							if (AllBlurScreenEffects[i].blurDistance > strongestBlurDistance)
								strongestBlurDistance = AllBlurScreenEffects[i].blurDistance;
						}
					}

					curBlurIntensity = Mathf.Lerp(curBlurIntensity, strongestIntensity, Time.deltaTime * BLUR_LERP_SPEED);
					curBlurDistance = Mathf.Lerp(curBlurDistance, strongestBlurDistance, Time.deltaTime * BLUR_LERP_SPEED);

					screenEffectTarget.material.SetFloat("_BlurPower", curBlurIntensity);
					screenEffectTarget.material.SetInt("_BlurRange", (int)curBlurDistance);
				}
				else //UNBLUR
				{
					curBlurIntensity = Mathf.Lerp(curBlurIntensity, 0, Time.deltaTime * BLUR_LERP_SPEED / 2);
					curBlurDistance = Mathf.Lerp(curBlurDistance, 0, Time.deltaTime * BLUR_LERP_SPEED / 2);

					screenEffectTarget.material.SetFloat("_BlurPower", curBlurIntensity);
					screenEffectTarget.material.SetInt("_BlurRange", (int)curBlurDistance);

					if (curBlurIntensity < 0.0005f)
						break;
				}
			}
			ResetBlurEffect();
			BlurScreenCoroutine = null;
		}
		private void ResetBlurEffect()
		{
			screenEffectTarget.material.SetFloat("_BlurPower", 0);
			screenEffectTarget.material.SetInt("_BlurRange", 0);
		}
		private class BlurScreenEffect
		{
			public float intensity;
			public float remainingTime;
			public int blurDistance;
			public BlurScreenEffect(float intensity, float remainingTime, int blurDistance)
			{
				this.intensity = intensity;
				this.remainingTime = remainingTime;
				this.blurDistance = blurDistance;
			}
		}
		#endregion
		#region SlowDown
		public static int HighestTimeDilutionDominanceIndex;
		private static float BaseFixedDeltaTime;
		private static List<TimeDilationEffect> AllTimeDilationsEffects = new List<TimeDilationEffect>();
		private static Coroutine TimeDilationCoroutine = null;
		public static float TimeDilation(int dominanceIndex, float targetSpeed, float timeIn, float timeStay, bool deleteOtherDilations = false, float? timeOut = null)
		{
			if (deleteOtherDilations)
			{
				AllTimeDilationsEffects = new List<TimeDilationEffect>();
			}
			AllTimeDilationsEffects.Add(new TimeDilationEffect(dominanceIndex, Mathf.Max(0.001f, targetSpeed), timeIn, timeStay, timeOut));
			float totalTime = AllTimeDilationsEffects[AllTimeDilationsEffects.Count - 1].totalTime;
			if (TimeDilationCoroutine == null)
			{
				TimeDilationCoroutine = instance.StartCoroutine(instance.TimeDilationC());
			}
			return totalTime;
		}
		private IEnumerator TimeDilationC()
		{
			while(AllTimeDilationsEffects.Count > 0)
			{
				yield return new WaitForEndOfFrame();
				if (Time.timeScale == 0) //Game is paused
					continue;

				HighestTimeDilutionDominanceIndex = -1;
				int targetIndex = 0;
				for(int i = 0; i < AllTimeDilationsEffects.Count; i++)
				{
					AllTimeDilationsEffects[i].passedTime += Time.unscaledDeltaTime;
					if(AllTimeDilationsEffects[i].passedTime > AllTimeDilationsEffects[i].totalTime)
					{
						AllTimeDilationsEffects.RemoveAt(i);
						i--;
					}
					else
					{
						if(HighestTimeDilutionDominanceIndex < AllTimeDilationsEffects[i].dominanceIndex)
						{
							HighestTimeDilutionDominanceIndex = AllTimeDilationsEffects[i].dominanceIndex;
							targetIndex = i;
						}
					}
				}
				if (AllTimeDilationsEffects.Count == 0)
					continue;

				float targetTimeScale = AllTimeDilationsEffects[targetIndex].GetTimeScale();
				Time.timeScale = targetTimeScale;
				Time.fixedDeltaTime = BaseFixedDeltaTime * targetTimeScale;
			}
			Time.timeScale = 1;
			Time.fixedDeltaTime = BaseFixedDeltaTime;

			TimeDilationCoroutine = null;
		}
		private class TimeDilationEffect
		{
			public int dominanceIndex;
			public float targetSpeed;
			public float timeIn;
			public float timeStay;
			public float timeOut;
			public float totalTime => timeIn + timeStay + timeOut;

			public float passedTime;
			private float scaleDif;

			public TimeDilationEffect(int dominanceIndex, float targetSpeed, float timeIn, float timeStay, float? timeOut)
			{
				this.dominanceIndex = dominanceIndex;
				this.targetSpeed = targetSpeed;
				this.timeIn = timeIn;
				this.timeStay = timeStay;
				this.timeOut = timeOut.HasValue ? timeOut.Value : timeIn;
				passedTime = 0;
				scaleDif = targetSpeed - 1;
			}
			public float GetTimeScale()
			{
				if(passedTime < timeIn)
				{
					return 1 + (passedTime / timeIn) * scaleDif;
				}
				else if(passedTime >= timeIn && passedTime < (timeStay + timeIn))
				{
					return targetSpeed;
				}
				else //passedTime >= timeStay + timeIn
				{
					float fractTime = passedTime - (timeIn + timeStay);
					return 1 + (1 - fractTime / timeOut) * scaleDif;
				}
			}
		}
		#endregion
		public static void WeaponHitEntitie(Vector3 hitPos)
		{

		}
		public static void WeaponHitTerrain(Vector3 hitPos)
		{

		}
		public static void CreateDamageNumber(Vector3 startPosition, Gradient colors, Vector3 numberVelocity, float damage, bool wasCrit, float overrideScale = 1)
		{
			DamageNumber newDamageNumber = instance.damageNumberPool.GetPoolObject();
			newDamageNumber.transform.position = startPosition;
			newDamageNumber.transform.localScale = Vector3.one * overrideScale;
			string displayedNumber = (Mathf.Round(damage * 100) / 100).ToString();
			newDamageNumber.BeginDisplay(numberVelocity, colors, displayedNumber, wasCrit);
		}

		public static void DisplayInfoText(Vector3 startPosition, Gradient colors, Vector3 numberVelocity, string text, float overrideScale = 1)
		{
			DamageNumber newDamageNumber = instance.damageNumberPool.GetPoolObject();
			newDamageNumber.transform.position = startPosition;
			newDamageNumber.transform.localScale = Vector3.one * overrideScale;
			newDamageNumber.BeginDisplay(numberVelocity, colors, text, false);
		}
#if UNITY_EDITOR
		private void OnApplicationQuit()
		{
			ResetBlurEffect();
		}
#endif
	}
}