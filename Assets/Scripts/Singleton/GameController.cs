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
		public Transform bgCanvas = default;
		public Transform mainCanvas = default;
		public Transform enemyHealthBarStorage = default;

		private void Start()
		{
			if (instance)
			{
				Destroy(instance.gameObject);
			}
			instance = this;
		}
	}
}