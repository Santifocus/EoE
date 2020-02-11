using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Behaviour.Entities
{
	public class CrawlerHitbox : MonoBehaviour
	{
		[SerializeField] private Collider coll = default;
		private bool collisionActive;
		private Crawler parent;
		public bool CollisionActive
		{
			get => collisionActive;
			set
			{
				coll.enabled = collisionActive = value;
			}
		}
		public void Setup(Crawler parent)
		{
			this.parent = parent;
			CollisionActive = false;
		}
		private void OnTriggerEnter(Collider other)
		{
			parent.HitCollider(other, coll);
		}
	}
}