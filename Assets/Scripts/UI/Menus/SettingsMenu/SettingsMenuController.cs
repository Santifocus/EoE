using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Controlls;
using static EoE.SettingsData;

namespace EoE.UI
{
	public class SettingsMenuController : MonoBehaviour
	{
		private const float AMBIENT_LIGHT_MUL = 0.5f;
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
		[SerializeField] private SettingsSlider brightSlider = default;
		[SerializeField] private GameObject targetAsToggleToggle = default;
		[SerializeField] private GameObject invertCameraXToggle = default;
		[SerializeField] private GameObject invertCameraYToggle = default;

		[SerializeField] private Light topAmbientLight = default;
		[SerializeField] private Light bottomAmbientLight = default;

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
			brightSlider.Value = ActiveBrightness + dir * sliderProgressionAmount;
			ActiveBrightness = brightSlider.Value;
			topAmbientLight.intensity = bottomAmbientLight.intensity = ActiveBrightness * AMBIENT_LIGHT_MUL;
		}
		public void ToggleTargetAsToggle()
		{
			ActiveTargetAsToggle = !ActiveTargetAsToggle;
			targetAsToggleToggle.SetActive(ActiveTargetAsToggle);
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
			topAmbientLight.intensity = bottomAmbientLight.intensity = ActiveBrightness * AMBIENT_LIGHT_MUL;
			SaveOrCreateSettings();
		}
		public void ResetToBase()
		{
			ActiveMusicVolumeScale = MusicVolumeScale;
			ActiveSoundVolumeScale = SoundVolumeScale;
			ActiveBrightness = Brightness;
			ActiveInvertCameraX = InvertCameraX;
			ActiveInvertCameraY = InvertCameraY;

			musicVolumeScaleSlider.SetValueNoLerp(ActiveMusicVolumeScale);
			soundVolumeScaleSlider.SetValueNoLerp(ActiveSoundVolumeScale);
			brightSlider.SetValueNoLerp(ActiveBrightness);
			VolumeScaleChanged();

			topAmbientLight.intensity = bottomAmbientLight.intensity = ActiveBrightness * AMBIENT_LIGHT_MUL;
			targetAsToggleToggle.SetActive(ActiveTargetAsToggle);
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