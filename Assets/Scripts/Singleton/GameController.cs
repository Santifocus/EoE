using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Information;
using EoE.Entities;

namespace EoE
{
	public class GameController : MonoBehaviour
	{
		public static GameController Instance { get; private set; }
		public static GameSettings CurrentGameSettings => Instance.gameSettings;

		private static bool gameIsPaused;
		public static bool GameIsPaused { get => gameIsPaused; set => SetPauseGamestate(value); }

		[SerializeField] private GameSettings gameSettings = default;
		public ItemDrop itemDropPrefab;

		public Transform bgCanvas = default;
		public Transform mainCanvas = default;
		public Transform enemyHealthBarStorage = default;

		private void Start()
		{
			if (Instance)
			{
				Destroy(Instance.gameObject);
			}
			Instance = this;
		}
		private static void SetPauseGamestate(bool state)
		{
			Instance.mainCanvas.gameObject.SetActive(!state);
			Time.timeScale = state ? 0 : 1;
			gameIsPaused = state;
		}
	}
}