using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EoE.Sounds
{
	public class MusicController : MonoBehaviour
	{
		private const float REMOVE_THRESHOLD = 0.01f;
		public static MusicController Instance { get; private set; }

		[SerializeField] private Sound[] MusicList = default;
		[SerializeField] private float musicFadeInTime = 2;
		[SerializeField] private float musicFadeOutTime = 2;

		private SoundPlayer[] musicPlayer;
		private List<MusicInstance> ActiveMusicInstances = new List<MusicInstance>();
		private void Start()
		{
			Instance = this;
			SceneManager.sceneLoaded += OnSceneLoaded;

			//Setup Music player
			musicPlayer = new SoundPlayer[MusicList.Length];

			for (int i = 0; i < MusicList.Length; i++)
			{
				SoundPlayer soundPlayer = (new GameObject(MusicList[i].soundName + " Sound Player")).AddComponent<SoundPlayer>();

				soundPlayer.transform.SetParent(transform);
				soundPlayer.Setup(MusicList[i]);

				musicPlayer[i] = soundPlayer;
			}
		}
		public void AddMusicInstance(MusicInstance newInstance)
		{
			newInstance.IsAdded = true;
			ActiveMusicInstances.Add(newInstance);
		}
		private void RemoveMusicInstance(MusicInstance instance)
		{
			instance.IsAdded = false;
			ActiveMusicInstances.Remove(instance);
		}
		private void Update()
		{
			//Find the music instance with the highest index that wants to be played
			int targetInstanceIndex = -1;
			int highestPriority = -1;
			for(int i = 0; i < ActiveMusicInstances.Count; i++)
			{
				if (!ActiveMusicInstances[i].WantsToPlay)
				{
					if (ActiveMusicInstances[i].Volume < REMOVE_THRESHOLD)
					{
						RemoveMusicInstance(ActiveMusicInstances[i]);
						i--;
					}
					continue;
				}

				if(ActiveMusicInstances[i].Priority > highestPriority)
				{
					highestPriority = ActiveMusicInstances[i].Priority;
					targetInstanceIndex = i;
				}
			}

			//Now update the volumes of all instances, the target instance will be increased to 1
			//All other will be decreased to 0
			float volumeIncrease = (musicFadeInTime > 0) ? (Time.unscaledDeltaTime / musicFadeInTime) : 1;
			float volumeDecrease = (musicFadeOutTime > 0) ? (Time.unscaledDeltaTime / musicFadeOutTime) : 1;
			for (int i = 0; i < ActiveMusicInstances.Count; i++)
			{
				if(i == targetInstanceIndex)
				{
					ActiveMusicInstances[i].Volume = Mathf.Min(1, ActiveMusicInstances[i].Volume + volumeIncrease);
				}
				else
				{
					ActiveMusicInstances[i].Volume = Mathf.Max(0, ActiveMusicInstances[i].Volume - volumeDecrease);
				}
			}

			//Now we apply the highest volume of all instances
			//To do that we first set all volumes to 0
			for(int i = 0; i < musicPlayer.Length; i++)
			{
				musicPlayer[i].FadePoint = 0;
			}
			//Then we go througth all instances and apply their volume if it is higher then the current volume
			for(int i = 0; i < ActiveMusicInstances.Count; i++)
			{
				if (musicPlayer[ActiveMusicInstances[i].MusicIndex].FullyStopped)
				{
					if (ActiveMusicInstances[i].AllowedToRestart)
					{
						ActiveMusicInstances[i].TotalRestarts++;
					}
					else
					{
						continue;
					}
				}
				if(ActiveMusicInstances[i].Volume > musicPlayer[ActiveMusicInstances[i].MusicIndex].FadePoint)
				{
					musicPlayer[ActiveMusicInstances[i].MusicIndex].FadePoint = ActiveMusicInstances[i].Volume;
				}
			}

			//Set the state of the music players based on if there volume is above zero
			for (int i = 0; i < musicPlayer.Length; i++)
			{
				if(musicPlayer[i].FadePoint > 0)
				{
					if (musicPlayer[i].FullyStopped)
					{
						musicPlayer[i].Play();
					}
				}
				else
				{
					musicPlayer[i].Stop();
				}
			}
		}
		public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			for(int i = 0; i < ActiveMusicInstances.Count; i++)
			{
				ActiveMusicInstances[i].WantsToPlay = false;
			}
		}
	}

	public class MusicInstance
	{
		public bool WantsToPlay;
		public bool IsAdded;
		public float Volume;
		public int Priority;
		public int MusicIndex;

		public bool AllowedToRestart => (MaxRestarts == -1) || (TotalRestarts < MaxRestarts);
		private int MaxRestarts;
		public int TotalRestarts;

		public MusicInstance(float volume, int priority, int musicIndex, int maxRestarts = -1)
		{
			WantsToPlay = false;
			Volume = volume;
			Priority = priority;
			MusicIndex = musicIndex;
			MaxRestarts = maxRestarts;
		}
	}
}