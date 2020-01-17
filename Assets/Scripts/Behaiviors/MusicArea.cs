using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Sounds
{
	public class MusicArea : MonoBehaviour
	{
		[SerializeField] private float radius = 1;
		[SerializeField] private int targetMusicIndex = 0;
		private float radiusSQRT;

		private void Start()
		{
			radiusSQRT = radius * radius;
		}
		private void Update()
		{
			if (Player.Existant)
			{
				float dist = (transform.position - Player.Instance.transform.position).sqrMagnitude;
			}
		}
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(transform.position, radius);
		}
	}
}