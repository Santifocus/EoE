using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Weapons
{
	public enum AttackAnimation { Stab, ToRightSlash, ToLeftSlash, TopDownSlash, Uppercut }
	public class AttackStyle : ScriptableObject
	{
		public Attack standAttack;
		public Attack jumpAttack;
		public Attack sprintAttack;
		public Attack jumpSprintAttack;

		public Attack standHeavyAttack;
		public Attack jumpHeavyAttack;
		public Attack sprintHeavyAttack;
		public Attack jumpSprintHeavyAttack;
	}

	[System.Serializable]
	public class Attack
	{
		public bool enabled;
		public AttackAnimation animation;
		public AttackInfo info;

		[Space(15)]
		[Header("Combo")]
		public bool hasCombo;
		public float comboMaxDelay;
		public Attack nextCombo = null;
	}

	[System.Serializable]
	public struct AttackInfo
	{
		public float damageMutliplier;
		public float enduranceMultiplier;
		public float knockbackMutliplier;
		public bool penetrateEntities;
		public bool penetrateTerrain;
	}
}