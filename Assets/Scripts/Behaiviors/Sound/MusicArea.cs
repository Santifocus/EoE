using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Sounds
{
	public class MusicArea : MonoBehaviour
	{
		[SerializeField] private float radius = 20;
		[Range(0, 1)] [SerializeField] private float startVolume = 0;
		[SerializeField] private int priority = 1;
		[SerializeField] private int targetMusicIndex = 0;
		private MusicInstance selfInstance;

		private void Start()
		{
			selfInstance = new MusicInstance(startVolume, priority, targetMusicIndex);
			CheckRange();
		}
		private void Update()
		{
			CheckRange();
		}
		private void CheckRange()
		{
			if (Player.Existant)
			{
				float dist = (transform.position - Player.Instance.transform.position).sqrMagnitude;
				bool playerInRange = dist < (radius * radius);
				selfInstance.WantsToPlay = playerInRange;

				if (!selfInstance.IsAdded && playerInRange)
				{
					MusicController.Instance.AddMusicInstance(selfInstance);
				}
			}
		}
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(transform.position, radius);
		}
	}
}