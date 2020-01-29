using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Entities;

namespace EoE.Information
{
	public class ItemGiver : MonoBehaviour
	{
		[SerializeField] private PlayerItemChange[] itemsToGive = default;
		private void Start()
		{
			for(int i = 0; i < itemsToGive.Length; i++)
			{
				itemsToGive[i].Activate();
			}

			Destroy(gameObject);
		}
	}
}