﻿using System;
using System.Collections;
using UnityEngine;
using EoE.Behaviour;
using EoE.Combatery;
using EoE.Information;
using EoE.UI;

namespace EoE
{
	public enum OnDelayConditionNotMet { ContinueTimerAndInvokeWhenMet, StopTimerTillMet, ResetTimer, Cancel }
	public enum TimeType { ScaledDeltaTime, FixedDeltaTime, Realtime }
	public enum GameDifficulty { Easy = 0, Normal = 1, Hard = 2}
	public class GameController : MonoBehaviour
	{
		public static GameController Instance { get; private set; }
		public static GameSettings CurrentGameSettings => Instance.gameSettings;
		public static Projectile ProjectilePrefab => Instance.projectilePrefab;
		public static Remenants RemenantsPrefab => Instance.remenantsPrefab;
		public static Shield ShieldPrefab => Instance.shieldPrefab;
		public static SoulDrop SoulDropPrefab => Instance.soulDropPrefab;
		public static ShopController Shop => Instance.shop;
		public static ItemCollector ItemCollection => Instance.itemCollector;
		public static bool GameIsPaused { get; private set; }
		public static int ActivePauses 
		{ 
		   get => activePauses;
			set
			{
				activePauses = Math.Max(0, value);
				bool newPauseState = activePauses > 0;
				if (newPauseState != GameIsPaused)
					SetPauseGamestate(newPauseState);
			}
		}
		public static GameDifficulty SetGameDifficulty = GameDifficulty.Easy;
		public static float DifficultyImpact
		{
			get
			{
				switch (SetGameDifficulty)
				{
					case GameDifficulty.Normal:
						return 0;
					case GameDifficulty.Easy:
						return CurrentGameSettings.EasyDifficultyImpact;
					case GameDifficulty.Hard:
						return CurrentGameSettings.HardDifficultyImpact;
				}
				throw new Exception("Invalid Difficulty Level Set: " + SetGameDifficulty);
			}
		}

		private static int activePauses;
		[SerializeField] private GameSettings gameSettings = default;
		[SerializeField] private Projectile projectilePrefab = default;
		[SerializeField] private Remenants remenantsPrefab = default;
		[SerializeField] private Shield shieldPrefab = default;
		[SerializeField] private SoulDrop soulDropPrefab = default;
		[SerializeField] private ShopController shop = default;
		public ItemDrop itemDropPrefab;

		public Transform MainCanvas => mainCanvas;
		[SerializeField] private Transform mainCanvas = default;
		public Transform MenuCanvas => menuCanvas;
		[SerializeField] private Transform menuCanvas = default;

		[Space(10)]
		public Transform enemyHealthBarStorage = default;
		[SerializeField] private ItemCollector itemCollector = default;

		private void Start()
		{
			Instance = this;
			ActivePauses = 0;
			SetPauseGamestate(false);
		}
		private static void SetPauseGamestate(bool state)
		{
			Time.timeScale = state ? 0 : 1;
			GameIsPaused = state;
		}
		public static Coroutine BeginDelayedCall(Action call, float delay, TimeType timeType, Func<bool> condition = null, OnDelayConditionNotMet conditionBehaivior = OnDelayConditionNotMet.ContinueTimerAndInvokeWhenMet)
		{
			return Instance.StartCoroutine(Instance.DelayCallCoroutine(call, delay, timeType, condition, conditionBehaivior));
		}
		private IEnumerator DelayCallCoroutine(Action call, float remainingTime, TimeType timeType, Func<bool> condition, OnDelayConditionNotMet conditionBehaivior)
		{
			float startTime = remainingTime;

			while (remainingTime > 0)
			{
				//Either we wait for physics update or normal frames
				if (timeType == TimeType.FixedDeltaTime)
					yield return new WaitForFixedUpdate();
				else
					yield return new WaitForEndOfFrame();

				//If the conditionbehaivior is any other then ContinueTimerAndInvokeWhenMet then we want to find out if we are currently meeting the condition
				if (conditionBehaivior != OnDelayConditionNotMet.ContinueTimerAndInvokeWhenMet)
				{
					if (!IsConditionMet(condition))
					{
						//Did we fail the condition and the behavior is cancel? jump out of the loop to the end without invoking the call
						if (conditionBehaivior == OnDelayConditionNotMet.Cancel)
						{
							goto Canceled;
						}

						//The 2 other cases both will cause the loop to stay here until the condition is met
						yield return new WaitUntil(condition);

						//If the behaivior is reset then we want to reset the timer after the condition is met again and reset the loop
						if (conditionBehaivior == OnDelayConditionNotMet.ResetTimer)
						{
							remainingTime = startTime;
							//Restart the loop so we skip the end of the loop
							continue;
						}
					}
				}

				//Now remove the passed time from the reamining time
				switch (timeType)
				{
					case TimeType.ScaledDeltaTime:
						remainingTime -= Time.deltaTime;
						break;
					case TimeType.FixedDeltaTime:
						remainingTime -= Time.fixedDeltaTime;
						break;
					case TimeType.Realtime:
						remainingTime -= Time.unscaledDeltaTime;
						break;
				}
			}

			//The remaining time hit zero so we can invoke the function now if the given condition is true
			//If we are here only one condition can possibly stop the invoke: OnDelayConditionNotMet.ContinueTimerAndInvokeWhenMet
			//The other 3 conditions either wouldnt be here or must be true:
			//OnDelayConditionNotMet.Cancel => At the cancel Flag
			//OnDelayConditionNotMet.ResetTimer => At the beginning of the delay loop
			//OnDelayConditionNotMet.StopTimerTillMet => True because it came from a WaitUntil yield instruction
			if (conditionBehaivior == OnDelayConditionNotMet.ContinueTimerAndInvokeWhenMet && !IsConditionMet(condition))
			{
				//If it turns out that the condition is not met then we wait here until it is (or the Coroutine will be canceled from outside)
				yield return new WaitUntil(condition);
			}

			call?.Invoke();

		//If we canceled the whole operation because the condition was not met and the condition behaivior was cancel then we jump here
		Canceled:;
		}
		private bool IsConditionMet(Func<bool> condition)
		{
			//If the condition input is nothing, then it will always be interpreted as true, otherwise we request the bool via invoke
			return condition == null || condition.Invoke();
		}
		private void OnDestroy()
		{
			ActivePauses = 0;
			SetPauseGamestate(false);
			StopAllCoroutines();
		}
	}
}