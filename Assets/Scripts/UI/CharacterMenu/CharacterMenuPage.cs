using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public abstract class CharacterMenuPage : MonoBehaviour
	{
		[SerializeField] protected TransitionUI pageSlider = default;
		protected bool ActivePage;
		protected virtual void Start()
		{
			gameObject.SetActive(false);
		}
		public void ShowPage(Vector2 slideStart, float slideTime, TransitionUI.OnFinishCall finishCall)
		{
			ActivePage = true;
			ResetPage();
			pageSlider.rTransform.anchoredPosition = slideStart;
			pageSlider.CustomTransition(Vector2.zero, finishCall, slideTime);
		}
		public void HidePage(Vector2 slideEnd, float slideTime)
		{
			ActivePage = false;
			pageSlider.CustomTransition(slideEnd, () => DeactivatePage(), slideTime);
		}
		protected abstract void ResetPage();
		protected abstract void DeactivatePage();
	}
}