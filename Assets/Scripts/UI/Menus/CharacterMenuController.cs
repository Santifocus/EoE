using EoE.Controlls;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class CharacterMenuController : MonoBehaviour
	{
		public static CharacterMenuController Instance { get; private set; }
		public bool CharacterMenuOpen { get; private set; }
		[SerializeField] private CharacterMenuPage[] menuPages = default;
		[SerializeField] private float pageScrollTime = 0.3f;
		[SerializeField] private float pageFinishScrollTime = 0.2f;
		[SerializeField] private Image characterMenuBackground = default;
		[SerializeField] private float scrollImageFadeTime = 0.3f;
		[SerializeField] private Image leftScrollImage = default;
		[SerializeField] private Image rightScrollImage = default;

		private int curMenuIndex;
		private bool allowedSlide;
		private Coroutine LeftScrollFadeC;
		private Coroutine RightScrollFadeC;

		private Vector2 LeftScreen => new Vector2(-Screen.width, 0) / transform.lossyScale;
		private Vector2 RightScreen => new Vector2(Screen.width, 0) / transform.lossyScale;

		private void Start()
		{
			allowedSlide = true;
			Instance = this;
			characterMenuBackground.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (!CharacterMenuOpen)
				return;

			if (InputController.LeftPage.Active && allowedSlide)
			{
				allowedSlide = false;

				menuPages[curMenuIndex].HidePage(RightScreen, pageScrollTime);

				curMenuIndex--;
				if (curMenuIndex < 0)
					curMenuIndex += menuPages.Length;

				menuPages[curMenuIndex].ShowPage(LeftScreen, pageScrollTime, AllowSlide);

				if (LeftScrollFadeC != null)
					StopCoroutine(LeftScrollFadeC);
				LeftScrollFadeC = StartCoroutine(FadeTrigger(leftScrollImage));
			}
			else if (InputController.RightPage.Active && allowedSlide)
			{
				allowedSlide = false;

				menuPages[curMenuIndex].HidePage(LeftScreen, pageScrollTime);

				curMenuIndex++;
				if (curMenuIndex >= menuPages.Length)
					curMenuIndex %= menuPages.Length;

				menuPages[curMenuIndex].ShowPage(RightScreen, pageScrollTime, AllowSlide);

				if (RightScrollFadeC != null)
					StopCoroutine(RightScrollFadeC);
				RightScrollFadeC = StartCoroutine(FadeTrigger(rightScrollImage));
			}
		}
		private IEnumerator FadeTrigger(Image target)
		{
			PlayScrollSound();
			target.gameObject.SetActive(true);
			target.color = Color.white;
			float timer = 0;

			while (timer < scrollImageFadeTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.unscaledDeltaTime;
				target.color = new Color(1, 1, 1, 1 - timer / scrollImageFadeTime);
			}
			target.color = new Color(1, 1, 1, 0);
		}
		private void PlayScrollSound()
		{
			Sounds.SoundManager.SetSoundState(ConstantCollector.MENU_SCROLL_SOUND, true);
		}
		public void ToggleState()
		{
			CharacterMenuOpen = !CharacterMenuOpen;
			if (CharacterMenuOpen)
				OpenMenu();
			else
				HideMenu();
		}
		private void OpenMenu()
		{
			GameController.ActivePauses++;
			allowedSlide = false;
			menuPages[0].ShowPage(LeftScreen, pageScrollTime / 2, AllowSlide);
			curMenuIndex = 0;
			characterMenuBackground.gameObject.SetActive(true);
		}
		private void HideMenu()
		{
			GameController.ActivePauses--;
			allowedSlide = false;
			menuPages[curMenuIndex].HidePage(RightScreen, pageScrollTime / 2);
			characterMenuBackground.gameObject.SetActive(false);
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