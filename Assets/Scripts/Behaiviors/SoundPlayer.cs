using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Sounds
{
	public class SoundPlayer : MonoBehaviour
	{
		private Sound baseData;
		private AudioSource source;

		private float multiplier;
		public float FadePoint { get => fadePoint; set { fadePoint = value; source.volume = baseData.volume * value * multiplier; } }
		private float fadePoint;

		public void Setup(Sound baseData, float multiplier = 1)
		{
			this.multiplier = multiplier;

			source = gameObject.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.Stop();
			RetrieveData(baseData);
			FadePoint = 1;
		}
		private void RetrieveData(Sound baseData)
		{
			this.baseData = baseData;
			source.clip = baseData.clip;
			source.pitch = baseData.pitch;
			source.loop = baseData.Loop;
			source.spatialBlend = baseData.spatialBlend;
			source.volume = baseData.volume * multiplier;
			source.priority = baseData.priority;
			source.outputAudioMixerGroup = baseData.audioGroup;
		}

		private void Update()
		{
			UpdateScale();
		}
		private void UpdateScale()
		{
			if (baseData.scaleStyle == SoundTimeScale.Unscaled)
				return;

			if (baseData.scaleStyle == SoundTimeScale.Scaled)
			{
				if (GameController.GameIsPaused == source.isPlaying)
				{
					if (GameController.GameIsPaused)
					{
						source.Pause();
					}
					else
					{
						source.UnPause();
					}
				}

				source.pitch = baseData.pitch * Time.timeScale;
			}
			else if (GameController.GameIsPaused == source.isPlaying)
			{
				if (GameController.GameIsPaused)
				{
					source.Pause();
				}
				else
				{
					source.UnPause();
				}
			}
		}
		public void Play()
		{
			if (source.isPlaying)
				source.Stop();
			source.Play();
		}
		public void Stop()
		{
			if (source.isPlaying)
				source.Stop();
		}
	}
}