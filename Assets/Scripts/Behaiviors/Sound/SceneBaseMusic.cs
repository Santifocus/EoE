using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Sounds
{
	public class SceneBaseMusic : MonoBehaviour
	{
		private static SceneBaseMusic Instance { get; set; }
		[SerializeField] private int musicIndex = default;
		[Range(0, 1)][SerializeField] private float startVolume = default;

		private MusicInstance baseSceneMusicInstance;
		private void Start()
		{
			if (Instance)
			{
				throw new System.Exception("Found more then one SceneBaseMusic in Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + ".");
			}
			Instance = this;

			//Send the instance
			baseSceneMusicInstance = new MusicInstance(startVolume, 0, musicIndex);
			baseSceneMusicInstance.WantsToPlay = true;
			MusicController.Instance.AddMusicInstance(baseSceneMusicInstance);
		}
	}
}