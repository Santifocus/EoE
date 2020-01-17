using EoE.Controlls;
using EoE.Events;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EoE.UI
{
	public class GameOverScreen : MonoBehaviour
	{
		[SerializeField] private Image blackPlane = default;
		[SerializeField] private float finalBlackAlpha = 0.9f;
		[SerializeField] private RawImage playerOnlyDisplay = default;
		[SerializeField] private Camera playerOnlyCamera = default;
		[SerializeField] private float fadeInTime = 2;
		[SerializeField] private ControllerMenuItem startMenuItem = default;
		[SerializeField] private GameObject[] onFadeFinishEnable = default;

		private bool fadingIn;
		private float fadedTime;

		private void Start()
		{
			gameObject.SetActive(false);
			(transform as RectTransform).anchoredPosition = Vector2.zero;
			EventManager.PlayerDiedEvent += Show;
			for (int i = 0; i < onFadeFinishEnable.Length; i++)
			{
				onFadeFinishEnable[i].SetActive(false);
			}
		}
		public void Show(Entities.Entity killer)
		{
			SetupCamera();

			gameObject.SetActive(true);
			blackPlane.color = Color.clear;
			playerOnlyDisplay.color = Color.clear;
			fadingIn = true;

		}
		private void SetupCamera()
		{
			RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 1);

			playerOnlyCamera.targetTexture = rt;
			playerOnlyDisplay.texture = rt;
			playerOnlyCamera.gameObject.SetActive(true);
			playerOnlyCamera.Render();
		}
		private void Update()
		{
			if (!fadingIn)
				return;

			if(fadedTime < fadeInTime)
			{
				fadedTime += Time.unscaledDeltaTime;
				if (fadedTime > fadeInTime)
				{
					fadedTime = fadeInTime;
					FinishedFading();
				}
			}

			float alpha = (fadedTime / fadeInTime) * finalBlackAlpha;
			blackPlane.color = new Color(0, 0, 0, alpha);
			playerOnlyDisplay.color = new Color(1, 1, 1, alpha);
		}
		private void FinishedFading()
		{
			fadingIn = false;
			for(int i = 0; i < onFadeFinishEnable.Length; i++)
			{
				onFadeFinishEnable[i].SetActive(true);
			}

			startMenuItem.Select();
		}
		public void OnRestart()
		{
			GameController.ActivePauses = 0;
			EffectUtils.ResetScreenEffects();
			SceneManager.LoadScene(ConstantCollector.GAME_SCENE_INDEX);
		}
		public void OnQuit()
		{
			GameController.ActivePauses = 0;
			EffectUtils.ResetScreenEffects();
			SceneManager.LoadScene(ConstantCollector.MAIN_MENU_SCENE_INDEX);
		}
		private void OnDisable()
		{
			EventManager.PlayerDiedEvent -= Show;
		}
	}
}