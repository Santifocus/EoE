using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE
{
	public class PersistantInstanceController : MonoBehaviour
	{
		private static bool Exists = false;

		private void Awake()
		{
			if (Exists)
			{
				Destroy(gameObject);
				return;
			}

			DontDestroyOnLoad(this);
			Exists = true;
		}
	}
}