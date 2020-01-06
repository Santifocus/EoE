﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Controlls;
using static EoE.SettingsData;

namespace EoE.UI
{
	public class SettingsMenuController : MonoBehaviour
	{
		public static SettingsMenuController Instance { get; private set; }

		public delegate void SettingsMenuSetState(bool state);
		public static SettingsMenuSetState ChangeStateEvent;

		public delegate void VolumeScalesChanged();
		public static VolumeScalesChanged VolumeScalesChangedEvent;

		[SerializeField] private ControllerMenuItem startMenuItem = default;
		[SerializeField] private float sliderProgressionAmount = 0.05f;

		[SerializeField] private GameObject settingsCanvas = default;
		[SerializeField] private SettingsSlider musicVolumeScaleSlider = default;
		[SerializeField] private SettingsSlider soundVolumeScaleSlider = default;
		[SerializeField] private SettingsSlider gammaSlider = default;
		[SerializeField] private GameObject invertCameraXToggle = default;
		[SerializeField] private GameObject invertCameraYToggle = default;

		private void Start()
		{
			Instance = this;

			ReadOrCreate();
			ResetToBase();
		}
		public static void Open()
		{
			if (Instance)
				Instance.OpenInternal();
		}
		private void OpenInternal()
		{
			ResetToBase();
			startMenuItem.Select();
			settingsCanvas.SetActive(true);
			ChangeStateEvent?.Invoke(true);
		}
		public void Close()
		{
			settingsCanvas.SetActive(false);
			ChangeStateEvent?.Invoke(false);
		}
		public void ChangeMusicScale(int dir)
		{
			musicVolumeScaleSlider.Value = ActiveMusicVolumeScale + dir * sliderProgressionAmount;
			ActiveMusicVolumeScale = musicVolumeScaleSlider.Value;
			VolumeScaleChanged();
		}
		public void ChangeSoundScale(int dir)
		{
			soundVolumeScaleSlider.Value = ActiveSoundVolumeScale + dir * sliderProgressionAmount;
			ActiveSoundVolumeScale = soundVolumeScaleSlider.Value;
			VolumeScaleChanged();
		}
		public void ChangeGamma(int dir)
		{
			gammaSlider.Value = ActiveGamma + dir * sliderProgressionAmount;
			ActiveGamma = gammaSlider.Value;
			RenderSettings.ambientLight = new Color(ActiveGamma, ActiveGamma, ActiveGamma, 1);
		}
		public void ToggleXInvert()
		{
			ActiveInvertCameraX = !ActiveInvertCameraX;
			invertCameraXToggle.SetActive(ActiveInvertCameraX);
		}
		public void ToggleYInvert()
		{
			ActiveInvertCameraY = !ActiveInvertCameraY;
			invertCameraYToggle.SetActive(ActiveInvertCameraY);
		}
		public void Apply()
		{
			VolumeScaleChanged();
			RenderSettings.ambientLight = new Color(ActiveGamma, ActiveGamma, ActiveGamma, 1);
			SaveSettings();
		}
		public void ResetToBase()
		{
			ActiveMusicVolumeScale = MusicVolumeScale;
			ActiveSoundVolumeScale = SoundVolumeScale;
			ActiveGamma = Gamma;
			ActiveInvertCameraX = InvertCameraX;
			ActiveInvertCameraY = InvertCameraY;

			musicVolumeScaleSlider.SetValueNoLerp(ActiveMusicVolumeScale);
			soundVolumeScaleSlider.SetValueNoLerp(ActiveSoundVolumeScale);
			gammaSlider.SetValueNoLerp(ActiveGamma);
			VolumeScaleChanged();

			RenderSettings.ambientLight = new Color(ActiveGamma, ActiveGamma, ActiveGamma, 1);
			invertCameraXToggle.SetActive(ActiveInvertCameraX);
			invertCameraYToggle.SetActive(ActiveInvertCameraY);
		}
		public void FullSettingsReset()
		{
			ResetSettings();
			ResetToBase();
		}
		private void VolumeScaleChanged()
		{
			VolumeScalesChangedEvent?.Invoke();
		}
	}
}