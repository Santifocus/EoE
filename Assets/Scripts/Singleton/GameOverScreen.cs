﻿using EoE.Combatery;
using EoE.Controlls;
using EoE.Behaviour.Entities;
using EoE.Events;
using EoE.Sounds;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EoE.UI
{
	public class GameOverScreen : MonoBehaviour
	{
		[SerializeField] private int gameOverMusicIndex = 2;

		[SerializeField] private Image blackPlane = default;
		[SerializeField] private float finalBlackAlpha = 0.9f;
		[SerializeField] private float blackPlaneFadeInTime = 2;

		[SerializeField] private RawImage playerOnlyDisplay = default;
		[SerializeField] private Camera playerOnlyCamera = default;

		[SerializeField] private Graphic[] fadeInGraphics = default;
		[SerializeField] private float graphicsFadeInTime = 2;
		[SerializeField] private ControllerMenuItem startMenuItem = default;

		private MusicInstance gameOverMusic = default;
		private bool fadingIn;
		private float totalFadeTime;
		private float[] alphaValues;

		private void Start()
		{
			gameObject.SetActive(false);
			(transform as RectTransform).anchoredPosition = Vector2.zero;
			EventManager.PlayerDiedEvent += Show;
			gameOverMusic = new MusicInstance(1, 6, gameOverMusicIndex, 1);

			//Fetch alpha values
			alphaValues = new float[fadeInGraphics.Length];
			for (int i = 0; i < fadeInGraphics.Length; i++)
			{
				alphaValues[i] = fadeInGraphics[i].color.a;
				fadeInGraphics[i].color = new Color(fadeInGraphics[i].color.r, fadeInGraphics[i].color.g, fadeInGraphics[i].color.b, 0);
			}
		}
		public void Show(Entity killer)
		{
			RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 1);
			EventManager.PlayerDiedEvent -= Show;

			if (!Player.Existant)
			{
				SetupCamera(rt);
			}
			else
			{
				playerOnlyDisplay.gameObject.SetActive(false);
				graphicsFadeInTime /= 3;
				Player.Instance.Stuns++;
			}

			gameObject.SetActive(true);
			blackPlane.color = Color.clear;
			blackPlane.material.SetTexture("_PushedTexture", rt);

			playerOnlyDisplay.color = Color.clear;
			fadingIn = true;

			gameOverMusic.WantsToPlay = true;
			MusicController.Instance.AddMusicInstance(gameOverMusic);
		}
		private void SetupCamera(RenderTexture rt)
		{
			playerOnlyCamera.targetTexture = rt;
			playerOnlyDisplay.texture = rt;
			playerOnlyCamera.gameObject.SetActive(true);
			playerOnlyCamera.Render();
		}
		private void Update()
		{
			if (!fadingIn)
				return;

			totalFadeTime += Time.unscaledDeltaTime;
			if(totalFadeTime < blackPlaneFadeInTime)
			{
				if (totalFadeTime > blackPlaneFadeInTime)
				{
					totalFadeTime = blackPlaneFadeInTime;
				}
			}

			float blackPlaneAlpha = (totalFadeTime / blackPlaneFadeInTime) * finalBlackAlpha;
			blackPlane.color = new Color(1, 1, 1, Mathf.Clamp01(blackPlaneAlpha));
			playerOnlyDisplay.color = new Color(1, 1, 1, Mathf.Clamp01(blackPlaneAlpha));

			float graphicsAlpha = (totalFadeTime - blackPlaneFadeInTime) / graphicsFadeInTime;
			for(int i = 0; i < fadeInGraphics.Length; i++)
			{
				fadeInGraphics[i].color = new Color(fadeInGraphics[i].color.r, fadeInGraphics[i].color.g, fadeInGraphics[i].color.b, alphaValues[i] * Mathf.Clamp01(graphicsAlpha));
			}

			if(graphicsAlpha >= 1)
			{
				FinishedFading();
			}
		}
		private void FinishedFading()
		{
			fadingIn = false;
			startMenuItem.Select();
			if (Player.Existant)
			{
				Player.Instance.gameObject.SetActive(false);
				if (WeaponController.Instance)
					WeaponController.Instance.gameObject.SetActive(false);
			}
		}
		public void OnRestart()
		{
			if (SceneLoader.Transitioning)
				return;

			SceneLoader.TransitionToScene(SceneManager.GetActiveScene().buildIndex, false);
		}
		public void OnQuit()
		{
			if (SceneLoader.Transitioning)
				return;

			SceneLoader.TransitionToScene(ConstantCollector.MAIN_MENU_SCENE_INDEX, true);
		}
		private void OnDestroy()
		{
			EventManager.PlayerDiedEvent -= Show;
		}
	}
}