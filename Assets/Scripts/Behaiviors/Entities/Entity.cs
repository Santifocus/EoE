using EoE.Combatery;
using EoE.Events;
using EoE.Information;
using EoE.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Entities
{
	public abstract class Entity : MonoBehaviour
	{
		#region Fields
		//Constants
		private const int VISIBLE_CHECK_RAY_COUNT = 40;
		public static List<Entity> AllEntities = new List<Entity>();

		//Inspector variables
		public Collider coll = default;
		public int StartLevel = 0;

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
		public float curTrueDamageDamageMultiplier { get; protected set; }

		//Entitie states
		public bool Alive { get; protected set; }

		public List<BuffInstance> activeBuffs { get; protected set; }
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
		public bool IsAttackStopped => MovementStops > 0;
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
		public float AttackCooldown { get; private set; }
		public float CastingCooldown { get; private set; }

		//Getter Helpers
		protected enum ColliderType : byte { Box, Sphere, Capsule }
		public abstract EntitySettings SelfSettings { get; }
		protected ColliderType selfColliderType;
		public Vector3 actuallWorldPosition => SelfSettings.MassCenter + transform.position;
		public float lowestPos => coll.bounds.center.y - coll.bounds.extents.y;
		public float highestPos => coll.bounds.center.y + coll.bounds.extents.y;
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
			curStates = new EntityState();
			entitieForceController = new ForceController();

			if (!(this is Player))
			{
				statDisplay = Instantiate(GameController.CurrentGameSettings.EntitieStatDisplayPrefab, GameController.Instance.enemyHealthBarStorage);
				statDisplay.Setup(SelfSettings.ShowEntitieLevel ? (int?)(EntitieLevel + 1) : null);
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
			curMaxHealth = SelfSettings.Health;
			curMaxMana = SelfSettings.Mana;
			curPhysicalDamage = SelfSettings.BasePhysicalDamage;
			curMagicalDamage = SelfSettings.BaseMagicalDamage;
			curDefense = SelfSettings.BaseDefense;
			curWalkSpeed = SelfSettings.WalkSpeed;
			curJumpPowerMultiplier = 1;
			curTrueDamageDamageMultiplier = 1;
		}
		protected virtual void ResetStatValues()
		{
			curHealth = curMaxHealth;
			curMana = curMaxMana;
		}
		private void BuffSetup()
		{
			activeBuffs = new List<BuffInstance>();
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
				if (SelfSettings.DoHealthRegen && healthRegenCooldown <= 0 && curHealth < curMaxHealth)
				{
					float regenAmount = SelfSettings.HealthRegen * GameController.CurrentGameSettings.SecondsPerEntitieHealthRegen * (inCombat ? SelfSettings.HealthRegenInCombatMultiplier : 1);
					if (regenAmount > 0)
					{
						ChangeInfo basis = new ChangeInfo(this, CauseType.Heal, TargetStat.Health, -regenAmount);
						ChangeInfo.ChangeResult regenResult = new ChangeInfo.ChangeResult(basis, this, true, true);
						curHealth = Mathf.Min(curHealth - regenResult.finalChangeAmount, curMaxHealth);

						regendHealth = regenResult.finalChangeAmount < 0;
					}
				}
				ControlRegenParticles(regendHealth);
			}
			//Mana Regen
			if (SelfSettings.DoManaRegen && curMana < curMaxMana)
			{
				float regenAmount = SelfSettings.ManaRegen * Time.deltaTime * (inCombat ? SelfSettings.ManaRegenInCombatMultiplier : 1);
				curMana = Mathf.Min(curMana + regenAmount, curMaxMana);
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
				healthRegenCooldown = SelfSettings.HealthRegenCooldownAfterTakingDamage;

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
				float curHealthNormalized = curHealth / curMaxHealth;
				if (statDisplay.HealthValue != curHealthNormalized)
					statDisplay.HealthValue = curHealthNormalized;

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
		protected virtual void Death()
		{
			if (!Alive)
				return;

			Alive = false;
			BuildDrops();
			if (statDisplay)
				Destroy(statDisplay.gameObject);

			gameObject.SetActive(false);
			AllEntities.Remove(this);
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
				newSoulDrop.transform.position = actuallWorldPosition;
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
					SelfSettings.PossibleDropsTable.PossibleDrops[i].Drop.CreateItemDrop(actuallWorldPosition, amount, false);
				}
			}
		}
		#endregion
		#region BuffControl
		private void BuffUpdate()
		{
			bool requiresRecalculate = false;
			for (int i = 0; i < activeBuffs.Count; i++)
			{
				bool shouldBeRemoved = activeBuffs[i].Update();

				for (int j = 0; j < activeBuffs[i].DOTCooldowns.Length; j++)
				{
					if (activeBuffs[i].DOTCooldowns[j] <= 0)
					{
						float cd = activeBuffs[i].Base.DOTs[j].DelayPerActivation;
						activeBuffs[i].DOTCooldowns[j] += cd;

						if (activeBuffs[i].Base.DOTs[j].TargetStat == TargetStat.Health)
						{
							ChangeHealth(new ChangeInfo(activeBuffs[i].Applier, CauseType.DOT, activeBuffs[i].Base.DOTs[j].Element, TargetStat.Health, actuallWorldPosition, Vector3.up, cd * activeBuffs[i].Base.DOTs[j].BaseDamage, false));
						}
						else if (activeBuffs[i].Base.DOTs[j].TargetStat == TargetStat.Mana)
						{
							ChangeMana(new ChangeInfo(activeBuffs[i].Applier, CauseType.DOT, TargetStat.Mana, cd * activeBuffs[i].Base.DOTs[j].BaseDamage));
						}
						else if (this is Player)//TargetStat.Endurance
						{
							(this as Player).ChangeEndurance(new ChangeInfo(activeBuffs[i].Applier, CauseType.DOT, TargetStat.Endurance, cd * activeBuffs[i].Base.DOTs[j].BaseDamage));
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
			for (int i = 0; i < activeBuffs.Count; i++)
			{
				if (activeBuffs[i].Applier == applier && activeBuffs[i].Base == buff)
				{
					return i;
				}
			}
			return null;
		}
		public (bool, BuffInstance) TryReapplyBuff(Buff buff, Entity applier)
		{
			for (int i = 0; i < activeBuffs.Count; i++)
			{
				if (activeBuffs[i].Applier == applier && activeBuffs[i].Base == buff)
				{
					activeBuffs[i].Reset();
					return (true, activeBuffs[i]);
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

			activeBuffs.Add(newBuff);

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

							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * player.curMaxEndurance : buffBase.Effects[i].Amount;
							change = Mathf.Max(-(player.curMaxEndurance), change);
							player.curMaxEndurance += change;

							if (clampRequired)
								player.ClampEndurance();
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
							change = Mathf.Max(-curWalkSpeed, change);
							curWalkSpeed += change;
							break;
						}
					case TargetBaseStat.JumpHeightMultiplier:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curJumpPowerMultiplier : buffBase.Effects[i].Amount;
							change = Mathf.Max(-curJumpPowerMultiplier, change);
							curJumpPowerMultiplier += change;
							break;
						}
					case TargetBaseStat.TrueDamageMultiplier:
						{
							change = buffBase.Effects[i].Percent ? (buffBase.Effects[i].Amount / 100) * curTrueDamageDamageMultiplier : buffBase.Effects[i].Amount;
							change = Mathf.Max(-curTrueDamageDamageMultiplier, change);
							curTrueDamageDamageMultiplier += change;
							break;
						}
				}

				buffInstance.FlatChanges[i] = change;
			}
		}
		public void RemoveBuff(int index)
		{
			BuffInstance targetBuff = activeBuffs[index];
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

			activeBuffs.RemoveAt(index);
			RecalculateBuffs();
		}
		public bool RemoveBuff(BuffInstance buffInstance)
		{
			for (int i = 0; i < activeBuffs.Count; i++)
			{
				if (activeBuffs[i] == buffInstance)
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

							player.curMaxEndurance -= change;
							if (clampRequired)
								player.ClampEndurance();
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
					case TargetBaseStat.JumpHeightMultiplier:
						{
							curJumpPowerMultiplier -= change;
							break;
						}
					case TargetBaseStat.TrueDamageMultiplier:
						{
							curTrueDamageDamageMultiplier -= change;
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
			for (int i = 0; i < activeBuffs.Count; i++)
			{
				AddBuffEffect(activeBuffs[i], CalculateValue.Flat, false);
			}
			//Percent
			for (int i = 0; i < activeBuffs.Count; i++)
			{
				AddBuffEffect(activeBuffs[i], CalculateValue.Percent, false);
			}

			if (curHealth > curMaxHealth)
				curHealth = curMaxHealth;
			if (curMana > curMaxMana)
				curMana = curMaxMana;
			if (this is Player)
				(this as Player).ClampEndurance();
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
					if(compound.CancelFromStun && IsStunned)
					{
						//Get rid of any restrictions/FXInstances and then exit the nested loop
						compound.Elements[i].Restrictions.ApplyRestriction(this, false);
						FXManager.FinishFX(ref elementBoundFX);
						goto CompoundFinished;
					}

					//Check if any condition is true if so we set their associated boolean
					for (int j = 0; j < compound.Elements[i].StopConditions.Length; i++)
					{
						if (compound.Elements[i].StopConditions[j].ConditionMet())
						{
							//Get rid of any restrictions/FXInstances and then exit the nested loop
							FXManager.FinishFX(ref elementBoundFX);
							compound.Elements[i].Restrictions.ApplyRestriction(this, false);
							goto CompoundFinished;
						}
					}

					//Yield conditions
					bool yielded = false;
					for (int j = 0; j < compound.Elements[i].YieldConditions.Length; i++)
					{
						if(compound.Elements[i].YieldConditions[j].ConditionMet())
						{
							yielded = true;
							break;
						}
					}

					//Pause conditions
					//We only need to check pause conditions if yielded is false
					bool paused = false;
					if (!yielded)
					{
						for (int j = 0; j < compound.Elements[i].PauseConditions.Length; i++)
						{
							if (compound.Elements[i].PauseConditions[j].ConditionMet())
							{
								paused = true;
								break;
							}
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
					CastingCooldown = compound.CausedCooldown;
				else if (compound.ActionType == ActionType.Attacking)
					AttackCooldown = compound.CausedCooldown;
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
		protected FXInstance[] ActivateActivationEffects(ActivationEffect[] activationEffects, float multiplier = 1)
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