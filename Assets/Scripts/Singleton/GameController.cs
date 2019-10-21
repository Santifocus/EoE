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
		[SerializeField] private DamageNumber damageNumberPrefab = default;
		private PoolableObject<DamageNumber> damageNumberPool;

		private void Start()
		{
			if (instance)
			{
				Destroy(instance.gameObject);
			}
			damageNumberPool = new PoolableObject<DamageNumber>(50, true, damageNumberPrefab, Storage.ParticleStorage);
			instance = this;
		}

		public void CreateDamageNumber(Vector3 startPosition, Gradient colors, Vector3 numberVelocity, float damage, bool wasCrit, float overrideScale = 1)
		{
			DamageNumber newDamageNumber = damageNumberPool.GetPoolObject();
			newDamageNumber.transform.position = startPosition;
			newDamageNumber.transform.localScale = Vector3.one * overrideScale;
			string displayedNumber = (Mathf.Round(damage * 100) / 100).ToString();
			newDamageNumber.BeginDisplay(numberVelocity, colors, displayedNumber, wasCrit);
		}

		public void DisplayInfoText(Vector3 startPosition, Gradient colors, Vector3 numberVelocity, string text, float overrideScale = 1)
		{
			DamageNumber newDamageNumber = damageNumberPool.GetPoolObject();
			newDamageNumber.transform.position = startPosition;
			newDamageNumber.transform.localScale = Vector3.one * overrideScale;
			newDamageNumber.BeginDisplay(numberVelocity, colors, text, false);
		}

	#if UNITY_EDITOR
		private void OnApplicationQuit()
		{

		}
	#endif
	}
}