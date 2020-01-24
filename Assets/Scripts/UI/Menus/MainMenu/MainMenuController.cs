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
		[SerializeField] private int mainMenuMusicIndex = 0;
		private MusicInstance mainMenuMusic = default;
		private void Start()
		{
			startMenuItem.Select();
			SettingsMenuController.ChangeStateEvent += SettingsMenuChangedState;

			mainMenuMusic = new MusicInstance(1, 7, mainMenuMusicIndex);
			mainMenuMusic.WantsToPlay = true;
			MusicController.Instance.AddMusicInstance(mainMenuMusic);
		}
		public void StartGame()
		{
			if (SceneLoader.Transitioning)
				return;

			SceneLoader.TransitionToScene(ConstantCollector.GAME_SCENE_INDEX);
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