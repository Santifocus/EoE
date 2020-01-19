using EoE.Controlls;
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
		public void Show(Entities.Entity killer)
		{
			SetupCamera();

			gameObject.SetActive(true);
			blackPlane.color = Color.clear;
			playerOnlyDisplay.color = Color.clear;
			fadingIn = true;

			gameOverMusic.WantsToPlay = true;
			MusicController.Instance.AddMusicInstance(gameOverMusic);
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

			totalFadeTime += Time.unscaledDeltaTime;
			if(totalFadeTime < blackPlaneFadeInTime)
			{
				if (totalFadeTime > blackPlaneFadeInTime)
				{
					totalFadeTime = blackPlaneFadeInTime;
				}
			}

			float blackPlaneAlpha = (totalFadeTime / blackPlaneFadeInTime) * finalBlackAlpha;
			blackPlane.color = new Color(0, 0, 0, Mathf.Clamp01(blackPlaneAlpha));
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
		}
		public void OnRestart()
		{
			GameController.ActivePauses = 0;
			EffectManager.ResetScreenEffects();
			SceneManager.LoadScene(ConstantCollector.GAME_SCENE_INDEX);
		}
		public void OnQuit()
		{
			GameController.ActivePauses = 0;
			EffectManager.ResetScreenEffects();
			SceneManager.LoadScene(ConstantCollector.MAIN_MENU_SCENE_INDEX);
		}
		private void OnDestroy()
		{
			EventManager.PlayerDiedEvent -= Show;
		}
	}
}