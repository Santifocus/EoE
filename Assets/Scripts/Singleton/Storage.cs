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
		[SerializeField] private Transform dropStorage = default;

		public static Transform ParticleStorage => instance.particleStorage;
		public static Transform EntitieStorage => instance.entitieStorage;
		public static Transform ProjectileStorage => instance.projectileStorage;
		public static Transform DropStorage => instance.dropStorage;

		private void Start()
		{
			instance = this;
		}
	}
}