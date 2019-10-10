using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EoE.Attacks
{
	public class AttackStyle : ScriptableObject
	{
		[HideInInspector] public AttackInstance[] attackInstances;

		[System.Serializable]
		public class AttackInstance
		{
			public float AnimationDelay;
			public float MaxDelayForNextAttackInstance;

			//Horizontal angle from player
			public float XAngleStart;
			public float XAngleEnd;

			//Vertical angle from player
			public float YAngleStart;
			public float YAngleEnd;

			//Distance from player
			public float MinHitDistance;
			public float MaxHitDistance;
		}
	}
}