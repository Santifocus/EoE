using System.Collections.Generic;
using UnityEngine;

namespace EoE.Sounds
{
	public class SoundManager : MonoBehaviour
	{
		public static SoundManager Instance { get; private set; }
		[SerializeField] private GlobalSounds globalSoundsSettings = default;
		private Dictionary<string, SoundPlayer> globalSounds;

		private void Start()
		{
			Instance = this;

			globalSounds = new Dictionary<string, SoundPlayer>(globalSoundsSettings.Sounds.Length);

			for (int i = 0; i < globalSoundsSettings.Sounds.Length; i++)
			{
				if (!globalSounds.ContainsKey(globalSoundsSettings.Sounds[i].soundName))
				{
					SoundPlayer soundPlayer =(new GameObject(globalSoundsSettings.Sounds[i].soundName + " Sound Player")).AddComponent<SoundPlayer>();
	
					soundPlayer.transform.SetParent(transform);
					soundPlayer.Setup(globalSoundsSettings.Sounds[i]);

					globalSounds.Add(globalSoundsSettings.Sounds[i].soundName, soundPlayer);
				}
				else
				{
					Debug.LogError("Global sound Nr. " + i + " has a duplicate name: " + globalSoundsSettings.Sounds[i].soundName + ". This is not permitted and will cause Errors.");
				}
			}
		}

		public static void SetSoundState(string soundName, bool state)
		{
			if(Instance.globalSounds.TryGetValue(soundName, out SoundPlayer result))
			{
				if (result.Exists)
				{
					if (state)
						result.Play();
					else
						result.Stop();
				}
			}
			else
			{
				Debug.LogError("Could not find the named sound '" + soundName + "'!");
			}
		}
	}
}