﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EoE.Information;
using EoE.Sounds;
using EoE.Entities;

namespace EoE.Utils
{
	public class EffectUtils : MonoBehaviour
	{
		#region Fields
		private static EffectUtils Instance;
		[SerializeField] private Material screenEffectMaterial = default;
		[SerializeField] private Transform cameraShakeCore = default;
		[SerializeField] private DamageNumber damageNumberPrefab = default;
		private PoolableObject<DamageNumber> damageNumberPool;
		#endregion
		#region Setups
		private void Start()
		{
			if (Instance)
				Destroy(Instance);

			BaseFixedDeltaTime = Time.fixedDeltaTime;
			ResetScreenEffectMat();
			damageNumberPool = new PoolableObject<DamageNumber>(50, true, damageNumberPrefab, Storage.ParticleStorage);
			Instance = this;
		}
		public static void ResetScreenEffects()
		{
			Instance.StopAllCoroutines();
			AllScreenShakes = new List<ScreenShakeInfo>();
			AllRumbles = new List<RumbleInfo>();
			AllScreenColorEffects = new List<ColorScreenEffect>();
			AllBlurScreenEffects = new List<BlurScreenEffect>();
			AllTimeDilationsEffects = new List<TimeDilationEffect>();

			Instance.ResetScreenEffectMat();
			Gamepad.current.SetMotorSpeeds(0, 0);
		}
		private void ResetScreenEffectMat()
		{
			screenEffectMaterial.SetFloat("_BorderDepth", 0);
			screenEffectMaterial.SetColor("_Color", Color.clear);

			screenEffectMaterial.SetFloat("_BlurPower", 0);
			screenEffectMaterial.SetInt("_BlurRange", 0);

			screenEffectMaterial.SetColor("_TintColor", Color.clear);
			screenEffectMaterial.SetFloat("_TintStrenght", 0);
		}
		private void OnApplicationQuit()
		{
			Gamepad.current?.SetMotorSpeeds(0, 0);
#if UNITY_EDITOR
			ResetScreenEffectMat();
#endif
		}
		#endregion
		#region ScreenShake
		private const float DELAY_PER_SHAKE = 0.01f;
		private const float UN_SHAKE_SPEED = 20;
		private static Coroutine ShakeScreenCoroutine = null;
		private static List<ScreenShakeInfo> AllScreenShakes = new List<ScreenShakeInfo>();
		public static void ShakeScreen(float lenght, float axisIntensity, float angleIntensity, Vector3? customAxisMultiplier = null, Vector3? customAngleMultiplier = null)
		{
			AllScreenShakes.Add(new ScreenShakeInfo(
				lenght,
				axisIntensity,
				angleIntensity,
				axisIntensity * (customAxisMultiplier.HasValue ? customAxisMultiplier.Value : Vector3.one),
				angleIntensity * (customAngleMultiplier.HasValue ? customAngleMultiplier.Value : Vector3.one)));

			if (ShakeScreenCoroutine == null)
			{
				ShakeScreenCoroutine = Instance.StartCoroutine(Instance.ShakeScreenC());
			}
		}
		public static void ShakeScreen(ScreenShake info, float multiplier = 1)
		{
			ShakeScreen(info.Time, info.AxisIntensity * multiplier, info.AngleIntensity * multiplier, info.CustomAxisMultiplier, info.CustomAngleMultiplier);
		}
		private IEnumerator ShakeScreenC()
		{
			float timeTillNextShake = 0;
			while (true)
			{
				yield return new WaitForEndOfFrame();
				if (GameController.GameIsPaused)
						continue;

				if (AllScreenShakes.Count > 0) //SHAKE
				{

					float strongestAxisIntensity = 0;
					float strongestAngleIntensity = 0;
					for (int i = 0; i < AllScreenShakes.Count; i++)
					{
						AllScreenShakes[i].remainingTime -= Time.deltaTime;
						if (AllScreenShakes[i].remainingTime <= 0)
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
					if (timeTillNextShake <= 0)
					{
						Shake(strongestAxisIntensity, strongestAngleIntensity);
						timeTillNextShake += DELAY_PER_SHAKE;
					}
				}
				else //UNSHAKE
				{
					float timeStep = Time.deltaTime * UN_SHAKE_SPEED;
					cameraShakeCore.localPosition = Vector3.Lerp(cameraShakeCore.localPosition, Vector3.zero, timeStep);
					cameraShakeCore.localEulerAngles = new Vector3(Mathf.LerpAngle(cameraShakeCore.localEulerAngles.x, 0, timeStep / 4),
						Mathf.LerpAngle(cameraShakeCore.localEulerAngles.y, 0, timeStep / 4),
						Mathf.LerpAngle(cameraShakeCore.localEulerAngles.z, 0, timeStep / 4));

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
			public Vector3 axisIntensityVector;
			public Vector3 angleIntensityVector;
			public ScreenShakeInfo(float remainingTime, float axisIntensity, float angleIntensity, Vector3 axisIntensityVector, Vector3 angleIntensityVector)
			{
				this.remainingTime = remainingTime;

				this.axisIntensity = axisIntensity;
				this.angleIntensity = angleIntensity;

				this.axisIntensityVector = axisIntensityVector;
				this.angleIntensityVector = angleIntensityVector;
			}
		}
		#endregion
		#region Rumble
		private static List<RumbleInfo> AllRumbles = new List<RumbleInfo>();
		private static Coroutine RumbleCoroutine = null;
		public static void RumbleController(float totalTime, float leftMotorIntensityStart, float rightMotorIntensityStart, float? leftMotorIntensityEnd = null, float? rightMotorIntensityEnd = null)
		{
			AllRumbles.Add(new RumbleInfo(
				totalTime,
				leftMotorIntensityStart,
				rightMotorIntensityStart,
				leftMotorIntensityEnd,
				rightMotorIntensityEnd));

			if (RumbleCoroutine == null)
			{
				RumbleCoroutine = Instance.StartCoroutine(Instance.RumbleC());
			}
		}
		public static void RumbleController(ControllerRumble info, float multiplier = 1)
		{
			RumbleController(info.Time, info.LeftStartIntensity * multiplier, info.RightStartIntensity * multiplier, info.LeftEndIntensity * multiplier, info.RightEndIntensity * multiplier);
		}
		private IEnumerator RumbleC()
		{
			while (AllRumbles.Count > 0)
			{
				yield return new WaitForEndOfFrame();
				if (GameController.GameIsPaused)
				{
					Gamepad.current.SetMotorSpeeds(0, 0);
					continue;
				}

				float highestLeftIntensity = 0;
				float highestRightIntensity = 0;

				//Update all remaining times and get the highest intensitys
				for (int i = 0; i < AllRumbles.Count; i++)
				{
					AllRumbles[i].remainingTime -= Time.deltaTime;
					(float left, float right) = AllRumbles[i].GetRumbleIntensitys();

					if (AllRumbles[i].remainingTime <= 0)
					{
						AllRumbles.RemoveAt(i);
						i--;
					}
					else
					{
						if (left > highestLeftIntensity)
							highestLeftIntensity = left;

						if (right > highestRightIntensity)
							highestRightIntensity = right;
					}
				}

				//Rumble
				if (Gamepad.current != null)
				{
					Gamepad.current.SetMotorSpeeds(highestLeftIntensity, highestRightIntensity);
				}
			}

			//Reset
			if (Gamepad.current != null)
			{
				Gamepad.current.SetMotorSpeeds(0, 0);
			}
			RumbleCoroutine = null;
		}

		private class RumbleInfo
		{
			public float totalTime;
			public float remainingTime;

			public float leftMotorIntensityStart;
			public float rightMotorIntensityStart;

			public float leftMotorIntensityEnd;
			public float rightMotorIntensityEnd;

			public RumbleInfo(float totalTime, float leftMotorIntensityStart, float rightMotorIntensityStart, float? leftMotorIntensityEnd = null, float? rightMotorIntensityEnd = null)
			{
				this.totalTime = totalTime;
				this.remainingTime = totalTime;

				this.leftMotorIntensityStart = leftMotorIntensityStart;
				this.rightMotorIntensityStart = rightMotorIntensityStart;
				this.leftMotorIntensityEnd = leftMotorIntensityEnd ?? leftMotorIntensityStart;
				this.rightMotorIntensityEnd = rightMotorIntensityEnd ?? rightMotorIntensityStart;

				this.leftMotorIntensityStart = Mathf.Clamp01(this.leftMotorIntensityStart);
				this.rightMotorIntensityStart = Mathf.Clamp01(this.rightMotorIntensityStart);
				this.leftMotorIntensityEnd = Mathf.Clamp01(this.leftMotorIntensityEnd);
				this.rightMotorIntensityEnd = Mathf.Clamp01(this.rightMotorIntensityEnd);
			}
			public (float, float) GetRumbleIntensitys()
			{
				//Gradient of 1 to 0
				float point = remainingTime / totalTime;
				float left = Mathf.LerpUnclamped(leftMotorIntensityEnd, leftMotorIntensityStart, point);
				float right = Mathf.LerpUnclamped(rightMotorIntensityEnd, rightMotorIntensityStart, point);

				return (left, right);
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
				ColorScreenCoroutine = Instance.StartCoroutine(Instance.ColorScreenC());
			}
		}
		public static void ColorScreenBorder(ScreenBorderColor info, float multiplier = 1)
		{
			ColorScreen(info.Color, info.Time, info.Depth * multiplier);
		}
		private IEnumerator ColorScreenC()
		{
			while (true)
			{
				yield return new WaitForEndOfFrame();
				if (GameController.GameIsPaused)
					continue;

				float averageDepth = 0;
				Color averageColor = Color.clear;
				for (int i = 0; i < AllScreenColorEffects.Count; i++)
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
				if (AllScreenColorEffects.Count == 0)
					break;

				averageDepth /= AllScreenColorEffects.Count;
				averageColor /= AllScreenColorEffects.Count;

				screenEffectMaterial.SetFloat("_BorderDepth", averageDepth);
				screenEffectMaterial.SetColor("_Color", averageColor);
			}

			screenEffectMaterial.SetFloat("_BorderDepth", 0);
			screenEffectMaterial.SetColor("_Color", Color.clear);
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
			if (BlurScreenCoroutine == null)
			{
				BlurScreenCoroutine = Instance.StartCoroutine(Instance.BlurScreenC());
			}
		}
		public static void BlurScreen(ScreenBlur info, float multiplier = 1)
		{
			BlurScreen(info.Intensity * multiplier, info.Time, info.BlurDistance);
		}
		private IEnumerator BlurScreenC()
		{
			float curBlurIntensity = 0;
			float curBlurDistance = 0;

			while (true)
			{
				yield return new WaitForEndOfFrame();
				if (GameController.GameIsPaused)
					continue;

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

					screenEffectMaterial.SetFloat("_BlurPower", curBlurIntensity);
					screenEffectMaterial.SetInt("_BlurRange", (int)curBlurDistance);
				}
				else //UNBLUR
				{
					curBlurIntensity = Mathf.Lerp(curBlurIntensity, 0, Time.deltaTime * BLUR_LERP_SPEED / 2);
					curBlurDistance = Mathf.Lerp(curBlurDistance, 0, Time.deltaTime * BLUR_LERP_SPEED / 2);

					screenEffectMaterial.SetFloat("_BlurPower", curBlurIntensity);
					screenEffectMaterial.SetInt("_BlurRange", (int)curBlurDistance);

					if (curBlurIntensity < 0.0005f)
						break;
				}
			}
			screenEffectMaterial.SetFloat("_BlurPower", 0);
			screenEffectMaterial.SetInt("_BlurRange", 0);
			BlurScreenCoroutine = null;
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
		#region ScreenTint
		private static List<TintScreenEffect> AllTintScreenEffects = new List<TintScreenEffect>();
		private static Coroutine TintScreenCoroutine = null;
		public static void TintScreen(Color tintColor, float timeIn, float timeStay, float timeOut, int dominance = 1)
		{
			AllTintScreenEffects.Add(new TintScreenEffect(tintColor, timeIn, timeStay, timeOut, dominance));
			if (TintScreenCoroutine == null)
			{
				TintScreenCoroutine = Instance.StartCoroutine(Instance.TintScreenC());
			}
		}
		public static void TintScreen(ScreenTint info, float multiplier = 1)
		{
			uint Dominance = System.Math.Min(info.Dominance, int.MaxValue);
			Color tint = info.TintColor;
			tint.a *= multiplier;
			TintScreen(tint, info.TimeIn, info.TimeStay, info.TimeOut, (int)Dominance);
		}
		private IEnumerator TintScreenC()
		{
			while (AllTintScreenEffects.Count > 0)
			{
				yield return new WaitForEndOfFrame();
				if (GameController.GameIsPaused)
					continue;

				//First get the total dominance level, and add passed time
				int dominanceCount = 0;
				for(int i = 0; i < AllTintScreenEffects.Count; i++)
				{
					AllTintScreenEffects[i].passedTime += Time.deltaTime;
					if(AllTintScreenEffects[i].passedTime > AllTintScreenEffects[i].totalTime)
					{
						AllTintScreenEffects.RemoveAt(i);
						i--;
					}
					else
					{
						dominanceCount += AllTintScreenEffects[i].dominance;
					}
				}

				//Better be safe then sorry
				if (dominanceCount < 1)
					continue;

				//Now use the total dominance level to lerp a color between all tints
				Color lerpedColor = Color.clear;
				for (int i = 0; i < AllTintScreenEffects.Count; i++)
				{
					lerpedColor += AllTintScreenEffects[i].GetCurrentColor() * ((float)AllTintScreenEffects[i].dominance / dominanceCount);
				}

				//Finally set the material color tint
				screenEffectMaterial.SetColor("_TintColor", lerpedColor);
			}

			screenEffectMaterial.SetColor("_TintColor", Color.clear);
			TintScreenCoroutine = null;
		}
		private class TintScreenEffect
		{
			public Color tintColor;
			public float timeIn;
			public float timeStay;
			public float timeOut;
			public int dominance;

			public float passedTime;
			public float totalTime => timeIn + timeStay + timeOut;

			public TintScreenEffect(Color tintColor, float timeIn, float timeStay, float timeOut, int dominance = 1)
			{
				this.tintColor = tintColor;
				this.timeIn = timeIn;
				this.timeStay = timeStay;
				this.timeOut = timeOut;
				this.dominance = dominance;
			}
			public Color GetCurrentColor()
			{
				Color bCol = new Color(tintColor.r, tintColor.g, tintColor.b, 0);
				Color bAlpha = new Color(0,0,0, tintColor.a);
				if(passedTime < timeIn)
				{
					return bCol + (bAlpha * passedTime / timeIn);
				}
				else if(passedTime < timeIn + timeStay)
				{
					return bCol + bAlpha;
				}
				else
				{
					return bCol + (bAlpha * (1-(passedTime - timeIn - timeStay) / timeOut));
				}
			}
		}
		#endregion
		#region TimeDilation
		public static int HighestTimeDilutionDominanceIndex;
		private static float BaseFixedDeltaTime;
		private static List<TimeDilationEffect> AllTimeDilationsEffects = new List<TimeDilationEffect>();
		private static Coroutine TimeDilationCoroutine = null;
		public static void DilateTime(int dominanceIndex, float targetSpeed, float timeIn, float timeStay, bool deleteOtherDilations = false, float? timeOut = null)
		{
			if (deleteOtherDilations)
			{
				AllTimeDilationsEffects = new List<TimeDilationEffect>();
			}
			AllTimeDilationsEffects.Add(new TimeDilationEffect(dominanceIndex, Mathf.Max(0.001f, targetSpeed), timeIn, timeStay, timeOut));
			if (TimeDilationCoroutine == null)
			{
				TimeDilationCoroutine = Instance.StartCoroutine(Instance.DilateTimeC());
			}
		}
		public static void DilateTime(TimeDilation info, float multiplier = 1)
		{
			uint Dominance = System.Math.Min(info.Dominance, int.MaxValue);
			float scale = 1 - ((1 - info.Scale) * multiplier);
			DilateTime((int)Dominance, info.Scale, info.TimeIn, info.TimeStay, info.OverwriteOtherTimeDilations, info.TimeOut);
		}
		private IEnumerator DilateTimeC()
		{
			while (AllTimeDilationsEffects.Count > 0)
			{
				yield return new WaitForEndOfFrame();
				if (GameController.GameIsPaused)
					continue;

				HighestTimeDilutionDominanceIndex = -1;
				int targetIndex = 0;
				for (int i = 0; i < AllTimeDilationsEffects.Count; i++)
				{
					AllTimeDilationsEffects[i].passedTime += Time.unscaledDeltaTime;
					if (AllTimeDilationsEffects[i].passedTime > AllTimeDilationsEffects[i].totalTime)
					{
						AllTimeDilationsEffects.RemoveAt(i);
						i--;
					}
					else
					{
						if (HighestTimeDilutionDominanceIndex < AllTimeDilationsEffects[i].dominanceIndex)
						{
							HighestTimeDilutionDominanceIndex = AllTimeDilationsEffects[i].dominanceIndex;
							targetIndex = i;
						}
					}
				}
				if (AllTimeDilationsEffects.Count == 0)
					break;

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
				if (passedTime < timeIn)
				{
					return 1 + (passedTime / timeIn) * scaleDif;
				}
				else if (passedTime >= timeIn && passedTime < (timeStay + timeIn))
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
		#region CameraFOVWarp
		public static int HighestCameraFOVWarpDominanceIndex;
		private static List<CameraFOVWarpEffect> AllCameraFOVWarpEffects = new List<CameraFOVWarpEffect>();
		private static Coroutine CameraFOVWarpCoroutine = null;
		private const float LERP_FOV_SPEED = 10;
		public static void WarpCameraFOV(int dominanceIndex, float targetFOV, float timeIn, float timeStay, float timeOut, bool overwriteOtherFOVWarps)
		{
			if (overwriteOtherFOVWarps)
			{
				AllCameraFOVWarpEffects = new List<CameraFOVWarpEffect>();
			}
			AllCameraFOVWarpEffects.Add(new CameraFOVWarpEffect(dominanceIndex, targetFOV, timeIn, timeStay, timeOut));
			if (CameraFOVWarpCoroutine == null)
			{
				CameraFOVWarpCoroutine = Instance.StartCoroutine(Instance.WarpCameraFOVC());
			}
		}
		public static void WarpCameraFOV(CameraFOVWarp info, float multiplier = 1)
		{
			float fov = PlayerCameraController.CameraFOV - ((PlayerCameraController.CameraFOV - info.TargetFOV) * multiplier);
			WarpCameraFOV(info.Dominance, fov, info.TimeIn, info.TimeStay, info.TimeOut, info.OverwriteOtherFOVWarps);
		}
		private IEnumerator WarpCameraFOVC()
		{
			while(true)
			{
				yield return new WaitForEndOfFrame();
				if (GameController.GameIsPaused)
					continue;

				if (AllCameraFOVWarpEffects.Count > 0)
				{
					HighestCameraFOVWarpDominanceIndex = -1;
					int targetIndex = 0;
					for (int i = 0; i < AllCameraFOVWarpEffects.Count; i++)
					{
						AllCameraFOVWarpEffects[i].passedTime += Time.deltaTime;
						if (AllCameraFOVWarpEffects[i].passedTime >= AllCameraFOVWarpEffects[i].totalTime)
						{
							AllCameraFOVWarpEffects.RemoveAt(i);
							i--;
						}
						else
						{
							if (HighestCameraFOVWarpDominanceIndex < AllCameraFOVWarpEffects[i].dominanceIndex)
							{
								HighestCameraFOVWarpDominanceIndex = AllCameraFOVWarpEffects[i].dominanceIndex;
								targetIndex = i;
							}
						}
					}

					if (AllCameraFOVWarpEffects.Count == 0)
						continue;

					PlayerCameraController.CameraFOV = Mathf.Lerp(PlayerCameraController.CameraFOV, AllCameraFOVWarpEffects[targetIndex].GetFOV(), LERP_FOV_SPEED * Time.deltaTime);
				}
				else
				{
					PlayerCameraController.CameraFOV = Mathf.Lerp(PlayerCameraController.CameraFOV, Player.PlayerSettings.CameraBaseFOV, LERP_FOV_SPEED * Time.deltaTime);
					if (Mathf.Abs(Player.PlayerSettings.CameraBaseFOV - PlayerCameraController.CameraFOV) < 0.5f)
						break;
				}
			}
			PlayerCameraController.CameraFOV = Player.PlayerSettings.CameraBaseFOV;
			CameraFOVWarpCoroutine = null;
		}
		private class CameraFOVWarpEffect
		{
			public int dominanceIndex;
			public float targetFOV;
			public float timeIn;
			public float timeStay;
			public float timeOut;
			public float totalTime => timeIn + timeStay + timeOut;
			public float passedTime;

			private float dif => targetFOV - Player.PlayerSettings.CameraBaseFOV;
			public CameraFOVWarpEffect(int dominanceIndex, float targetFOV, float timeIn, float timeStay, float timeOut)
			{
				this.dominanceIndex = dominanceIndex;
				this.targetFOV = targetFOV;
				this.timeIn = timeIn;
				this.timeStay = timeStay;
				this.timeOut = timeOut;
				passedTime = 0;
			}
			public float GetFOV()
			{
				if (passedTime < timeIn)
				{
					return Player.PlayerSettings.CameraBaseFOV + (passedTime / timeIn) * dif;
				}
				else if (passedTime >= timeIn && passedTime < (timeStay + timeIn))
				{
					return targetFOV;
				}
				else //passedTime >= timeStay + timeIn
				{
					float fractTime = passedTime - (timeIn + timeStay);
					return Player.PlayerSettings.CameraBaseFOV + (1 - fractTime / timeOut) * dif;
				}
			}
		}
		#endregion
		#region SingleSound
		public static void PlaySound(SoundEffect info, Transform target, float multiplier = 1)
		{
			SoundPlayer soundPlayer = (new GameObject(info.TargetSound.soundName + " Sound Player")).AddComponent<SoundPlayer>();
			soundPlayer.transform.SetParent(Storage.SoundStorage);

			soundPlayer.transform.position = target ? target.position + info.OffsetToTarget : info.OffsetToTarget;

			soundPlayer.Setup(info, !info.Loop, multiplier);
			if (info.FollowTarget)
				soundPlayer.FollowTargetSetup(target, info.OffsetToTarget, info.OnParentDeathBehaivior, info.fadeOutTime);

			soundPlayer.Play();
		}
		#endregion
		#region Particle Effects
		public static void PlayParticleEffect(GameObject mainObject, Vector3 offsetToTarget, bool destroyAfterDelay = true, float destroyDelay = 0, Transform parent = null, BehaiviorOnParentDeath parentDeathBehaivior = BehaiviorOnParentDeath.Fade, bool inheritRotation = false, bool asInstantiation = true)
		{
			GameObject targetObject;
			if (asInstantiation)
			{
				targetObject = Instantiate(mainObject, Storage.ParticleStorage);
			}
			else
			{
				targetObject = mainObject;
			}
			targetObject.transform.position = parent ? parent.position + offsetToTarget : offsetToTarget;

			if (parent)
				Instance.StartCoroutine(Instance.ParticleFollowTarget(targetObject.transform, parent, offsetToTarget, inheritRotation, parentDeathBehaivior));

			ParticleSystem[] containedSystems = targetObject.GetComponentsInChildren<ParticleSystem>();
			for(int i = 0; i < containedSystems.Length; i++)
			{
				if(containedSystems[i].isPlaying)
					containedSystems[i].Stop();
				containedSystems[i].Play();
			}

			if(destroyAfterDelay)
				FadeAndDestroyParticles(targetObject, destroyDelay);
		}
		public static void PlayParticleEffect(ParticleEffect info, Transform target, float multiplier = 1)
		{
			PlayParticleEffect(info.ParticleMainObject, info.FollowTarget ? info.OffsetToTarget : (target ? target.position + info.OffsetToTarget : info.OffsetToTarget), true, info.DestroyDelay, info.FollowTarget ? target : null, info.OnTargetDeathBehaivior, info.InheritRotationOfTarget);
		}
		public static void PlayParticleEffect(ParticleEffect info, Entities.Entitie target, float multiplier = 1)
		{
			PlayParticleEffect(info.ParticleMainObject, info.FollowTarget ? info.OffsetToTarget : (target ? target.actuallWorldPosition + info.OffsetToTarget : info.OffsetToTarget), true, info.DestroyDelay, info.FollowTarget ? target.transform : null, info.OnTargetDeathBehaivior, info.InheritRotationOfTarget);
		}
		public static void FadeAndDestroyParticles(GameObject target, float? delay)
		{
			Instance.StartCoroutine(Instance.FadeAndDestroyParticlesC(target, delay));
		}
		private IEnumerator ParticleFollowTarget(Transform follower, Transform target, Vector3 offset, bool inheritRotation, BehaiviorOnParentDeath deathBehaivior)
		{
			while (follower)
			{
				if (target)
				{
					follower.position = target.position + offset;
					if (inheritRotation)
						follower.rotation = target.rotation;
				}
				else
				{
					if(deathBehaivior == BehaiviorOnParentDeath.Remove)
					{
						Destroy(follower.gameObject);
					}
					else if(deathBehaivior == BehaiviorOnParentDeath.Fade)
					{
						StartCoroutine(FadeAndDestroyParticlesC(follower.gameObject, null));
						break;
					}
					else
					{
						break;
					}
				}
				yield return new WaitForFixedUpdate();
			}
		}
		private IEnumerator FadeAndDestroyParticlesC(GameObject target, float? baseDelay)
		{
			if (baseDelay.HasValue)
				yield return new WaitForSeconds(baseDelay.Value);

			if (!target)
				goto FadeFinished;

			ParticleSystem[] particleSystems = target.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
			}

			while (true)
			{
				yield return new WaitForEndOfFrame();
				if (!target)
					goto FadeFinished;

				if (GameController.GameIsPaused)
					continue;

				bool foundParticle = false;
				for(int i = 0; i < particleSystems.Length; i++)
				{
					if(particleSystems[i].particleCount > 0)
					{
						foundParticle = true;
						break;
					}
				}

				if (!foundParticle)
					break;
			}

			Destroy(target);

			//We can jump to this in case of external removal of the target
			FadeFinished:;
		}
		#endregion
		#region Entitie Text
		public static void CreateDamageNumber(Vector3 startPosition, Gradient colors, Vector3 numberVelocity, float damage, bool wasCrit, float overrideScale = 1)
		{
			DamageNumber newDamageNumber = Instance.damageNumberPool.GetPoolObject();
			newDamageNumber.transform.position = startPosition;

			newDamageNumber.transform.localScale = Vector3.one * overrideScale;
			float roundedNumber = Mathf.Round(damage * 100) / 100;
			int fullNumber = Mathf.FloorToInt(roundedNumber);
			int afterComma = Mathf.RoundToInt((roundedNumber - fullNumber) * 100);
			string displayedNumber;

			if (afterComma < Mathf.Epsilon)
				displayedNumber = fullNumber.ToString();
			else
				displayedNumber = fullNumber + ".<size=" + (newDamageNumber.display.fontSize / 1.75f) + ">" + afterComma + "</size>";

			newDamageNumber.BeginDisplay(numberVelocity, colors, displayedNumber, wasCrit);
		}

		public static void DisplayInfoText(Vector3 startPosition, Gradient colors, Vector3 numberVelocity, string text, float overrideScale = 1)
		{
			DamageNumber newDamageNumber = Instance.damageNumberPool.GetPoolObject();
			newDamageNumber.transform.position = startPosition;
			newDamageNumber.transform.localScale = Vector3.one * overrideScale;
			newDamageNumber.BeginDisplay(numberVelocity, colors, text, false);
		}
		#endregion
	}
}