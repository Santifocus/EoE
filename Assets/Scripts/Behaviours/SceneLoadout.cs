using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Behaviour.Entities;

namespace EoE.Information
{
	public class SceneLoadout : MonoBehaviour
	{
		[SerializeField] private PlayerItemChange[] itemsToGive = default;
		[SerializeField] private int startCurrency = 0;
		private void Start()
		{
			if (!Player.Existant)
				return;

			for(int i = 0; i < itemsToGive.Length; i++)
			{
				itemsToGive[i].Activate();
			}
			Player.Instance.ChangeCurrency(startCurrency);
			Destroy(gameObject);
		}
	}
}