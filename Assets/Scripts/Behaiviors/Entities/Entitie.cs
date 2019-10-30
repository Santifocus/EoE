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
		#region Fields
		//Constants
		private const float GROUND_TEST_WIDHT_MUL				= 0.95f;
		private const float JUMP_GROUND_COOLDOWN				= 0.2f;
		private const float IS_FALLING_THRESHOLD				= -1;
		private const float LANDED_VELOCITY_THRESHOLD			= 0.5f;
		private const float NON_TURNING_THRESHOLD				= 60;
		private const float LERP_TURNING_AREA					= 0.5f;
		private const float RUN_ANIM_THRESHOLD					= 0.75f;
		private const int VISIBLE_CHECK_RAY_COUNT				= 40;

		private static readonly Vector3 GroundTestOffset		= new Vector3(0, -0.05f, 0);

		public static List<Entitie> AllEntities = new List<Entitie>();

		//Inspector variables
		public Rigidbody body				= default;
		public Collider coll				= default;
		[SerializeField] protected Animator animationControl	= default;

		//Stats
		public float curMaxHealth { get; protected set; }
		public float curHealth { get; protected set; }
		public float curMaxMana { get; protected set; }
		public float curMana { get; protected set; }
		public float curWalkSpeed { get; protected set; }
		public float curJumpPower { get; protected set; }
		public float physicalDamageMultiplier { get; protected set; }
		public float magicalDamageMultiplier { get; protected set; }

		//Entitie states
		private List<BuffInstance> nonPermanentBuffs;
		private List<BuffInstance> permanentBuffs;
		public EntitieState curStates;
		private float regenTimer;
		private float combatEndCooldown;

		//Velocity Control
		protected float intendedAcceleration;
		protected float curAcceleration;
		private float jumpGroundCooldown;
		private float lastFallVelocity;

		protected Vector3 curMoveForce;
		protected Vector3 curExtraForce;

		protected float curRotation;
		protected float intendedRotation;

		//Getter Helpers
		protected enum ColliderType : byte { Box, Sphere, Capsule }
		public abstract EntitieSettings SelfSettings { get; }
		protected ColliderType selfColliderType;
		public Vector3 actuallWorldPosition => SelfSettings.MassCenter + transform.position;
		public float lowestPos => coll.bounds.center.y - coll.bounds.extents.y;
		public float highestPos => coll.bounds.center.y + coll.bounds.extents.y;
		public Vector3 curVelocity => new Vector3(curMoveForce.x + curExtraForce.x, body.velocity.y, curMoveForce.z + curExtraForce.z);
		#endregion
		#region Basic Monobehaivior
		protected virtual void Start()
		{
			AllEntities.Add(this);
			ResetStats();
			GetColliderType();
			EntitieStart();
		}
		private void ResetStats()
		{
			curMaxHealth = curHealth = SelfSettings.Health;
			curMaxMana = curMana = SelfSettings.Mana;
			curWalkSpeed = SelfSettings.WalkSpeed;
			curJumpPower = 1;
			physicalDamageMultiplier = 1;
			magicalDamageMultiplier = 1;

			curStates = new EntitieState();

			nonPermanentBuffs = new List<BuffInstance>();
			permanentBuffs = new List<BuffInstance>();
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
		protected virtual void Update()
		{
			TurnControl();
			UpdateAcceleration();
			UpdateMovementSpeed();

			Regen();
			EntitieStateControl();
			EntitieUpdate();
		}
		protected void FixedUpdate()
		{
			IsGroundedTest();
			CheckForFalling();
			EntitieFixedUpdate();
		}
		protected virtual void EntitieUpdate() { }
		protected virtual void EntitieFixedUpdate() { }
		#endregion
		#region Movement
		private void UpdateAcceleration()
		{
			if (curStates.IsMoving && !curStates.IsTurning)
			{
				float clampedIntent = Mathf.Clamp01(intendedAcceleration);
				if(curAcceleration < clampedIntent)
				{
					if (SelfSettings.MoveAcceleration > 0)
						curAcceleration = Mathf.Min(clampedIntent, curAcceleration + intendedAcceleration * Time.deltaTime / SelfSettings.MoveAcceleration / SelfSettings.EntitieMass * (curStates.IsGrounded ? 1 : SelfSettings.InAirAccelerationMultiplier));
					else
						curAcceleration = clampedIntent;
				}
			}
			else //decelerate
			{
				if (curAcceleration > 0)
				{
					if (SelfSettings.NoMoveDeceleration > 0)
						curAcceleration = Mathf.Max(0, curAcceleration - Time.deltaTime / SelfSettings.NoMoveDeceleration / SelfSettings.EntitieMass * (curStates.IsGrounded ? 1 : SelfSettings.InAirAccelerationMultiplier));
					else
						curAcceleration = 0;
				}
			}
		}
		protected void TargetPosition(Vector3 pos)
		{
			Vector2 direction = new Vector2(pos.x - actuallWorldPosition.x, pos.z - actuallWorldPosition.z).normalized;
			intendedRotation = -Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

			Debug.DrawLine(actuallWorldPosition, actuallWorldPosition + new Vector3(direction.x, 0, direction.y) * (pos - actuallWorldPosition).magnitude, Color.red, Time.deltaTime);
		}
		private void TurnControl()
		{
			float turnAmount = Time.deltaTime * SelfSettings.TurnSpeed * (curStates.IsGrounded ? 1 : SelfSettings.InAirTurnSpeedMultiplier);
			float normalizedDif = Mathf.Abs(curRotation - intendedRotation) / NON_TURNING_THRESHOLD;
			turnAmount *= Mathf.Min(normalizedDif * LERP_TURNING_AREA, 1);
			curRotation = Mathf.MoveTowardsAngle(curRotation, intendedRotation, turnAmount);

			curStates.IsTurning = normalizedDif > 1;

			transform.localEulerAngles = new Vector3(	transform.localEulerAngles.x,
														curRotation,
														transform.localEulerAngles.z);
		}
		private void UpdateMovementSpeed()
		{
			bool turning = curStates.IsTurning;
			bool moving = curStates.IsMoving;
			bool running = curStates.IsRunning;

			//If the Entitie doesnt move intentionally but is in run mode, then stop the run mode
			if (running && !moving)
				curStates.IsRunning = false;

			//Set the animation state to either Turning, Walking or Running
			animationControl.SetBool("Turning", turning);
			animationControl.SetBool("Walking", !turning && curAcceleration > 0 && !(running && curAcceleration > RUN_ANIM_THRESHOLD));
			animationControl.SetBool("Running", !turning && curAcceleration > 0 &&	(running && curAcceleration > RUN_ANIM_THRESHOLD));

			//We find out in which direction the Entitie should move according to its movement
			float baseTargetSpeed = curWalkSpeed * (curStates.IsRunning ? SelfSettings.RunSpeedMultiplicator : 1) * curAcceleration;
			curMoveForce = baseTargetSpeed * transform.forward;

			//Lerp knockback / other forces to zero based on the entities deceleration stat
			if (SelfSettings.NoMoveDeceleration > 0)
				curExtraForce = Vector3.Lerp(curExtraForce, Vector3.zero, Time.deltaTime / SelfSettings.NoMoveDeceleration);
			else
				curExtraForce = Vector3.zero;

			//Now combine those forces as the current speed
			body.velocity = curVelocity;
		}
		private void CheckForFalling()
		{
			//Find out wether the entitie is falling or not
			bool playerWantsToFall = this is Player && (curStates.IsFalling || !Controlls.InputController.Jump.Active);
			bool falling = !curStates.IsGrounded && jumpGroundCooldown <= 0 && (body.velocity.y < IS_FALLING_THRESHOLD || playerWantsToFall);

			//If so: we enable the falling animation and add extra velocity for a better looking fallcurve
			curStates.IsFalling = falling;
			animationControl.SetBool("IsFalling", falling);
			if (falling)
			{
				body.velocity += GameController.CurrentGameSettings.WhenFallingExtraVelocity * Physics.gravity * Time.fixedDeltaTime;
			}
		}
		protected void Jump()
		{
			body.velocity = new Vector3(body.velocity.x, Mathf.Min(body.velocity.y + SelfSettings.JumpPower.y * curJumpPower, SelfSettings.JumpPower.y * curJumpPower), body.velocity.z);
			curExtraForce += SelfSettings.JumpPower.x * transform.right * curJumpPower;
			curExtraForce += SelfSettings.JumpPower.z * transform.forward * curJumpPower;

			jumpGroundCooldown = JUMP_GROUND_COOLDOWN;
			curStates.IsGrounded = false;

			animationControl.SetTrigger("JumpStart");
		}
		#endregion
		#region Ground Contact
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
							Vector3 boxCenter = coll.bounds.center;
							boxCenter.y = lowestPos + GroundTestOffset.y + 0.01f; //A little bit of clipping into the original bounding box

							curStates.IsGrounded = Physics.CheckBox(boxCenter, new Vector3(bColl.bounds.extents.x * GROUND_TEST_WIDHT_MUL, -GroundTestOffset.y, bColl.bounds.extents.z * GROUND_TEST_WIDHT_MUL), coll.transform.rotation, ConstantCollector.TERRAIN_LAYER);
							break;
						}
					case ColliderType.Capsule:
						{
							CapsuleCollider cColl = coll as CapsuleCollider;
							Vector3 extraOffset = new Vector3(0, -cColl.radius * (1 - GROUND_TEST_WIDHT_MUL), 0);
							Vector3 spherePos = coll.bounds.center - new Vector3(0, cColl.height/2 * coll.transform.lossyScale.y - cColl.radius, 0) + GroundTestOffset + extraOffset;

							curStates.IsGrounded = Physics.CheckSphere(spherePos, cColl.radius * GROUND_TEST_WIDHT_MUL, ConstantCollector.TERRAIN_LAYER);
							break;
						}
					case ColliderType.Sphere:
						{
							SphereCollider sColl = coll as SphereCollider;
							Vector3 extraOffset = new Vector3(0, -sColl.radius * GROUND_TEST_WIDHT_MUL, 0);
							curStates.IsGrounded = Physics.CheckSphere(coll.bounds.center + GroundTestOffset + extraOffset, sColl.radius * GROUND_TEST_WIDHT_MUL, ConstantCollector.TERRAIN_LAYER);
							break;
						}
				}
			}

			//Check if we landed
			float velDif = curVelocity.y - lastFallVelocity;
			if (velDif > LANDED_VELOCITY_THRESHOLD
				&& jumpGroundCooldown <= 0) //We stopped falling for a certain amount, and we didnt change velocity because we just jumped
			{
				Landed(velDif);
			}
			lastFallVelocity = curVelocity.y;
		}
		private void Landed(float velDif)
		{
			//Check if there is any fall damage to give
			float damageAmount = GameController.CurrentGameSettings.FallDamageCurve.Evaluate(velDif);
			if (damageAmount > 0)
				ChangeHealth(new InflictionInfo(this, CauseType.Physical, ElementType.None, actuallWorldPosition, Vector3.up, damageAmount, false));

			//Decrease the x/z velocity of the Entitie because it landed
			float curSpeed = new Vector2(body.velocity.x, body.velocity.z).magnitude;
			curAcceleration = Mathf.Clamp01(curAcceleration - velDif / Mathf.Max(curSpeed, 1) * GameController.CurrentGameSettings.GroundHitVelocityLoss);
		}
		#endregion
		#region State Control
		protected virtual void Regen()
		{
			regenTimer += Time.deltaTime;
			if (regenTimer >= GameController.CurrentGameSettings.SecondsPerEntititeRegen)
			{
				bool inCombat = curStates.IsInCombat;
				regenTimer -= GameController.CurrentGameSettings.SecondsPerEntititeRegen;
				if (SelfSettings.DoHealthRegen && curHealth < curMaxHealth)
				{
					float regenAmount = SelfSettings.HealthRegen * GameController.CurrentGameSettings.SecondsPerEntititeRegen * (inCombat ? SelfSettings.HealthRegenInCombatMultiplier : 1);
					if (regenAmount > 0)
					{
						InflictionInfo basis = new InflictionInfo(this, CauseType.Heal, ElementType.None, actuallWorldPosition, Vector3.up, -regenAmount, false);
						InflictionInfo.InflictionResult regenResult = new InflictionInfo.InflictionResult(basis, this, true, true);

						curHealth = Mathf.Min(curMaxHealth, curHealth - regenResult.finalDamage);
					}
				}

				if (SelfSettings.DoManaRegen && curMana < curMaxMana)
				{
					float regenAmount = SelfSettings.ManaRegen * GameController.CurrentGameSettings.SecondsPerEntititeRegen * (inCombat ? SelfSettings.ManaRegenInCombatMultiplier : 1);
					curMana = Mathf.Min(curMaxMana, curMana + regenAmount);
				}
			}
		}

		private void EntitieStateControl()
		{
			if(combatEndCooldown > 0)
			{
				combatEndCooldown -= Time.deltaTime;
				if (combatEndCooldown <= 0)
					curStates.IsInCombat = false;
			}
			BuffUpdate();
		}
		#region BuffControl
		private void BuffUpdate()
		{
			bool requiresRecalculate = false;
			for(int i = 0; i < nonPermanentBuffs.Count; i++)
			{
				nonPermanentBuffs[i].RemainingTime -= Time.deltaTime;
				for(int j = 0; j < nonPermanentBuffs[i].DOTCooldowns.Length; j++)
				{
					nonPermanentBuffs[i].DOTCooldowns[j] -= Time.deltaTime;
					if(nonPermanentBuffs[i].DOTCooldowns[j] <= 0)
					{
						float cd = nonPermanentBuffs[i].Base.DOTs[j].DelayPerActivation;
						nonPermanentBuffs[i].DOTCooldowns[j] += cd;
						ChangeHealth(new InflictionInfo(nonPermanentBuffs[i].Applier, CauseType.DOT, nonPermanentBuffs[i].Base.DOTs[j].Element, actuallWorldPosition, Vector3.up, cd * nonPermanentBuffs[i].Base.DOTs[j].BaseDamage, false));
					}
				}
				if (nonPermanentBuffs[i].RemainingTime <= 0)
				{
					requiresRecalculate = true;
					RemoveBuff(i, false);
					i--;
				}
			}
			PermanentBuffsControl();
			if (requiresRecalculate)
				RecalculateBuffs();
		}
		public void AddBuff(Buff buff, Entitie applier)
		{
			BuffInstance newBuff = new BuffInstance(buff, applier);
			AddBuffEffect(newBuff);

			if (buff.Permanent)
				permanentBuffs.Add(newBuff);
			else
				nonPermanentBuffs.Add(newBuff);
		}
		private void AddBuffEffect(BuffInstance buffInstance)
		{
			Buff buffBase = buffInstance.Base;
			for (int i = 0; i < buffBase.Effects.Length; i++)
			{
				float change = 0;

				switch (buffBase.Effects[i].targetStat)
				{
					case TargetStat.Health:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curMaxHealth : buffBase.Effects[i].Amount;
							change = Mathf.Max(-(curMaxHealth - 1), change);
							curMaxHealth += change;
							curHealth = Mathf.Min(curMaxHealth, curHealth);
							break;
						}
					case TargetStat.Mana:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curMaxMana : buffBase.Effects[i].Amount;
							change = Mathf.Max(-(curMaxMana), change);
							curMaxMana += change;
							curMana = Mathf.Min(curMaxMana, curMana);
							break;
						}
					case TargetStat.MoveSpeed:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curWalkSpeed : buffBase.Effects[i].Amount;
							curWalkSpeed += change;
							break;
						}
					case TargetStat.JumpHeight:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curJumpPower : buffBase.Effects[i].Amount;
							curJumpPower += change;
							break;
						}
					case TargetStat.PhysicalDamage:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * physicalDamageMultiplier : buffBase.Effects[i].Amount;
							change = Mathf.Max(-physicalDamageMultiplier, change);
							physicalDamageMultiplier += change;
							break;
						}
					case TargetStat.MagicalDamage:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * magicalDamageMultiplier : buffBase.Effects[i].Amount;
							change = Mathf.Max(-magicalDamageMultiplier, change);
							magicalDamageMultiplier += change;
							break;
						}
				}

				buffInstance.FlatChanges[i] = change;
			}
		}
		private void RemoveBuff(int index, bool fromPermanent)
		{
			BuffInstance targetBuff = fromPermanent ? permanentBuffs[index] : nonPermanentBuffs[index];
			RemoveBuffEffect(targetBuff);

			if (fromPermanent)
				permanentBuffs.RemoveAt(index);
			else
				nonPermanentBuffs.RemoveAt(index);

			if (fromPermanent)
				RecalculateBuffs();
		}
		private void RemoveBuffEffect(BuffInstance buffInstance)
		{
			Buff buffBase = buffInstance.Base;
			for (int i = 0; i < buffBase.Effects.Length; i++)
			{
				float change = buffInstance.FlatChanges[i];

				switch (buffBase.Effects[i].targetStat)
				{
					case TargetStat.Health:
						{
							curMaxHealth -= change;
							if (curHealth > curMaxHealth)
								curHealth = curMaxHealth;
							break;
						}
					case TargetStat.Mana:
						{
							curMaxMana -= change;
							if (curMana > curMaxMana)
								curMana = curMaxMana;
							break;
						}
					case TargetStat.MoveSpeed:
						{
							curWalkSpeed -= change;
							break;
						}
					case TargetStat.JumpHeight:
						{
							curJumpPower -= change;
							break;
						}
					case TargetStat.PhysicalDamage:
						{
							physicalDamageMultiplier -= change;
							break;
						}
					case TargetStat.MagicalDamage:
						{
							magicalDamageMultiplier -= change;
							break;
						}
				}
			}
		}
		private void RecalculateBuffs()
		{
			//Remove all effects
			for (int i = 0; i < nonPermanentBuffs.Count; i++)
			{
				RemoveBuffEffect(nonPermanentBuffs[i]);
			}
			for (int i = 0; i < permanentBuffs.Count; i++)
			{
				RemoveBuffEffect(permanentBuffs[i]);
			}

			//Now readd them so the values can be recalculated
			for (int i = 0; i < nonPermanentBuffs.Count; i++)
			{
				AddBuffEffect(nonPermanentBuffs[i]);
			}
			for (int i = 0; i < permanentBuffs.Count; i++)
			{
				AddBuffEffect(permanentBuffs[i]);
			}
		}
		private void PermanentBuffsControl()
		{
			for(int i = 0; i < permanentBuffs.Count; i++)
			{
				for(int j = 0; j < permanentBuffs[i].DOTCooldowns.Length; j++)
				{
					permanentBuffs[i].DOTCooldowns[j] -= Time.deltaTime;
					if(permanentBuffs[i].DOTCooldowns[j] <= 0)
					{
						float cd = permanentBuffs[i].Base.DOTs[j].DelayPerActivation;
						permanentBuffs[i].DOTCooldowns[j] += cd;
						ChangeHealth(new InflictionInfo(permanentBuffs[i].Applier, CauseType.DOT, permanentBuffs[i].Base.DOTs[j].Element, actuallWorldPosition, Vector3.up, cd * permanentBuffs[i].Base.DOTs[j].BaseDamage, false));
					}
				}
			}
		}
		#endregion

		public void ChangeHealth(InflictionInfo causedDamage)
		{
			if (causedDamage.attacker != null && causedDamage.attacker != this)
			{
				StartCombat();
				causedDamage.attacker.StartCombat();
			}
			InflictionInfo.InflictionResult damageResult = new InflictionInfo.InflictionResult(causedDamage, this, true);

			curHealth -= damageResult.finalDamage;
			curExtraForce += damageResult.causedKnockback;
			curHealth = Mathf.Min(curMaxHealth, curHealth);

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

		protected void StartCombat()
		{
			curStates.IsInCombat = true;
			combatEndCooldown = GameController.CurrentGameSettings.CombatCooldown;
		}
		protected virtual void Death()
		{
			AllEntities.Remove(this);
			BuildDrops();
			Destroy(gameObject);
		}
		private void BuildDrops()
		{
			//Create souls drops
			if(SelfSettings.SoulWorth > 0)
			{
				SoulDrop newSoulDrop = Instantiate(GameController.CurrentGameSettings.SoulDropPrefab, Storage.DropStorage);
				newSoulDrop.transform.position = actuallWorldPosition + new Vector3(0, 2, 0);
				newSoulDrop.Setup(SelfSettings.SoulWorth);
			}

			//Create item / other drops
			if (SelfSettings.PossibleDropsTable == null)
				return;

			for(int i = 0; i < SelfSettings.PossibleDropsTable.PossibleDrops.Length; i++)
			{
				if (Utils.BaseUtils.Chance01(SelfSettings.PossibleDropsTable.PossibleDrops[i].DropChance))
				{
					int amount = Random.Range(SelfSettings.PossibleDropsTable.PossibleDrops[i].MinDropAmount, SelfSettings.PossibleDropsTable.PossibleDrops[i].MaxDropAmount + 1);
					for(int j = 0; j < amount; j++)
					{
						GameObject newDrop = Instantiate(SelfSettings.PossibleDropsTable.PossibleDrops[i].Drop, Storage.DropStorage);
						newDrop.transform.position = actuallWorldPosition;
					}
				}
			}
		}
		#endregion
		#region Helper Functions

		protected bool CheckIfCanSeeEntitie(Entitie target)
		{
			//First we check the middle and corners of the entitie
			//Middle
			if (CheckPointVisible(target.actuallWorldPosition))
				return true;

			//Bounding corners
			Vector3[] boundPoints = GetBoundPoints();
			for (int i = 0; i < boundPoints.Length; i++)
			{
				if (CheckPointVisible(boundPoints[i]))
					return true;
			}

			//Now we check a random point inside the bounding points
			for (int i = 0; i < VISIBLE_CHECK_RAY_COUNT; i++)
			{
				if (CheckPointVisible(GetRandomPointInBounds()))
					return true;
			}

			//None of the test rays hit, so we decide: We cant see the entitie
			return false;

			bool CheckPointVisible(Vector3 endPos)
			{
				Vector3 dif = endPos - PlayerCameraController.PlayerCamera.transform.position;
				float dist = dif.magnitude;
				Vector3 direction = dif / dist;

				return !Physics.Raycast(PlayerCameraController.PlayerCamera.transform.position, direction, dist, ConstantCollector.TERRAIN_LAYER);
			}
			Vector3[] GetBoundPoints()
			{
				Vector3[] points = new Vector3[8];
				Vector3 center = target.coll.bounds.center;
				Vector3 extents = target.coll.bounds.extents;

				//This will not account for rotation, however this does not have to be extremly accurate therefore we will save some performance by not rotating the points
				points[0] = center + new Vector3(extents.x * 1, extents.y * 1, extents.z * 1);
				points[1] = center + new Vector3(extents.x * 1, extents.y * 1, extents.z * -1);
				points[2] = center + new Vector3(extents.x * -1, extents.y * 1, extents.z * 1);
				points[3] = center + new Vector3(extents.x * -1, extents.y * 1, extents.z * -1);

				points[4] = center + new Vector3(extents.x * 1, extents.y * -1, extents.z * 1);
				points[5] = center + new Vector3(extents.x * 1, extents.y * -1, extents.z * -1);
				points[6] = center + new Vector3(extents.x * -1, extents.y * -1, extents.z * 1);
				points[7] = center + new Vector3(extents.x * -1, extents.y * -1, extents.z * -1);

				return points;
			}
			Vector3 GetRandomPointInBounds()
			{
				return target.coll.bounds.center +
					new Vector3(target.coll.bounds.extents.x * (Random.value - 0.5f) * 2,
									target.coll.bounds.extents.y * (Random.value - 0.5f) * 2,
									target.coll.bounds.extents.z * (Random.value - 0.5f) * 2);
			}
		}
		#endregion
	}
}