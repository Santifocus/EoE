using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Sounds
{
	public class MusicController : MonoBehaviour
	{
		public static MusicController Instance { get; private set; }
		private void Start()
		{
			Instance = this;
		}
	}
}