using EoE.Controlls;
using EoE.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class GameOverScreen : MonoBehaviour
	{
		private const float NAV_COOLDOWN = 0.2f;
		[SerializeField] private Graphic[] fadeInComponents = default;
		[SerializeField] private float fadeInTime = 2;

		[SerializeField] private CMenuItem[] gameOverOptions = default;
		private int selectedIndex;

		private float[] alphaValues;
		private bool fadingIn;
		private float navigationCooldown;
		private void Start()
		{
			gameObject.SetActive(false);
			EventManager.PlayerDiedEvent += Show;
		}
		public void Show(Entities.Entitie killer)
		{
			selectedIndex = 0;
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
				if(InputController.MenuRight.Down || InputController.MenuLeft.Down || InputController.MenuEnter.Down)
				{
					StopAllCoroutines();
					SelectIndex();
					FinishFadeIn();
				}
				return;
			}

			if(navigationCooldown > 0)
			{
				navigationCooldown -= Time.deltaTime;
				return;
			}

			if (InputController.MenuRight.Pressed)
			{
				navigationCooldown = NAV_COOLDOWN;
				selectedIndex++;
				SelectIndex();
			}
			else if (InputController.MenuLeft.Pressed)
			{
				navigationCooldown = NAV_COOLDOWN;
				selectedIndex--;
				SelectIndex();
			}
		}
		private void SelectIndex()
		{
			if (selectedIndex < 0)
				selectedIndex += gameOverOptions.Length;
			else if (selectedIndex >= gameOverOptions.Length)
				selectedIndex = 0;

			gameOverOptions[selectedIndex].SelectMenuItem();
		}
		private IEnumerator StartFadeIn()
		{
			fadingIn = true;
			alphaValues = new float[fadeInComponents.Length];
			for(int i = 0; i < fadeInComponents.Length; i++)
			{
				Color col = fadeInComponents[i].color;
				alphaValues[i] = col.a;
				fadeInComponents[i].color = new Color(col.r, col.g, col.b, 0);
			}

			float timer = 0;
			while(timer < fadeInTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;

				float point = timer / fadeInTime;
				for(int i  = 0; i < fadeInComponents.Length; i++)
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
		}
	}
}