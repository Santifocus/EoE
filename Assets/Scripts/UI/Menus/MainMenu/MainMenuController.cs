﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Sounds;

namespace EoE.UI
{
	public class MainMenuController : MonoBehaviour
	{
		[SerializeField] private ControllerMenuItem startMenuItem = default;
		[SerializeField] private UnityEngine.Video.VideoClip introAnimation = default;
		[Space(5)]
		[SerializeField] private int mainMenuMusicIndex = default;
		[SerializeField] private GameObject difficultyCanvas = default;
		[SerializeField] private ControllerMenuItem difficultyRequestStartMenuItem = default;
		[SerializeField] private GameObject tutorialCanvas = default;
		[SerializeField] private ControllerMenuItem tutorialRequestStartMenuItem = default;
		[SerializeField] private CreditsController creditsController = default;

		private MusicInstance mainMenuMusicInstance;
		private void Start()
		{
			startMenuItem.Select();
			SettingsMenuController.ChangeStateEvent += SettingsMenuChangedState;
			mainMenuMusicInstance = new MusicInstance(1, 2, mainMenuMusicIndex);
			mainMenuMusicInstance.WantsToPlay = true;
			MusicController.Instance.AddMusicInstance(mainMenuMusicInstance);
		}
		public void RequestStartGame()
		{
			difficultyCanvas.SetActive(true);
			difficultyRequestStartMenuItem.Select();
		}
		public void StartGame()
		{
			if (SceneLoader.Transitioning)
				return;

			AnimationSceneController.RequestAnimation(introAnimation, ConstantCollector.GAME_SCENE_INDEX, true, false, true);
		}
		public void StartTuorial()
		{
			if (SceneLoader.Transitioning)
				return;

			SceneLoader.TransitionToScene(ConstantCollector.TUTORIAL_SCENE_INDEX, true);
		}
		public void CancelStartRequest()
		{
			difficultyCanvas.SetActive(false);
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
			creditsController.OpenCredits();
		}
		public void CreditsClosed()
		{
			startMenuItem.OpenSelfLayer();
		}
		public void SetDifficulty(int difficulty)
		{
			GameController.SetGameDifficulty = (GameDifficulty)difficulty;

			difficultyCanvas.SetActive(false);
			tutorialCanvas.SetActive(true);
			tutorialRequestStartMenuItem.Select();
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