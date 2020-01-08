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
		[SerializeField] private Graphic[] fadeInComponents = default;
		[SerializeField] private float fadeInTime = 2;
		[SerializeField] private ControllerMenuItem startMenuItem = default;

		private float[] alphaValues;
		private bool fadingIn;
		private void Start()
		{
			gameObject.SetActive(false);
			(transform as RectTransform).anchoredPosition = Vector2.zero;
			EventManager.PlayerDiedEvent += Show;
		}
		public void Show(Entities.Entitie killer)
		{
			gameObject.SetActive(true);
			StartCoroutine(StartFadeIn());
		}
		private void OnDestroy()
		{
			EventManager.PlayerDiedEvent -= Show;
		}
		private void Update()
		{
			if (fadingIn)
			{
				if (InputController.MenuRight.Down || InputController.MenuLeft.Down || InputController.MenuEnter.Down)
				{
					StopAllCoroutines();
					FinishFadeIn();
					startMenuItem.SetNavigationCooldown(0.3f);
				}
				return;
			}
		}
		private IEnumerator StartFadeIn()
		{
			fadingIn = true;
			alphaValues = new float[fadeInComponents.Length];
			for (int i = 0; i < fadeInComponents.Length; i++)
			{
				Color col = fadeInComponents[i].color;
				alphaValues[i] = col.a;
				fadeInComponents[i].color = new Color(col.r, col.g, col.b, 0);
			}

			float timer = 0;
			while (timer < fadeInTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;

				float point = timer / fadeInTime;
				for (int i = 0; i < fadeInComponents.Length; i++)
				{
					Color col = fadeInComponents[i].color;
					fadeInComponents[i].color = new Color(col.r, col.g, col.b, point * alphaValues[i]);
				}
			}
			FinishFadeIn();
		}
		private void FinishFadeIn()
		{
			fadingIn = false;
			for (int i = 0; i < fadeInComponents.Length; i++)
			{
				Color col = fadeInComponents[i].color;
				fadeInComponents[i].color = new Color(col.r, col.g, col.b, alphaValues[i]);
			}
			startMenuItem.Select();
		}
		public void OnRestart()
		{
			EffectUtils.ResetScreenEffects();
			SceneManager.LoadScene(ConstantCollector.GAME_SCENE_INDEX);
		}
		public void OnQuit()
		{
			EffectUtils.ResetScreenEffects();
			SceneManager.LoadScene(ConstantCollector.MAIN_MENU_SCENE_INDEX);
		}
	}
}