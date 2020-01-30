using EoE.Entities;
using EoE.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class PlayerPortraitDisplay : MonoBehaviour
	{
		private Sprite this[PortraitState targetState]
		{
			get
			{
				switch(targetState)
				{
					case PortraitState.Normal:
						return normalState;
					case PortraitState.InCombat:
						return lowHealthState;
					case PortraitState.LowHealth:
						return lowHealthState;
					case PortraitState.TookDamage:
						return gotHitState;
					case PortraitState.LeveledUp:
						return levelUpState;

					default:
						return null;
				}
			}
		}
		[SerializeField] private float levelUpDisplayTime = 2f;
		[SerializeField] private float tookDamageDisplayTime = 1f;
		[SerializeField] private Image display = default;
		[SerializeField] private Sprite normalState = default;
		[SerializeField] private Sprite inCombatState = default;
		[SerializeField] private Sprite lowHealthState = default;
		[SerializeField] private Sprite gotHitState = default;
		[SerializeField] private Sprite levelUpState = default;

		private enum PortraitState { None = 0, Normal = 1, InCombat = 2, LowHealth = 3, TookDamage = 4, LeveledUp = 5 }
		private PortraitState mainState;
		private PortraitState topState;

		private float topStateLifeTime;
		private void Start()
		{
			mainState = PortraitState.Normal;
			topState = PortraitState.None;

			EventManager.PlayerTookDamageEvent += PlayerTookDamage;
			EventManager.PlayerLevelupEvent += PlayerLevelup;
		}
		private void Update()
		{
			UpdateMainState();
			UpdateTopState();
		}
		private void UpdateMainState()
		{
			bool lowHealth = (Player.Instance.curHealth / Player.Instance.curMaxHealth) < Player.PlayerSettings.EffectsHealthThreshold;
			PortraitState mainShouldState = lowHealth ? PortraitState.LowHealth : (Player.Instance.curStates.Fighting ? PortraitState.InCombat : PortraitState.Normal);

			if (mainShouldState != mainState)
			{
				mainState = mainShouldState;
				UpdateDisplay();
			}
		}
		private void UpdateTopState()
		{
			if(topStateLifeTime > 0)
			{
				topStateLifeTime -= Time.deltaTime;
				if(topStateLifeTime <= 0)
				{
					topStateLifeTime = 0;
					topState = PortraitState.None;
					UpdateDisplay();
				}
			}
		}
		private void PlayerLevelup()
		{
			topState = PortraitState.LeveledUp;
			topStateLifeTime = levelUpDisplayTime;
			UpdateDisplay();
		}
		private void PlayerTookDamage(float causedDamage, float? knockBack)
		{
			topState = PortraitState.TookDamage;
			topStateLifeTime = tookDamageDisplayTime;
			UpdateDisplay();
		}
		private void UpdateDisplay()
		{
			if(topState == PortraitState.None)
			{
				display.sprite = this[mainState];
			}
			else
			{
				display.sprite = this[topState];
			}
		}
		private void OnDestroy()
		{
			EventManager.PlayerTookDamageEvent -= PlayerTookDamage;
			EventManager.PlayerLevelupEvent -= PlayerLevelup;
		}
	}
}