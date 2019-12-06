using EoE.Information;
using UnityEngine;

namespace EoE.Combatery
{
	public enum MultiplicationType { FlatValue = 1, Curve = 2 }
	[System.Flags] public enum AttackEffectType { FX = (1 << 0), AOE = (1 << 1), CreateProjectile = (1 << 3) }
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
		public AttackSequence StandAttackSequence = new AttackSequence();
		public AttackSequence RunAttackSequence = new AttackSequence();
		public AttackSequence JumpAttackSequence = new AttackSequence();
		public AttackSequence RunJumpAttackSequence = new AttackSequence();

		public ComboInfo[] ComboEffects = new ComboInfo[0];

		public bool HasMaskFlag(AttackStyleParts flag)
		{
			return (flag | ContainedParts) == ContainedParts;
		}
	}
	[System.Serializable]
	public class AttackSequence
	{
		public AttackStyle[] AttackSequenceParts = new AttackStyle[0];
		public float[] PartsMaxDelays = new float[0];
	}
	[System.Serializable]
	public class AttackStyle
	{
		//Animation Settings
		public AttackAnimation animationTarget = AttackAnimation.Attack1;

		public MultiplicationType AnimationMultiplicationType = MultiplicationType.FlatValue;
		public float AnimationSpeedFlatValue = 1;
		public AnimationCurve AnimationSpeedCurve = new AnimationCurve();

		//Collision
		public ColliderMask collisionMask = (ColliderMask)(-1);
		public ColliderMask stopOnCollisionMask = (ColliderMask)(-1);

		//Base multipliers
		public float DamageMultiplier = 1;
		public float ManaCostMultiplier = 1;
		public float EnduranceCostMultiplier = 1;
		public float KnockbackMultiplier = 1;
		public float CritChanceMultiplier = 1;

		//Damage Info
		public bool OverrideElement = false;
		public ElementType OverridenElement = ElementType.None;
		public bool OverrideCauseType = false;
		public CauseType OverridenCauseType = CauseType.Physical;

		//Combo Info
		public float ToNextComboMaxDelay = 1;
		public int OnHitComboWorth = 1;

		//Direct hit
		public EffectSingle DirectHit;

		//Attack effects
		public AttackEffect[] AttackEffects = new AttackEffect[0];
	}
	[System.Serializable]
	public class AttackEffect
	{
		public float AtAnimationPoint = 0.1f;
		public float ChanceToActivate = 1;

		public AttackEffectType ContainedEffectType = AttackEffectType.FX;

		//FX
		public CustomFXObject[] FXObjects = new CustomFXObject[0];

		//AOE
		public EffectAOE[] Effects = new EffectAOE[0];

		//Projectile
		public ProjectileData[] ProjectileInfo = new ProjectileData[0];
		public float[] DelayToNextProjectile = new float[0];

		public bool HasMaskFlag(AttackEffectType flag)
		{
			return (flag | ContainedEffectType) == ContainedEffectType;
		}
	}
	[System.Serializable]
	public class ComboInfo
	{
		public int RequiredComboCount = 1;
		public ComboEffect Effect = new ComboEffect();
	}
	[System.Serializable]
	public class ComboEffect
	{
		//Text settings
		public bool AffectText = false;
		public Gradient TextColor = new Gradient();
		public float ColorScrollSpeed = 1;
	}
}