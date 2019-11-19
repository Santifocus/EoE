﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace EoE.Sounds
{
	public enum SoundTimeScale { Scaled, Unscaled, StopOnPause }
	public class Sound : ScriptableObject
	{
		public string soundName = "Sound";
		public AudioClip clip = null;
		public AudioMixerGroup audioGroup = null;
		[Range(0, 256)] public int priority = 128;
		[Range(-3, 3)] public float pitch = 1;
		[Range(0, 1)] public float volume = 1;
		[Range(0, 1)] public float spatialBlend = 0;
		public SoundTimeScale scaleStyle = SoundTimeScale.Scaled;
	}
}