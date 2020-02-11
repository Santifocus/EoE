using EoE.Combatery;
using EoE.Events;
using EoE.Information;
using EoE.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Behaviour.Entities
{
	public abstract class Entity : MonoBehaviour
	{
		#region Fields
		//Constants
		private const int VISIBLE_CHECK_RAY_COUNT = 25;
		public static List<Entity> AllEntities = new List<Entity>();

		//Inspector variables
		public Collider coll = default;
		public int StartLevel = 0;

		//Stats
		public int EntitieLevel { get; protected set; }
		public float CurMaxHealth { get; protected set; }
		public float CurHealth { get; protected set; }
		public float CurMaxMana { get; protected set; }
		public float CurMana { get; protected set; }
		public float CurPhysicalDamage { get; protected set; }
		public float CurMagicalDamage { get; protected set; }
		public float CurDefense { get; protected set; }
		public float CurWalkSpeed { get; protected set; }
		public float CurJumpPowerMultiplier { get; protected set; }
		public float CurTrueDamageDamageMultiplier { get; protected set; }

		//Entitie states
		public bool Alive { get; protected set; }

		public List<BuffInstance> ActiveBuffs { get; protected set; }
		public EntityState curStates;
		private float healthRegenCooldown;
		private float healthRegenDelay;
		private float combatEndCooldown;
		public Vector3? targetPosition = null;

		//Iteratable States
		public int Invincibilities { get; set; }
		public int Stuns { get; set; }
		public int MovementStops { get; set; }
		public int TurnStops { get; set; }
		public int AttackStops { get; set; }
		public int CastingStops { get; set; }
		public bool IsInvincible => Invincibilities > 0;
		public bool IsStunned => Stuns > 0;
		public bool IsMovementStopped => MovementStops > 0;
		public bool IsTurnStopped => TurnStops > 0;
		public bool IsAttackStopped => AttackStops > 0;
		public bool IsCastingStopped => CastingStops > 0;

		//Velocity Control
		protected Vector2 impactForce;

		protected float curRotation;
		protected float intendedRotation;

		//Regen particles
		private bool showingHealthRegenParticles;
		private GameObject mainHealthRegenParticleObject;
		private ParticleSystem[] healthRegenParticleSystems;

		//Action Cooldowns
		public bool InActivationCompound { get; set; }
		public float AttackCooldown { get; set; }
		public float CastingCooldown { get; set; }

		//Getter Helpers
		protected enum ColliderType : byte { Box, Sphere, Capsule }
		public abstract EntitySettings SelfSettings { get; }
		protected ColliderType selfColliderType;
		public Vector3 ActuallWorldPosition => SelfSettings.MassCenter + transform.position;
		public float LowestBoundingPos => coll.bounds.center.y - coll.bounds.extents.y;
		public float HighestBoundingPos => coll.bounds.center.y + coll.bounds.extents.y;
		public ForceController entitieForceController;
		public virtual Vector3 CurVelocity => new Vector3(impactForce.x, 0, impactForce.y) + entitieForceController.currentTotalForce;

		//Armor
		public InventoryItem EquipedArmor;
		public BuffInstance ArmorBuff;

		//Other
		private EntityStatDisplay statDisplay;
		public Buff LevelingBaseBuff { get; protected set; }
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
			if (!Alive)
				return;
			EntitieStateControl();
			EntitieUpdate();
		}
		protected virtual void FixedUpdate()
		{
			if (!Alive)
				return;
			entitieForceController.FixedUpdate();

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
			curStates = new EntityState();
			entitieForceController = new ForceController();

			if (!(this is Player))
			{
				statDisplay = Instantiate(GameController.CurrentGameSettings.EntitieStatDisplayPrefab, GameController.Instance.enemyHealthBarStorage);
				statDisplay.Setup(SelfSettings.ShowEntitieLevel ? (int?)(StartLevel + 1) : null);
			}

			ResetStats();
			ResetStatValues();
			GetColliderType();
			SetupHealingParticles();
			BuffSetup();

			Stuns = 0;
			Invincibilities = 0;
			intendedRotation = curRotation = transform.eulerAngles.y;
		}
		protected virtual void ResetStats()
		{
			CurMaxHealth = SelfSettings.Health;
			CurMaxMana = SelfSettings.Mana;
			CurPhysicalDamage = SelfSettings.BasePhysicalDamage;
			CurMagicalDamage = SelfSettings.BaseMagicalDamage;
			CurDefense = SelfSettings.BaseDefense;
			CurWalkSpeed = SelfSettings.WalkSpeed;
			CurJumpPowerMultiplier = 1;
			CurTrueDamageDamageMultiplier = 1;
		}
		protected virtual void ResetStatValues()
		{
			CurHealth = CurMaxHealth;
			CurMana = CurMaxMana;
		}
		private void BuffSetup()
		{
			ActiveBuffs = new List<BuffInstance>();
			LevelSetup();
		}
		protected virtual void LevelSetup()
		{
			LevelingBaseBuff = ScriptableObject.CreateInstance<Buff>();
			{
				LevelingBaseBuff.Name = "LevelingBase";
				LevelingBaseBuff.Quality = BuffType.Positive;
				LevelingBaseBuff.Icon = null;
				LevelingBaseBuff.FinishConditions = new FinishConditions()
				{
					OnTimeout = false,
				};
				LevelingBaseBuff.DOTs = new DOT[0];
			}

			int incremtingStats = System.Enum.GetNames(typeof(TargetBaseStat)).Length;
			LevelingBaseBuff.Effects = new Effect[incremtingStats];

			for (int i = 0; i < incremtingStats; i++)
			{
				LevelingBaseBuff.Effects[i] =
					new Effect
					{
						Amount = 0,
						Percent = false,
						TargetBaseStat = (TargetBaseStat)i
					};
			}
			Buff.ApplyBuff(LevelingBaseBuff, this, this);
			for (int i = 0; i < StartLevel; i++)
				LevelUpEntitie();
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
			if (AttackCooldown > 0)
				AttackCooldown -= Time.deltaTime;
			if (CastingCooldown > 0)
				CastingCooldown -= Time.deltaTime;
			if (combatEndCooldown > 0)
			{
				combatEndCooldown -= Time.deltaTime;
				if (combatEndCooldown <= 0)
				{
					curStates.Fighting = false;
					CombatEnd();
				}
			}

			//Update buffs
			BuffUpdate();
			Regen();
		}
		protected virtual void CombatEnd() { }
		protected virtual void Regen()
		{
			healthRegenDelay -= Time.deltaTime;
			if (healthRegenCooldown > 0)
				healthRegenCooldown -= Time.deltaTime;

			bool inCombat = curStates.Fighting;
			//Health Regen
			if (healthRegenDelay <= 0)
			{
				healthRegenDelay += GameController.CurrentGameSettings.SecondsPerEntitieHealthRegen;
				bool regendHealth = false;
				if (SelfSettings.DoHealthRegen && healthRegenCooldown <= 0 && CurHealth < CurMaxHealth)
				{
					float regenAmount = SelfSettings.HealthRegen * GameController.CurrentGameSettings.SecondsPerEntitieHealthRegen * (inCombat ? SelfSettings.HealthRegenInCombatMultiplier : 1);
					if (regenAmount > 0)
					{
						ChangeInfo basis = new ChangeInfo(this, CauseType.Heal, TargetStat.Health, -regenAmount);
						ChangeInfo.ChangeResult regenResult = new ChangeInfo.ChangeResult(basis, this, true, true);
						CurHealth = Mathf.Min(CurHealth - regenResult.finalChangeAmount, CurMaxHealth);

						regendHealth = regenResult.finalChangeAmount < 0;
					}
				}
				ControlRegenParticles(regendHealth);
			}
			//Mana Regen
			if (SelfSettings.DoManaRegen && CurMana < CurMaxMana)
			{
				float regenAmount = SelfSettings.ManaRegen * Time.deltaTime * (inCombat ? SelfSettings.ManaRegenInCombatMultiplier : 1);
				CurMana = Mathf.Min(CurMana + regenAmount, CurMaxMana);
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
			if (!Alive)
				return;

			ChangeInfo.ChangeResult changeResult = new ChangeInfo.ChangeResult(causedChange, this, true);
			if (causedChange.attacker != null && causedChange.attacker != this)
			{
				StartCombat();
				causedChange.attacker.StartCombat();
			}

			//If this Entitie is invincible and the change causes damage and/or knockback then we stop here

			if (IsInvincible && (changeResult.finalChangeAmount > 0 || changeResult.causedKnockback.HasValue))
				return;

			//Cause the health change and clamp max
			CurHealth -= changeResult.finalChangeAmount;
			CurHealth = Mathf.Min(CurMaxHealth, CurHealth);

			//Apply knockback
			if (changeResult.causedKnockback.HasValue)
			{
				ApplyKnockback(changeResult.causedKnockback.Value);
			}

			if (changeResult.finalChangeAmount > 0)
				healthRegenCooldown = SelfSettings.HealthRegenCooldownAfterTakingDamage;

			//Did this cause the health to drop below zero? Then kill this entity
			if (CurHealth <= 0)
			{
				BaseDeath(causedChange.attacker);
			}

			if(changeResult.finalChangeAmount != 0 && Alive)
				OnHealthChange(causedChange, changeResult);
		}
		protected virtual void ApplyKnockback(Vector3 causedKnockback)
		{
			impactForce += new Vector2(causedKnockback.x, causedKnockback.z);
		}
		protected virtual void OnHealthChange(ChangeInfo causedChange, ChangeInfo.ChangeResult resultInfo) { }
		public void ChangeMana(ChangeInfo change)
		{
			ChangeInfo.ChangeResult changeResult = new ChangeInfo.ChangeResult(change, this, true);
			CurMana -= changeResult.finalChangeAmount;
			CurMana = Mathf.Min(CurMaxMana, CurMana);
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
				float curHealthNormalized = CurHealth / CurMaxHealth;
				if (statDisplay.HealthValue != curHealthNormalized)
					statDisplay.HealthValue = curHealthNormalized;

				Vector3 pos = PlayerCameraController.PlayerCamera.WorldToScreenPoint(new Vector3(coll.bounds.center.x, HighestBoundingPos, coll.bounds.center.z));
				if (pos.z > 0)
					statDisplay.Position = pos + new Vector3(0, statDisplay.Height);
				else
					intendedState = false;
			}

			//If we are in a fight => show the display, otherwise hide it
			if (statDisplay.gameObject.activeInHierarchy != intendedState)
				statDisplay.gameObject.SetActive(intendedState);
		}
		public virtual void StartCombat()
		{
			if(!curStates.Fighting)
				curStates.Fighting = true;
			combatEndCooldown = GameController.CurrentGameSettings.CombatCooldown;
		}
		protected virtual void LevelUpEntitie()
		{
			if (!SelfSettings.LevelSettings)
				return;
			EntitieLevel++;
			const int enumOffset = (int)TargetBaseStat.PhysicalDamage;

			float baseRotationAmount = (EntitieLevel / 10) * SelfSettings.LevelSettings.PerTenLevelsBasePoints + 1;
			int rotationIndex = (EntitieLevel + 1) % 3 + enumOffset;
			for (int i = enumOffset; i < enumOffset + 3; i++)
			{
				LevelingBaseBuff.Effects[i].Amount += baseRotationAmount;
				if (i == rotationIndex)
					LevelingBaseBuff.Effects[i].Amount += SelfSettings.LevelSettings.RotationExtraPoints;
			}
			RecalculateBuffs();
		}
		public void BaseDeath(Entity killer)
		{
			if (!Alive)
				return;

			Alive = false;
			if (this is Player)
			{
				EventManager.PlayerDiedInvoke(killer);
			}
			else
			{
				EventManager.EntitieDiedInvoke(this, killer);
			}

			AllEntities.Remove(this);
			ActivateActivationEffects(SelfSettings.DeathEffects, 1);

			Death();
		}
		protected virtual void Death()
		{
			BuildDrops();
			if (statDisplay)
				Destroy(statDisplay.gameObject);
			coll.gameObject.layer = ConstantCollector.TERRAIN_COLLIDING_LAYER;

			StartCoroutine(DelayedBodyRemoval());
		}
		private IEnumerator DelayedBodyRemoval()
		{
			yield return new WaitForSeconds(SelfSettings.BodyRemoveDelay);
			gameObject.SetActive(false);
		}
		private void OnDestroy()
		{
			AllEntities.Remove(this);
		}
		private void BuildDrops()
		{
			//Create Experience drops
			if ((SelfSettings.ExperienceWorth > 0) && !(this is Player))
			{
				SoulDrop newSoulDrop = Instantiate(GameController.SoulDropPrefab, Storage.DropStorage);
				newSoulDrop.transform.position = ActuallWorldPosition;
				newSoulDrop.Setup(SelfSettings.ExperienceWorth);
			}

			//Create item / other drops
			if (SelfSettings.PossibleDropsTable == null)
				return;

			for (int i = 0; i < SelfSettings.PossibleDropsTable.PossibleDrops.Length; i++)
			{
				if (Utils.Chance01(SelfSettings.PossibleDropsTable.PossibleDrops[i].DropChance))
				{
					int amount = Random.Range(SelfSettings.PossibleDropsTable.PossibleDrops[i].MinDropAmount, SelfSettings.PossibleDropsTable.PossibleDrops[i].MaxDropAmount + 1);
					SelfSettings.PossibleDropsTable.PossibleDrops[i].Drop.CreateItemDrop(ActuallWorldPosition, amount, false);
				}
			}
		}
		#endregion
		#region BuffControl
		private void BuffUpdate()
		{
			bool requiresRecalculate = false;
			for (int i = 0; i < ActiveBuffs.Count; i++)
			{
				bool shouldBeRemoved = ActiveBuffs[i].Update();

				for (int j = 0; j < ActiveBuffs[i].DOTCooldowns.Length; j++)
				{
					if (ActiveBuffs[i].DOTCooldowns[j] <= 0)
					{
						float cd = ActiveBuffs[i].Base.DOTs[j].DelayPerActivation;
						ActiveBuffs[i].DOTCooldowns[j] += cd;

						if (ActiveBuffs[i].Base.DOTs[j].TargetStat == TargetStat.Health)
						{
							ChangeHealth(new ChangeInfo(ActiveBuffs[i].Applier, CauseType.DOT, ActiveBuffs[i].Base.DOTs[j].Element, TargetStat.Health, ActuallWorldPosition, Vector3.up, cd * ActiveBuffs[i].Base.DOTs[j].BaseDamage, false));
						}
						else if (ActiveBuffs[i].Base.DOTs[j].TargetStat == TargetStat.Mana)
						{
							ChangeMana(new ChangeInfo(ActiveBuffs[i].Applier, CauseType.DOT, TargetStat.Mana, cd * ActiveBuffs[i].Base.DOTs[j].BaseDamage));
						}
						else if (this is Player)
						{
							if (ActiveBuffs[i].Base.DOTs[j].TargetStat == TargetStat.Stamina)
								(this as Player).ChangeStamina(new ChangeInfo(ActiveBuffs[i].Applier, CauseType.DOT, TargetStat.Stamina, cd * ActiveBuffs[i].Base.DOTs[j].BaseDamage));
							else if (ActiveBuffs[i].Base.DOTs[j].TargetStat == TargetStat.UltimateCharge && WeaponController.Instance)
								WeaponController.Instance.AddUltimateCharge(cd * ActiveBuffs[i].Base.DOTs[j].BaseDamage);
						}
					}
				}
				if (shouldBeRemoved)
				{
					requiresRecalculate = true;
					RemoveBuff(i);
					i--;
				}
			}

			if (requiresRecalculate)
				RecalculateBuffs();
		}
		public int? HasBuffActive(Buff buff, Entity applier)
		{
			for (int i = 0; i < ActiveBuffs.Count; i++)
			{
				if (ActiveBuffs[i].Applier == applier && ActiveBuffs[i].Base == buff)
				{
					return i;
				}
			}
			return null;
		}
		public (bool, BuffInstance) TryReapplyBuff(Buff buff, Entity applier)
		{
			for (int i = 0; i < ActiveBuffs.Count; i++)
			{
				if (ActiveBuffs[i].Applier == applier && ActiveBuffs[i].Base == buff)
				{
					ActiveBuffs[i].Reset();
					return (true, ActiveBuffs[i]);
				}
			}

			return (false, null);
		}
		public BuffInstance AddBuff(Buff buff, Entity applier, float multiplier = 1)
		{
			BuffInstance newBuff = new BuffInstance(buff, applier, this, multiplier);
			AddBuffEffect(newBuff, CalculateValue.Flat);
			AddBuffEffect(newBuff, CalculateValue.Percent);

			//Custom Buff Effects
			if (buff.CustomEffects.ApplyMoveStun)
				Stuns++;
			if (buff.CustomEffects.Invincible)
				Invincibilities++;

			if (this is Player)
				Player.Instance.buffDisplay.AddBuffIcon(newBuff);
			else
				statDisplay.AddBuffIcon(newBuff);

			ActiveBuffs.Add(newBuff);

			return newBuff;
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

				switch (buffBase.Effects[i].TargetBaseStat)
				{
					case TargetBaseStat.Health:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * CurMaxHealth : buffBase.Effects[i].Amount;
							change = Mathf.Max(-(CurMaxHealth - 1), change);
							CurMaxHealth += change;
							if (clampRequired)
								CurHealth = Mathf.Min(CurMaxHealth, CurHealth);
							break;
						}
					case TargetBaseStat.Mana:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * CurMaxMana : buffBase.Effects[i].Amount;
							change = Mathf.Max(-(CurMaxMana), change);
							CurMaxMana += change;
							if (clampRequired)
								CurMana = Mathf.Min(CurMaxMana, CurMana);
							break;
						}
					case TargetBaseStat.Stamina:
						{
							//If this is not a player we can just ignore this buff/debuff
							Player player = this as Player;
							if (player == null)
								break;

							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * player.CurMaxStamina : buffBase.Effects[i].Amount;
							change = Mathf.Max(-(player.CurMaxStamina), change);
							player.CurMaxStamina += change;

							if (clampRequired)
								player.ClampStamina();
							break;
						}
					case TargetBaseStat.PhysicalDamage:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * CurPhysicalDamage : buffBase.Effects[i].Amount;
							change = Mathf.Max(-CurPhysicalDamage, change);
							CurPhysicalDamage += change;
							break;
						}
					case TargetBaseStat.MagicalDamage:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * CurMagicalDamage : buffBase.Effects[i].Amount;
							change = Mathf.Max(-CurMagicalDamage, change);
							CurMagicalDamage += change;
							break;
						}
					case TargetBaseStat.Defense:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * CurDefense : buffBase.Effects[i].Amount;
							CurDefense += change;
							break;
						}
					case TargetBaseStat.MoveSpeed:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * CurWalkSpeed : buffBase.Effects[i].Amount;
							change = Mathf.Max(-CurWalkSpeed, change);
							CurWalkSpeed += change;
							break;
						}
					case TargetBaseStat.JumpHeightMultiplier:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * CurJumpPowerMultiplier : buffBase.Effects[i].Amount;
							change = Mathf.Max(-CurJumpPowerMultiplier, change);
							CurJumpPowerMultiplier += change;
							break;
						}
					case TargetBaseStat.TrueDamageMultiplier:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * CurTrueDamageDamageMultiplier : buffBase.Effects[i].Amount;
							change = Mathf.Max(-CurTrueDamageDamageMultiplier, change);
							CurTrueDamageDamageMultiplier += change;
							break;
						}
				}

				buffInstance.FlatChanges[i] = change;
			}
		}
		public void RemoveBuff(int index)
		{
			BuffInstance targetBuff = ActiveBuffs[index];
			RemoveBuffEffect(targetBuff);
			targetBuff.OnRemove();

			//Custom Buff Effects
			if (targetBuff.Base.CustomEffects.ApplyMoveStun)
				Stuns--;
			if (targetBuff.Base.CustomEffects.Invincible)
				Invincibilities--;

			if (this is Player)
				Player.Instance.buffDisplay.RemoveBuffIcon(targetBuff);
			else
				statDisplay.RemoveBuffIcon(targetBuff);

			ActiveBuffs.RemoveAt(index);
			RecalculateBuffs();
		}
		public bool RemoveBuff(BuffInstance buffInstance)
		{
			for (int i = 0; i < ActiveBuffs.Count; i++)
			{
				if (ActiveBuffs[i] == buffInstance)
				{
					RemoveBuff(i);
					return true;
				}
			}
			return false;
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
							CurMaxHealth -= change;
							if (clampRequired && CurHealth > CurMaxHealth)
								CurHealth = CurMaxHealth;
							break;
						}
					case TargetBaseStat.Mana:
						{
							CurMaxMana -= change;
							if (clampRequired && CurMana > CurMaxMana)
								CurMana = CurMaxMana;
							break;
						}
					case TargetBaseStat.Stamina:
						{
							//If this is not a player we can just ignore this buff/debuff
							Player player = this as Player;
							if (player == null)
								break;

							player.CurMaxStamina -= change;
							if (clampRequired)
								player.ClampStamina();
							break;
						}
					case TargetBaseStat.PhysicalDamage:
						{
							CurPhysicalDamage -= change;
							break;
						}
					case TargetBaseStat.MagicalDamage:
						{
							CurMagicalDamage -= change;
							break;
						}
					case TargetBaseStat.Defense:
						{
							CurDefense -= change;
							break;
						}
					case TargetBaseStat.MoveSpeed:
						{
							CurWalkSpeed -= change;
							break;
						}
					case TargetBaseStat.JumpHeightMultiplier:
						{
							CurJumpPowerMultiplier -= change;
							break;
						}
					case TargetBaseStat.TrueDamageMultiplier:
						{
							CurTrueDamageDamageMultiplier -= change;
							break;
						}
				}
			}
		}
		public void RecalculateBuffs()
		{
			//Reset to base stats
			ResetStats();

			//Now re-add them, so the values can be recalculated
			//First add flat values and then percent
			//Flat
			for (int i = 0; i < ActiveBuffs.Count; i++)
			{
				AddBuffEffect(ActiveBuffs[i], CalculateValue.Flat, false);
			}
			//Percent
			for (int i = 0; i < ActiveBuffs.Count; i++)
			{
				AddBuffEffect(ActiveBuffs[i], CalculateValue.Percent, false);
			}

			if (CurHealth > CurMaxHealth)
				CurHealth = CurMaxHealth;
			if (CurMana > CurMaxMana)
				CurMana = CurMaxMana;
			if (this is Player)
				(this as Player).ClampStamina();
		}
		#endregion
		#region Compound Casting
		public bool ActivateCompound(ActivationCompound compound)
		{
			if (!compound.CanActivate(this, 1, 1, 1))
				return false;

			StartCoroutine(ActivateCompoundC(compound));
			return true;
		}
		private IEnumerator ActivateCompoundC(ActivationCompound compound)
		{
			InActivationCompound = true;
			ApplyCooldown();
			Transform targeterTransform = new GameObject("Targeter Transform").transform;
			targeterTransform.SetParent(Storage.ProjectileStorage);

			for (int i = 0; i < compound.Elements.Length; i++)
			{
				if(compound.CostActivationIndex == i)
				{
					if(compound.CancelIfCostActivationIsImpossible && !compound.Cost.CanAfford(this, 1, 1, 1))
					{
						break;
					}
					compound.Cost.PayCost(this, 1, 1, 1);
				}

				//Activate start effects
				compound.Elements[i].Restrictions.ApplyRestriction(this, true);
				FXInstance[] elementBoundFX = ActivateElementActivationEffects(compound.Elements[i].StartEffects, true);
				PositionTargeterTranform();
				FXInstance[] elementAtTargetBoundFX = ActivateElementActivationEffects(compound.Elements[i].AtTargetStartEffects, true, targeterTransform);

				//Setup the loop for this element
				float elementTime = 0;
				float whileTickTime = 0;

				while(elementTime < compound.Elements[i].ElementDuration)
				{
					ApplyCooldown();
					yield return new WaitForEndOfFrame();

					//If this compound can be canceled by stuns and this entity is stunned => we stop the nested loop
					if((compound.CancelFromStun && IsStunned) || !Alive)
					{
						//Get rid of any restrictions/FXInstances and then exit the nested loop
						compound.Elements[i].Restrictions.ApplyRestriction(this, false);
						FXManager.FinishFX(ref elementBoundFX);
						goto CompoundFinished;
					}

					//Check if any condition is true if so we set their associated boolean
					if (compound.Elements[i].StopCondition && compound.Elements[i].StopCondition.True)
					{
						//Get rid of any restrictions/FXInstances and then exit the nested loop
						FXManager.FinishFX(ref elementBoundFX);
						compound.Elements[i].Restrictions.ApplyRestriction(this, false);
						goto CompoundFinished;
					}
					

					//Yield conditions
					bool yielded = false;
					if(compound.Elements[i].YieldCondition && compound.Elements[i].YieldCondition.True)
					{
						yielded = true;
					}

					//Pause conditions
					//We only need to check pause conditions if yielded is false
					bool paused = false;
					if (!yielded)
					{
						if (compound.Elements[i].PauseCondition && compound.Elements[i].PauseCondition.True)
						{
							paused = true;
						}
					}
					
					if (!yielded && !paused)
						elementTime += Time.deltaTime;

					if (!yielded)
					{
						whileTickTime += Time.deltaTime;

						while (whileTickTime >= compound.Elements[i].WhileTickTime)
						{
							whileTickTime -= compound.Elements[i].WhileTickTime;
							ActivateElementActivationEffects(compound.Elements[i].WhileEffects, false);
							ActivateElementActivationEffects(compound.Elements[i].AtTargetWhileEffects, false, targeterTransform);
						}
					}
				}

				//Finish fx if needed
				FXManager.FinishFX(ref elementBoundFX);
				FXManager.FinishFX(ref elementAtTargetBoundFX);
				//Undo any restrictions
				compound.Elements[i].Restrictions.ApplyRestriction(this, false);
			}

			CompoundFinished:;
			InActivationCompound = false;
			Destroy(targeterTransform.gameObject);

			FXInstance[] ActivateElementActivationEffects(ActivationEffect[] effects, bool binding, Transform overrideTransform = null)
			{
				List<FXInstance> createdFXInstances = new List<FXInstance>();
				for (int i = 0; i < effects.Length; i++)
				{
					if (binding)
						createdFXInstances.AddRange(effects[i].Activate(this, compound, 1, overrideTransform));
					else
						effects[i].Activate(this, compound, 1, overrideTransform);
				}
				if (binding)
					return createdFXInstances.ToArray();
				else
					return null;
			}
			void ApplyCooldown()
			{
				if (compound.ActionType == ActionType.Casting)
					CastingCooldown = Mathf.Max(CastingCooldown, compound.CausedCooldown);
				else if (compound.ActionType == ActionType.Attacking)
					AttackCooldown = Mathf.Max(AttackCooldown, compound.CausedCooldown);
			}
			void PositionTargeterTranform()
			{
				targeterTransform.rotation = transform.rotation;
				targeterTransform.position = targetPosition ?? transform.position;
			}
		}
		#endregion
		#region Helper Functions
		protected bool CheckIfCanSeeEntitie(Transform eye, Entity target, bool lowPriority = false)
		{
			//First we check the middle and corners of the entitie
			//Middle
			if (CheckPointVisible(target.ActuallWorldPosition))
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
				Vector3 dif = endPos - eye.transform.position;
				float dist = dif.magnitude;
				Vector3 direction = dif / dist;

				return !Physics.Raycast(eye.transform.position, direction, dist, ConstantCollector.TERRAIN_LAYER_MASK);
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
					new Vector3(	target.coll.bounds.extents.x * (Random.value - 0.5f) * 2,
									target.coll.bounds.extents.y * (Random.value - 0.5f) * 2,
									target.coll.bounds.extents.z * (Random.value - 0.5f) * 2);
			}
		}
		public FXInstance[] ActivateActivationEffects(ActivationEffect[] activationEffects, float multiplier = 1)
		{
			CombatObject combatData = ScriptableObject.CreateInstance<CombatObject>();
			combatData.BasePhysicalDamage = SelfSettings.BasePhysicalDamage;
			combatData.BaseMagicalDamage = SelfSettings.BaseMagicalDamage;

			//Crit and knockback are set to 1 because they will be used in multiplication
			combatData.BaseCritChance = combatData.BaseKnockback = 1;

			List<FXInstance> createdFX = new List<FXInstance>();
			for (int i = 0; i < activationEffects.Length; i++)
			{
				createdFX.AddRange(activationEffects[i].Activate(this, combatData, multiplier));
			}
			return createdFX.ToArray();
		}
		#endregion
	}
}