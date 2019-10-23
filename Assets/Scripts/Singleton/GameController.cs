using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Information;
using EoE.Entities;

namespace EoE
{
	public class GameController : MonoBehaviour
	{
		private static GameController instance;
		public static GameController Instance => instance;
		public static GameSettings CurrentGameSettings => instance.gameSettings;

		[SerializeField] private GameSettings gameSettings = default;

		private void Start()
		{
			if (instance)
			{
				Destroy(instance.gameObject);
			}
			instance = this;
		}

	#if UNITY_EDITOR
		private void OnApplicationQuit()
		{

		}
	#endif
	}
}