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
		private bool isPaused;

		private float multiplier;

		private bool following;
		private Transform toFollow;
		private Vector3 offset;
		private BehaiviorOnParentDeath onParentDeathBehaivior;
		private float fadeOutTime;

		public void Setup(Sound baseData, bool destroyOnFinish, float multiplier = 1)
		{
			this.baseData = baseData;
			this.multiplier = multiplier;

			source = gameObject.AddComponent<AudioSource>();

			source.playOnAwake = false;
			source.clip = baseData.clip;
			source.pitch = baseData.pitch;
			source.spatialBlend = baseData.spatialBlend;
			source.volume = baseData.volume * multiplier;
			source.priority = baseData.priority;
			source.outputAudioMixerGroup = baseData.audioGroup;

			source.Stop();

			if (destroyOnFinish)
				StartCoroutine(WaitForEnd());
		}
		public void FollowTargetSetup(Transform toFollow, Vector3 offset, BehaiviorOnParentDeath onParentDeathBehaivior, float fadeOutTime)
		{
			following = true;
			this.toFollow = toFollow;
			this.offset = offset;
			this.onParentDeathBehaivior = onParentDeathBehaivior;
			this.fadeOutTime = fadeOutTime;
		}

		private IEnumerator WaitForEnd()
		{
			do
			{
				yield return new WaitForEndOfFrame();
			} while (source.isPlaying || isPaused);

			Destroy(gameObject);
		}

		private IEnumerator FadeOut()
		{
			float timer = 0;
			while(timer < fadeOutTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;
				float point = 1 - timer / fadeOutTime;

				source.volume = baseData.volume * point * multiplier;
			}
			Destroy(gameObject);
		}

		private void Update()
		{
			UpdateScale();
		}
		private void FixedUpdate()
		{
			FollowTarget();
		}
		private void FollowTarget()
		{
			if (!following)
				return;

			if (toFollow)
			{
				transform.position = toFollow.position + offset;
			}
			else
			{
				following = false;
				if (onParentDeathBehaivior == BehaiviorOnParentDeath.Fade)
				{
					StartCoroutine(FadeOut());
				}
				else if (onParentDeathBehaivior == BehaiviorOnParentDeath.Remove)
				{
					Destroy(gameObject);
				}
			}
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