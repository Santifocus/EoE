using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EoE.UI;

namespace EoE
{
	public class MainMenuController : MonoBehaviour
	{
		[SerializeField] private ControllerMenuItem startMenuItem = default;
		private void Start()
		{
			startMenuItem.Select();
			SettingsMenuController.ChangeStateEvent += SettingsMenuChangedState;
		}
		public void StartGame()
		{
			SceneManager.LoadScene(ConstantCollector.GAME_SCENE_INDEX);
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
		public void OpenCredits()
		{

		}
		public void CloseGame()
		{
			Application.Quit();
		}
		private void OnDisable()
		{
			SettingsMenuController.ChangeStateEvent -= SettingsMenuChangedState;
		}
	}
}