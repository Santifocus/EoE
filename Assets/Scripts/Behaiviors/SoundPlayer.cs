using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Sounds
{
	public class SoundPlayer : MonoBehaviour
	{
		private Sound baseData;
		private AudioSource source;
		private bool isPaused;
		public void Setup(Sound baseData, bool destroyOnFinish)
		{
			this.baseData = baseData;

			source = gameObject.AddComponent<AudioSource>();

			source.playOnAwake = false;
			source.clip = baseData.clip;
			source.pitch = baseData.pitch;
			source.spatialBlend = baseData.spatialBlend;
			source.volume = baseData.volume;
			source.priority = baseData.priority;
			source.outputAudioMixerGroup = baseData.audioGroup;

			source.Stop();

			if (destroyOnFinish)
				StartCoroutine(WaitForEnd());
		}

		private IEnumerator WaitForEnd()
		{
			do
			{
				yield return new WaitForEndOfFrame();
			} while (source.isPlaying || isPaused);

			Destroy(gameObject);
		}

		private void Update()
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
						isPaused = true;
					}
					else
					{ 
						source.UnPause();
						isPaused = true;
					}
				}

				source.pitch = baseData.pitch * Time.timeScale;
			}
			else if (GameController.GameIsPaused == source.isPlaying)
			{
				if (GameController.GameIsPaused)
				{
					source.Pause();
					isPaused = true;
				}
				else
				{
					source.UnPause();
					isPaused = true;
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