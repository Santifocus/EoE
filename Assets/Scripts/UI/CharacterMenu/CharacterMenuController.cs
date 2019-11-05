using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EoE.Controlls;

namespace EoE.UI
{
	public class CharacterMenuController : MonoBehaviour
	{
		public static bool MenuOpen;
		[SerializeField] private CharacterMenuPage[] menuPages = default;
		[SerializeField] private float pageScrollTime = 0.3f;
		[SerializeField] private float pageFinishScrollTime = 0.2f;

		private int curMenuIndex;
		private bool allowedSlide;
		private TransitionUI.OnFinishCall pageFinishSlide;

		private Vector2 LeftScreen => new Vector2(-Screen.width, 0) / transform.lossyScale;
		private Vector2 RightScreen => new Vector2(Screen.width, 0) / transform.lossyScale;

		private void Start()
		{
			allowedSlide = true;
			pageFinishSlide += AllowSlide;
		}

		private void Update()
		{
			if (!MenuOpen && InputController.PlayerMenu.Down)
			{
				OpenMenu();
				return;
			}

			if (!MenuOpen)
				return;

			if (InputController.MenuPlayerMenu.Down)
			{
				HideMenu();
				return;
			}

			if (InputController.LeftPage.Active && allowedSlide)
			{
				allowedSlide = false;

				menuPages[curMenuIndex].HidePage(RightScreen, pageScrollTime);

				curMenuIndex--;
				if (curMenuIndex < 0)
					curMenuIndex += menuPages.Length;

				menuPages[curMenuIndex].ShowPage(LeftScreen, pageScrollTime, pageFinishSlide);
			}
			else if (InputController.RightPage.Active && allowedSlide)
			{
				allowedSlide = false;

				menuPages[curMenuIndex].HidePage(LeftScreen, pageScrollTime);

				curMenuIndex++;
				if (curMenuIndex >= menuPages.Length)
					curMenuIndex %= menuPages.Length;

				menuPages[curMenuIndex].ShowPage(RightScreen, pageScrollTime, pageFinishSlide);
			}
		}
		public void OpenMenu()
		{
			MenuOpen = true;
			GameController.GameIsPaused = true;
			allowedSlide = false;
			menuPages[0].ShowPage(LeftScreen, pageScrollTime / 2, pageFinishSlide);
			curMenuIndex = 0;
		}
		public void HideMenu()
		{
			MenuOpen = false;
			GameController.GameIsPaused = false;
			allowedSlide = false;
			menuPages[curMenuIndex].HidePage(RightScreen, pageScrollTime / 2);
		}
		private void AllowSlide()
		{
			StartCoroutine(DelayAllow());
		}
		private IEnumerator DelayAllow()
		{
			yield return new WaitForSecondsRealtime(pageFinishScrollTime);
			allowedSlide = true;
		}
	}
}