using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE
{
	public class Storage : MonoBehaviour
	{
		private static Storage instance;
		[SerializeField] private Transform particleStorage = default;
		[SerializeField] private Transform entitieStorage = default;
		[SerializeField] private Transform projectileStorage = default;

		public static Transform ParticleStorage => instance.particleStorage;
		public static Transform EntitieStorage => instance.entitieStorage;
		public static Transform ProjectileStorage => instance.projectileStorage;

		private void Start()
		{
			instance = this;
		}
	}
}