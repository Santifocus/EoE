using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Information;
using EoE.Events;
using EoE.Weapons;

namespace EoE.Entities
{
	public abstract class Entitie : MonoBehaviour
	{
		private static readonly Vector3 GroundTestOffset = new Vector3(0, -0.15f, 0);
		private const float JUMP_GROUND_COOLDOWN = 0.2f;
		private const float IS_FALLING_THRESHOLD = -1;

		[SerializeField] protected Rigidbody body = default;
		[SerializeField] protected Collider coll = default;
		[Header("Animation")]
		[SerializeField] protected Transform modelTransform = default;
		[SerializeField] protected Animator animationControl = default;
		public abstract EntitieSettings SelfSettings { get; }

		private enum ColliderType : byte { Box, Sphere, Capsule }
		private ColliderType selfColliderType;

		//Stats
		public float CurHealth => curHealth;
		protected float curHealth;
		public float CurEndurance => curEndurance;
		protected float curEndurance;
		public float CurMana => curMana;
		protected float curMana;

		private float curWalkSpeed;
		private float curAcceleration;

		private float regenTimer;
		protected EntitieState curStates;

		private float jumpGroundCooldown;
		private float lastFallVelocity;

		protected void Start()
		{
			ResetStats();
			GetColliderType();
			EntitieStart();
		}
		private void ResetStats()
		{
			curHealth = SelfSettings.Health;
			curEndurance = SelfSettings.Endurance;
			curMana = SelfSettings.Mana;
			curWalkSpeed = SelfSettings.WalkSpeed;

			curAcceleration = 0;
			regenTimer = 0;
			curStates = new EntitieState();
		}
		private void GetColliderType()
		{
			if(!coll)
			{
				Debug.LogError("Entitie: '" + name + "' has no collision Collider attached. Please attach one and assign it in the inspector.");
				return;
			}

			if(coll is BoxCollider)
			{
				selfColliderType = ColliderType.Box;
				return;
			}
			else if (coll is SphereCollider)
			{
				selfColliderType = ColliderType.Sphere;
				return;
			}
			else if (coll is CapsuleCollider)
			{
				selfColliderType = ColliderType.Capsule;
				return;
			}
			else
			{
				Debug.LogError("Entitie: " + name + " has a collision Collider attached, however the type '" + coll.GetType() + "' is not supported. Supported types are 'BoxCollider','SphereCollider' and 'CapsuleCollider'.");
			}
		}
		protected virtual void EntitieStart(){}

		protected void Update()
		{
			Regen();
			UpdateMovementSpeed();
			EntitieUpdate();
		}
		protected void FixedUpdate()
		{
			IsGroundedTest();
		}
		protected virtual void EntitieUpdate() { }
		protected float UpdateAcceleration(float intendedFactor = 1)
		{
			if (curStates.IsMoving)
			{
				if(curAcceleration < intendedFactor)
				{
					if (SelfSettings.MoveAcceleration > 0)
						curAcceleration = Mathf.Min(intendedFactor, curAcceleration + intendedFactor * Time.deltaTime / SelfSettings.MoveAcceleration / SelfSettings.EntitieMass);
					else
						curAcceleration = intendedFactor;
				}
			}
			else //decelerate
			{
				if (curAcceleration > 0)
				{
					if (SelfSettings.NoMoveDeceleration > 0)
						curAcceleration = Mathf.Max(0, curAcceleration - Time.deltaTime / SelfSettings.NoMoveDeceleration / SelfSettings.EntitieMass);
					else
						curAcceleration = 0;
				}
			}
			return curAcceleration;
		}
		protected float TurnTo(Vector3 turnDirection)
		{
			float directionDistance = (modelTransform.forward - turnDirection).magnitude;
			float fraction = Mathf.Min(1, Time.deltaTime / SelfSettings.TurnSpeed * Mathf.Max(1, directionDistance * directionDistance));
			modelTransform.forward = ((1 - fraction) * modelTransform.forward + fraction * turnDirection).normalized;

			return GameController.CurrentGameSettings.TurnSpeedCurve.Evaluate(1 - (directionDistance / 2));
		}
		private void UpdateMovementSpeed()
		{
			bool moving = curStates.IsMoving;

			//If the Entitie doesnt move intentionally but is in run mode, then stop the run mode
			if (!moving && curStates.IsRunning)
				curStates.IsRunning = false;

			//We find out in which direction the Entitie should move according to its movement
			float baseTargetSpeed = curWalkSpeed * (curStates.IsRunning ? SelfSettings.RunSpeedMultiplicator : 1) * curAcceleration;
			Vector3 targetVelocity = baseTargetSpeed  * modelTransform.forward;

			//And find out how fast it is currently moving
			Vector2 curVelocity = new Vector2(body.velocity.x, body.velocity.z);
			float curSpeed = curVelocity.magnitude;

			//Now we want to interpolate based on targetSpeed/curSPeed fraction
			float interpolatePoint = 0;
			if (baseTargetSpeed > curSpeed)
				interpolatePoint = 1;
			else
				interpolatePoint = (baseTargetSpeed / Mathf.Max(0.000001f, curSpeed)) * Time.deltaTime;

			float newXVel = curVelocity.x + (targetVelocity.x - curVelocity.x) * Mathf.Clamp01(interpolatePoint); 
			float newZVel = curVelocity.y + (targetVelocity.z - curVelocity.y) * Mathf.Clamp01(interpolatePoint);

			body.velocity = new Vector3(newXVel, body.velocity.y, newZVel);
		}
		protected void Jump()
		{
			Vector3 addedForce = new Vector3(0, SelfSettings.JumpPower.y, 0);
			addedForce += SelfSettings.JumpPower.x * transform.right;
			addedForce += SelfSettings.JumpPower.z * transform.forward;

			body.velocity += addedForce;
			jumpGroundCooldown = JUMP_GROUND_COOLDOWN;

			animationControl.SetTrigger("JumpStart");
		}
		private void IsGroundedTest()
		{
			if (jumpGroundCooldown > 0)
			{
				jumpGroundCooldown -= Time.deltaTime;
				curStates.IsGrounded = false;
			}
			else
			{
				switch (selfColliderType)
				{
					case ColliderType.Box:
						{
							BoxCollider bColl = coll as BoxCollider;
							curStates.IsGrounded = Physics.CheckBox(bColl.center + transform.position + GroundTestOffset, new Vector3(bColl.bounds.extents.x * 0.95f, bColl.bounds.extents.y, bColl.bounds.extents.z * 0.95f), transform.rotation, ConstantCollector.TERRAIN_LAYER);
							break;
						}
					case ColliderType.Capsule:
						{
							curStates.IsGrounded = CapsuleGroundTest();
							break;
						}
					case ColliderType.Sphere:
						{
							SphereCollider sColl = coll as SphereCollider;
							curStates.IsGrounded = Physics.CheckSphere(transform.position + sColl.center + GroundTestOffset, sColl.radius, ConstantCollector.TERRAIN_LAYER);
							break;
						}
				}
			}

			//Check for falldamage
			float velDif = body.velocity.y - lastFallVelocity;
			if(velDif > 0 && jumpGroundCooldown <= 0) //We stopped falling for a certain amount, and we didnt change velocity because we just jumped
			{
				Landed(velDif);
			}
			lastFallVelocity = body.velocity.y;

			//Velocity control when falling
			bool falling = !curStates.IsGrounded && (body.velocity.y < IS_FALLING_THRESHOLD || (this is Player && (curStates.IsFalling || !Controlls.PlayerControlls.Buttons.Jump.Active)));
			curStates.IsFalling = falling;
			animationControl.SetBool("IsFalling", falling);
			if (falling)
			{
				body.velocity += GameController.CurrentGameSettings.WhenFallingExtraVelocity * Physics.gravity * Time.fixedDeltaTime;
			}
		}
		private bool CapsuleGroundTest()
		{
			CapsuleCollider cColl = coll as CapsuleCollider;
			Vector3 middle = cColl.center + transform.position + GroundTestOffset;
			Vector3 offset = Vector3.zero;
			switch (cColl.direction)
			{
				case 0: //X
					offset.x = cColl.height / 2;
					break;
				case 1: //Y
					offset.y = cColl.height / 2;
					break;
				case 2: //Z
					offset.z = cColl.height / 2;
					break;
			}

			return Physics.CheckCapsule(middle  - offset, middle + offset, cColl.radius, ConstantCollector.TERRAIN_LAYER);
		}
		private void Landed(float velDif)
		{
			float damageAmount = GameController.CurrentGameSettings.FallDamageCurve.Evaluate(velDif);
			if(damageAmount > 0)
				ChangeHealth(new InflictionInfo(this, CauseType.Physical, ElementType.None, transform.position + SelfSettings.MassCenter, Vector3.up, damageAmount, false));

			float curSpeed = new Vector2(body.velocity.x, body.velocity.z).magnitude;
			curAcceleration = Mathf.Clamp01(curAcceleration - velDif / Mathf.Max(curSpeed, 1) * GameController.CurrentGameSettings.GroundHitVelocityLoss);
		}
		protected virtual void Regen()
		{
			regenTimer += Time.deltaTime;
			if (regenTimer >= GameController.CurrentGameSettings.SecondsPerEntititeRegen)
			{
				bool inCombat = curStates.IsInCombat;
				regenTimer -= GameController.CurrentGameSettings.SecondsPerEntititeRegen;
				if (SelfSettings.DoHealthRegen && curHealth < SelfSettings.Health)
				{
					float regenAmount = SelfSettings.HealthRegen * GameController.CurrentGameSettings.SecondsPerEntititeRegen * (inCombat ? SelfSettings.HealthRegenInCombatFactor : 1);
					if (regenAmount > 0)
					{
						InflictionInfo basis = new InflictionInfo(this, CauseType.Heal, ElementType.None, transform.position + SelfSettings.MassCenter, Vector3.up, -regenAmount, false);
						InflictionInfo.InflictionResult regenResult = new InflictionInfo.InflictionResult(basis, this, true, true);

						curHealth = Mathf.Min(SelfSettings.Health, curHealth - regenResult.finalDamage);
					}
				}

				if(!(this is Player))
				{
					if (SelfSettings.DoEnduranceRegen && curEndurance < SelfSettings.Endurance)
					{
						float regenAmount = SelfSettings.EnduranceRegen * GameController.CurrentGameSettings.SecondsPerEntititeRegen * (inCombat ? SelfSettings.EnduranceRegen : 1);
						curEndurance = Mathf.Min(SelfSettings.Endurance, curEndurance + SelfSettings.EnduranceRegen * GameController.CurrentGameSettings.SecondsPerEntititeRegen);
					}
				}

				if (SelfSettings.DoManaRegen && curMana < SelfSettings.Mana)
				{
					float regenAmount = SelfSettings.ManaRegen * GameController.CurrentGameSettings.SecondsPerEntititeRegen * (inCombat ? SelfSettings.ManaRegenInCombatFactor : 1);
					curMana = Mathf.Min(SelfSettings.Mana, curMana + regenAmount);
				}
			}
		}

		public void ChangeHealth(InflictionInfo causedDamage)
		{
			InflictionInfo.InflictionResult damageResult = new InflictionInfo.InflictionResult(causedDamage, this, true);

			curHealth -= damageResult.finalDamage;
			body.velocity += damageResult.causedKnockback;
			curHealth = Mathf.Min(SelfSettings.Health, curHealth);

			if(curHealth <= 0)
			{
				if(this is Player)
				{
					EventManager.PlayerDiedInvoke(causedDamage.attacker);
				}
				else
				{
					EventManager.EntitieDiedInvoke(this, causedDamage.attacker);
				}
				Death();
			}
		}

		protected virtual void Death()
		{
			Destroy(gameObject);
		}

		public struct EntitieState
		{
			private byte state;

			private const byte GroundedReset = 254;
			private const byte MovingReset = 253;
			private const byte RunningReset = 251;
			private const byte BlockingReset = 247;
			private const byte CombatReset = 239;
			private const byte FallingReset = 223;

			public bool IsGrounded
			{
				get => ((state | 1) == state);
				set
				{
					state = (byte)(state & GroundedReset);
					if (value)
					{
						state |= 1;
					}
				}
			}
			public bool IsMoving
			{
				get => ((state | 2) == state);
				set
				{
					state = (byte)(state & MovingReset);
					if (value)
					{
						state |= 2;
					}
				}
			}
			public bool IsRunning
			{
				get => ((state | 4) == state);
				set
				{
					state = (byte)(state & RunningReset);
					if (value)
					{
						state |= 4;
					}
				}
			}
			public bool IsBlocking
			{
				get => ((state | 8) == state);
				set
				{
					state = (byte)(state & BlockingReset);
					if (value)
					{
						state |= 8;
					}
				}
			}
			public bool IsInCombat
			{
				get => ((state | 16) == state);
				set
				{
					state = (byte)(state & CombatReset);
					if (value)
					{
						state |= 16;
					}
				}
			}
			public bool IsFalling
			{
				get => ((state | 32) == state);
				set
				{
					state = (byte)(state & FallingReset);
					if (value)
					{
						state |= 32;
					}
				}
			}
		}
	}
}