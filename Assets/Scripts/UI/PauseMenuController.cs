using EoE.Controlls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EoE.UI
{
	public class PauseMenuController : MonoBehaviour
	{
		[SerializeField] private GameObject pauseMenuObject = default;
		[SerializeField] private ControllerMenuItem startMenuItem = default;
		private bool open;
		private void Start()
		{
			pauseMenuObject.SetActive(false);
			SettingsMenuController.ChangeStateEvent += SettingsMenuChangedState;
		}
		private void Update()
		{
			if (InputController.Pause.Down && !(GameController.GameIsPaused && !open))
			{
				SetState(!open);
			}
		}
		public void Close()
		{
			SetState(false);
		}
		private void SetState(bool state)
		{
			open = state;
			GameController.GameIsPaused = open;
			pauseMenuObject.SetActive(open);
			if (open)
				startMenuItem.Select();
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