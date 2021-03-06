﻿using UnityEngine;

namespace EoE.UI
{
	public abstract class CharacterMenuPage : MonoBehaviour
	{
		protected const float NAV_COOLDOWN = 0.2f;
		[SerializeField] protected TransitionUI pageSlider = default;
		protected bool ActivePage;
		protected virtual void Start()
		{
			gameObject.SetActive(false);
		}
		public void ShowPage(Vector2 slideStart, float slideTime, System.Action finishCall)
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