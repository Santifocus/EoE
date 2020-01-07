using EoE.Controlls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EoE.UI
{
	public class PauseMenuController : MonoBehaviour
	{
		public static PauseMenuController Instance { get; private set; }
		public bool PauseMenuOpen { get; private set; }
		[SerializeField] private RectTransform pauseMenuObject = default;
		[SerializeField] private ControllerMenuItem startMenuItem = default;
		private void Start()
		{
			pauseMenuObject.gameObject.SetActive(false);
			pauseMenuObject.anchoredPosition = Vector2.zero;
			 Instance = this;
			SettingsMenuController.ChangeStateEvent += SettingsMenuChangedState;
		}
		public void ToggleState()
		{
			SetState(!PauseMenuOpen);
		}
		public void Close()
		{
			SetState(false);
		}
		private void SetState(bool state)
		{
			PauseMenuOpen = state;
			SettingsMenuController.Instance.Close();
			pauseMenuObject.gameObject.SetActive(PauseMenuOpen);
			if (PauseMenuOpen)
				startMenuItem.Select();
			GameController.GameIsPaused = PauseMenuOpen;
		}
		public void OpenSettings()
		{
			SettingsMenuController.Open();
		}
		private void SettingsMenuChangedState(bool state)
		{
			if (!state)
				startMenuItem.OpenSelfLayer();
		}
		public void Quit()
		{
			EffectUtils.ResetScreenEffects();
			SceneManager.LoadScene(ConstantCollector.MAIN_MENU_SCENE_INDEX);
		}
		private void OnDestroy()
		{
			SettingsMenuController.ChangeStateEvent -= SettingsMenuChangedState;
		}
	}
}