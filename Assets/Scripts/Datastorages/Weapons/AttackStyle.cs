using UnityEngine;

namespace EoE.Weapons
{
	public enum AttackAnimation { Stab, ToRightSlash, ToLeftSlash, TopDownSlash, Uppercut }
	public enum AnimationCancelCondition { Ignore = 0, True = 1, False = 2 }
	public enum AttackVelocityIntent { Off = 0, Add = 1, Set = 2 }
	public class AttackStyle : ScriptableObject
	{
		public AttackCombo this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return standAttack;
					case 1:
						return sprintAttack;
					case 2:
						return jumpAttack;
					case 3:
						return sprintJumpAttack;
					case 4:
						return standHeavyAttack;
					case 5:
						return sprintHeavyAttack;
					case 6:
						return jumpHeavyAttack;
					case 7:
						return sprintJumpHeavyAttack;
				}
				//Index out of bounds
				return null;
			}
		}

		public AttackCombo standAttack;
		public AttackCombo sprintAttack;
		public AttackCombo jumpAttack;
		public AttackCombo sprintJumpAttack;

		public AttackCombo standHeavyAttack;
		public AttackCombo sprintHeavyAttack;
		public AttackCombo jumpHeavyAttack;
		public AttackCombo sprintJumpHeavyAttack;
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
		public AttackInfo info = new AttackInfo(1, 1, 1, 1);
		public AttackAnimationInfo animationInfo;
		public AttackVelocityEffect velocityEffect;
	}

	[System.Serializable]
	public struct AttackInfo
	{
		public float damageMutliplier;
		public float enduranceMultiplier;
		public float knockbackMutliplier;
		public float critChanceMultiplier;

		public AttackInfo(float damageMutliplier, float enduranceMultiplier, float knockbackMutliplier, float critChanceMultiplier)
		{
			this.damageMutliplier = damageMutliplier;
			this.enduranceMultiplier = enduranceMultiplier;
			this.knockbackMutliplier = knockbackMutliplier;
			this.critChanceMultiplier = critChanceMultiplier;
		}
	}

	[System.Serializable]
	public struct AttackAnimationInfo
	{
		public AttackAnimation animation;

		public bool haltAnimationTillCancel;

		public bool bothStates;
		public AnimationCancelCondition cancelWhenOnGround;
		public AnimationCancelCondition cancelWhenSprinting;

		public bool penetrateEntities;
		public bool penetrateTerrain;
	}

	[System.Serializable]
	public struct AttackVelocityEffect
	{
		public AttackVelocityIntent velocityIntent;
		public bool ignoreVerticalVelocity;

		public bool applyForceAfterAnimationCharge;
		public float applyForceDelay;

		public bool useRightValue;
		public float rightValue;

		public bool useUpValue;
		public float upValue;

		public bool useForwardValue;
		public float forwardValue;
	}
}