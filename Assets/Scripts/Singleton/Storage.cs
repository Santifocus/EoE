using UnityEngine;

namespace EoE
{
	public class Storage : MonoBehaviour
	{
		private static Storage Instance;
		[SerializeField] private Transform particleStorage = default;
		[SerializeField] private Transform soundStorage = default;
		[SerializeField] private Transform projectileStorage = default;
		[SerializeField] private Transform dropStorage = default;

		public static Transform ParticleStorage => Instance.particleStorage;
		public static Transform SoundStorage => Instance.soundStorage;
		public static Transform ProjectileStorage => Instance.projectileStorage;
		public static Transform DropStorage => Instance.dropStorage;

		private void Start()
		{
			Instance = this;
		}
	}
}