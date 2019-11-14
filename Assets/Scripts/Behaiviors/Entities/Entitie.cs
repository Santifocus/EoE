using EoE.Events;
using EoE.Information;
using EoE.UI;
using EoE.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Entities
{
	public abstract class Entitie : MonoBehaviour
	{
		#region Fields
		//Constants
		private const float GROUND_TEST_WIDHT_MUL = 0.95f;
		private const float JUMP_GROUND_COOLDOWN = 0.2f;
		private const float IS_FALLING_THRESHOLD = -1;
		private const float LANDED_VELOCITY_THRESHOLD = 0.5f;
		private const int VISIBLE_CHECK_RAY_COUNT = 40;

		private static readonly Vector3 GroundTestOffset = new Vector3(0, -0.05f, 0);

		public static List<Entitie> AllEntities = new List<Entitie>();

		//Inspector variables
		public Rigidbody body = default;
		public Collider coll = default;
		[SerializeField] protected Animator animationControl = default;

		//Stats
		public int EntitieLevel { get; protected set; }
		public float curMaxHealth { get; protected set; }
		public float curHealth { get; protected set; }
		public float curMaxMana { get; protected set; }
		public float curMana { get; protected set; }
		public float curPhysicalDamage { get; protected set; }
		public float curMagicalDamage { get; protected set; }
		public float curDefense { get; protected set; }
		public float curWalkSpeed { get; protected set; }
		public float curJumpPowerMultiplier { get; protected set; }

		//Entitie states
		[HideInInspector] public int invincible;
		public List<BuffInstance> nonPermanentBuffs { get; protected set; }
		public List<BuffInstance> permanentBuffs { get; protected set; }
		public EntitieState curStates;
		private float regenTimer;
		private float combatEndCooldown;

		//Velocity Control
		protected int appliedMoveStuns;
		private float jumpGroundCooldown;
		private float lastFallVelocity;

		protected Vector3 impactForce;

		protected float curRotation;
		protected float intendedRotation;

		//Getter Helpers
		protected enum ColliderType : byte { Box, Sphere, Capsule }
		public abstract EntitieSettings SelfSettings { get; }
		protected ColliderType selfColliderType;
		public Vector3 actuallWorldPosition => SelfSettings.MassCenter + transform.position;
		public float lowestPos => coll.bounds.center.y - coll.bounds.extents.y;
		public float highestPos => coll.bounds.center.y + coll.bounds.extents.y;
		public ForceController entitieForceController;
		public Vector3 curVelocity => new Vector3(0, body.velocity.y, 0) + impactForce + entitieForceController.currentTotalForce;
		public bool IsInvincible => invincible > 0;
		public bool IsStunned => appliedMoveStuns > 0;

		//Other
		private EntitieStatDisplay statDisplay;
		#endregion
		#region Basic Monobehaivior
		protected virtual void Start()
		{
			AllEntities.Add(this);

			FullEntitieReset();
			EntitieStart();
		}
		protected virtual void FullEntitieReset()
		{
			ResetStats();
			GetColliderType();
			curStates = new EntitieState();

			nonPermanentBuffs = new List<BuffInstance>();
			permanentBuffs = new List<BuffInstance>();
			if (!(this is Player))
				statDisplay = Instantiate(GameController.CurrentGameSettings.EntitieStatDisplayPrefab, GameController.Instance.enemyHealthBarStorage);

			entitieForceController = new ForceController();
		}
		private void ResetStats()
		{
			curMaxHealth = curHealth = SelfSettings.Health;
			curMaxMana = curMana = SelfSettings.Mana;
			curPhysicalDamage = SelfSettings.BaseAttackDamage;
			curMagicalDamage = SelfSettings.BaseMagicDamage;
			curDefense = SelfSettings.BaseDefense;
			curWalkSpeed = SelfSettings.WalkSpeed;
			curJumpPowerMultiplier = 1;
			appliedMoveStuns = 0;
		}
		private void GetColliderType()
		{
			if (!coll)
			{
				Debug.LogError("Entitie: '" + name + "' has no collision Collider attached. Please attach one and assign it in the inspector.");
				return;
			}

			if (coll is BoxCollider)
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
		protected virtual void EntitieStart() { }
		protected virtual void Update()
		{
			Regen();
			UpdateStatDisplay();
			EntitieStateControl();
			EntitieUpdate();
		}
		protected virtual void FixedUpdate()
		{
			entitieForceController.Update();

			IsGroundedTest();
			CheckForFalling();
			EntitieFixedUpdate();
		}
		protected virtual void EntitieUpdate() { }
		protected virtual void EntitieFixedUpdate() { }
		#endregion
		#region Movement
		private void CheckForFalling()
		{
			//Find out wether the entitie is falling or not
			bool playerWantsToFall = this is Player && (curStates.Falling || !Controlls.InputController.Jump.Active);
			bool falling = !curStates.Grounded && jumpGroundCooldown <= 0 && (body.velocity.y < IS_FALLING_THRESHOLD || playerWantsToFall);

			//If so: we enable the falling animation and add extra velocity for a better looking fallcurve
			curStates.Falling = falling;
			animationControl.SetBool("IsFalling", falling);
			if (falling)
			{
				body.velocity += GameController.CurrentGameSettings.WhenFallingExtraVelocity * Physics.gravity * Time.fixedDeltaTime;
			}
		}
		protected void Jump()
		{
			body.velocity = new Vector3(body.velocity.x, Mathf.Min(body.velocity.y + SelfSettings.JumpPower.y * curJumpPowerMultiplier, SelfSettings.JumpPower.y * curJumpPowerMultiplier), body.velocity.z);
			impactForce += SelfSettings.JumpPower.x * transform.right * curJumpPowerMultiplier;
			impactForce += SelfSettings.JumpPower.z * transform.forward * curJumpPowerMultiplier;

			jumpGroundCooldown = JUMP_GROUND_COOLDOWN;
			curStates.Grounded = false;

			animationControl.SetTrigger("JumpStart");
		}
		#endregion
		#region Ground Contact
		private void IsGroundedTest()
		{
			if (jumpGroundCooldown > 0)
			{
				jumpGroundCooldown -= Time.deltaTime;
				curStates.Grounded = false;
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

							curStates.Grounded = Physics.CheckBox(boxCenter, new Vector3(bColl.bounds.extents.x * GROUND_TEST_WIDHT_MUL, -GroundTestOffset.y, bColl.bounds.extents.z * GROUND_TEST_WIDHT_MUL), coll.transform.rotation, ConstantCollector.TERRAIN_LAYER);
							break;
						}
					case ColliderType.Capsule:
						{
							CapsuleCollider cColl = coll as CapsuleCollider;
							Vector3 extraOffset = new Vector3(0, -cColl.radius * (1 - GROUND_TEST_WIDHT_MUL), 0);
							Vector3 spherePos = coll.bounds.center - new Vector3(0, cColl.height / 2 * coll.transform.lossyScale.y - cColl.radius, 0) + GroundTestOffset + extraOffset;

							curStates.Grounded = Physics.CheckSphere(spherePos, cColl.radius * GROUND_TEST_WIDHT_MUL, ConstantCollector.TERRAIN_LAYER);
							break;
						}
					case ColliderType.Sphere:
						{
							SphereCollider sColl = coll as SphereCollider;
							Vector3 extraOffset = new Vector3(0, -sColl.radius * GROUND_TEST_WIDHT_MUL, 0);
							curStates.Grounded = Physics.CheckSphere(coll.bounds.center + GroundTestOffset + extraOffset, sColl.radius * GROUND_TEST_WIDHT_MUL, ConstantCollector.TERRAIN_LAYER);
							break;
						}
				}
			}

			//Check if we landed
			float velDif = curVelocity.y - lastFallVelocity;
			if (velDif > LANDED_VELOCITY_THRESHOLD && jumpGroundCooldown <= 0) //We stopped falling for a certain amount, and we didnt change velocity because we just jumped
			{
				Landed(velDif);
			}
			lastFallVelocity = curVelocity.y;

			//Lerp knockback to zero based on the entities deceleration stat
			if (SelfSettings.NoMoveDeceleration > 0)
				impactForce = Vector3.Lerp(impactForce, Vector3.zero, Time.fixedDeltaTime / SelfSettings.NoMoveDeceleration);
			else
				impactForce = Vector3.zero;
		}
		private void Landed(float velDif)
		{
			//Check if there is any fall damage to give
			float damageAmount = GameController.CurrentGameSettings.FallDamageCurve.Evaluate(velDif);
			if (damageAmount > 0)
				ChangeHealth(new ChangeInfo(null, CauseType.Physical, ElementType.None, actuallWorldPosition, Vector3.up, damageAmount, false));
		}
		#endregion
		#region State Control
		protected virtual void Regen()
		{
			regenTimer += Time.deltaTime;
			if (regenTimer >= GameController.CurrentGameSettings.SecondsPerEntititeRegen)
			{
				bool inCombat = curStates.Fighting;
				regenTimer -= GameController.CurrentGameSettings.SecondsPerEntititeRegen;
				if (SelfSettings.DoHealthRegen && curHealth < curMaxHealth)
				{
					float regenAmount = SelfSettings.HealthRegen * GameController.CurrentGameSettings.SecondsPerEntititeRegen * (inCombat ? SelfSettings.HealthRegenInCombatMultiplier : 1);
					if (regenAmount > 0)
					{
						ChangeInfo basis = new ChangeInfo(this, CauseType.Heal, -regenAmount);
						ChangeInfo.ChangeResult regenResult = new ChangeInfo.ChangeResult(basis, this, true, true);

						curHealth = Mathf.Min(curMaxHealth, curHealth - regenResult.finalChangeAmount);
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
			if (combatEndCooldown > 0)
			{
				combatEndCooldown -= Time.deltaTime;
				if (combatEndCooldown <= 0)
					curStates.Fighting = false;
			}
			BuffUpdate();
		}
		#region BuffControl
		private void BuffUpdate()
		{
			bool requiresRecalculate = false;
			for (int i = 0; i < nonPermanentBuffs.Count; i++)
			{
				nonPermanentBuffs[i].RemainingTime -= Time.deltaTime;
				for (int j = 0; j < nonPermanentBuffs[i].DOTCooldowns.Length; j++)
				{
					nonPermanentBuffs[i].DOTCooldowns[j] -= Time.deltaTime;
					if (nonPermanentBuffs[i].DOTCooldowns[j] <= 0)
					{
						float cd = nonPermanentBuffs[i].Base.DOTs[j].DelayPerActivation;
						nonPermanentBuffs[i].DOTCooldowns[j] += cd;
						ChangeHealth(new ChangeInfo(nonPermanentBuffs[i].Applier, CauseType.DOT, nonPermanentBuffs[i].Base.DOTs[j].Element, actuallWorldPosition, Vector3.up, cd * nonPermanentBuffs[i].Base.DOTs[j].BaseDamage, false));
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

			if (this is Player)
				Player.BuffDisplay.AddBuffIcon(newBuff);
			else
				statDisplay.AddBuffIcon(newBuff);

			if (buff.Permanent)
				permanentBuffs.Add(newBuff);
			else
				nonPermanentBuffs.Add(newBuff);
		}
		private enum CalculateValue : byte { Both = 0, Flat = 1, Percent = 2 }
		private void AddBuffEffect(BuffInstance buffInstance, CalculateValue toCalculate = CalculateValue.Both, bool clampRequired = true)
		{
			Buff buffBase = buffInstance.Base;
			for (int i = 0; i < buffBase.Effects.Length; i++)
			{
				if (toCalculate != CalculateValue.Both)
				{
					if (buffBase.Effects[i].Percent)
					{
						if (toCalculate == CalculateValue.Flat)
							continue;
					}
					else if (toCalculate == CalculateValue.Percent)
						continue;
				}
				float change = 0;

				switch (buffBase.Effects[i].targetStat)
				{
					case TargetStat.Health:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curMaxHealth : buffBase.Effects[i].Amount;
							change = Mathf.Max(-(curMaxHealth - 1), change);
							curMaxHealth += change;
							if (clampRequired)
								curHealth = Mathf.Min(curMaxHealth, curHealth);
							break;
						}
					case TargetStat.Mana:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curMaxMana : buffBase.Effects[i].Amount;
							change = Mathf.Max(-(curMaxMana), change);
							curMaxMana += change;
							if (clampRequired)
								curMana = Mathf.Min(curMaxMana, curMana);
							break;
						}
					case TargetStat.Endurance:
						{
							//If this is not a player we can just ignore this buff/debuff
							Player player = this as Player;
							if (player == null)
								break;

							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * player.trueEnduranceAmount : buffBase.Effects[i].Amount;
							change = Mathf.Max(-(player.trueEnduranceAmount), change);
							player.trueEnduranceAmount += change;

							if (clampRequired)
								player.UpdateEnduranceStat();
							break;
						}
					case TargetStat.PhysicalDamage:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curPhysicalDamage : buffBase.Effects[i].Amount;
							change = Mathf.Max(-curPhysicalDamage, change);
							curPhysicalDamage += change;
							break;
						}
					case TargetStat.MagicalDamage:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curMagicalDamage : buffBase.Effects[i].Amount;
							change = Mathf.Max(-curMagicalDamage, change);
							curMagicalDamage += change;
							break;
						}
					case TargetStat.Defense:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curDefense : buffBase.Effects[i].Amount;
							curDefense += change;
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
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curJumpPowerMultiplier : buffBase.Effects[i].Amount;
							curJumpPowerMultiplier += change;
							break;
						}
				}

				buffInstance.FlatChanges[i] = change;
			}

			//Custom buff Effects
			if (buffInstance.Base.CustomEffects.ApplyMoveStun)
				appliedMoveStuns++;
			if (buffInstance.Base.CustomEffects.Invincible)
				invincible++;
		}
		public void RemoveBuff(int index, bool fromPermanent)
		{
			BuffInstance targetBuff = fromPermanent ? permanentBuffs[index] : nonPermanentBuffs[index];
			RemoveBuffEffect(targetBuff);

			if (this is Player)
				Player.BuffDisplay.RemoveBuffIcon(targetBuff);
			else
				statDisplay.RemoveBuffIcon(targetBuff);

			if (fromPermanent)
				permanentBuffs.RemoveAt(index);
			else
				nonPermanentBuffs.RemoveAt(index);

			if (fromPermanent)
				RecalculateBuffs();
		}
		private void RemoveBuffEffect(BuffInstance buffInstance, bool clampRequired = true)
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
							if (clampRequired && curHealth > curMaxHealth)
								curHealth = curMaxHealth;
							break;
						}
					case TargetStat.Mana:
						{
							curMaxMana -= change;
							if (clampRequired && curMana > curMaxMana)
								curMana = curMaxMana;
							break;
						}
					case TargetStat.Endurance:
						{
							//If this is not a player we can just ignore this buff/debuff
							Player player = this as Player;
							if (player == null)
								break;

							player.trueEnduranceAmount -= change;
							if (clampRequired)
								player.UpdateEnduranceStat();
							break;
						}
					case TargetStat.PhysicalDamage:
						{
							curPhysicalDamage -= change;
							break;
						}
					case TargetStat.MagicalDamage:
						{
							curMagicalDamage -= change;
							break;
						}
					case TargetStat.Defense:
						{
							curDefense -= change;
							break;
						}
					case TargetStat.MoveSpeed:
						{
							curWalkSpeed -= change;
							break;
						}
					case TargetStat.JumpHeight:
						{
							curJumpPowerMultiplier -= change;
							break;
						}
				}
			}

			//Custom buff Effects
			if (buffInstance.Base.CustomEffects.ApplyMoveStun)
				appliedMoveStuns--;
			if (buffInstance.Base.CustomEffects.Invincible)
				invincible--;
		}
		public void RecalculateBuffs()
		{
			//Remove all effects
			curMaxHealth = SelfSettings.Health;
			curMaxMana = SelfSettings.Mana;
			if (this is Player)
				(this as Player).trueEnduranceAmount = Player.PlayerSettings.EnduranceBars * Player.PlayerSettings.EndurancePerBar;
			curPhysicalDamage = SelfSettings.BaseAttackDamage;
			curMagicalDamage = SelfSettings.BaseMagicDamage;
			curDefense = SelfSettings.BaseDefense;
			curWalkSpeed = SelfSettings.WalkSpeed;
			curJumpPowerMultiplier = 1;
			appliedMoveStuns = 0;

			//Now re-add them, so the values can be recalculated
			//First add flat values and then percent
			//Flat
			for (int i = 0; i < nonPermanentBuffs.Count; i++)
			{
				AddBuffEffect(nonPermanentBuffs[i], CalculateValue.Flat, false);
			}
			for (int i = 0; i < permanentBuffs.Count; i++)
			{
				AddBuffEffect(permanentBuffs[i], CalculateValue.Flat, false);
			}
			//Percent
			for (int i = 0; i < nonPermanentBuffs.Count; i++)
			{
				AddBuffEffect(nonPermanentBuffs[i], CalculateValue.Percent, false);
			}
			for (int i = 0; i < permanentBuffs.Count; i++)
			{
				AddBuffEffect(permanentBuffs[i], CalculateValue.Percent, false);
			}

			if (curHealth > curMaxHealth)
				curHealth = curMaxHealth;
			if (curMana > curMaxMana)
				curMana = curMaxMana;
			if (this is Player)
				(this as Player).UpdateEnduranceStat();
		}
		private void PermanentBuffsControl()
		{
			for (int i = 0; i < permanentBuffs.Count; i++)
			{
				//The only thing that needs to be controlled about permanent buffs are DOTs
				//If there are none nothing will change
				for (int j = 0; j < permanentBuffs[i].DOTCooldowns.Length; j++)
				{
					permanentBuffs[i].DOTCooldowns[j] -= Time.deltaTime;
					if (permanentBuffs[i].DOTCooldowns[j] <= 0)
					{
						float cd = permanentBuffs[i].Base.DOTs[j].DelayPerActivation;
						permanentBuffs[i].DOTCooldowns[j] += cd;
						ChangeHealth(new ChangeInfo(permanentBuffs[i].Applier, CauseType.DOT, permanentBuffs[i].Base.DOTs[j].Element, actuallWorldPosition, Vector3.up, cd * permanentBuffs[i].Base.DOTs[j].BaseDamage, false));
					}
				}
			}
		}
		#endregion

		public void ChangeHealth(ChangeInfo causedChange)
		{
			if (causedChange.attacker != null && causedChange.attacker != this)
			{
				StartCombat();
				causedChange.attacker.StartCombat();
			}
			ChangeInfo.ChangeResult changeResult = new ChangeInfo.ChangeResult(causedChange, this, true);

			//If this Entitie is invincible and the change causes damage and/or knockback then we stop here
			if (IsInvincible && (changeResult.finalChangeAmount > 0 || changeResult.causedKnockback.HasValue))
				return;

			//Cause the health change and clamp max
			curHealth -= changeResult.finalChangeAmount;
			curHealth = Mathf.Min(curMaxHealth, curHealth);

			//Apply knockback
			if (changeResult.causedKnockback.HasValue)
			{
				impactForce += new Vector3(changeResult.causedKnockback.Value.x, 0, changeResult.causedKnockback.Value.z);
				body.velocity = new Vector3(body.velocity.x, body.velocity.y + changeResult.causedKnockback.Value.y, body.velocity.z);
			}

			if (changeResult.finalChangeAmount > 0)
				ReceivedHealthDamage(causedChange, changeResult);

			//VFX for player
			if (this is Player)
			{
				if (changeResult.finalChangeAmount > 0)
				{
					if (Player.PlayerSettings.ColorScreenOnDamage)
					{
						EffectUtils.ColorScreen(Player.PlayerSettings.ColorScreenColor, Player.PlayerSettings.ColorScreenDuration, Player.PlayerSettings.ColorScreenDepth);
					}
					if (Player.PlayerSettings.BlurScreenOnDamage)
					{
						float healthNormalized = curHealth / curMaxHealth;
						if (healthNormalized < Player.PlayerSettings.BlurScreenHealthThreshold)
						{
							float intensity = Mathf.Min(1, (1 - healthNormalized / Player.PlayerSettings.BlurScreenHealthThreshold) * Player.PlayerSettings.BlurScreenBaseIntensity * changeResult.finalChangeAmount);
							EffectUtils.BlurScreen(intensity, Player.PlayerSettings.BlurScreenDuration);
						}
					}
				}
				if (changeResult.causedKnockback.HasValue && Player.PlayerSettings.ShakeScreenOnKnockback)
				{
					float shakeForce = changeResult.causedKnockback.Value.magnitude;
					EffectUtils.ShakeScreen(Player.PlayerSettings.ShakeTimeOnKnockback, shakeForce * Player.PlayerSettings.ShakeScreenAxisIntensity, shakeForce * Player.PlayerSettings.ShakeScreenAngleIntensity);
				}
			}

			//Below zero health means death
			if (curHealth <= 0)
			{
				if (this is Player)
				{
					EventManager.PlayerDiedInvoke(causedChange.attacker);
				}
				else
				{
					EventManager.EntitieDiedInvoke(this, causedChange.attacker);
				}
				Death();
			}
		}
		protected virtual void ReceivedHealthDamage(ChangeInfo causedChange, ChangeInfo.ChangeResult resultInfo) { }
		public void ChangeMana(ChangeInfo change)
		{
			ChangeInfo.ChangeResult changeResult = new ChangeInfo.ChangeResult(change, this, true);
			curMana -= changeResult.finalChangeAmount;
			curMana = Mathf.Min(curMaxMana, curMana);
		}
		protected virtual void UpdateStatDisplay()
		{
			//In world healthbar doesnt exist for player, because it is on the normal HUD
			if (this is Player)
				return;

			bool inCombat = curStates.Fighting;
			bool intendedState = inCombat;
			if (inCombat)
			{
				statDisplay.HealthValue = curHealth / curMaxHealth;
				Vector3 pos = PlayerCameraController.PlayerCamera.WorldToScreenPoint(new Vector3(coll.bounds.center.x, highestPos, coll.bounds.center.z));
				if (pos.z > 0)
					statDisplay.Position = pos + new Vector3(0, statDisplay.Height);
				else
					intendedState = false;
			}

			//If we are in a fight => show the display, otherwise hide it
			if (statDisplay.gameObject.activeInHierarchy != intendedState)
				statDisplay.gameObject.SetActive(intendedState);
		}
		protected void StartCombat()
		{
			curStates.Fighting = true;
			combatEndCooldown = GameController.CurrentGameSettings.CombatCooldown;
		}
		protected virtual void Death()
		{
			AllEntities.Remove(this);
			BuildDrops();
			if (statDisplay)
				Destroy(statDisplay.gameObject);
			Destroy(gameObject);
		}
		private void BuildDrops()
		{
			//Create souls drops
			if (SelfSettings.SoulWorth > 0)
			{
				SoulDrop newSoulDrop = Instantiate(GameController.CurrentGameSettings.SoulDropPrefab, Storage.DropStorage);
				newSoulDrop.transform.position = actuallWorldPosition;
				newSoulDrop.Setup(SelfSettings.SoulWorth);
			}

			//Create item / other drops
			if (SelfSettings.PossibleDropsTable == null)
				return;

			for (int i = 0; i < SelfSettings.PossibleDropsTable.PossibleDrops.Length; i++)
			{
				if (BaseUtils.Chance01(SelfSettings.PossibleDropsTable.PossibleDrops[i].DropChance))
				{
					int amount = Random.Range(SelfSettings.PossibleDropsTable.PossibleDrops[i].MinDropAmount, SelfSettings.PossibleDropsTable.PossibleDrops[i].MaxDropAmount + 1);
					SelfSettings.PossibleDropsTable.PossibleDrops[i].Drop.CreateItemDrop(actuallWorldPosition, amount, false);
				}
			}
		}
		#endregion
		#region Helper Functions

		protected bool CheckIfCanSeeEntitie(Entitie target, bool lowPriority = false)
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

			//Is this a low priority check? If so, then getting here means we just answer with false
			if (lowPriority)
				return false;

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