using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EoE.UI;
using EoE.Sounds;

namespace EoE
{
	public class MainMenuController : MonoBehaviour
	{
		[SerializeField] private ControllerMenuItem startMenuItem = default;
		[Space(5)]
		[SerializeField] private GameObject tutorialCanvas = default;
		[SerializeField] private ControllerMenuItem tutorialRequestStartMenuItem = default;
		private void Start()
		{
			startMenuItem.Select();
			SettingsMenuController.ChangeStateEvent += SettingsMenuChangedState;
		}
		public void RequestStartGame()
		{
			tutorialCanvas.SetActive(true);
			tutorialRequestStartMenuItem.Select();
		}
		public void StartGame()
		{
			if (SceneLoader.Transitioning)
				return;

			SceneLoader.TransitionToScene(ConstantCollector.GAME_SCENE_INDEX, true);
		}
		public void StartTuorial()
		{
			if (SceneLoader.Transitioning)
				return;

			SceneLoader.TransitionToScene(ConstantCollector.TUTORIAL_SCENE_INDEX, true);
		}
		public void CancelTutorialRequest()
		{
			tutorialCanvas.SetActive(false);
			startMenuItem.OpenSelfLayer();
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
		private void OnDestroy()
		{
			SettingsMenuController.ChangeStateEvent -= SettingsMenuChangedState;
		}
	}
}