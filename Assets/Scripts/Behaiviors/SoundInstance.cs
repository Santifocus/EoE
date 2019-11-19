using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Sound
{
	public enum SoundScaleStyle { Scaled, UnScaled, StopOnPaused}
	public class SoundInstance : MonoBehaviour
	{
		[SerializeField] private AudioSource source = default;
		[SerializeField] private SoundScaleStyle scaling;
		public void Setup(SoundScaleStyle scaling, AudioClip clip)
		{
			source.clip = clip;
			this.scaling = scaling;
			switch (scaling)
			{
				case SoundScaleStyle.Scaled:
					source.pitch = Time.timeScale;
					break;
				case SoundScaleStyle.StopOnPaused:
					source.pitch = GameController.GameIsPaused ? 0 : 1;
					break;
				case SoundScaleStyle.UnScaled:
					source.pitch = 1;
					break;
			}
		}
		private void Update()
		{
			switch (scaling)
			{
				case SoundScaleStyle.Scaled:
					source.pitch = Time.timeScale;
					break;
				case SoundScaleStyle.StopOnPaused:
					source.pitch = GameController.GameIsPaused ? 0 : 1;
					break;
			}
		}
	}
}