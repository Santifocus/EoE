using EoE.Controlls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class CreditsController : MonoBehaviour
	{
		[Header("Scroll Settings")]
		[SerializeField] private float scrollSpeed = 5;
		[SerializeField] private float firstWaitTime = 1.5f;
		[SerializeField] private float inputSkipForce = 1;
		[SerializeField] private float inputSkipTime = 1;

		[Space(5)]
		[Header("References")]
		[SerializeField] private GameObject parent = default;
		[SerializeField] private RectTransform scrollTarget = default;
		[SerializeField] private MainMenuController mainMenuController = default;
		[SerializeField] private ControllerMenuItem holderButton = default;

		[Space(5)]
		[Header("Finish")]
		[SerializeField] private RectTransform lastTarget = default;
		[SerializeField] private float fadeTime = 0.5f;

		private bool open;
		private Vector2 startPos;
		private float delay;
		private Graphic[] finishToFade = default;
		private float[] finishToFadeAlphaValues;
		private bool closing;

		private float scrollMul;
		private float curInputSkipTime;
		private void Start()
		{
			startPos = scrollTarget.anchoredPosition;
			parent.SetActive(false);
			finishToFade = parent.GetComponentsInChildren<Graphic>();
			finishToFadeAlphaValues = Utils.FetchAlphaValues(finishToFade);
		}

		public void OpenCredits()
		{
			open = true;
			delay = firstWaitTime;
			scrollTarget.anchoredPosition = startPos;

			scrollMul = 1;
			curInputSkipTime = 0;

			for (int i = 0; i < finishToFade.Length; i++)
			{
				finishToFade[i].color = new Color(	finishToFade[i].color.r,
													finishToFade[i].color.g,
													finishToFade[i].color.b,
													finishToFadeAlphaValues[i]);
			}
			parent.SetActive(true);
			holderButton.Select();
		}

		private void Update()
		{
			if (!open)
				return;

			if (delay > 0)
			{
				delay -= Time.unscaledDeltaTime;
				return;
			}

			if (closing)
				return;

			if (curInputSkipTime > 0)
				curInputSkipTime -= Time.unscaledDeltaTime;

			if (InputController.MenuEnter.Down)
			{
				curInputSkipTime = inputSkipTime;
			}
			scrollMul = Mathf.Lerp(1, 1 + inputSkipForce, curInputSkipTime / inputSkipTime);

			scrollTarget.anchoredPosition += new Vector2(0, Time.unscaledDeltaTime * scrollSpeed * scrollMul);
			if((lastTarget.position.y > Screen.height) || InputController.MenuBack.Down)
			{
				StartCoroutine(CloseCredits());
			}
		}

		private IEnumerator CloseCredits()
		{
			closing = true;

			float timer = 0;
			while(timer < fadeTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.unscaledDeltaTime;

				float alphaMul = 1 - timer / fadeTime;
				for(int i = 0; i < finishToFade.Length; i++)
				{
					finishToFade[i].color = new Color(	finishToFade[i].color.r,
														finishToFade[i].color.g,
														finishToFade[i].color.b,
														alphaMul * finishToFadeAlphaValues[i]);
				}
			}

			closing = false;
			open = false;
			mainMenuController.CreditsClosed();
			parent.SetActive(false);
		}
	}
}