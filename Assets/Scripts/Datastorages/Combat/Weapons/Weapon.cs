﻿using EoE.Information;
using UnityEngine;

namespace EoE.Combatery
{
	public enum MultiplicationType { FlatValue = 1, Curve = 2 }
	[System.Flags] public enum AttackStylePart { StandAttack = (1 << 0), RunAttack = (1 << 1), JumpAttack = (1 << 2), RunJumpAttack = (1 << 3) }
	[System.Flags] public enum AttackChargeEffectMask { Damage = (1 << 0), Knockback = (1 << 1), CritChance = (1 << 2), ComboWorth = (1 << 3) }
	public enum AttackStylePartFallback { None = (0), StandAttack = (1 << 0), RunAttack = (1 << 1), JumpAttack = (1 << 2), RunJumpAttack = (1 << 3) }
	public class Weapon : CombatObject
	{
		public AttackSequence this[AttackStylePart part]
		{
			get
			{
				switch (part)
				{
					case AttackStylePart.RunAttack:
						return RunAttackSequence;
					case AttackStylePart.JumpAttack:
						return JumpAttackSequence;
					case AttackStylePart.RunJumpAttack:
						return RunJumpAttackSequence;
					default: //AttackStyleParts.StandAttack 
						return StandAttackSequence;
				}
			}
		}

		//Base data
		public WeaponController WeaponPrefab;
		public ElementType WeaponElement = ElementType.None;
		public CauseType WeaponCauseType = CauseType.Physical;
		public AttackStylePart ContainedParts = (AttackStylePart)(-1);
		public AttackStylePartFallback FallBackPart = AttackStylePartFallback.StandAttack;
		public bool HasUltimate = false;

		//Offsets
		public Vector3 WeaponPositionOffset;
		public Vector3 WeaponRotationOffset;

		//Attack styles
		public AttackSequence StandAttackSequence = new AttackSequence();
		public AttackSequence RunAttackSequence = new AttackSequence();
		public AttackSequence JumpAttackSequence = new AttackSequence();
		public AttackSequence RunJumpAttackSequence = new AttackSequence();

		public ComboSet ComboEffects;
		public WeaponUltimate UltimateSettings = new WeaponUltimate();

		public bool HasMaskFlag(AttackStylePart flag)
		{
			return (flag | ContainedParts) == ContainedParts;
		}
	}
	[System.Serializable]
	public class AttackSequence
	{
		public AttackStyle[] AttackSequenceParts = new AttackStyle[1];
		public float[] PartsMaxDelays = new float[0];
	}
	[System.Serializable]
	public class AttackStyle
	{
		//Animation Settings
		public AttackAnimation AnimationTarget = AttackAnimation.Attack1;
		public bool StopMovement;
		public bool StopRotation;

		public MultiplicationType AnimationMultiplicationType = MultiplicationType.FlatValue;
		public float AnimationSpeedFlatValue = 1;

		public AnimationCurve AnimationSpeedCurve = new AnimationCurve();
		public float AnimationSpeedCurveTimeframe = 2;
		public float AnimationSpeedCurveMultiplier = 1;

		//Charge settings
		public bool NeedsCharging = false;
		public AttackChargeSettings ChargeSettings = new AttackChargeSettings();

		//Wait settings
		public WaitSetting[] WaitSettings = new WaitSetting[0];

		//Base multipliers
		public float DamageMultiplier = 1;
		public float HealthCostMultiplier = 1;
		public float ManaCostMultiplier = 1;
		public float EnduranceCostMultiplier = 1;
		public float KnockbackMultiplier = 1;
		public float CritChanceMultiplier = 1;

		//Collision
		public ColliderMask CollisionMask = (ColliderMask)(-1);
		public ColliderMask StopOnCollisionMask = (ColliderMask)(-1);
		public EffectSingle DirectHit;

		//Overrides Info
		public bool OverrideElement = false;
		public ElementType OverridenElement = ElementType.None;
		public bool OverrideCauseType = false;
		public CauseType OverridenCauseType = CauseType.Physical;

		//Combo Info
		public float ComboIncreaseMaxDelay = 1;
		public int OnHitComboWorth = 1;

		//Attack effects
		public AttackActivationEffect[] AttackEffects = new AttackActivationEffect[0];

		public static bool HasCollisionMask(ColliderMask collisionMask, ColliderMask flag)
		{
			return (flag | collisionMask) == collisionMask;
		}
	}
	[System.Serializable]
	public class WaitSetting
	{
		public ConditionObject WaitCondition;
		public float MinAnimtionPoint;
		public float MaxAnimtionPoint;
	}
	[System.Serializable]
	public class AttackActivationEffect
	{
		public float AtAnimationPoint = 0.1f;
		public ActivationEffect Effect = new ActivationEffect();
	}
	[System.Serializable]
	public class AttackChargeSettings
	{
		public AttackChargeEffectMask EffectMask = AttackChargeEffectMask.Damage | AttackChargeEffectMask.Knockback;

		public float AnimationChargeStartpoint = 0.1f;
		public bool StopMovementWhileCharging = false;
		public bool StopRotationWhileCharging = false;
		public bool WaitAtFullChargeForRelease = true;

		public float ChargeTime = 1;
		public float StartCharge = 0;
		public float MaximumCharge = 1;
		public float MinRequiredCharge = 0.1f;

		//DirectHit overrides
		public ChargeBasedDirectHit[] ChargeBasedDirectHits = new ChargeBasedDirectHit[0];

		//FX
		public CustomFXObject[] FXObjects = new CustomFXObject[0];
		public CustomFXObject[] FXObjectsWithMutliplier = new CustomFXObject[0];

		//Buffs
		public Buff[] BuffOnUserWhileCharging = new Buff[0];
		public bool HasMaskFlag(AttackChargeEffectMask flag)
		{
			return (flag | EffectMask) == EffectMask;
		}
	}
	[System.Serializable]
	public class ChargeBasedDirectHit
	{
		public float MinRequiredCharge = 0;
		public float MaxRequiredCharge = 1;
		public EffectSingle DirectHitOverride = null;
	}
	[System.Serializable]
	public class WeaponUltimate
	{
		public Ultimate Ultimate;
		public float TotalRequiredCharge = 20;
		public float OnUseChargeRemove = 1;

		//Charge options
		public float OnHitCharge = 1;
		public float OnCritHitCharge = 2;
		public float OnKillCharge = 2;
		public float PerComboPointCharge = 0;
		public float ChargeOverTimeOnCombat = 0;
		public float OutOfCombatDecrease = 0;
		//public float OnSuccessfullParry = 4;
	}
}