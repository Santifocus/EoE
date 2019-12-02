using EoE.Events;
using EoE.Information;
using EoE.UI;
using EoE.Utils;
using EoE.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Entities
{
	public abstract class Entitie : MonoBehaviour
	{
		#region Fields
		//Constants
		private const int VISIBLE_CHECK_RAY_COUNT = 40;
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
		public bool Alive;
		public List<BuffInstance> nonPermanentBuffs { get; protected set; }
		public List<BuffInstance> permanentBuffs { get; protected set; }
		public EntitieState curStates;
		private float regenTimer;
		private float combatEndCooldown;
		protected Vector3? targetPosition = null;

		//Velocity Control
		protected int appliedMoveStuns;
		protected Vector2 impactForce;

		protected float curRotation;
		protected float intendedRotation;

		//Regen particles
		private bool showingHealthRegenParticles;
		private GameObject mainHealthRegenParticleObject;
		private ParticleSystem[] healthRegenParticleSystems;

		//Spell Casting
		public bool CastingSpell { get; private set; }
		public float CastingCooldown { get; private set; }

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
		protected virtual void EntitieStart() { }
		protected virtual void Update()
		{
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
		#region Setups
		protected virtual void FullEntitieReset()
		{
			Alive = true;
			ResetStats();
			GetColliderType();
			SetupHealingParticles();
			curStates = new EntitieState();
			intendedRotation = curRotation = transform.eulerAngles.y;

			nonPermanentBuffs = new List<BuffInstance>();
			permanentBuffs = new List<BuffInstance>();
			if (!(this is Player))
			{
				statDisplay = Instantiate(GameController.CurrentGameSettings.EntitieStatDisplayPrefab, GameController.Instance.enemyHealthBarStorage);
				statDisplay.Setup();
			}
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
		#endregion
		#region State Control
		private void EntitieStateControl()
		{
			CastingCooldown -= Time.deltaTime;
			if (combatEndCooldown > 0)
			{
				combatEndCooldown -= Time.deltaTime;
				if (combatEndCooldown <= 0)
					curStates.Fighting = false;
			}

			//Update buffs
			BuffUpdate();
			Regen();
		}
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
						ChangeInfo basis = new ChangeInfo(this, CauseType.Heal, TargetStat.Health, -regenAmount);
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
			if (!Alive)
				return;

			Alive = false;
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

						if (nonPermanentBuffs[i].Base.DOTs[j].TargetStat == TargetStat.Health)
							ChangeHealth(new ChangeInfo(nonPermanentBuffs[i].Applier, CauseType.DOT, nonPermanentBuffs[i].Base.DOTs[j].Element, TargetStat.Health, actuallWorldPosition, Vector3.up, cd * nonPermanentBuffs[i].Base.DOTs[j].BaseDamage, false));
						else if (nonPermanentBuffs[i].Base.DOTs[j].TargetStat == TargetStat.Mana)
							ChangeMana(new ChangeInfo(nonPermanentBuffs[i].Applier, CauseType.DOT, TargetStat.Mana, cd * nonPermanentBuffs[i].Base.DOTs[j].BaseDamage));
						else if (this is Player)//TargetStat.Endurance
							(this as Player).ChangeEndurance(new ChangeInfo(nonPermanentBuffs[i].Applier, CauseType.DOT, TargetStat.Endurance, cd * nonPermanentBuffs[i].Base.DOTs[j].BaseDamage));
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
		public bool HasBuffActive(Buff buff, Entitie applier)
		{
			List<BuffInstance> toSearch = buff.Permanent ? permanentBuffs : nonPermanentBuffs;
			for (int i = 0; i < toSearch.Count; i++)
			{
				if (toSearch[i].Applier == applier && toSearch[i].Base == buff)
				{
					return true;
				}
			}
			return false;
		}
		public (bool, BuffInstance) TryReapplyBuff(Buff buff, Entitie applier)
		{
			List<BuffInstance> toSearch = buff.Permanent ? permanentBuffs : nonPermanentBuffs;
			for (int i = 0; i < toSearch.Count; i++)
			{
				if (toSearch[i].Applier == applier && toSearch[i].Base == buff)
				{
					toSearch[i].RemainingTime = buff.BuffTime;
					return (true, toSearch[i]);
				}
			}

			return (false, null);
		}
		public BuffInstance AddBuff(Buff buff, Entitie applier)
		{
			BuffInstance newBuff = new BuffInstance(buff, applier, this);
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
			targetBuff.OnRemove();

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
				for (int i = 0; i < permanentBuffs.Count; i++)
				{
					if (permanentBuffs[i] == buffInstance)
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
							ChangeHealth(new ChangeInfo(permanentBuffs[i].Applier, CauseType.DOT, permanentBuffs[i].Base.DOTs[j].Element, TargetStat.Health, actuallWorldPosition, Vector3.up, cd * permanentBuffs[i].Base.DOTs[j].BaseDamage, false));
						else if (permanentBuffs[i].Base.DOTs[j].TargetStat == TargetStat.Mana)
							ChangeMana(new ChangeInfo(permanentBuffs[i].Applier, CauseType.DOT, TargetStat.Mana, cd * permanentBuffs[i].Base.DOTs[j].BaseDamage));
						else if (this is Player)//TargetStat.Endurance
							(this as Player).ChangeEndurance(new ChangeInfo(permanentBuffs[i].Applier, CauseType.DOT, TargetStat.Endurance, cd * permanentBuffs[i].Base.DOTs[j].BaseDamage));
					}
				}
			}
		}
		#endregion
		#region Spell Casting
		public bool CastSpell(Spell spell)
		{
			if (curMana < spell.ManaCost || CastingSpell || CastingCooldown > 0)
				return false;

			ChangeMana(new ChangeInfo(this, CauseType.Magic, TargetStat.Mana, spell.ManaCost));
			StartCoroutine(CastSpellC(spell));
			return true;
		}
		private IEnumerator CastSpellC(Spell spell)
		{
			CastingSpell = true;
			//Casting
			FXInstance[] curParticles = null;
			if (spell.ContainedParts.HasFlag(SpellPart.Cast))
			{
				bool appliedStun = spell.MovementRestrictions.HasFlag(SpellMovementRestrictionsMask.WhileCasting);
				if (appliedStun)
					appliedMoveStuns++;

				for(int i = 0; i < spell.CastInfo.StartEffects.Length; i++)
				{
					ActivateSpellEffect(this, spell.CastInfo.StartEffects[i], transform, spell);
				}
				curParticles = new FXInstance[spell.CastInfo.CustomEffects.Length];
				for (int i = 0; i < spell.CastInfo.CustomEffects.Length; i++)
				{
					curParticles[i] = FXManager.PlayFX(	spell.CastInfo.CustomEffects[i].FX, 
														transform, 
														this is Player,
														1,
														spell.CastInfo.CustomEffects[i].HasCustomOffset ? ((Vector3?)spell.CastInfo.CustomEffects[i].CustomOffset) : (null),
														spell.CastInfo.CustomEffects[i].HasCustomRotationOffset ? ((Vector3?)spell.CastInfo.CustomEffects[i].CustomRotation) : (null),
														spell.CastInfo.CustomEffects[i].HasCustomScale ? ((Vector3?)spell.CastInfo.CustomEffects[i].CustomScale) : (null)
														);
				}

				float castTime = 0;
				float effectTick = 0;
				while(castTime < spell.CastInfo.Duration)
				{
					yield return new WaitForEndOfFrame();
					castTime += Time.deltaTime;
					effectTick += Time.deltaTime;

					if (appliedMoveStuns > (appliedStun ? 1 : 0))
						goto StoppedSpell;

					if (effectTick > GameController.CurrentGameSettings.SpellEffectTickSpeed)
					{
						effectTick -= GameController.CurrentGameSettings.SpellEffectTickSpeed;
						for (int i = 0; i < spell.CastInfo.WhileEffects.Length; i++)
						{
							ActivateSpellEffect(this, spell.CastInfo.WhileEffects[i], transform, spell);
						}
					}
				}

				if (appliedStun)
					appliedMoveStuns--;
			}

			if(curParticles != null)
			{
				for(int i = 0; i < curParticles.Length; i++)
				{
					curParticles[i].FinishFX();
				}
				curParticles = null;
			}

			//Start
			if (spell.ContainedParts.HasFlag(SpellPart.Start))
			{
				for (int i = 0; i < spell.StartInfo.Effects.Length; i++)
				{
					ActivateSpellEffect(this, spell.StartInfo.Effects[i], transform, spell);
				}
				curParticles = new FXInstance[spell.StartInfo.CustomEffects.Length];
				for (int i = 0; i < spell.StartInfo.CustomEffects.Length; i++)
				{
					curParticles[i] = FXManager.PlayFX(spell.StartInfo.CustomEffects[i].FX,
														transform,
														this is Player,
														1,
														spell.StartInfo.CustomEffects[i].HasCustomOffset ? ((Vector3?)spell.StartInfo.CustomEffects[i].CustomOffset) : (null),
														spell.StartInfo.CustomEffects[i].HasCustomRotationOffset ? ((Vector3?)spell.StartInfo.CustomEffects[i].CustomRotation) : (null),
														spell.StartInfo.CustomEffects[i].HasCustomScale ? ((Vector3?)spell.StartInfo.CustomEffects[i].CustomScale) : (null)
														);
				}
			}

			if (curParticles != null)
			{
				for (int i = 0; i < curParticles.Length; i++)
				{
					curParticles[i].FinishFX();
				}
			}

			//Projectile
			if (spell.ContainedParts.HasFlag(SpellPart.Projectile))
			{
				bool appliedStun = spell.MovementRestrictions.HasFlag(SpellMovementRestrictionsMask.WhileShooting);
				if (appliedStun)
					appliedMoveStuns++;

				for (int i = 0; i < spell.ProjectileInfo.Length; i++)
				{
					for(int j = 0; j < spell.ProjectileInfo[i].ExecutionCount; j++)
					{
						CreateSpellProjectile(spell, i);
						if (j < spell.ProjectileInfo[i].ExecutionCount - 1)
						{
							float timer = 0;
							while(timer < spell.ProjectileInfo[i].DelayPerExecution)
							{
								yield return new WaitForEndOfFrame();
								timer += Time.deltaTime;
								if (appliedMoveStuns > (appliedStun ? 1 : 0))
									goto StoppedSpell;
							}
						}
					}

					if (i < spell.ProjectileInfo.Length - 1)
					{
						float timer = 0;
						while (timer < spell.DelayToNextProjectile[i])
						{
							yield return new WaitForEndOfFrame();
							timer += Time.deltaTime;
							if (appliedMoveStuns > (appliedStun ? 1 : 0))
								goto StoppedSpell;
						} 
					}
				}
				if (appliedStun)
					appliedMoveStuns--;
			}
		StoppedSpell:;
			CastingSpell = false;
			CastingCooldown = spell.SpellCooldown;
			if (curParticles != null)
			{
				for (int i = 0; i < curParticles.Length; i++)
				{
					curParticles[i].FinishFX();
				}
			}
		}
		#region ProjectileControl
		private SpellProjectile CreateSpellProjectile(Spell spellBase, int index)
		{
			Vector3 spawnOffset = spellBase.ProjectileInfo[index].CreateOffsetToCaster.x * transform.right + spellBase.ProjectileInfo[index].CreateOffsetToCaster.y * transform.up + spellBase.ProjectileInfo[index].CreateOffsetToCaster.z * transform.forward;

			//First find out what direction the projectile should fly
			Vector3 direction;
			(Vector3, bool) dirInfo = Spell.EnumDirToDir(spellBase.ProjectileInfo[index].Direction);
			if (spellBase.ProjectileInfo[index].DirectionStyle == InherritDirection.Target)
			{
				if (targetPosition.HasValue)
				{
					direction = (targetPosition.Value - (actuallWorldPosition + spawnOffset)).normalized;
				}
				else
				{
					direction = DirectionFromStyle(spellBase.ProjectileInfo[index].FallbackDirectionStyle);
				}
			}
			else
			{
				direction = DirectionFromStyle(spellBase.ProjectileInfo[index].DirectionStyle);
			}

			//Now create the projectile prefab, and let it start flying
			SpellProjectile projectile = Instantiate(GameController.ProjectilePrefab, Storage.ProjectileStorage);
			projectile.Setup(spellBase, index, this, direction);
			projectile.transform.position = actuallWorldPosition + spawnOffset;

			return projectile;
			Vector3 DirectionFromStyle(InherritDirection style)
			{
				if (style == InherritDirection.World)
				{
					return dirInfo.Item1 * (dirInfo.Item2 ? -1 : 1);
				}
				else //style == InherritDirection.Local
				{
					if (dirInfo.Item1 == new Vector3Int(0, 0, 1))
					{
						return transform.forward * (dirInfo.Item2 ? -1 : 1);
					}
					else if (dirInfo.Item1 == new Vector3Int(1, 0, 0))
					{
						return transform.right * (dirInfo.Item2 ? -1 : 1);
					}
					else //dir.Item1 == new Vector3Int(0, 1, 0)
					{
						return transform.up * (dirInfo.Item2 ? -1 : 1);
					}
				}
			}
		}
		#endregion
		#region SpellActivation
		//Here is where the magic literally happens
		public static void ActivateSpellEffect(Entitie caster, SpellEffect effect, Transform origin, Spell spellBase)
		{
			//If the effect is not applying force based on its center we can find out the direction here
			//so we dont have to recalculate it everytime a new eligible target is added
			Vector3 localDirection = Vector3.up;
			if (effect.KnockbackOrigin != EffectiveDirection.Center)
			{
				(Vector3Int, bool) dirInfo = Spell.EnumDirToDir(effect.KnockbackDirection);
				if (effect.KnockbackOrigin == EffectiveDirection.World)
				{
					localDirection = dirInfo.Item1 * (dirInfo.Item2 ? -1 : 1);
				}
				else //effect.KnockbackDirection == EffectiveDirection.Local
				{
					if (dirInfo.Item1 == new Vector3Int(0, 0, 1))
					{
						localDirection = origin.forward * (dirInfo.Item2 ? -1 : 1);
					}
					else if (dirInfo.Item1 == new Vector3Int(1, 0, 0))
					{
						localDirection = origin.right * (dirInfo.Item2 ? -1 : 1);
					}
					else //dir.Item1 == new Vector3Int(0, 1, 0)
					{
						localDirection = origin.up * (dirInfo.Item2 ? -1 : 1);
					}
				}
			}

			//First find the targets that are eligible
			float innerSphereDist = effect.BaseEffectRadius * effect.BaseEffectRadius;
			float outerSphereDist = effect.ZeroOutDistance * effect.ZeroOutDistance;

			//In order to dodge using a list so we dont spam the garbage collector we first find out how many entities will be added
			//And then build a array with that size, for that we could use Linq aswell but this methode is faster (probably)
			int requiredCapacity = 0;
			for (int i = 0; i < AllEntities.Count; i++)
			{
				if ((AllEntities[i].actuallWorldPosition - origin.position).sqrMagnitude < outerSphereDist)
				{
					//Check if this entitie should be a targetable entitie
					if (Spell.IsAllowedEntitie(AllEntities[i], caster, effect.AffectedTargets))
						requiredCapacity++;
				}
			}
			CollectedEntitieData[] eligibleTargets = new CollectedEntitieData[requiredCapacity];

			int addedEntities = 0;
			for (int i = 0; i < AllEntities.Count; i++)
			{
				Vector3 dif = AllEntities[i].actuallWorldPosition - origin.position;
				float sqrDist = dif.sqrMagnitude;

				//Generally onbly allow to keep going if the distance is smaller then the other sphere distance
				if (sqrDist < outerSphereDist)
				{
					//Check if this entitie should be a targetable entitie
					if (!Spell.IsAllowedEntitie(AllEntities[i], caster, effect.AffectedTargets))
						continue;

					CollectedEntitieData data = new CollectedEntitieData()
					{
						Target = AllEntities[i],
						SqrDist = sqrDist
					};

					//If the target is in the outer sphere and we want to normalize the direction based on the center
					//then we can save one square root by only calculating it in the multiplier calculation
					float? distance = null;

					//Calculate the multiplier
					if (sqrDist <= innerSphereDist)
					{
						//Max is 1 and inner sphere will always be max
						data.Multiplier = 1;
					}
					else //(sqrDist < outerSphereDist) && (sqrDist > innerSphereDist)
					{
						distance = Mathf.Sqrt(sqrDist);
						float outerDistance = distance.Value - effect.BaseEffectRadius;
						//The divider will never be zero because of previous if statements so we dont have to catch it
						data.Multiplier = outerDistance / (effect.ZeroOutDistance - effect.BaseEffectRadius);
					}

					//Now find out in what direction we want to apply forces
					if(effect.KnockbackOrigin == EffectiveDirection.Center)
					{
						if (!distance.HasValue)
							distance = Mathf.Sqrt(sqrDist);

						data.ApplyDirection = (distance.Value > 0) ? (dif / distance.Value) : (Vector3.up);
					}
					else//effect.KnockbackDirection == EffectiveDirection.Local || effect.KnockbackDirection == EffectiveDirection.World
					{
						data.ApplyDirection = localDirection;
					}
					eligibleTargets[addedEntities] = data;
					addedEntities++;
				}
			}

			//If there is a limited amount of hits we sort based on distance and later we only use the first 'effect.MaximumHits' results in the list
			if ((effect.HasMaximumHits) && (eligibleTargets.Length > effect.MaximumHits))
			{
				System.Array.Sort(eligibleTargets, (x, y) => x.SqrDist.CompareTo(y.SqrDist));
			}

			//Now we have to apply effects: Damage, Knockback, Buffs and FXEffects
			int targetCount = effect.HasMaximumHits ? System.Math.Min(eligibleTargets.Length, effect.MaximumHits) : eligibleTargets.Length;
			bool effectWasCrit = Random.value < effect.CritChance;
			float baseKnockBack = effect.KnockbackMultiplier * spellBase.BaseKnockback;

			for (int i = 0; i < targetCount; i++)
			{
				//Damage / Knockback
				float damage = spellBase.BaseDamage * effect.DamageMultiplier * eligibleTargets[i].Multiplier;
				float? knockbackAmount = (baseKnockBack != 0) ? (float?)(baseKnockBack * eligibleTargets[i].Multiplier) : (null);

				eligibleTargets[i].Target.ChangeHealth(new ChangeInfo(
					caster, 
					CauseType.Magic, 
					effect.DamageElement, 
					TargetStat.Health, 
					eligibleTargets[i].Target.actuallWorldPosition,
					eligibleTargets[i].ApplyDirection, 
					damage, 
					effectWasCrit, 
					knockbackAmount));

				//Buffs
				for(int j = 0; j < effect.BuffsToApply.Length; j++)
				{
					if(effect.BuffStackStyle == BuffStackingStyle.Stack)
					{
						eligibleTargets[i].Target.AddBuff(effect.BuffsToApply[j], caster);
					}
					else if (effect.BuffStackStyle == BuffStackingStyle.Reapply)
					{
						if (!(eligibleTargets[i].Target.TryReapplyBuff(effect.BuffsToApply[j], caster).Item1))
						{
							eligibleTargets[i].Target.AddBuff(effect.BuffsToApply[j], caster);
						}
					}
					else //effect.BuffStackStyle == BuffStackingStyle.DoNothing
					{
						if (!(eligibleTargets[i].Target.HasBuffActive(effect.BuffsToApply[j], caster)))
						{
							eligibleTargets[i].Target.AddBuff(effect.BuffsToApply[j], caster);
						}
					}
				}

				//FXEffects
				for (int j = 0; j < effect.Effects.Length; j++)
				{
					FXManager.PlayFX(effect.Effects[j], eligibleTargets[i].Target.transform, eligibleTargets[i].Target is Player, eligibleTargets[i].Multiplier);
				}
			}
		}
		private struct CollectedEntitieData
		{
			public Entitie Target;
			public float Multiplier;
			public float SqrDist;
			public Vector3 ApplyDirection;
		}
		#endregion
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