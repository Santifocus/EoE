using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Sounds
{
	public class SoundManager : MonoBehaviour
	{
		public static SoundManager Instance { get; private set; }
		private SoundPlayer[] soundPlayers;
		private int[] soundHashes;

		private void Start()
		{
			Instance = this;

			soundPlayers = new SoundPlayer[GameController.CurrentGameSettings.globalSounds.Length];
			soundHashes = new int[GameController.CurrentGameSettings.globalSounds.Length];

			for (int i = 0; i < soundPlayers.Length; i++)
			{
				soundPlayers[i] = (new GameObject(GameController.CurrentGameSettings.globalSounds[i].soundName + " Sound Player")).AddComponent<SoundPlayer>();
				soundPlayers[i].Setup(GameController.CurrentGameSettings.globalSounds[i], false);
				soundHashes[i] = GameController.CurrentGameSettings.globalSounds[i].soundName.GetHashCode();
			}
			CheckForDupeNames();
		}

		private void CheckForDupeNames()
		{
#if UNITY_EDITOR
			for (int i = 0; i < soundHashes.Length; i++)
			{
				for (int j = 0; j < soundHashes.Length; j++)
				{
					if(i != j && soundHashes[i] == soundHashes[j])
					{
						Debug.LogError("Global sounds Nr. " + i + " & Nr. " + j + "have identical names (" + GameController.CurrentGameSettings.globalSounds[i].soundName + "). This is not permitted and will cause Errors.");
					}
				}
			}
#endif
		}

		public static void SetSoundState(string soundName, bool state)
		{
			int hash = soundName.GetHashCode();
			for(int i = 0; i < Instance.soundHashes.Length; i++)
			{
				if (hash == Instance.soundHashes[i])
				{
					if(state)
						Instance.soundPlayers[i].Play();
					else
						Instance.soundPlayers[i].Stop();
					return;
				}
			}

			Debug.LogError("Could not find the named sound '" + soundName + "'!");
		}
	}
}