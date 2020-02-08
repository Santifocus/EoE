using EoE.Entities;
using EoE.Information;
using EoE.Sounds;
using EoE.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace EoE
{
	public class EffectManager : MonoBehaviour
	{
		#region Fields
		private const int DAMAGE_NUMBER_BASE_POOLSIZE = 25;
		private static EffectManager Instance;
		[SerializeField] private Material screenEffectMaterial = default;
		[SerializeField] private Transform cameraShakeCore = default;
		[SerializeField] private DamageNumber damageNumberPrefab = default;
		private PoolableObject<DamageNumber> damageNumberPool;
		#endregion
		#region Setups
		private void Start()
		{
			Instance = this;
			BaseFixedDeltaTime = Time.fixedDeltaTime;
			ResetScreenEffectMat();
			StartCoroutine(DamageNumberPoolCreation());
		}
		private IEnumerator DamageNumberPoolCreation()
		{
			//We slowly add objects to the pool so the stress on the first frame is reduced
			//In case of a sudden spike of required damage numbers the pool will automatically create missing ones
			//So we dont have to worry about running out of instances
			damageNumberPool = new PoolableObject<DamageNumber>(0, true, damageNumberPrefab, Storage.ParticleStorage);

			while(damageNumberPool.PoolSize < DAMAGE_NUMBER_BASE_POOLSIZE)
			{ 
				yield return new WaitForEndOfFrame();
				damageNumberPool.PoolSize++;
			}
		}
		public static void ResetFX()
		{
			if (Instance)
			{
				Instance.ResetScreenEffectMat();
				Instance.StopAllCoroutines();
			}

			AllScreenShakes = new List<ScreenShakeInstance>();
			AllRumbles = new List<ControllerRumbleInstance>();
			AllScreenColorEffects = new List<ScreenBorderColorInstance>();
			AllBlurScreenEffects = new List<ScreenBlurInstance>();
			AllTintScreenEffects = new List<ScreenTintInstance>();
			AllTimeDilationsEffects = new List<TimeDilationInstance>();
			AllCameraFOVWarpEffects = new List<CameraFOVWarpInstance>();
			AllDialogues = new List<DialogueInstance>();
			AllNotifications = new List<NotificationInstance>();
			AllCustomUIs = new List<CustomUIInstance>();
			AllSoundFXs = new List<SoundEffectInstance>();
			AllParticleFXs = new List<ParticleEffectInstance>();

			if (Gamepad.current != null)
			{
				Gamepad.current.SetMotorSpeeds(0, 0);
			}
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
			ResetScreenEffectMat();
			if (Gamepad.current != null)
			{
				Gamepad.current.SetMotorSpeeds(0, 0);
			}
		}
		#endregion
		#region ScreenShake
		private const float DELAY_PER_SHAKE = 0.01f;
		private Coroutine ShakeScreenCoroutine = null;
		private static List<ScreenShakeInstance> AllScreenShakes = new List<ScreenShakeInstance>();
		public static FXInstance ShakeScreen(ScreenShake info, float multiplier = 1)
		{
			ScreenShakeInstance newScreenShakeInfo = new ScreenShakeInstance(info);
			newScreenShakeInfo.BaseSetup(multiplier, Player.Instance.transform);
			AllScreenShakes.Add(newScreenShakeInfo);

			if (Instance.ShakeScreenCoroutine == null)
			{
				Instance.ShakeScreenCoroutine = Instance.StartCoroutine(Instance.ShakeScreenC());
			}
			return newScreenShakeInfo;
		}
		private IEnumerator ShakeScreenC()
		{
			float timeTillNextShake = 0;
			while (AllScreenShakes.Count > 0)
			{
				yield return new WaitForEndOfFrame();
				if (GameController.GameIsPaused)
					continue;

				timeTillNextShake -= Time.deltaTime;
				if (timeTillNextShake <= 0)
				{
					float strongestAxisIntensity = 0;
					Vector3 strongestAxisIntensityVector = Vector3.zero;
					float strongestAngleIntensity = 0;
					Vector3 strongestAngleIntensityVector = Vector3.zero;

					for (int i = 0; i < AllScreenShakes.Count; i++)
					{
						AllScreenShakes[i].Update(Time.deltaTime);
						if (AllScreenShakes[i].ShouldBeRemoved && AllScreenShakes[i].AllowedToRemove())
						{
							AllScreenShakes[i].Remove();
							AllScreenShakes.RemoveAt(i);
							i--;
						}
						else
						{
							(Vector3 axisIntensityVector, Vector3 angleIntensityVector) = AllScreenShakes[i].GetIntensities();
							float axisIntensity = axisIntensityVector.sqrMagnitude;
							if (axisIntensity > strongestAxisIntensity)
							{
								strongestAxisIntensity = axisIntensity;
								strongestAxisIntensityVector = axisIntensityVector;
							}

							float angleIntensity = angleIntensityVector.sqrMagnitude;
							if (angleIntensity > strongestAngleIntensity)
							{
								strongestAngleIntensity = angleIntensity;
								strongestAngleIntensityVector = angleIntensityVector;
							}
						}
					}

					Shake(strongestAxisIntensityVector, strongestAngleIntensityVector);
					timeTillNextShake += DELAY_PER_SHAKE;
				}
			}

			cameraShakeCore.localPosition = Vector3.zero;
			cameraShakeCore.localEulerAngles = Vector3.zero;

			ShakeScreenCoroutine = null;
			void Shake(Vector3 axisIntensity, Vector3 angleIntensity)
			{
				axisIntensity *= Time.timeScale;
				angleIntensity *= Time.timeScale;
				cameraShakeCore.localPosition = (Random.value - 0.5f) * cameraShakeCore.transform.right * axisIntensity.x +
												(Random.value - 0.5f) * cameraShakeCore.transform.up * axisIntensity.y +
												(Random.value - 0.5f) * cameraShakeCore.transform.forward * axisIntensity.z;
				cameraShakeCore.localEulerAngles = new Vector3(	(Random.value - 0.5f) * angleIntensity.x,
																(Random.value - 0.5f) * angleIntensity.y,
																(Random.value - 0.5f) * angleIntensity.z);
			}
		}
		private class ScreenShakeInstance : FXInstance
		{
			public override FXType Type => FXType.Player;
			public override FXObject BaseInfo => ScreenShakeInfo;
			private readonly ScreenShake ScreenShakeInfo;

			public ScreenShakeInstance(ScreenShake ScreenShakeInfo)
			{
				this.ScreenShakeInfo = ScreenShakeInfo;
			}
			public (Vector3, Vector3) GetIntensities()
			{
				float multiplier = GetCurMultiplier();
				return (ScreenShakeInfo.AxisIntensity * ScreenShakeInfo.CustomAxisMultiplier * multiplier, ScreenShakeInfo.AngleIntensity * ScreenShakeInfo.CustomAngleMultiplier * multiplier);
			}
		}
		#endregion
		#region Rumble
		private static List<ControllerRumbleInstance> AllRumbles = new List<ControllerRumbleInstance>();
		private Coroutine RumbleCoroutine = null;
		public static FXInstance RumbleController(ControllerRumble info, float multiplier = 1)
		{
			ControllerRumbleInstance newRumbleInfo = new ControllerRumbleInstance(info);
			newRumbleInfo.BaseSetup(multiplier, Player.Instance.transform);
			AllRumbles.Add(newRumbleInfo);

			if (Instance.RumbleCoroutine == null)
			{
				Instance.RumbleCoroutine = Instance.StartCoroutine(Instance.RumbleC());
			}
			return newRumbleInfo;
		}
		private IEnumerator RumbleC()
		{
			while (AllRumbles.Count > 0)
			{
				yield return new WaitForEndOfFrame();
				if (GameController.GameIsPaused)
				{
					if (Gamepad.current != null)
					{
						Gamepad.current.SetMotorSpeeds(0, 0);
					}
					continue;
				}

				float highestLeftIntensity = 0;
				float highestRightIntensity = 0;

				//Update all remaining times and get the highest intensitys
				for (int i = 0; i < AllRumbles.Count; i++)
				{
					AllRumbles[i].Update(Time.unscaledDeltaTime);
					if (AllRumbles[i].ShouldBeRemoved && AllRumbles[i].AllowedToRemove())
					{
						AllRumbles[i].Remove();
						AllRumbles.RemoveAt(i);
						i--;
					}
					else
					{
						(float left, float right) = AllRumbles[i].GetRumbleIntensitys();

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

		private class ControllerRumbleInstance : FXInstance
		{
			public override FXType Type => FXType.Player;
			public override FXObject BaseInfo => ControllerRumbleInfo;
			private readonly ControllerRumble ControllerRumbleInfo;

			public ControllerRumbleInstance(ControllerRumble ControllerRumbleInfo)
			{
				this.ControllerRumbleInfo = ControllerRumbleInfo;
			}
			public (float, float) GetRumbleIntensitys()
			{
				float point = GetCurMultiplier();
				float left = Mathf.LerpUnclamped(ControllerRumbleInfo.LeftMinIntensity, ControllerRumbleInfo.LeftMaxIntensity, point);
				float right = Mathf.LerpUnclamped(ControllerRumbleInfo.RightMinIntensity, ControllerRumbleInfo.RightMaxIntensity, point);

				return (left, right);
			}
		}
		#endregion
		#region ColorScreen
		private static List<ScreenBorderColorInstance> AllScreenColorEffects = new List<ScreenBorderColorInstance>();
		private Coroutine ColorScreenCoroutine;
		public static FXInstance ColorScreenBorder(ScreenBorderColor info, float multiplier = 1)
		{
			ScreenBorderColorInstance newColorScreenEffect = new ScreenBorderColorInstance(info);
			newColorScreenEffect.BaseSetup(multiplier, Player.Instance.transform);
			AllScreenColorEffects.Add(newColorScreenEffect);

			if (Instance.ColorScreenCoroutine == null)
			{
				Instance.ColorScreenCoroutine = Instance.StartCoroutine(Instance.ColorScreenC());
			}
			return newColorScreenEffect;
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
					AllScreenColorEffects[i].Update(Time.deltaTime);
					if (AllScreenColorEffects[i].ShouldBeRemoved && AllScreenColorEffects[i].AllowedToRemove())
					{
						AllScreenColorEffects[i].Remove();
						AllScreenColorEffects.RemoveAt(i);
						i--;
					}
					else
					{
						averageDepth += AllScreenColorEffects[i].GetDepth();
						averageColor += AllScreenColorEffects[i].BorderColor;
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
		private class ScreenBorderColorInstance : FXInstance
		{
			public override FXType Type => FXType.Player;
			public override FXObject BaseInfo => ScreenBorderColorInfo;
			private readonly ScreenBorderColor ScreenBorderColorInfo;
			public Color BorderColor => ScreenBorderColorInfo.Color;
			public ScreenBorderColorInstance(ScreenBorderColor ScreenBorderColorInfo)
			{
				this.ScreenBorderColorInfo = ScreenBorderColorInfo;
			}
			public float GetDepth()
			{
				return GetCurMultiplier() * ScreenBorderColorInfo.Depth;
			}
		}
		#endregion
		#region BlurScreen
		private const float BLUR_LERP_SPEED = 8;
		private static List<ScreenBlurInstance> AllBlurScreenEffects = new List<ScreenBlurInstance>();
		private Coroutine BlurScreenCoroutine = null;
		public static FXInstance BlurScreen(ScreenBlur info, float multiplier = 1)
		{
			ScreenBlurInstance newBlurScreenEffect = new ScreenBlurInstance(info);
			newBlurScreenEffect.BaseSetup(multiplier, Player.Instance.transform);
			AllBlurScreenEffects.Add(newBlurScreenEffect);

			if (Instance.BlurScreenCoroutine == null)
			{
				Instance.BlurScreenCoroutine = Instance.StartCoroutine(Instance.BlurScreenC());
			}

			return newBlurScreenEffect;
		}
		private IEnumerator BlurScreenC()
		{
			float curBlurIntensity = 0;
			float curBlurDistance = 0;

			while (AllBlurScreenEffects.Count > 0)
			{
				yield return new WaitForEndOfFrame();
				if (GameController.GameIsPaused)
					continue;

				float strongestIntensity = 0;
				int strongestBlurDistance = 0;
				for (int i = 0; i < AllBlurScreenEffects.Count; i++)
				{
					AllBlurScreenEffects[i].Update(Time.deltaTime);
					if (AllBlurScreenEffects[i].ShouldBeRemoved && AllBlurScreenEffects[i].AllowedToRemove())
					{
						AllBlurScreenEffects[i].Remove();
						AllBlurScreenEffects.RemoveAt(i);
						i--;
					}
					else
					{
						(float intensity, int blurDistance) = AllBlurScreenEffects[i].GetIntensityAndDistance();
						if (intensity > strongestIntensity)
							strongestIntensity = intensity;

						if (blurDistance > strongestBlurDistance)
							strongestBlurDistance = blurDistance;
					}
				}

				curBlurIntensity = Mathf.Lerp(curBlurIntensity, strongestIntensity, Time.deltaTime * BLUR_LERP_SPEED);
				curBlurDistance = Mathf.Lerp(curBlurDistance, strongestBlurDistance, Time.deltaTime * BLUR_LERP_SPEED);

				screenEffectMaterial.SetFloat("_BlurPower", curBlurIntensity);
				screenEffectMaterial.SetInt("_BlurRange", (int)curBlurDistance);
			}

			screenEffectMaterial.SetFloat("_BlurPower", 0);
			screenEffectMaterial.SetInt("_BlurRange", 0);
			BlurScreenCoroutine = null;
		}
		private class ScreenBlurInstance : FXInstance
		{
			public override FXType Type => FXType.Player;
			public override FXObject BaseInfo => ScreenBlurInfo;
			private readonly ScreenBlur ScreenBlurInfo;
			public ScreenBlurInstance(ScreenBlur ScreenBlurInfo)
			{
				this.ScreenBlurInfo = ScreenBlurInfo;
			}
			public (float, int) GetIntensityAndDistance()
			{
				float multiplier = GetCurMultiplier();
				return (ScreenBlurInfo.Intensity * multiplier, Mathf.RoundToInt(ScreenBlurInfo.BlurDistance * multiplier));
			}
		}
		#endregion
		#region ScreenTint
		private static List<ScreenTintInstance> AllTintScreenEffects = new List<ScreenTintInstance>();
		private Coroutine TintScreenCoroutine = null;
		public static FXInstance TintScreen(ScreenTint info, float multiplier = 1)
		{
			ScreenTintInstance newTintScreenEffect = new ScreenTintInstance(info);
			newTintScreenEffect.BaseSetup(multiplier, Player.Instance.transform);
			AllTintScreenEffects.Add(newTintScreenEffect);

			if (Instance.TintScreenCoroutine == null)
			{
				Instance.TintScreenCoroutine = Instance.StartCoroutine(Instance.TintScreenC());
			}

			return newTintScreenEffect;
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
				for (int i = 0; i < AllTintScreenEffects.Count; i++)
				{
					AllTintScreenEffects[i].Update(Time.deltaTime);
					if (AllTintScreenEffects[i].ShouldBeRemoved && AllTintScreenEffects[i].AllowedToRemove())
					{
						AllTintScreenEffects[i].Remove();
						AllTintScreenEffects.RemoveAt(i);
						i--;
					}
					else
					{
						dominanceCount += (int)AllTintScreenEffects[i].DominanceIndex;
					}
				}

				//Better be safe then sorry
				if (dominanceCount < 1)
					continue;

				//Now use the total dominance level to lerp a color between all tints
				Color lerpedColor = Color.clear;
				for (int i = 0; i < AllTintScreenEffects.Count; i++)
				{
					lerpedColor += AllTintScreenEffects[i].GetCurrentColor() * ((float)AllTintScreenEffects[i].DominanceIndex / dominanceCount);
				}

				//Finally set the material color tint
				screenEffectMaterial.SetColor("_TintColor", lerpedColor);
			}

			screenEffectMaterial.SetColor("_TintColor", Color.clear);
			TintScreenCoroutine = null;
		}
		private class ScreenTintInstance : FXInstance
		{
			public override FXType Type => FXType.Player;
			public override FXObject BaseInfo => ScreenTintInfo;
			private readonly ScreenTint ScreenTintInfo;
			public uint DominanceIndex => ScreenTintInfo.Dominance;

			public ScreenTintInstance(ScreenTint ScreenTintInfo)
			{
				this.ScreenTintInfo = ScreenTintInfo;
			}
			public Color GetCurrentColor()
			{
				return new Color(ScreenTintInfo.TintColor.r, ScreenTintInfo.TintColor.g, ScreenTintInfo.TintColor.b, ScreenTintInfo.TintColor.a * GetCurMultiplier());
			}
		}
		#endregion
		#region TimeDilation
		public static int HighestTimeDilutionDominanceIndex;
		private static float BaseFixedDeltaTime;
		private static List<TimeDilationInstance> AllTimeDilationsEffects = new List<TimeDilationInstance>();
		private Coroutine TimeDilationCoroutine = null;
		public static FXInstance DilateTime(TimeDilation info, float multiplier = 1)
		{
			if (info.OverwriteOtherTimeDilations)
			{
				AllTimeDilationsEffects = new List<TimeDilationInstance>();
			}
			TimeDilationInstance newTimeDilationEffect = new TimeDilationInstance(info);
			newTimeDilationEffect.BaseSetup(multiplier, Player.Instance.transform);
			AllTimeDilationsEffects.Add(newTimeDilationEffect);

			if (Instance.TimeDilationCoroutine == null)
			{
				Instance.TimeDilationCoroutine = Instance.StartCoroutine(Instance.DilateTimeC());
			}

			return newTimeDilationEffect;
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
					AllTimeDilationsEffects[i].Update(Time.unscaledDeltaTime);
					if (AllTimeDilationsEffects[i].ShouldBeRemoved && AllTimeDilationsEffects[i].AllowedToRemove())
					{
						AllTimeDilationsEffects[i].Remove();
						AllTimeDilationsEffects.RemoveAt(i);
						i--;
					}
					else
					{
						if (HighestTimeDilutionDominanceIndex < AllTimeDilationsEffects[i].DominanceIndex)
						{
							HighestTimeDilutionDominanceIndex = (int)AllTimeDilationsEffects[i].DominanceIndex;
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
		private class TimeDilationInstance : FXInstance
		{
			public override FXType Type => FXType.Player;
			public override FXObject BaseInfo => TimeDilationInfo;
			private readonly TimeDilation TimeDilationInfo;
			public uint DominanceIndex => TimeDilationInfo.Dominance;
			private float ScaleDiffference => TimeDilationInfo.Scale - 1;

			public TimeDilationInstance(TimeDilation TimeDilationInfo)
			{
				this.TimeDilationInfo = TimeDilationInfo;
			}
			public float GetTimeScale()
			{
				return 1 + GetCurMultiplier() * ScaleDiffference;
			}
		}
		#endregion
		#region CameraFOVWarp
		public static int HighestCameraFOVWarpDominanceIndex;
		private static List<CameraFOVWarpInstance> AllCameraFOVWarpEffects = new List<CameraFOVWarpInstance>();
		private Coroutine CameraFOVWarpCoroutine = null;
		private const float LERP_FOV_SPEED = 10;
		public static FXInstance WarpCameraFOV(CameraFOVWarp info, float multiplier = 1)
		{
			if (info.OverwriteOtherFOVWarps)
			{
				AllCameraFOVWarpEffects = new List<CameraFOVWarpInstance>();
			}
			CameraFOVWarpInstance newCameraFOVWarpEffect = new CameraFOVWarpInstance(info);
			newCameraFOVWarpEffect.BaseSetup(multiplier, Player.Instance.transform);
			AllCameraFOVWarpEffects.Add(newCameraFOVWarpEffect);

			if (Instance.CameraFOVWarpCoroutine == null)
			{
				Instance.CameraFOVWarpCoroutine = Instance.StartCoroutine(Instance.WarpCameraFOVC());
			}
			return newCameraFOVWarpEffect;
		}
		private IEnumerator WarpCameraFOVC()
		{
			while (true)
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
						AllCameraFOVWarpEffects[i].Update(Time.deltaTime);
						if (AllCameraFOVWarpEffects[i].ShouldBeRemoved && AllCameraFOVWarpEffects[i].AllowedToRemove())
						{
							AllCameraFOVWarpEffects[i].Remove();
							AllCameraFOVWarpEffects.RemoveAt(i);
							i--;
						}
						else
						{
							if (HighestCameraFOVWarpDominanceIndex < AllCameraFOVWarpEffects[i].DominanceIndex)
							{
								HighestCameraFOVWarpDominanceIndex = AllCameraFOVWarpEffects[i].DominanceIndex;
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
					PlayerCameraController.CameraFOV = Mathf.Lerp(PlayerCameraController.CameraFOV, Player.PlayerSettings.CameraBaseFOV, LERP_FOV_SPEED * Time.deltaTime * 1.5f);
					if (Mathf.Abs(PlayerCameraController.CameraFOV - Player.PlayerSettings.CameraBaseFOV) < 0.01f)
						break;
				}
			}
			PlayerCameraController.CameraFOV = Player.PlayerSettings.CameraBaseFOV;
			CameraFOVWarpCoroutine = null;
		}
		private class CameraFOVWarpInstance : FXInstance
		{
			public override FXType Type => FXType.Player;
			public override FXObject BaseInfo => CameraFOVWarpInfo;
			private readonly CameraFOVWarp CameraFOVWarpInfo;
			public int DominanceIndex => CameraFOVWarpInfo.Dominance;
			private float FOVDifference => CameraFOVWarpInfo.TargetFOV - Player.PlayerSettings.CameraBaseFOV;
			public CameraFOVWarpInstance(CameraFOVWarp CameraFOVWarpInfo)
			{
				this.CameraFOVWarpInfo = CameraFOVWarpInfo;
			}
			public float GetFOV()
			{
				return Player.PlayerSettings.CameraBaseFOV + GetCurMultiplier() * FOVDifference;
			}
		}
		#endregion
		#region Dialogue
		private static List<DialogueInstance> AllDialogues = new List<DialogueInstance>();
		private Coroutine DialogueCoroutine = null;
		public static FXInstance ShowDialogue(Dialogue info)
		{
			DialogueInstance newDialogue = new DialogueInstance(info);
			newDialogue.BaseSetup(1, Player.Instance.transform);
			AllDialogues.Add(newDialogue);

			if (Instance.DialogueCoroutine == null)
			{
				Instance.DialogueCoroutine = Instance.StartCoroutine(Instance.DialgoueC());
			}

			return newDialogue;
		}
		private IEnumerator DialgoueC()
		{
			while (AllDialogues.Count > 0)
			{
				yield return new WaitForEndOfFrame();

				for (int i = 0; i < AllDialogues.Count; i++)
				{
					AllDialogues[i].Update(Time.unscaledDeltaTime);
					if (AllDialogues[i].ShouldBeRemoved && AllDialogues[i].AllowedToRemove())
					{
						AllDialogues[i].Remove();
						AllDialogues.RemoveAt(i);
						i--;
					}
				}
			}

			DialogueCoroutine = null;
		}
		private class DialogueInstance : FXInstance
		{
			public override FXType Type => FXType.Player;

			public override FXObject BaseInfo => DialogueInfo;
			private readonly Dialogue DialogueInfo;
			private readonly QueuedDialogue queuedDialogue;
			private bool wantsToBeRemoved;
			public DialogueInstance(Dialogue DialogueInfo)
			{
				this.DialogueInfo = DialogueInfo;
				queuedDialogue = DialogueController.Instance.QueueDialogue(DialogueInfo);
				allowBaseUpdate = false;
				if (DialogueInfo.pauseTimeWhenDisplaying)
					GameController.ActivePauses++;
			}
			protected override void InternalUpdate()
			{
				if (!wantsToBeRemoved)
					wantsToBeRemoved = CheckForFinishCondition();
				if (!allowBaseUpdate && queuedDialogue.DoneDisplaying)
				{
					allowBaseUpdate = true;
					if (wantsToBeRemoved)
						FinishFX();
				}
			}
			public override void OnRemove()
			{
				queuedDialogue.ShouldRemove = true;
				if(DialogueInfo.pauseTimeWhenDisplaying)
					GameController.ActivePauses--;
			}
		}
		#endregion
		#region Notification
		private static List<NotificationInstance> AllNotifications = new List<NotificationInstance>();
		private Coroutine NotificationCoroutine = null;
		public static FXInstance ShowNotification(Notification info, float multiplier)
		{
			NotificationInstance newNotification = new NotificationInstance(info);
			newNotification.BaseSetup(multiplier, Player.Instance.transform);
			AllNotifications.Add(newNotification);

			if (Instance.NotificationCoroutine == null)
			{
				Instance.NotificationCoroutine = Instance.StartCoroutine(Instance.NotificationC());
			}

			return newNotification;
		}
		private IEnumerator NotificationC()
		{
			while (AllNotifications.Count > 0)
			{
				yield return new WaitForEndOfFrame();
				if (GameController.GameIsPaused)
					continue;

				for (int i = 0; i < AllNotifications.Count; i++)
				{
					AllNotifications[i].Update(Time.deltaTime);
					if (AllNotifications[i].ShouldBeRemoved && AllNotifications[i].AllowedToRemove())
					{
						AllNotifications[i].Remove();
						AllNotifications.RemoveAt(i);
						i--;
					}
				}
			}

			NotificationCoroutine = null;
		}
		private class NotificationInstance : FXInstance
		{
			public override FXType Type => FXType.Player;

			public override FXObject BaseInfo => NotificationInfo;
			private readonly Notification NotificationInfo;
			private readonly NotificationDisplay notifcationObject;
			public NotificationInstance(Notification NotificationInfo)
			{
				this.NotificationInfo = NotificationInfo;
				notifcationObject = NotificationController.Instance.AddNotification(NotificationInfo);
			}
			protected override void InternalUpdate()
			{
				notifcationObject.AlphaMultiplier = GetCurMultiplier();
			}
			public override void OnRemove()
			{
				NotificationController.Instance.RemoveNotification(notifcationObject);
			}
		}
		#endregion
		#region CustomUI
		private static List<CustomUIInstance> AllCustomUIs = new List<CustomUIInstance>();
		private Coroutine CustomUICoroutine = null;
		public static FXInstance PlayCustomUI(CustomUI info, float multiplier)
		{
			Transform parent = info.CanvasTarget == CanvasTarget.Main ? GameController.Instance.MainCanvas : GameController.Instance.MenuCanvas;
			CustomUIInstance newCustomUI = new CustomUIInstance(info, parent);
			newCustomUI.BaseSetup(multiplier, parent);
			AllCustomUIs.Add(newCustomUI);

			if (Instance.CustomUICoroutine == null)
			{
				Instance.CustomUICoroutine = Instance.StartCoroutine(Instance.CustomUIC());
			}

			return newCustomUI;
		}
		private IEnumerator CustomUIC()
		{
			while (AllCustomUIs.Count > 0)
			{
				yield return new WaitForEndOfFrame();
				if (GameController.GameIsPaused)
					continue;

				for (int i = 0; i < AllCustomUIs.Count; i++)
				{
					AllCustomUIs[i].Update(Time.deltaTime);
					if (AllCustomUIs[i].ShouldBeRemoved && AllCustomUIs[i].AllowedToRemove())
					{
						AllCustomUIs[i].Remove();
						AllCustomUIs.RemoveAt(i);
						i--;
					}
				}
			}

			CustomUICoroutine = null;
		}
		private class CustomUIInstance : FXInstance
		{
			public override FXType Type => FXType.Player;

			public override FXObject BaseInfo => CustomUIInfo;
			public readonly CustomUI CustomUIInfo;
			private readonly GameObject mainObject;
			private readonly Graphic[] containedGraphics;
			private readonly float[] containedBaseAlphas;
			private float curMultiplier = -1;
			public CustomUIInstance(CustomUI CustomUIInfo, Transform parent)
			{
				this.CustomUIInfo = CustomUIInfo;

				mainObject = Instantiate(CustomUIInfo.UIPrefabObject, parent);
				containedGraphics = mainObject.GetComponentsInChildren<Graphic>();
				containedBaseAlphas = new float[containedGraphics.Length];
				for (int i = 0; i < containedBaseAlphas.Length; i++)
				{
					containedBaseAlphas[i] = containedGraphics[i].color.a;
				}
				mainObject.transform.SetSiblingIndex(CustomUIInfo.CustomChildIndex);
				InternalUpdate();
			}
			protected override void InternalUpdate()
			{
				float newMultiplier = GetCurMultiplier();
				if (curMultiplier != newMultiplier)
				{
					curMultiplier = newMultiplier;
					for (int i = 0; i < containedGraphics.Length; i++)
					{
						containedGraphics[i].color = new Color(containedGraphics[i].color.r, containedGraphics[i].color.g, containedGraphics[i].color.b, containedBaseAlphas[i] * curMultiplier);
					}
				}
			}
			public override void OnRemove()
			{
				Destroy(mainObject);
			}
		}
		#endregion
		#region SingleSound
		private static List<SoundEffectInstance> AllSoundFXs = new List<SoundEffectInstance>();
		private Coroutine SoundFXCoroutine = null;
		public static FXInstance PlaySound(SoundEffect info, Transform target, Vector3? customOffset, float multiplier = 1)
		{
			SoundPlayer soundPlayer = (new GameObject(info.TargetSound.soundName + " Sound Player")).AddComponent<SoundPlayer>();
			soundPlayer.transform.SetParent(Storage.SoundStorage);

			soundPlayer.transform.position = target ? target.position + info.OffsetToTarget : info.OffsetToTarget;

			soundPlayer.Setup(info.TargetSound);
			soundPlayer.Play();

			SoundEffectInstance newSoundFX = new SoundEffectInstance(info, soundPlayer, customOffset);
			newSoundFX.BaseSetup(multiplier, target);
			AllSoundFXs.Add(newSoundFX);

			if (Instance.SoundFXCoroutine == null)
			{
				Instance.SoundFXCoroutine = Instance.StartCoroutine(Instance.SoundFXC());
			}

			return newSoundFX;
		}
		private IEnumerator SoundFXC()
		{
			while (AllSoundFXs.Count > 0)
			{
				yield return new WaitForFixedUpdate();
				if (GameController.GameIsPaused)
					continue;

				for (int i = 0; i < AllSoundFXs.Count; i++)
				{
					AllSoundFXs[i].Update(Time.fixedDeltaTime);
					if (AllSoundFXs[i].ShouldBeRemoved && AllSoundFXs[i].AllowedToRemove())
					{
						AllSoundFXs[i].Remove();
						AllSoundFXs.RemoveAt(i);
						i--;
					}
				}
			}

			SoundFXCoroutine = null;
		}
		private class SoundEffectInstance : FXInstance
		{
			public override FXType Type => FXType.World;
			public override FXObject BaseInfo => SoundEffectInfo;
			private readonly SoundEffect SoundEffectInfo;
			private readonly SoundPlayer soundPlayer;

			private Vector3 positionOffset;

			public SoundEffectInstance(SoundEffect SoundInfo, SoundPlayer SelfPlayer, Vector3? positionOffset)
			{
				this.SoundEffectInfo = SoundInfo;
				this.soundPlayer = SelfPlayer;
				this.positionOffset = positionOffset ?? SoundInfo.OffsetToTarget;
			}
			protected override void InternalUpdate()
			{
				if (SoundEffectInfo.FollowTarget && parent)
				{
					Vector3 localOffset = positionOffset.x * soundPlayer.transform.right + positionOffset.y * soundPlayer.transform.up + positionOffset.z * soundPlayer.transform.forward;
					soundPlayer.transform.position = parent.position + localOffset;
				}
				soundPlayer.FadePoint = GetCurMultiplier();

				if (soundPlayer.FullyStopped)
					FinishFX();
			}
			public override void OnRemove()
			{
				Destroy(soundPlayer.gameObject);
			}
		}
		#endregion
		#region Particle Effects
		private static List<ParticleEffectInstance> AllParticleFXs = new List<ParticleEffectInstance>();
		private Coroutine ParticleFXCoroutine = null;
		public static FXInstance PlayParticleEffect(ParticleEffect info, Transform parent, Vector3? customOffset, Vector3? customRotation, Vector3? customScale, float multiplier = 1)
		{
			Transform mainObject = Instantiate(info.ParticleMainObject, Storage.ParticleStorage).transform;
			Vector3 offset = customOffset ?? info.OffsetToTarget;
			mainObject.position = parent ? parent.position + offset : offset;
			mainObject.localScale = customScale ?? mainObject.localScale;

			if (parent && info.InheritRotationOfTarget)
			{
				mainObject.transform.forward = parent.transform.forward;
				mainObject.transform.eulerAngles += info.RotationOffset;
			}

			ParticleEffectInstance newParticleFX = new ParticleEffectInstance(info, mainObject, customOffset, customRotation);
			newParticleFX.BaseSetup(multiplier, parent);
			AllParticleFXs.Add(newParticleFX);

			if (Instance.ParticleFXCoroutine == null)
			{
				Instance.ParticleFXCoroutine = Instance.StartCoroutine(Instance.ParticleFXC());
			}
			return newParticleFX;
		}
		private IEnumerator ParticleFXC()
		{
			while (AllParticleFXs.Count > 0)
			{
				yield return new WaitForFixedUpdate();
				if (GameController.GameIsPaused)
					continue;

				for (int i = 0; i < AllParticleFXs.Count; i++)
				{
					AllParticleFXs[i].Update(Time.fixedDeltaTime);
					if (AllParticleFXs[i].ShouldBeRemoved && AllParticleFXs[i].AllowedToRemove())
					{
						AllParticleFXs[i].Remove();
						AllParticleFXs.RemoveAt(i);
						i--;
					}
				}
			}

			ParticleFXCoroutine = null;
		}
		private class ParticleEffectInstance : FXInstance
		{
			public override FXType Type => FXType.World;
			public override FXObject BaseInfo => ParticleEffectInfo;
			public readonly ParticleEffect ParticleEffectInfo;

			private readonly Transform particleTransform;

			private readonly ParticleSystem[] containedSystems;
			private readonly float[] emissionBasis;

			private float curMultiplier = -1;
			private Vector3 positionOffset;
			private Vector3 rotationOffset;

			public ParticleEffectInstance(ParticleEffect ParticleEffectInfo, Transform particleTransform, Vector3? positionOffset, Vector3? rotationOffset)
			{
				this.ParticleEffectInfo = ParticleEffectInfo;
				this.particleTransform = particleTransform;

				this.positionOffset = positionOffset ?? ParticleEffectInfo.OffsetToTarget;
				this.rotationOffset = rotationOffset ?? ParticleEffectInfo.RotationOffset;

				//Record the base states of the particle systems
				containedSystems = particleTransform.GetComponentsInChildren<ParticleSystem>();
				for (int i = 0; i < containedSystems.Length; i++)
				{
					if (containedSystems[i].isPlaying)
						containedSystems[i].Stop();
					containedSystems[i].Play();
				}
				emissionBasis = new float[containedSystems.Length];

				for (int i = 0; i < containedSystems.Length; i++)
				{
					emissionBasis[i] = containedSystems[i].emission.rateOverTimeMultiplier;
				}
				UpdateEmission();
			}
			protected override void InternalUpdate()
			{
				if (ParticleEffectInfo.FollowTarget && parent)
				{
					if (ParticleEffectInfo.InheritRotationOfTarget)
					{
						particleTransform.rotation = Quaternion.Lerp(particleTransform.rotation, parent.transform.rotation, Time.fixedDeltaTime * ParticleEffectInfo.RotationInheritLerpSpeed);
						particleTransform.transform.eulerAngles += rotationOffset;
					}
					Vector3 localOffset = positionOffset.x * particleTransform.right + positionOffset.y * particleTransform.up + positionOffset.z * particleTransform.forward;
					particleTransform.position = parent.position + localOffset;
				}
				UpdateEmission();
			}
			private void UpdateEmission()
			{
				float newMultiplier = GetCurMultiplier();
				if (newMultiplier != curMultiplier)
				{
					curMultiplier = newMultiplier;
					if (CurrentFXState == FXState.End && Mathf.RoundToInt(newMultiplier * 100) == 0)
					{
						for (int i = 0; i < containedSystems.Length; i++)
						{
							containedSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
						}
					}
					else
					{
						for (int i = 0; i < containedSystems.Length; i++)
						{
							ParticleSystem.EmissionModule em = containedSystems[i].emission;
							em.rateOverTimeMultiplier = newMultiplier * emissionBasis[i];
						}
					}
				}
			}
			public override bool AllowedToRemove()
			{
				for (int i = 0; i < containedSystems.Length; i++)
				{
					if (containedSystems[i].particleCount > 0)
					{
						return false;
					}
				}
				return true;
			}
			public override void OnRemove()
			{
				Destroy(particleTransform.gameObject);
			}
		}
		#region FadeParticleSystem
		public static void FadeAndDestroyParticles(GameObject target, float? delay)
		{
			Instance.StartCoroutine(Instance.FadeAndDestroyParticlesC(target, delay));
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
				for (int i = 0; i < particleSystems.Length; i++)
				{
					if (particleSystems[i].particleCount > 0)
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

			if (afterComma == 0)
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
	#region FXInstance BaseClass
	public enum FXType { Player, World }
	public enum FXState { Start = 1, Stay = 2, End = 4 }
	public abstract class FXInstance
	{
		public abstract FXType Type { get; }
		public abstract FXObject BaseInfo { get; }
		public bool ShouldBeRemoved { get; private set; }
		public bool IsRemoved { get; private set; }
		public float baseMultiplier;

		protected Transform parent;

		protected FXState CurrentFXState { get; private set; }
		protected bool allowBaseUpdate = true;
		private float passedTime;
		private float stateMultiplier;

		public void Update(float timeStep)
		{
			if (allowBaseUpdate)
			{
				if (!ShouldBeRemoved)
					passedTime += timeStep;

				if (CheckForFinishCondition())
					FinishFX();
				UpdateCurrentState();
				UpdateMultiplier();
			}
			InternalUpdate();
		}
		protected bool CheckForFinishCondition()
		{
			if (CurrentFXState != FXState.End)
			{
				if (BaseInfo.FinishConditions.OnParentDeath && (!parent || !parent.gameObject.activeSelf))
					return true;
				else if (BaseInfo.FinishConditions.OnConditionMet && BaseInfo.FinishConditions.ConditionMet())
					return true;
			}
			return false;
		}
		private void UpdateCurrentState()
		{
			if (CurrentFXState == FXState.Start && passedTime > BaseInfo.TimeIn)
			{
				passedTime -= BaseInfo.TimeIn;
				CurrentFXState = FXState.Stay;
			}

			if (CurrentFXState == FXState.Stay && BaseInfo.FinishConditions.OnTimeout && passedTime > BaseInfo.FinishConditions.TimeStay)
			{
				passedTime -= BaseInfo.FinishConditions.TimeStay;
				CurrentFXState = FXState.End;
			}

			if (CurrentFXState == FXState.End && !ShouldBeRemoved)
			{
				if (passedTime > BaseInfo.TimeOut)
				{
					ShouldBeRemoved = true;
					passedTime = BaseInfo.TimeOut;
				}
			}
		}
		private void UpdateMultiplier()
		{
			if (CurrentFXState == FXState.Start)
			{
				if (BaseInfo.TimeIn > 0)
					stateMultiplier = passedTime / BaseInfo.TimeIn;
				else
					stateMultiplier = 1;
			}
			else if (CurrentFXState == FXState.Stay)
			{
				stateMultiplier = 1;
			}
			else //currentState == FXState.End
			{
				if (BaseInfo.TimeOut > 0)
					stateMultiplier = 1 - (passedTime / BaseInfo.TimeOut);
				else
					stateMultiplier = 0;
			}

		}
		public void BaseSetup(float baseMultiplier, Transform parent)
		{
			CurrentFXState = FXState.Start;
			this.baseMultiplier = baseMultiplier;
			this.parent = parent;
		}
		protected float GetCurMultiplier()
		{
			return stateMultiplier * baseMultiplier;
		}
		public virtual void FinishFX()
		{
			if (CurrentFXState != FXState.End)
			{
				passedTime = 0;
				CurrentFXState = FXState.End;
			}
		}
		public virtual bool AllowedToRemove() => true;
		protected virtual void InternalUpdate() { }
		public void Remove()
		{
			IsRemoved = true;
			OnRemove();
		}
		public virtual void OnRemove() { }
	}
	#endregion
}