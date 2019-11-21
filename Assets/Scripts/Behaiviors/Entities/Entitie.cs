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
		private const int VISIBLE_CHECK_RAY_COUNT = 40;

		private static readonly Vector3 GroundTestOffset = new Vector3(0, -0.05f, 0);

		public static List<Entitie> AllEntities = new List<Entitie>();

		//Inspector variables
		public Collider coll = default;

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
		protected Vector2 impactForce;

		protected float curRotation;
		protected float intendedRotation;

		//Regen particles
		private bool showingHealthRegenParticles;
		private GameObject mainHealthRegenParticleObject;
		private ParticleSystem[] healthRegenParticleSystems;

		//Getter Helpers
		protected enum ColliderType : byte { Box, Sphere, Capsule }
		public abstract EntitieSettings SelfSettings { get; }
		protected ColliderType selfColliderType;
		public Vector3 actuallWorldPosition => SelfSettings.MassCenter + transform.position;
		public float lowestPos => coll.bounds.center.y - coll.bounds.extents.y;
		public float highestPos => coll.bounds.center.y + coll.bounds.extents.y;
		public ForceController entitieForceController;
		public virtual Vector3 curVelocity => new Vector3(impactForce.x, 0, impactForce.y) + entitieForceController.currentTotalForce;
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
			SetupHealingParticles();
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
			else if (coll is CapsuleCollider || coll is CharacterController)
			{
				selfColliderType = ColliderType.Capsule;
				return;
			}
			else
			{
				Debug.LogError("Entitie: " + name + " has a collision Collider attached, however the type '" + coll.GetType() + "' is not supported. Supported types are 'BoxCollider','SphereCollider' and 'CapsuleCollider'.");
			}
		}
		private void SetupHealingParticles()
		{
			if (mainHealthRegenParticleObject)
				Destroy(mainHealthRegenParticleObject);

			if (SelfSettings.HealthRegenParticles)
			{
				mainHealthRegenParticleObject = Instantiate(SelfSettings.HealthRegenParticles, transform);
				mainHealthRegenParticleObject.transform.localPosition = SelfSettings.HealthRegenParticlesOffset;

				healthRegenParticleSystems = mainHealthRegenParticleObject.GetComponentsInChildren<ParticleSystem>();

				for (int i = 0; i < healthRegenParticleSystems.Length; i++)
				{
					if (healthRegenParticleSystems[i].isPlaying)
						healthRegenParticleSystems[i].Stop();
				}
				showingHealthRegenParticles = false;
			}
			else
			{
				healthRegenParticleSystems = null;
			}
		}
		protected virtual void EntitieStart() { }
		protected virtual void Update()
		{
			Regen();
			EntitieStateControl();
			EntitieUpdate();
		}
		protected virtual void FixedUpdate()
		{
			entitieForceController.Update();

			//Lerp knockback to zero based on the entities deceleration stat
			if (SelfSettings.NoMoveDeceleration > 0)
				impactForce = Vector3.Lerp(impactForce, Vector3.zero, Time.fixedDeltaTime * SelfSettings.NoMoveDeceleration);
			else
				impactForce = Vector3.zero;

			UpdateStatDisplay();
			EntitieFixedUpdate();
		}
		protected virtual void EntitieUpdate() { }
		protected virtual void EntitieFixedUpdate() { }
		#endregion
		#region State Control
		protected virtual void Regen()
		{
			regenTimer += Time.deltaTime;
			if (regenTimer >= GameController.CurrentGameSettings.SecondsPerEntititeRegen)
			{
				bool inCombat = curStates.Fighting;
				regenTimer -= GameController.CurrentGameSettings.SecondsPerEntititeRegen;
				bool regendHealth = false;
				if (SelfSettings.DoHealthRegen && curHealth < curMaxHealth)
				{
					float regenAmount = SelfSettings.HealthRegen * GameController.CurrentGameSettings.SecondsPerEntititeRegen * (inCombat ? SelfSettings.HealthRegenInCombatMultiplier : 1);
					if (regenAmount > 0)
					{
						ChangeInfo basis = new ChangeInfo(this, CauseType.Heal, -regenAmount);
						ChangeInfo.ChangeResult regenResult = new ChangeInfo.ChangeResult(basis, this, true, true);
						curHealth = Mathf.Min(curMaxHealth, curHealth - regenResult.finalChangeAmount);

						regendHealth = regenResult.finalChangeAmount < 0;
					}
				}
				ControlRegenParticles(regendHealth);

				if (SelfSettings.DoManaRegen && curMana < curMaxMana)
				{
					float regenAmount = SelfSettings.ManaRegen * GameController.CurrentGameSettings.SecondsPerEntititeRegen * (inCombat ? SelfSettings.ManaRegenInCombatMultiplier : 1);
					curMana = Mathf.Min(curMaxMana, curMana + regenAmount);
				}
			}
		}
		private void ControlRegenParticles(bool regendHealth)
		{
			if (mainHealthRegenParticleObject)
			{
				if (showingHealthRegenParticles != regendHealth)
				{
					showingHealthRegenParticles = regendHealth;
					if (regendHealth)
					{
						for (int i = 0; i < healthRegenParticleSystems.Length; i++)
						{
							healthRegenParticleSystems[i].Play();
						}
					}
					else
					{

						for (int i = 0; i < healthRegenParticleSystems.Length; i++)
						{
							healthRegenParticleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
						}
					}
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

			//Update buffs
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

						if(nonPermanentBuffs[i].Base.DOTs[j].TargetStat == TargetStat.Health)
							ChangeHealth(new ChangeInfo(nonPermanentBuffs[i].Applier, CauseType.DOT, nonPermanentBuffs[i].Base.DOTs[j].Element, actuallWorldPosition, Vector3.up, cd * nonPermanentBuffs[i].Base.DOTs[j].BaseDamage, false));
						else if (nonPermanentBuffs[i].Base.DOTs[j].TargetStat == TargetStat.Mana)
							ChangeMana(new ChangeInfo(nonPermanentBuffs[i].Applier, CauseType.DOT, cd * nonPermanentBuffs[i].Base.DOTs[j].BaseDamage, false));
						else if(this is Player)//TargetStat.Endurance
							(this as Player).ChangeEndurance(new ChangeInfo(nonPermanentBuffs[i].Applier, CauseType.DOT, cd * nonPermanentBuffs[i].Base.DOTs[j].BaseDamage, false));
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
		public BuffInstance AddBuff(Buff buff, Entitie applier)
		{
			BuffInstance newBuff = new BuffInstance(buff, applier);
			AddBuffEffect(newBuff, CalculateValue.Flat);
			AddBuffEffect(newBuff, CalculateValue.Percent);

			if (this is Player)
				Player.BuffDisplay.AddBuffIcon(newBuff);
			else
				statDisplay.AddBuffIcon(newBuff);

			if (buff.Permanent)
				permanentBuffs.Add(newBuff);
			else
				nonPermanentBuffs.Add(newBuff);

			return newBuff;
		}
		private enum CalculateValue : byte { Both = 0, Flat = 1, Percent = 2 }
		private void AddBuffEffect(BuffInstance buffInstance, CalculateValue toCalculate = CalculateValue.Both, bool clampRequired = true)
		{
			Buff buffBase = buffInstance.Base;

			//Custom buff Effects
			//Apply only when either both flat and percent get calculated or only flat
			if (toCalculate != CalculateValue.Percent)
			{
				if (buffInstance.Base.CustomEffects.ApplyMoveStun)
					appliedMoveStuns++;
				if (buffInstance.Base.CustomEffects.Invincible)
					invincible++;
			}

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

				switch (buffBase.Effects[i].TargetBaseStat)
				{
					case TargetBaseStat.Health:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curMaxHealth : buffBase.Effects[i].Amount;
							change = Mathf.Max(-(curMaxHealth - 1), change);
							curMaxHealth += change;
							if (clampRequired)
								curHealth = Mathf.Min(curMaxHealth, curHealth);
							break;
						}
					case TargetBaseStat.Mana:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curMaxMana : buffBase.Effects[i].Amount;
							change = Mathf.Max(-(curMaxMana), change);
							curMaxMana += change;
							if (clampRequired)
								curMana = Mathf.Min(curMaxMana, curMana);
							break;
						}
					case TargetBaseStat.Endurance:
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
					case TargetBaseStat.PhysicalDamage:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curPhysicalDamage : buffBase.Effects[i].Amount;
							change = Mathf.Max(-curPhysicalDamage, change);
							curPhysicalDamage += change;
							break;
						}
					case TargetBaseStat.MagicalDamage:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curMagicalDamage : buffBase.Effects[i].Amount;
							change = Mathf.Max(-curMagicalDamage, change);
							curMagicalDamage += change;
							break;
						}
					case TargetBaseStat.Defense:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curDefense : buffBase.Effects[i].Amount;
							curDefense += change;
							break;
						}
					case TargetBaseStat.MoveSpeed:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curWalkSpeed : buffBase.Effects[i].Amount;
							curWalkSpeed += change;
							break;
						}
					case TargetBaseStat.JumpHeight:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curJumpPowerMultiplier : buffBase.Effects[i].Amount;
							curJumpPowerMultiplier += change;
							break;
						}
				}

				buffInstance.FlatChanges[i] = change;
			}
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
		public void RemoveBuff(BuffInstance buffInstance)
		{
			if (buffInstance.Base.Permanent)
			{
				for(int i = 0; i < permanentBuffs.Count; i++)
				{
					if(permanentBuffs[i] == buffInstance)
					{
						RemoveBuff(i, true);
					}
				}
			}
			else
			{
				for (int i = 0; i < nonPermanentBuffs.Count; i++)
				{
					if (nonPermanentBuffs[i] == buffInstance)
					{
						RemoveBuff(i, false);
					}
				}
			}
		}
		private void RemoveBuffEffect(BuffInstance buffInstance, bool clampRequired = true)
		{
			Buff buffBase = buffInstance.Base;

			for (int i = 0; i < buffBase.Effects.Length; i++)
			{
				float change = buffInstance.FlatChanges[i];

				switch (buffBase.Effects[i].TargetBaseStat)
				{
					case TargetBaseStat.Health:
						{
							curMaxHealth -= change;
							if (clampRequired && curHealth > curMaxHealth)
								curHealth = curMaxHealth;
							break;
						}
					case TargetBaseStat.Mana:
						{
							curMaxMana -= change;
							if (clampRequired && curMana > curMaxMana)
								curMana = curMaxMana;
							break;
						}
					case TargetBaseStat.Endurance:
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
					case TargetBaseStat.PhysicalDamage:
						{
							curPhysicalDamage -= change;
							break;
						}
					case TargetBaseStat.MagicalDamage:
						{
							curMagicalDamage -= change;
							break;
						}
					case TargetBaseStat.Defense:
						{
							curDefense -= change;
							break;
						}
					case TargetBaseStat.MoveSpeed:
						{
							curWalkSpeed -= change;
							break;
						}
					case TargetBaseStat.JumpHeight:
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

						if (permanentBuffs[i].Base.DOTs[j].TargetStat == TargetStat.Health)
							ChangeHealth(new ChangeInfo(permanentBuffs[i].Applier, CauseType.DOT, permanentBuffs[i].Base.DOTs[j].Element, actuallWorldPosition, Vector3.up, cd * permanentBuffs[i].Base.DOTs[j].BaseDamage, false));
						else if (permanentBuffs[i].Base.DOTs[j].TargetStat == TargetStat.Mana)
							ChangeMana(new ChangeInfo(permanentBuffs[i].Applier, CauseType.DOT, cd * permanentBuffs[i].Base.DOTs[j].BaseDamage, false));
						else if (this is Player)//TargetStat.Endurance
							(this as Player).ChangeEndurance(new ChangeInfo(permanentBuffs[i].Applier, CauseType.DOT, cd * permanentBuffs[i].Base.DOTs[j].BaseDamage, false));
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
				ApplyKnockback(changeResult.causedKnockback.Value);
			}

			if (changeResult.finalChangeAmount > 0)
				ReceivedHealthDamage(causedChange, changeResult);

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
		protected virtual void ApplyKnockback(Vector3 causedKnockback)
		{
			impactForce += new Vector2(causedKnockback.x, causedKnockback.z);
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
			BuildDrops();
			if (statDisplay)
				Destroy(statDisplay.gameObject);
			Destroy(gameObject);
		}
		private void OnDestroy()
		{
			AllEntities.Remove(this);
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

				return !Physics.Raycast(PlayerCameraController.PlayerCamera.transform.position, direction, dist, ConstantCollector.TERRAIN_LAYER_MASK);
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