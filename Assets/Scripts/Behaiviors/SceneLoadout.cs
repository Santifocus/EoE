using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Entities;

namespace EoE.Information
{
	public class SceneLoadout : MonoBehaviour
	{
		[SerializeField] private PlayerItemChange[] itemsToGive = default;
		[SerializeField] private int startCurrency = 0;
		private void Start()
		{
			for(int i = 0; i < itemsToGive.Length; i++)
			{
				itemsToGive[i].Activate();
			}
			Player.Instance.ChangeCurrency(startCurrency);
			Destroy(gameObject);
		}
	}
}