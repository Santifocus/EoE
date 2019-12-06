using EoE.Information;
using UnityEngine;

namespace EoE.Combatery
{
	[System.Flags] public enum AttackStyleParts { StandAttack = (1 << 0), RunAttack = (1 << 1), JumpAttack = (1 << 2), RunJumpAttack = (1 << 3) }
	public class Weapon : CombatObject
	{
		//Base data
		public WeaponController WeaponPrefab;
		public ElementType WeaponElement = ElementType.None;
		public CauseType WeaponCauseType = CauseType.Physical;
		public AttackStyleParts ContainedParts = (AttackStyleParts)(-1);

		//Offsets
		public Vector3 WeaponPositionOffset;
		public Vector3 WeaponRotationOffset;

		//Attack styles
		public AttackStyle StandAttackStyle = new AttackStyle();
		public AttackStyle RunAttackStyle = new AttackStyle();
		public AttackStyle JumpAttackStyle = new AttackStyle();
		public AttackStyle RunJumpAttackStyle = new AttackStyle();
	}
	[System.Serializable]
	public class AttackStyle
	{
		public AttackAnimation animationTarget = AttackAnimation.Attack1;

		//Base multipliers
		public float DamageMultiplier;
		public float EnduranceDrainMultiplier;
		public float KnockbackMultiplier;

		//Damage Info
		public bool OverrideElement = false;
		public ElementType OverridenElement = ElementType.None;
		public bool OverrideCauseType = false;
		public CauseType OverridenCauseType = CauseType.Physical;

		//Combo info
		public float ToNextComboMaxDelay = 1;
		public int OnHitComboWorth = 1;

	}
}