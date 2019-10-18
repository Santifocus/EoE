using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Weapons
{
	public enum AttackAnimation { Stab, ToRightSlash, ToLeftSlash, TopDownSlash, Uppercut }
	public class AttackStyle : ScriptableObject
	{
		public AttackCombo standAttack;
		public AttackCombo jumpAttack;
		public AttackCombo sprintAttack;
		public AttackCombo jumpSprintAttack;

		public AttackCombo standHeavyAttack;
		public AttackCombo jumpHeavyAttack;
		public AttackCombo sprintHeavyAttack;
		public AttackCombo jumpSprintHeavyAttack;
	}

	[System.Serializable]
	public class AttackCombo
	{
		public Attack[] attacks;
		public float[] delays;
	}

	[System.Serializable]
	public class Attack
	{
		public bool enabled = false;
		public AttackAnimation animation;
		public AttackInfo info = new AttackInfo(1, 1, 1, false, false);
	}

	[System.Serializable]
	public struct AttackInfo
	{
		public float damageMutliplier;
		public float enduranceMultiplier;
		public float knockbackMutliplier;
		public bool penetrateEntities;
		public bool penetrateTerrain;
		public AttackInfo(float damageMutliplier, float enduranceMultiplier, float knockbackMutliplier, bool penetrateEntities, bool penetrateTerrain)
		{
			this.damageMutliplier = damageMutliplier;
			this.enduranceMultiplier = enduranceMultiplier;
			this.knockbackMutliplier = knockbackMutliplier;
			this.penetrateEntities = penetrateEntities;
			this.penetrateTerrain = penetrateTerrain;
		}
	}
}