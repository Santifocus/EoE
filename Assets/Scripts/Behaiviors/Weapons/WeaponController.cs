using EoE.Entities;
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
		private const float COMBO_WAIT_THRESHOLD = 0.65f;
		//Static info
		private static readonly Dictionary<AttackAnimation, (float, float)> animationDelayLookup = new Dictionary<AttackAnimation, (float, float)>()
		{
			{ AttackAnimation.Attack1, (0.333f, 0.2f) },
			{ AttackAnimation.Attack2, (0.5f, 0.22f) },
			{ AttackAnimation.Attack3, (0.833f, 0.583f) },
		};
		public static WeaponController PlayerWeaponController;
		public bool InAttackSequence { get; private set; }
		public AttackStyle ActiveAttackStyle { get; private set; }

		//Inspector variables
		[SerializeField] private WeaponHitbox[] weaponHitboxes = default;
		[SerializeField] private GameObject[] objectsToActivateOnActive = default;

		//Behaivior Control
		private bool colliderActive;
		private bool wantsToBeginNextSequence;
		private Weapon weaponInfo;
		private List<Collider> ignoredColliders = new List<Collider>();

		//Combo Control
		private float curAttackAnimationPoint = 0;
		private int curCombo;
		private float timeToNextCombo;
		private List<FXInstance> comboBoundFX = new List<FXInstance>();
		private List<BuffInstance> comboBoundBuffs = new List<BuffInstance>();

		#endregion
		#region Setups
		public void Setup(Weapon weaponInfo)
		{
			PlayerWeaponController = this;
			this.weaponInfo = weaponInfo;
			for (int i = 0; i < weaponHitboxes.Length; i++)
			{
				weaponHitboxes[i].Setup(this);
			}
			ComboDisplayController.Instance.ResetCombo(weaponInfo.ComboEffects);
			ChangeWeaponState(false, null);
			FollowPlayer();
		}
		private void ChangeWeaponState(bool state, AttackStyle style)
		{
			colliderActive = state;
			for (int i = 0; i < weaponHitboxes.Length; i++)
			{
				weaponHitboxes[i].SetColliderStyle(state ? style : null);
				if (!state)
				{
					for(int j = 0; j < ignoredColliders.Count; j++)
					{
						weaponHitboxes[i].IgnoreCollision(ignoredColliders[j], false);
					}
				}
			}
			for (int i = 0; i < objectsToActivateOnActive.Length; i++)
			{
				objectsToActivateOnActive[i].SetActive(state);
			}
			ignoredColliders = new List<Collider>();
		}
		public GameObject CloneModel()
		{
			WeaponController clone = Instantiate(this, Storage.ParticleStorage);

			//deactivate behaiviors
			clone.enabled = false;
			for(int i = 0; i < clone.weaponHitboxes.Length; i++)
			{
				clone.weaponHitboxes[i].enabled = false;
			}

			//deactivate all custom objects
			for (int i = 0; i < clone.objectsToActivateOnActive.Length; i++)
			{
				clone.objectsToActivateOnActive[i].SetActive(false);
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
			if (timeToNextCombo > 0)
			{
				timeToNextCombo -= Time.deltaTime;
				if(timeToNextCombo <= 0)
				{
					timeToNextCombo = 0;
					ComboFinish();
				}
			}
		}
		#endregion
		#region FollowPlayer
		private void LateUpdate()
		{
			FollowPlayer();
		}
		private void FixedUpdate()
		{
			FollowPlayer();
		}
		private void FollowPlayer()
		{
			if (!Player.Instance.Alive)
			{
				Destroy(gameObject);
				return;
			}

			Vector3 worldOffset =	weaponInfo.WeaponPositionOffset.x * Player.Instance.weaponHoldPoint.right +
									weaponInfo.WeaponPositionOffset.y * Player.Instance.weaponHoldPoint.up +
									weaponInfo.WeaponPositionOffset.z * Player.Instance.weaponHoldPoint.forward;

			transform.position = Player.Instance.weaponHoldPoint.position + worldOffset;
			transform.eulerAngles = Player.Instance.weaponHoldPoint.eulerAngles + weaponInfo.WeaponRotationOffset;
		}
		#endregion
		public void StartAttack()
		{
			if (Player.Instance.IsCasting)
				return;

			if(InAttackSequence)
			{
				if(curAttackAnimationPoint > COMBO_WAIT_THRESHOLD)
					wantsToBeginNextSequence = true;
				return;
			}

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
			StartCoroutine(Attack(weaponInfo[part]));
		}
		private IEnumerator Attack(AttackSequence targetSequence)
		{
			InAttackSequence = true;
			int curSequenceIndex = 0;
			while (true)
			{
				ActiveAttackStyle = targetSequence.AttackSequenceParts[curSequenceIndex];

				if (ActiveAttackStyle.StopMovement)
					Player.Instance.AppliedMoveStuns++;

				//First check if this attack sequence is allowed if not we stop the while loop here
				float enduranceCost = weaponInfo.BaseEnduranceCost * ActiveAttackStyle.EnduranceCostMultiplier;
				float manaCost = weaponInfo.BaseManaCost * ActiveAttackStyle.ManaCostMultiplier;
				if ((Player.Instance.curEndurance >= enduranceCost) && (Player.Instance.curMana >= manaCost))
				{
					Player.Instance.ChangeEndurance(new ChangeInfo(Player.Instance, CauseType.Magic, TargetStat.Endurance, enduranceCost));
					Player.Instance.ChangeMana(new ChangeInfo(Player.Instance, CauseType.Magic, TargetStat.Mana, manaCost));
				}
				else
				{
					break;
				}


				//Setup timers and blend variables
				float totalTime = 0;
				float animationTime = animationDelayLookup[ActiveAttackStyle.AnimationTarget].Item1;
				float animationActivationDelay = animationDelayLookup[ActiveAttackStyle.AnimationTarget].Item2;
				float animationTimer = 0; 
				curAttackAnimationPoint = 0;
				ChangeWeaponState(false, null);

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
				Player.Instance.animationControl.SetTrigger(ActiveAttackStyle.AnimationTarget.ToString());
				do
				{
					yield return new WaitForEndOfFrame();
					if (GameController.GameIsPaused)
						continue;

					totalTime += Time.deltaTime;
					//Find the current animationSpeed multiplier
					if(ActiveAttackStyle.AnimationMultiplicationType == MultiplicationType.Curve)
					{
						multiplier = ActiveAttackStyle.AnimationSpeedCurve.Evaluate(Mathf.Clamp01(totalTime / ActiveAttackStyle.AnimationSpeedCurveTimeframe)) * ActiveAttackStyle.AnimationSpeedCurveMultiplier;
						SetAnimationSpeed(multiplier);
					}
					animationTimer += multiplier * Time.deltaTime;

					//Check if we reached the collision activation point
					bool shouldState = animationTimer > animationActivationDelay;
					if (shouldState != colliderActive)
					{
						ChangeWeaponState(shouldState, ActiveAttackStyle);
					}

					//Debug
					if(GameController.CurrentGameSettings.IsDebugEnabled && shouldState)
					{
						Debug.DrawLine(transform.position - Vector3.up / 4, transform.position + Vector3.up / 4, Color.green / 2, 1);
						Debug.DrawLine(transform.position - Vector3.right / 4, transform.position + Vector3.right / 4, Color.cyan / 2, 1);
					}

					//Check if we crossed any attack event point
					float newAnimationPoint = animationTimer / animationTime;
					float smallerPoint = Mathf.Min(newAnimationPoint, curAttackAnimationPoint);
					float biggerPoint = Mathf.Max(newAnimationPoint, curAttackAnimationPoint);
					curAttackAnimationPoint = newAnimationPoint;
					for(int i = 0; i < ActiveAttackStyle.AttackEffects.Length; i++)
					{
						float point = ActiveAttackStyle.AttackEffects[i].AtAnimationPoint;
						if ((point >= smallerPoint) && (point < biggerPoint))
						{
							if(Utils.Chance01(ActiveAttackStyle.AttackEffects[i].ChanceToActivate))
							{
								ActiveAttackStyle.AttackEffects[i].ActivateEffect(Player.Instance, weaponInfo);
							}
						}
					}

				} while (((curAttackAnimationPoint > 0 || multiplier > 0) && (curAttackAnimationPoint < 1)) || GameController.GameIsPaused);

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

					if (ActiveAttackStyle.StopMovement)
						Player.Instance.AppliedMoveStuns--;
					continue;
				}

				//If the player did not try to start the next sequence in the given delay we stop here
				break;
			}
			if (ActiveAttackStyle.StopMovement)
				Player.Instance.AppliedMoveStuns--;

			Player.Instance.animationControl.SetTrigger("FightEnd");
			ChangeWeaponState(InAttackSequence = false, ActiveAttackStyle = null);
		}
		public void HitObject(Vector3 hitPos, Collider hit, Vector3 direction)
		{
			for (int i = 0; i < weaponHitboxes.Length; i++)
			{
				weaponHitboxes[i].IgnoreCollision(hit, true);
			}
			ignoredColliders.Add(hit);
			
			if(hit.gameObject.layer == ConstantCollector.ENTITIE_LAYER)
			{
				Entitie hitEntitie = hit.gameObject.GetComponent<Entitie>();
				if (ActiveAttackStyle.DirectHit)
				{
					EffectOverrides overrides = new EffectOverrides()
					{
						ExtraDamageMultiplier = ActiveAttackStyle.DamageMultiplier,
						ExtraCritChanceMultiplier = ActiveAttackStyle.CritChanceMultiplier,
						ExtraKnockbackMultiplier = ActiveAttackStyle.KnockbackMultiplier,
						OverridenElement = ActiveAttackStyle.OverrideElement ? ((ElementType?)ActiveAttackStyle.OverridenElement) : null,
						OverridenCauseType = ActiveAttackStyle.OverrideCauseType ? ((CauseType?)ActiveAttackStyle.OverridenCauseType) : null
					};

					ActiveAttackStyle.DirectHit.ActivateEffectSingle(	Player.Instance,
																		hitEntitie,
																		weaponInfo,
																		direction,
																		hitPos,
																		overrides);
					ComboHit(hitEntitie, direction, hitPos);
					CreateParticles(GameController.CurrentGameSettings.HitEntitieParticles, hitPos, direction);
				}
			}
			else
			{
				CreateParticles(GameController.CurrentGameSettings.HitTerrainParticles, hitPos, direction);
			}
		}
		private void CreateParticles(GameObject prefab, Vector3 hitPos, Vector3 direction)
		{
			GameObject newParticleSystem = Instantiate(prefab, Storage.ParticleStorage);
			newParticleSystem.transform.forward = direction;
			newParticleSystem.transform.position = hitPos;
			EffectUtils.FadeAndDestroyParticles(newParticleSystem, 1);
		}

		private void ComboHit(Entitie hitEntitie, Vector3 direction, Vector3 hitPos)
		{
			int newComboAmount = curCombo + ActiveAttackStyle.OnHitComboWorth;
			timeToNextCombo = ActiveAttackStyle.ComboIncreaseMaxDelay;

			for(int i = 0; i < weaponInfo.ComboEffects.ComboData.Length; i++)
			{
				if(	curCombo < weaponInfo.ComboEffects.ComboData[i].RequiredComboCount &&
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
						weaponInfo.ComboEffects.ComboData[i].Effect.EffectOnTarget.ActivateEffectSingle(Player.Instance, hitEntitie, weaponInfo, direction, hitPos);
					if(weaponInfo.ComboEffects.ComboData[i].Effect.EffectAOE != null)
						weaponInfo.ComboEffects.ComboData[i].Effect.EffectAOE.ActivateEffectAOE(Player.Instance, Player.Instance.transform, weaponInfo);

					//Heal effects
					for(int j = 0; j < weaponInfo.ComboEffects.ComboData[i].Effect.HealEffects.Length; j++)
					{
						weaponInfo.ComboEffects.ComboData[i].Effect.HealEffects[j].Activate(Player.Instance);
					}

					//FX
					for(int j = 0; j < weaponInfo.ComboEffects.ComboData[i].Effect.EffectsTillComboEnds.Length; j++)
					{
						comboBoundFX.Add(FXManager.PlayFX(weaponInfo.ComboEffects.ComboData[i].Effect.EffectsTillComboEnds[j], Player.Instance.transform, true));
					}

					//Buffs
					for (int j = 0; j < weaponInfo.ComboEffects.ComboData[i].Effect.BuffsTillComboEnds.Length; j++)
					{
						comboBoundBuffs.Add(Player.Instance.AddBuff(weaponInfo.ComboEffects.ComboData[i].Effect.BuffsTillComboEnds[j], Player.Instance));
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
			for(int i = 0; i < comboBoundFX.Count; i++)
			{
				comboBoundFX[i].FinishFX();
			}
			comboBoundFX = new List<FXInstance>();

			for (int i = 0; i < comboBoundBuffs.Count; i++)
			{
				Player.Instance.RemoveBuff(comboBoundBuffs[i]);
			}
			comboBoundBuffs = new List<BuffInstance>();
			ComboDisplayController.Instance.ResetCombo(weaponInfo.ComboEffects);
		}

		private void SetAnimationSpeed(float speed) => Player.Instance.animationControl.SetFloat("AttackAnimationSpeed", speed);
	}
}