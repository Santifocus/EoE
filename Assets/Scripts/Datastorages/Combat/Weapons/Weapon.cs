using EoE.Entities;
using EoE.Information;
using System.Collections;
using UnityEngine;

namespace EoE.Combatery
{
	public enum MultiplicationType { FlatValue = 1, Curve = 2 }
	[System.Flags] public enum AttackEffectType { ImpulseVelocity = (1 << 0), FX = (1 << 1), AOE = (1 << 2), CreateProjectile = (1 << 3) }
	[System.Flags] public enum AttackStylePart { StandAttack = (1 << 0), RunAttack = (1 << 1), JumpAttack = (1 << 2), RunJumpAttack = (1 << 3) }
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

		//Offsets
		public Vector3 WeaponPositionOffset;
		public Vector3 WeaponRotationOffset;

		//Attack styles
		public AttackSequence StandAttackSequence = new AttackSequence();
		public AttackSequence RunAttackSequence = new AttackSequence();
		public AttackSequence JumpAttackSequence = new AttackSequence();
		public AttackSequence RunJumpAttackSequence = new AttackSequence();

		public ComboSet ComboEffects;

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

		public MultiplicationType AnimationMultiplicationType = MultiplicationType.FlatValue;
		public float AnimationSpeedFlatValue = 1;

		public AnimationCurve AnimationSpeedCurve = new AnimationCurve();
		public float AnimationSpeedCurveTimeframe = 2;
		public float AnimationSpeedCurveMultiplier = 1;

		//Base multipliers
		public float DamageMultiplier = 1;
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
		public AttackEffect[] AttackEffects = new AttackEffect[0];

		public static bool HasCollisionMask(ColliderMask collisionMask, ColliderMask flag)
		{
			return (flag | collisionMask) == collisionMask;
		}
	}
	[System.Serializable]
	public class AttackEffect
	{
		public float AtAnimationPoint = 0.1f;
		public float ChanceToActivate = 1;

		public AttackEffectType ContainedEffectType = 0;

		//Impulse Velocity
		public float ImpulseVelocity = 5;
		public float ImpulseVelocityFallOffTime = 0.5f;
		public InherritDirection ImpulseVelocityDirection = InherritDirection.Target;
		public InherritDirection ImpulseVelocityFallbackDirection = InherritDirection.Local;
		public DirectionBase ImpulseDirectionBase = DirectionBase.Forward;

		//FX
		public CustomFXObject[] FXObjects = new CustomFXObject[0];

		//AOE
		public EffectAOE[] AOEEffects = new EffectAOE[0];

		//Projectile
		public ProjectileInfo[] ProjectileInfos = new ProjectileInfo[0];

		public void ActivateEffect(Entitie activator, CombatObject baseObject)
		{
			if (HasMaskFlag(AttackEffectType.ImpulseVelocity))
			{
				Vector3 direction = CombatObject.CalculateDirection(ImpulseVelocityDirection, ImpulseVelocityFallbackDirection, ImpulseDirectionBase, activator, Vector3.zero);
				activator.entitieForceController.ApplyForce(direction * ImpulseVelocity, 1 / ImpulseVelocityFallOffTime, true);
			}
			if (HasMaskFlag(AttackEffectType.FX))
			{
				for (int i = 0; i < FXObjects.Length; i++)
				{
					FXManager.PlayFX(FXObjects[i], activator.transform, activator is Player);
				}
			}
			if (HasMaskFlag(AttackEffectType.AOE))
			{
				for(int i = 0; i < AOEEffects.Length; i++)
				{
					AOEEffects[i].ActivateEffectAOE(activator, activator.transform, baseObject);
				}
			}
			if (HasMaskFlag(AttackEffectType.CreateProjectile))
			{
				GameController.Instance.StartCoroutine(ProjectileCreation(activator, baseObject));
			}
		}

		#region ProjectileCreation
		private IEnumerator ProjectileCreation(Entitie activator, CombatObject baseObject)
		{
			for (int i = 0; i < ProjectileInfos.Length; i++)
			{
				for (int j = 0; j < ProjectileInfos[i].ExecutionCount; j++)
				{
					float timer = 0;
					while (timer < ProjectileInfos[i].ExecutionDelay)
					{
						yield return new WaitForEndOfFrame();
						timer += Time.deltaTime;
						if (activator.IsStunned)
							goto ProjectileCreationFinished;
					}

					CreateProjectile(activator, baseObject, ProjectileInfos[i].Projectile);
					if (j < ProjectileInfos[i].ExecutionCount - 1)
					{
						float repeatTimer = 0;
						while (repeatTimer < ProjectileInfos[i].ExecutionRepeatDelay)
						{
							yield return new WaitForEndOfFrame();
							repeatTimer += Time.deltaTime;
							if (activator.IsStunned)
								goto ProjectileCreationFinished;
						}
					}
				}
			}
		ProjectileCreationFinished:;
		}
		private Projectile CreateProjectile(Entitie activator, CombatObject baseObject, ProjectileData data)
		{
			//Calculate the spawnoffset
			Vector3 spawnOffset = data.CreateOffsetToCaster.x * activator.transform.right + data.CreateOffsetToCaster.y * activator.transform.up + data.CreateOffsetToCaster.z * activator.transform.forward;

			//First find out what direction the projectile should fly
			Vector3 direction = CombatObject.CalculateDirection(data.DirectionStyle, 
																data.FallbackDirectionStyle, 
																data.Direction, 
																activator, 
																spawnOffset
																);
			return Projectile.CreateProjectile(baseObject, data, activator, direction, activator.actuallWorldPosition + spawnOffset);
		}
		#endregion

		public bool HasMaskFlag(AttackEffectType flag)
		{
			return (flag | ContainedEffectType) == ContainedEffectType;
		}
	}
}