using EoE.Combatery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
    public class TutorialSettings : ScriptableObject
	{
		public TutorialPart[] Parts = new TutorialPart[0];
	}
	[System.Serializable]
	public class TutorialPart
	{
		public float DelayTime = 3;
		public ActivationEffect[] Effects = default;

		public bool SpawnDummy = false;
		public bool DeleteDummyOnFinish = false;

		public ItemGiveInfo[] ItemsToGive = default;
	}
}