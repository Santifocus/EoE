using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Sounds
{
	public class MusicController : MonoBehaviour
	{
		public static MusicController Instance { get; private set; }

		[SerializeField] private Sound[] MusicList = default;
		[SerializeField] private float musicFadeInTime = 2;
		[SerializeField] private float musicFadeOutTime = 2;

		private SoundPlayer[] musicPlayer;
		private List<MusicInstance> ActiveMusicInstances = new List<MusicInstance>();
		private void Start()
		{
			Instance = this;

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
			ActiveMusicInstances.Add(newInstance);
		}
		public void RemoveMusicInstance(MusicInstance instance)
		{
			ActiveMusicInstances.Remove(instance);
		}
		private void Update()
		{
			//Find the music instance with the highest index that wants to be played
			int targetInstance = -1;
			int highestPriority = -1;
			for(int i = 0; i < ActiveMusicInstances.Count; i++)
			{
				if (!ActiveMusicInstances[i].WantsToPlay)
					continue;

				if(ActiveMusicInstances[i].Priority > highestPriority)
				{
					highestPriority = ActiveMusicInstances[i].Priority;
					targetInstance = -1;
				}
			}

			//Now update the volumes of all instances, the target instance will be increased to 1
			//All other will be decreased to 0
			for (int i = 0; i < ActiveMusicInstances.Count; i++)
			{
				if(i == targetInstance)
				{

				}
				else
				{

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
				if(ActiveMusicInstances[i].Volume > musicPlayer[ActiveMusicInstances[i].MusicIndex].FadePoint)
					musicPlayer[ActiveMusicInstances[i].MusicIndex].FadePoint = ActiveMusicInstances[i].Volume;
			}
		}
	}

	public class MusicInstance
	{
		public bool WantsToPlay;
		public float Volume;
		public int Priority;
		public int MusicIndex;
	}
}