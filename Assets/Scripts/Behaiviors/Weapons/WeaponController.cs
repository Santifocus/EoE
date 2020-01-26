using EoE.Controlls;
using EoE.Entities;
using EoE.Events;
using EoE.Information;
using EoE.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	public enum AttackAnimation { Attack1 = 1, Attack2 = 2, Attack3 = 4 }
	public class WeaponController : MonoBehaviour
	{
		#region Fields
		private const float COMBO_WAIT_THRESHOLD = 0.25f;
		private const float ANIMATION_RESET_COOLDOWN = 0.35f;
		//Static info
		private static readonly Dictionary<AttackAnimation, (float, float)> animationDelayLookup = new Dictionary<AttackAnimation, (float, float)>()
		{
			{ AttackAnimation.Attack1, (0.833f, 0.566f) },
			{ AttackAnimation.Attack2, (0.75f, 0.335f) },
			{ AttackAnimation.Attack3, (1.067f, 0.75f) },
		};
		public static WeaponController Instance { get; private set; }
		public bool InAttackSequence { get; private set; }
		public AttackStyle ActiveAttackStyle { get; private set; }

		//Inspector variables
		[SerializeField] private WeaponHitbox[] weaponHitboxes = default;
		public CustomHitboxGroup[] customHitboxGroups = default;
		[SerializeField] private ParticleSystem[] particlesToActivateOnEnable = default;
		[SerializeField] private GameObject dropCollision = default;
		[SerializeField] private GameObject[] nonClonable = default;

		//Base Data
		public Weapon weaponInfo { get; private set; }
		public CombatObject overrideBaseObject { get; set; }
		private CombatObject curBaseData => overrideBaseObject ?? weaponInfo;

		//Behaivior Control
		private float animationResetCooldown;
		private bool colliderActive;
		private bool isChargingAttack;
		private float curChargeMultiplier;
		private bool wantsToBeginNextSequence;
		private List<Collider> ignoredColliders = new List<Collider>();
		private Coroutine attackCoroutine;
		private int targetHitboxGroup = -1;

		//Combo Control
		private float curAttackAnimationPoint = 0;
		private int curCombo;
		private float timeToNextCombo;
		private List<FXInstance> comboBoundFX = new List<FXInstance>();
		private List<BuffInstance> comboBoundBuffs = new List<BuffInstance>();

		//Ultimate control
		public float ultimateCharge { get; set; }
		private FXInstance[] ultimateChargedBoundFX;
		private FXInstance[] ultimateChargedBoundFXPlayer;

		//Charge effects
		private FXInstance[] chargeBoundFX;
		private FXInstance[] chargeBoundFXMultiplied;
		private BuffInstance[] chargeBoundBuffs;

		private WeaponHitbox[] activeHitboxGroup
		{
			get
			{
				if (targetHitboxGroup == -1)
					return weaponHitboxes;
				else
					return customHitboxGroups[targetHitboxGroup].Hitboxes;
			}
		}

		#endregion
		#region Setups
		public void Setup(Weapon weaponInfo)
		{
			Instance = this;
			this.weaponInfo = weaponInfo;
			colliderActive = false;
			//Standard hitboxes
			for (int i = 0; i < weaponHitboxes.Length; i++)
			{
				weaponHitboxes[i].Setup(this);
				weaponHitboxes[i].SetColliderStyle(null);
			}
			//Custom hitboxes
			for (int i = 0; i < customHitboxGroups.Length; i++)
			{
				for (int j = 0; j < customHitboxGroups[i].Hitboxes.Length; j++)
				{
					customHitboxGroups[i].Hitboxes[j].Setup(this);
					customHitboxGroups[i].Hitboxes[j].SetColliderStyle(null);
				}
			}

			ComboDisplayController.Instance.ResetCombo(weaponInfo.ComboEffects);
			SetParticlesState(false);
			AnchorToWeaponHoldPoint();

			if (weaponInfo.HasUltimate)
				EventManager.EntitieDiedEvent += EntitieDeath;

			UltimateBarController.Instance.Setup(weaponInfo.HasUltimate ? weaponInfo.UltimateSettings : null);
		}
		private void ChangeWeaponState(bool state, AttackStyle style)
		{
			colliderActive = state;
			for (int i = 0; i < activeHitboxGroup.Length; i++)
			{
				activeHitboxGroup[i].SetColliderStyle(state ? style : null);
				if (!state)
				{
					for (int j = 0; j < ignoredColliders.Count; j++)
					{
						activeHitboxGroup[i].IgnoreCollision(ignoredColliders[j], false);
					}
				}
			}
			ignoredColliders = new List<Collider>();
			SetParticlesState(state);
		}
		private void SetParticlesState(bool state)
		{
			for (int i = 0; i < particlesToActivateOnEnable.Length; i++)
			{
				if(state)
					particlesToActivateOnEnable[i].Play(true);
				else
					particlesToActivateOnEnable[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
			}
		}
		public GameObject CloneModel()
		{
			WeaponController clone = Instantiate(this, Storage.ParticleStorage);

			//deactivate behaiviors
			//Standard hitboxes
			clone.enabled = false;
			for (int i = 0; i < clone.weaponHitboxes.Length; i++)
			{
				clone.weaponHitboxes[i].enabled = false;
			}
			//Custom hitboxes
			for (int i = 0; i < clone.customHitboxGroups.Length; i++)
			{
				for (int j = 0; j < clone.customHitboxGroups[i].Hitboxes.Length; j++)
				{
					clone.customHitboxGroups[i].Hitboxes[j].enabled = false;
				}
			}

			//deactivate all custom objects
			for (int i = 0; i < clone.particlesToActivateOnEnable.Length; i++)
			{
				Destroy(clone.particlesToActivateOnEnable[i]);
			}
			for (int i = 0; i < clone.nonClonable.Length; i++)
			{
				Destroy(clone.nonClonable[i]);
			}

			//Set layers to default
			clone.gameObject.layer = 0;
			for (int i = 0; i < clone.transform.childCount; i++)
			{
				clone.transform.GetChild(i).gameObject.layer = 0;
			}
			return clone.gameObject;
		}
		#endregion
		#region BasicMonobehaivior
		private void Update()
		{
			if(animationResetCooldown > 0)
			{
				animationResetCooldown -= Time.deltaTime;
			}

			if (timeToNextCombo > 0)
			{
				timeToNextCombo -= Time.deltaTime;
				if (timeToNextCombo <= 0)
				{
					timeToNextCombo = 0;
					ComboFinish();
				}
			}

			if (weaponInfo.HasUltimate)
			{
				UltimateControl();
			}
		}
		#endregion
		#region FollowPlayer
		private void LateUpdate()
		{
			AnchorToWeaponHoldPoint();
		}
		private void FixedUpdate()
		{
			AnchorToWeaponHoldPoint();
		}
		private void AnchorToWeaponHoldPoint()
		{
			if (!Player.Existant)
			{
				DropWeapon();
				return;
			}

			Vector3 worldOffset = weaponInfo.WeaponPositionOffset.x * Player.Instance.weaponHoldPoint.right +
									weaponInfo.WeaponPositionOffset.y * Player.Instance.weaponHoldPoint.up +
									weaponInfo.WeaponPositionOffset.z * Player.Instance.weaponHoldPoint.forward;

			transform.position = Player.Instance.weaponHoldPoint.position + worldOffset;
			transform.eulerAngles = Player.Instance.weaponHoldPoint.eulerAngles + weaponInfo.WeaponRotationOffset;
		}
		#endregion
		#region Attack Execution
		public void StartAttack()
		{
			if (InAttackSequence)
			{
				if (curAttackAnimationPoint > COMBO_WAIT_THRESHOLD && !isChargingAttack)
					wantsToBeginNextSequence = true;
				return;
			}

			if (!weaponInfo.AllowedAction(Player.Instance) || animationResetCooldown > 0)
				return;

			//Find out what attacksequence the player should start
			//Then check if that sequence is enabled and the player has enougth resources
			int mask = ((Player.Instance.curStates.Running) ? 1 : 0) + ((!Player.Instance.charController.isGrounded) ? 2 : 0);
			AttackStylePart part;
			switch (mask)
			{
				case 1:
					part = AttackStylePart.RunAttack;
					break;
				case 2:
					part = AttackStylePart.JumpAttack;
					break;
				case 3:
					part = AttackStylePart.RunJumpAttack;
					break;
				default: //0
					part = AttackStylePart.StandAttack;
					break;
			}

			//Check the flag, if the attempted flag doesnt exist we check for a fallback
			//if there is none we stop this attempt
			if (!weaponInfo.HasMaskFlag(part))
			{
				if (weaponInfo.FallBackPart == AttackStylePartFallback.None)
				{
					return;
				}
				else
				{
					part = (AttackStylePart)weaponInfo.FallBackPart;
				}
			}

			//Start the attack
			attackCoroutine = StartCoroutine(Attack(weaponInfo[part]));
		}
		public void ForceAttackStart(AttackSequence targetSequence)
		{
			if (InAttackSequence)
				StopCoroutine(attackCoroutine);

			attackCoroutine = StartCoroutine(Attack(targetSequence));
		}
		private IEnumerator Attack(AttackSequence targetSequence)
		{
			InAttackSequence = true;
			Player.Instance.animationControl.SetBool("InFight", true);
			int curSequenceIndex = 0;
			List<FXInstance> whileBoundEffects = null;
			while (true)
			{
				ChangeWeaponState(false, null);
				ActiveAttackStyle = targetSequence.AttackSequenceParts[curSequenceIndex];

				//First check if this attack sequence is allowed if not we stop the while loop here
				if (curBaseData.CanActivate(Player.Instance, ActiveAttackStyle.HealthCostMultiplier, ActiveAttackStyle.ManaCostMultiplier, ActiveAttackStyle.EnduranceCostMultiplier))
				{
					curBaseData.Cost.PayCost(Player.Instance, ActiveAttackStyle.HealthCostMultiplier, ActiveAttackStyle.ManaCostMultiplier, ActiveAttackStyle.EnduranceCostMultiplier);
				}
				else
				{
					goto AttackFinished;
				}

				ActiveAttackStyle.Restrictions.ApplyRestriction(Player.Instance, true);
				targetHitboxGroup = ActiveAttackStyle.UseCustomHitboxGroup ? ActiveAttackStyle.CustomHitboxGroup : -1;

				//Setup timers and blend variables
				curAttackAnimationPoint = ActiveAttackStyle.AnimationStartPoint;

				float totalTime = 0;
				float animationTime = animationDelayLookup[ActiveAttackStyle.AnimationTarget].Item1;
				float animationActivationDelay = animationDelayLookup[ActiveAttackStyle.AnimationTarget].Item2;
				float animationTimer = curAttackAnimationPoint * animationTime;
				wantsToBeginNextSequence = false;
				curChargeMultiplier = ActiveAttackStyle.ChargeSettings.StartCharge;
				ChangeWeaponState(false, null);

				FXManager.FinishFX(ref whileBoundEffects);
				whileBoundEffects = new List<FXInstance>();
				TryForEventEffects(0, curAttackAnimationPoint);

				float multiplier;
				if (ActiveAttackStyle.AnimationMultiplicationType == MultiplicationType.FlatValue)
				{
					multiplier = ActiveAttackStyle.AnimationSpeedFlatValue;
				}
				else
				{
					multiplier = ActiveAttackStyle.AnimationSpeedCurve.Evaluate(0) * ActiveAttackStyle.AnimationSpeedCurveMultiplier;
				}
				SetAnimationSpeed(multiplier);

				if (ActiveAttackStyle.AnimationStartPoint == 0)
					Player.Instance.animationControl.SetTrigger(ActiveAttackStyle.AnimationTarget.ToString());
				else
					Player.Instance.animationControl.Play(ActiveAttackStyle.AnimationTarget.ToString(), -1, ActiveAttackStyle.AnimationStartPoint);

				do
				{
					yield return new WaitForEndOfFrame();
					if (GameController.GameIsPaused)
						continue;

					totalTime += Time.deltaTime;
					animationTimer += multiplier * Time.deltaTime;

					//Find the current animationSpeed multiplier
					if (ActiveAttackStyle.AnimationMultiplicationType == MultiplicationType.Curve)
					{
						multiplier = ActiveAttackStyle.AnimationSpeedCurve.Evaluate(Mathf.Clamp01(totalTime / ActiveAttackStyle.AnimationSpeedCurveTimeframe)) * ActiveAttackStyle.AnimationSpeedCurveMultiplier;
						SetAnimationSpeed(multiplier);
					}

					//Check if we reached the collision activation point
					bool shouldState = animationTimer >= animationActivationDelay;
					if (shouldState != colliderActive)
					{
						ChangeWeaponState(shouldState, ActiveAttackStyle);
					}

					//Debug
					if (GameController.CurrentGameSettings.IsDebugEnabled && shouldState)
					{
						Debug.DrawLine(transform.position - Vector3.up / 4, transform.position + Vector3.up / 4, Color.green / 1.5f, 1.5f);
						Debug.DrawLine(transform.position - Vector3.forward / 4, transform.position + Vector3.forward / 4, Color.yellow / 1.5f, 1.5f);
						Debug.DrawLine(transform.position - Vector3.right / 4, transform.position + Vector3.right / 4, Color.cyan / 1.5f, 1.5f);
					}

					//Update the animationPoint and try for event effects
					float newAnimationPoint = animationTimer / animationTime;
					float smallerPoint = Mathf.Min(newAnimationPoint, curAttackAnimationPoint);
					float biggerPoint = Mathf.Max(newAnimationPoint, curAttackAnimationPoint);
					TryForEventEffects(smallerPoint, biggerPoint);
					curAttackAnimationPoint = newAnimationPoint;

					//Check if any wait conditions are true
					//First check if the animation point is correct, and then check the condition state
					//if both are true: stop the animation and wait until they are false
					for (int i = 0; i < ActiveAttackStyle.WaitSettings.Length; i++)
					{
						if((ActiveAttackStyle.WaitSettings[i].MinAnimtionPoint <= curAttackAnimationPoint) && (ActiveAttackStyle.WaitSettings[i].MaxAnimtionPoint > curAttackAnimationPoint))
						{
							if (ActiveAttackStyle.WaitSettings[i].WaitCondition.ConditionMet())
							{
								SetAnimationSpeed(0);
								while (ActiveAttackStyle.WaitSettings[i].WaitCondition.ConditionMet())
								{
									yield return new WaitForEndOfFrame();
								}
								SetAnimationSpeed(multiplier);
							}
						}
					}

					//Check if this attackstyle charges, if so then we check if we crossed the point at which the charging should start
					if (ActiveAttackStyle.NeedsCharging)
					{
						if ((ActiveAttackStyle.ChargeSettings.AnimationChargeStartpoint >= smallerPoint) && (ActiveAttackStyle.ChargeSettings.AnimationChargeStartpoint < biggerPoint))
						{
							//Setup charge time and apply stun if enabled
							float chargeTime = curChargeMultiplier * ActiveAttackStyle.ChargeSettings.ChargeTime;

							//Setup effects of charge
							ActiveAttackStyle.ChargeSettings.Restrictions.ApplyRestriction(Player.Instance, true);

							//FX
							FXManager.ExecuteFX(ActiveAttackStyle.ChargeSettings.FXObjects, transform, true, out chargeBoundFX, curChargeMultiplier);
							FXManager.ExecuteFX(ActiveAttackStyle.ChargeSettings.FXObjectsWithMutliplier, transform, true, out chargeBoundFXMultiplied, curChargeMultiplier);
							//Buffs
							chargeBoundBuffs = new BuffInstance[ActiveAttackStyle.ChargeSettings.BuffOnUserWhileCharging.Length];
							for (int i = 0; i < chargeBoundBuffs.Length; i++)
							{
								chargeBoundBuffs[i] = Buff.ApplyBuff(ActiveAttackStyle.ChargeSettings.BuffOnUserWhileCharging[i], Player.Instance, Player.Instance);
							}

							//Begin the charging
							SetAnimationSpeed(0);
							isChargingAttack = true;
							while (curChargeMultiplier < ActiveAttackStyle.ChargeSettings.MaximumCharge || (ActiveAttackStyle.ChargeSettings.WaitAtFullChargeForRelease && InputController.Attack.Pressed))
							{
								yield return new WaitForEndOfFrame();
								if (GameController.GameIsPaused || (curChargeMultiplier >= ActiveAttackStyle.ChargeSettings.MaximumCharge && InputController.Attack.Pressed))
									continue;

								if (InputController.Attack.Pressed)
								{
									chargeTime += Time.deltaTime;
									curChargeMultiplier = chargeTime / ActiveAttackStyle.ChargeSettings.ChargeTime;
									if (curChargeMultiplier > ActiveAttackStyle.ChargeSettings.MaximumCharge)
										curChargeMultiplier = ActiveAttackStyle.ChargeSettings.MaximumCharge;

									//Update multipliers of FX
									for (int i = 0; i < chargeBoundFXMultiplied.Length; i++)
									{
										chargeBoundFXMultiplied[i].baseMultiplier = curChargeMultiplier;
									}
								}
								else
								{
									if (curChargeMultiplier < ActiveAttackStyle.ChargeSettings.MinRequiredCharge)
									{
										RemoveChargeBoundEffects();
										goto AttackFinished;
									}
									else
									{
										break;
									}
								}
							}

							//Reset to the previous state
							RemoveChargeBoundEffects();
							SetAnimationSpeed(multiplier);
						}
					}
				} while (((curAttackAnimationPoint > 0 || multiplier > 0) && (curAttackAnimationPoint < 1)) || GameController.GameIsPaused);

				void TryForEventEffects(float smallerPoint, float biggerPoint)
				{
					//OnUserStart
					for (int i = 0; i < ActiveAttackStyle.StartEffectsOnUser.Length; i++)
					{
						float point = ActiveAttackStyle.StartEffectsOnUser[i].AtAnimationPoint;
						if ((point >= smallerPoint) && (point < biggerPoint))
						{
							ActiveAttackStyle.StartEffectsOnUser[i].Effect.Activate(Player.Instance, curBaseData);
						}
					}

					//OnUserWhile
					for (int i = 0; i < ActiveAttackStyle.WhileEffectsOnUser.Length; i++)
					{
						float point = ActiveAttackStyle.WhileEffectsOnUser[i].AtAnimationPoint;
						if ((point >= smallerPoint) && (point < biggerPoint))
						{
							whileBoundEffects.AddRange(ActiveAttackStyle.WhileEffectsOnUser[i].Effect.Activate(Player.Instance, curBaseData));
						}
					}

					//OnWeaponStart
					for (int i = 0; i < ActiveAttackStyle.StartEffectsOnWeapon.Length; i++)
					{
						float point = ActiveAttackStyle.StartEffectsOnWeapon[i].AtAnimationPoint;
						if ((point >= smallerPoint) && (point < biggerPoint))
						{
							ActiveAttackStyle.StartEffectsOnWeapon[i].Effect.Activate(Player.Instance, curBaseData, 1, transform);
						}
					}

					//OnWeaponWhile
					for (int i = 0; i < ActiveAttackStyle.WhileEffectsOnWeapon.Length; i++)
					{
						float point = ActiveAttackStyle.WhileEffectsOnWeapon[i].AtAnimationPoint;
						if ((point >= smallerPoint) && (point < biggerPoint))
						{
							whileBoundEffects.AddRange(ActiveAttackStyle.WhileEffectsOnWeapon[i].Effect.Activate(Player.Instance, curBaseData, 1, transform));
						}
					}
				}

				//Debug for attackstyle end
				if (GameController.CurrentGameSettings.IsDebugEnabled)
				{
					Debug.DrawLine(transform.position - Vector3.up / 2, transform.position + Vector3.up / 2, Color.red, 1.75f);
					Debug.DrawLine(transform.position - Vector3.forward / 2, transform.position + Vector3.forward / 2, Color.white, 1.75f);
					Debug.DrawLine(transform.position - Vector3.right / 2, transform.position + Vector3.right / 2, Color.magenta, 1.75f);
				}

				if (curSequenceIndex >= targetSequence.AttackSequenceParts.Length - 1)
				{
					break;
				}

				//Now we wait for the player to either do nothing for a given delay, or until he presses the attack button
				float delayTimer = 0;
				do
				{
					if (wantsToBeginNextSequence)
					{
						break;
					}

					yield return new WaitForEndOfFrame();
					delayTimer += Time.deltaTime;
				} while (delayTimer < targetSequence.PartsMaxDelays[curSequenceIndex]);

				if (wantsToBeginNextSequence)
				{
					curSequenceIndex++;
					wantsToBeginNextSequence = false;

					ActiveAttackStyle.Restrictions.ApplyRestriction(Player.Instance, false);
					continue;
				}

				//If the player did not try to start the next sequence in the given delay we stop here
				break;
			}

			ActiveAttackStyle.Restrictions.ApplyRestriction(Player.Instance, false);
		AttackFinished:;

			FXManager.FinishFX(ref whileBoundEffects);

			//Apply cooldowns
			animationResetCooldown = ANIMATION_RESET_COOLDOWN;
			if (curBaseData.ActionType == ActionType.Casting)
				Player.Instance.CastingCooldown = Mathf.Max(Player.Instance.CastingCooldown, ActiveAttackStyle.CausedCooldown);
			else if (curBaseData.ActionType == ActionType.Attacking)
				Player.Instance.AttackCooldown = Mathf.Max(Player.Instance.AttackCooldown, ActiveAttackStyle.CausedCooldown);

			//Reset all states
			SetAnimationSpeed(1);
			Player.Instance.animationControl.SetBool("InFight", false);
			ChangeWeaponState(InAttackSequence = false, ActiveAttackStyle = null);

		}
		#endregion
		#region Events
		public void HitObject(Vector3 hitPos, Collider hit, Vector3 direction)
		{
			for (int i = 0; i < activeHitboxGroup.Length; i++)
			{
				activeHitboxGroup[i].IgnoreCollision(hit, true);
			}
			ignoredColliders.Add(hit);

			if (hit.gameObject.layer == ConstantCollector.ENTITIE_LAYER)
			{
				Entity hitEntitie = hit.gameObject.GetComponent<Entity>();
				OnEntitieHit(hitEntitie, direction, hitPos);

				//Entitie hit Feedback
				if(weaponInfo.HitEntitieParticles)
					CreateParticles(weaponInfo.HitEntitieParticles, hitPos, direction);

				FXManager.ExecuteFX(weaponInfo.EntitieHitEffectsOnUser, Player.Instance.transform, true);
				FXManager.ExecuteFX(weaponInfo.EntitieHitEffectsOnWeapon, transform, true);
			}
			else
			{
				//Terrain hit Feedback
				if (weaponInfo.HitTerrainParticles)
					CreateParticles(weaponInfo.HitTerrainParticles, hitPos, direction);

				FXManager.ExecuteFX(weaponInfo.TerrainHitEffectsOnUser, Player.Instance.transform, true);
				FXManager.ExecuteFX(weaponInfo.TerrainHitEffectsOnWeapon, transform, true);
			}
		}
		private void OnEntitieHit(Entity hitEntitie, Vector3 direction, Vector3 hitPos)
		{
			//First calculate the generall damage apply
			float chargeMultiplier = ActiveAttackStyle.NeedsCharging ? curChargeMultiplier : 1;

			float damageMultiplier = (ActiveAttackStyle.ChargeSettings.HasMaskFlag(AttackChargeEffectMask.Damage) ? chargeMultiplier : 1);
			float critChanceMultiplier = (ActiveAttackStyle.ChargeSettings.HasMaskFlag(AttackChargeEffectMask.CritChance) ? chargeMultiplier : 1);
			float knockbackMultiplier = (ActiveAttackStyle.ChargeSettings.HasMaskFlag(AttackChargeEffectMask.Knockback) ? chargeMultiplier : 1);

			float damage =				curBaseData.BasePhysicalDamage * ActiveAttackStyle.DamageMultiplier * damageMultiplier;
			float critChance =			curBaseData.BaseCritChance * ActiveAttackStyle.CritChanceMultiplier * critChanceMultiplier;
			float? knockbackAmount =	curBaseData.BaseKnockback * ActiveAttackStyle.KnockbackMultiplier * knockbackMultiplier;

			bool isCrit = Utils.Chance01(critChance);

			knockbackAmount = knockbackAmount.Value > 0 ? knockbackAmount : null;
			ElementType attackElement = ActiveAttackStyle.OverrideElement ? ActiveAttackStyle.OverridenElement : weaponInfo.WeaponElement;
			CauseType attackCauseType = ActiveAttackStyle.OverrideCauseType ? ActiveAttackStyle.OverridenCauseType : weaponInfo.WeaponCauseType;

			hitEntitie.ChangeHealth(new ChangeInfo(Player.Instance, attackCauseType, attackElement, TargetStat.Health, hitPos, direction, damage, isCrit, knockbackAmount));

			//Now invoke the custom effects
			ActivateSingleHitEffects(hitEntitie, direction, hitPos);

			//If we are currently overriding the base data then we want to stop here, this will happen for example if a ultimate is active
			if (curBaseData != weaponInfo)
				return;

			//Combo addtion
			int comboIncrease = Mathf.RoundToInt(ActiveAttackStyle.OnHitComboWorth * (ActiveAttackStyle.NeedsCharging ? (ActiveAttackStyle.ChargeSettings.HasMaskFlag(AttackChargeEffectMask.ComboWorth) ? curChargeMultiplier : 1) : 1));
			ComboHit(hitEntitie, direction, hitPos, comboIncrease);

			//Ultimate charge change
			if (!weaponInfo.HasUltimate)
				return;

			float ultimateChargeAdd = isCrit ? weaponInfo.UltimateSettings.OnCritHitCharge : weaponInfo.UltimateSettings.OnHitCharge;
			ultimateChargeAdd += comboIncrease * weaponInfo.UltimateSettings.PerComboPointCharge;
			AddUltimateCharge(ultimateChargeAdd);
		}
		private void ActivateSingleHitEffects(Entity hitEntitie, Vector3 direction, Vector3 hitPos)
		{
			List<EffectSingle> activatedDirectHits = new List<EffectSingle>();

			if (ActiveAttackStyle.DirectHit)
				activatedDirectHits.Add(ActiveAttackStyle.DirectHit);

			if (ActiveAttackStyle.NeedsCharging)
			{
				activatedDirectHits.Capacity = System.Math.Max(activatedDirectHits.Capacity, activatedDirectHits.Count + ActiveAttackStyle.ChargeSettings.ChargeBasedDirectHits.Length);
				for (int i = 0; i < ActiveAttackStyle.ChargeSettings.ChargeBasedDirectHits.Length; i++)
				{
					if (ActiveAttackStyle.ChargeSettings.ChargeBasedDirectHits[i].MinRequiredCharge <= curChargeMultiplier &&
						ActiveAttackStyle.ChargeSettings.ChargeBasedDirectHits[i].MaxRequiredCharge >= curChargeMultiplier)
					{
						activatedDirectHits.Add(ActiveAttackStyle.ChargeSettings.ChargeBasedDirectHits[i].DirectHitOverride);
					}
				}
			}

			if (activatedDirectHits.Count > 0)
			{
				float chargeMultiplier = ActiveAttackStyle.NeedsCharging ? curChargeMultiplier : 1;

				float damageMultiplier = (ActiveAttackStyle.ChargeSettings.HasMaskFlag(AttackChargeEffectMask.Damage) ? chargeMultiplier : 1);
				float critChanceMultiplier = (ActiveAttackStyle.ChargeSettings.HasMaskFlag(AttackChargeEffectMask.CritChance) ? chargeMultiplier : 1);
				float knockbackMultiplier = (ActiveAttackStyle.ChargeSettings.HasMaskFlag(AttackChargeEffectMask.Knockback) ? chargeMultiplier : 1);

				for (int i = 0; i < activatedDirectHits.Count; i++)
				{
					EffectOverrides overrides = new EffectOverrides()
					{
						ExtraDamageMultiplier = ActiveAttackStyle.DamageMultiplier * damageMultiplier,
						ExtraCritChanceMultiplier = ActiveAttackStyle.CritChanceMultiplier * critChanceMultiplier,
						ExtraKnockbackMultiplier = ActiveAttackStyle.KnockbackMultiplier * knockbackMultiplier,
						EffectMultiplier = 1,
						OverridenElement = ActiveAttackStyle.OverrideElement ? ((ElementType?)ActiveAttackStyle.OverridenElement) : null,
						OverridenCauseType = ActiveAttackStyle.OverrideCauseType ? ((CauseType?)ActiveAttackStyle.OverridenCauseType) : null
					};

					activatedDirectHits[i].Activate(			Player.Instance,
																hitEntitie,
																curBaseData,
																direction,
																hitPos,
																overrides);
				}
			}
		}
		private void ComboHit(Entity hitEntitie, Vector3 direction, Vector3 hitPos, int comboIncrease)
		{
			int newComboAmount = curCombo + comboIncrease;
			timeToNextCombo = ActiveAttackStyle.ComboIncreaseMaxDelay;

			for (int i = 0; i < weaponInfo.ComboEffects.ComboData.Length; i++)
			{
				if (curCombo < weaponInfo.ComboEffects.ComboData[i].RequiredComboCount &&
					newComboAmount >= weaponInfo.ComboEffects.ComboData[i].RequiredComboCount)
				{
					if (weaponInfo.ComboEffects.ComboData[i].Effect.OverrideTextColor)
					{
						ComboDisplayController.Instance.OverrideColorSettings(weaponInfo.ComboEffects.ComboData[i].Effect.TextColor, weaponInfo.ComboEffects.ComboData[i].Effect.ColorScrollSpeed);
					}
					if (weaponInfo.ComboEffects.ComboData[i].Effect.OverrideTextPunch)
					{
						ComboDisplayController.Instance.OverridePunchSettings(weaponInfo.ComboEffects.ComboData[i].Effect.TextPunch, weaponInfo.ComboEffects.ComboData[i].Effect.PunchResetSpeed);
					}

					//Single / AOE effects
					if (weaponInfo.ComboEffects.ComboData[i].Effect.EffectOnTarget != null)
						weaponInfo.ComboEffects.ComboData[i].Effect.EffectOnTarget.Activate(Player.Instance, hitEntitie, weaponInfo, direction, hitPos);
					if (weaponInfo.ComboEffects.ComboData[i].Effect.EffectAOE != null)
						weaponInfo.ComboEffects.ComboData[i].Effect.EffectAOE.Activate(Player.Instance, Player.Instance.transform, weaponInfo);

					//Heal effects
					for (int j = 0; j < weaponInfo.ComboEffects.ComboData[i].Effect.HealEffects.Length; j++)
					{
						weaponInfo.ComboEffects.ComboData[i].Effect.HealEffects[j].Activate(Player.Instance);
					}

					//FX
					FXManager.ExecuteFX(weaponInfo.ComboEffects.ComboData[i].Effect.EffectsTillComboEnds, Player.Instance.transform, true, ref comboBoundFX);

					//Buffs
					comboBoundBuffs.Capacity = System.Math.Max(comboBoundBuffs.Capacity, comboBoundBuffs.Count + weaponInfo.ComboEffects.ComboData[i].Effect.BuffsTillComboEnds.Length);
					for (int j = 0; j < weaponInfo.ComboEffects.ComboData[i].Effect.BuffsTillComboEnds.Length; j++)
					{
						comboBoundBuffs.Add(Buff.ApplyBuff(weaponInfo.ComboEffects.ComboData[i].Effect.BuffsTillComboEnds[j], Player.Instance, Player.Instance));
					}
				}
			}

			//Update the cached amount and inform the UI about the change
			curCombo = newComboAmount;
			ComboDisplayController.Instance.SetCombo(curCombo);
		}
		private void ComboFinish()
		{
			curCombo = 0;
			FXManager.FinishFX(ref comboBoundFX);

			for (int i = 0; i < comboBoundBuffs.Count; i++)
			{
				Player.Instance.RemoveBuff(comboBoundBuffs[i]);
			}
			comboBoundBuffs = new List<BuffInstance>();
			ComboDisplayController.Instance?.ResetCombo(weaponInfo.ComboEffects);
		}
		private void EntitieDeath(Entity killed, Entity killer)
		{
			if(killer is Player)
			{
				AddUltimateCharge(weaponInfo.UltimateSettings.OnKillCharge);
			}
		}
		#endregion
		#region Effect Control
		private void SetAnimationSpeed(float speed) => Player.Instance.animationControl.SetFloat("AttackAnimationSpeed", speed);
		private void RemoveChargeBoundEffects()
		{
			isChargingAttack = false;
			ActiveAttackStyle.ChargeSettings.Restrictions.ApplyRestriction(Player.Instance, false);

			FXManager.FinishFX(ref chargeBoundFX);
			FXManager.FinishFX(ref chargeBoundFXMultiplied);
			for (int i = 0; i < chargeBoundBuffs.Length; i++)
			{
				Player.Instance.RemoveBuff(chargeBoundBuffs[i]);
			}

			chargeBoundFX = null;
			chargeBoundFXMultiplied = null;
			chargeBoundBuffs = null;
		}
		private void CreateParticles(GameObject prefab, Vector3 hitPos, Vector3 direction)
		{
			GameObject newParticleSystem = Instantiate(prefab, Storage.ParticleStorage);
			newParticleSystem.transform.forward = direction;
			newParticleSystem.transform.position = hitPos;
			EffectManager.FadeAndDestroyParticles(newParticleSystem, 1);
		}
		private void DropWeapon()
		{
			StopAllCoroutines();
			RemoveBoundEffects();
			dropCollision.SetActive(true);
			Rigidbody b = gameObject.AddComponent<Rigidbody>();
			b.velocity = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)) * 5;
			b.angularVelocity = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * 540;
			enabled = false;
		}
		#endregion
		#region Ultimate Control
		private void UltimateControl()
		{
			if (Player.Instance.curStates.Fighting)
			{
				float change = weaponInfo.UltimateSettings.ChargeOverTimeOnCombat * Time.deltaTime;
				if (change != 0)
					AddUltimateCharge(change);
			}
			else
			{
				float change = weaponInfo.UltimateSettings.OutOfCombatDecrease * Time.deltaTime;
				if(change != 0)
					AddUltimateCharge(change);
			}

			if (InputController.Special.Down && ultimateCharge == weaponInfo.UltimateSettings.TotalRequiredCharge)
			{
				if (weaponInfo.UltimateSettings.Ultimate.CanActivate(Player.Instance, 1, 1, 1))
				{
					weaponInfo.UltimateSettings.Ultimate.Activate();
					AddUltimateCharge(-weaponInfo.UltimateSettings.OnUseChargeRemove * weaponInfo.UltimateSettings.TotalRequiredCharge);
				}
			}
		}
		public void AddUltimateCharge(float value)
		{
			bool ultimateWasReady = ultimateCharge >= weaponInfo.UltimateSettings.TotalRequiredCharge;
			ultimateCharge = Mathf.Clamp(ultimateCharge + value, 0, weaponInfo.UltimateSettings.TotalRequiredCharge);
			bool ultimateIsReady = ultimateCharge >= weaponInfo.UltimateSettings.TotalRequiredCharge;

			//Was not ready, but now is
			if (!ultimateWasReady && ultimateIsReady)
			{
				FXManager.ExecuteFX(weaponInfo.UltimateSettings.OnUltimateChargedEffects, transform, true);
				FXManager.ExecuteFX(Player.PlayerSettings.EffectsOnUltimateCharged, Player.Instance.transform, true);

				FXManager.ExecuteFX(weaponInfo.UltimateSettings.WhileUltimateIsChargedEffects, transform, true, out ultimateChargedBoundFX);
				FXManager.ExecuteFX(Player.PlayerSettings.EffectsWhileUltimateCharged, Player.Instance.transform, true, out ultimateChargedBoundFXPlayer);
			}
			//Was ready, but not anymore
			else if(ultimateWasReady && !ultimateIsReady)
			{
				FXManager.FinishFX(ref ultimateChargedBoundFX);
				FXManager.FinishFX(ref ultimateChargedBoundFXPlayer);
			}
		}
		#endregion
		#region Remove Control
		public void Remove()
		{
			if (this != Instance)
			{
				return;
			}

			Instance = null;
			Destroy(gameObject);
			ComboFinish();

			if (ActiveAttackStyle != null)
			{
				if (Player.Existant)
				{
					ActiveAttackStyle.Restrictions.ApplyRestriction(Player.Instance, false);
					SetAnimationSpeed(1);
					Player.Instance.animationControl.SetBool("InFight", false);
				}
			}
			RemoveBoundEffects();

			if (UltimateBarController.Instance)
				UltimateBarController.Instance.gameObject.SetActive(false);
		}
		private void RemoveBoundEffects()
		{
			if (isChargingAttack)
				RemoveChargeBoundEffects();

			FXManager.FinishFX(ref ultimateChargedBoundFX);
			FXManager.FinishFX(ref ultimateChargedBoundFXPlayer);
		}
		private void OnDestroy()
		{
			EventManager.EntitieDiedEvent -= EntitieDeath;
		}
		#endregion
	}
	[System.Serializable]
	public class CustomHitboxGroup
	{
		public string Identifier = "New Hitbox Group";
		public WeaponHitbox[] Hitboxes = new WeaponHitbox[0];
	}
}