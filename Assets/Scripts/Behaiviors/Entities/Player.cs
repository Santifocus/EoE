using EoE.Controlls;
using EoE.Information;
using EoE.Utils;
using EoE.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Entities
{
	public class Player : Entitie
	{
		#region Fields
		//Inspector variables
		[Space(10)]
		[SerializeField] private PlayerSettings selfSettings = default;
		[SerializeField] private Transform rightHand = default;
		[SerializeField] private Weapon equipedWeapon = default;
		public TMPro.TextMeshProUGUI debugText = default;

		//Endurance
		[HideInInspector] public bool recentlyUsedEndurance;
		[HideInInspector] public List<float> enduranceContainers;
		[HideInInspector] public int totalEnduranceContainers;
		[HideInInspector] public int activeEnduranceContainerIndex;
		[HideInInspector] public float lockedEndurance;
		private float usedEnduranceCooldown;

		#region Physical Attack
		//Attack
		public Weapon PlayerWeapon { get => equipedWeapon; set => ChangeWeapon(value); }
		[HideInInspector] public Attack activeAttack;
		private WeaponController heldWeapon;
		private AttackState lastAttackType;
		private bool isCurrentlyAttacking;
		private bool canceledAnimation;

		//Combos
		private float? comboTimer;
		private int comboIndex;

		//Attack animations
		private Dictionary<AttackAnimation, (float, float)> animationDelayLookup = new Dictionary<AttackAnimation, (float, float)>()
		{
			{ AttackAnimation.Stab, (0.4f, 0.15f) },
			{ AttackAnimation.ToLeftSlash, (0.533f, 0) },
			{ AttackAnimation.TopDownSlash, (0.533f, 0) },
			{ AttackAnimation.ToRightSlash, (0.533f, 0.22f) },
			{ AttackAnimation.Uppercut, (0.533f, 0) },
		};
		#endregion

		//Getter Helpers
		public static bool Alive { get; private set; }
		private enum GetEnduranceType : byte { NotAvailable = 0, Available = 1, AvailableWithNewBar = 2 }
		public enum AttackState : short { NormalStand = 0, NormalSprint = 1, NormalJump = 2, NormalSprintJump = 3, HeavyStand = 4, HeavySprint = 5, HeavyJump = 6, HeavySprintJump = 7 }
		private static Player instance;
		public static Player Instance => instance;
		public override EntitieSettings SelfSettings => selfSettings;
		public static PlayerSettings PlayerSettings => instance.selfSettings;
		public static Entitie targetedEntitie;

		//Other
		private bool pressedInteract;

		#endregion
		#region Basic Monobehaivior
		protected override void EntitieStart()
		{
			Alive = true;
			instance = this;

			ChangeWeapon(equipedWeapon);
			SetupEndurance();
		}
		protected override void EntitieUpdate()
		{
			EnduranceRegen();
			CameraControl();
			MovementControl();
			InteractControl();
			AttackControl();
			TargetEnemyControl();
			PositionHeldWeapon();
		}
		protected override void EntitieFixedUpdate()
		{
			PositionHeldWeapon();
		}
		#endregion
		#region Setups
		private void ChangeWeapon(Weapon newWeapon)
		{
			if (heldWeapon)
			{
				Destroy(heldWeapon.gameObject);
			}
			equipedWeapon = newWeapon;

			if(equipedWeapon != null)
			{
				heldWeapon = Instantiate(equipedWeapon.weaponPrefab);
				heldWeapon.Setup();
			}
		}
		private void SetupEndurance()
		{
			activeEnduranceContainerIndex = PlayerSettings.EnduranceBars - 1;
			totalEnduranceContainers = PlayerSettings.EnduranceBars;

			enduranceContainers = new List<float>(totalEnduranceContainers);
			for(int i = 0; i < totalEnduranceContainers; i++)
			{
				enduranceContainers.Add(PlayerSettings.EndurancePerBar);
			}
		}
		protected void PositionHeldWeapon()
		{
			Vector3 worldOffset =		equipedWeapon.weaponHandleOffset.x * rightHand.right + 
										equipedWeapon.weaponHandleOffset.y * rightHand.up + 
										equipedWeapon.weaponHandleOffset.z * rightHand.forward;
			heldWeapon.transform.position = rightHand.position + worldOffset;
			heldWeapon.transform.rotation = rightHand.rotation;
		}
		#endregion
		#region Endurance Control
		private GetEnduranceType CanAffordEnduranceCost(float cost)
		{
			if (enduranceContainers[activeEnduranceContainerIndex] >= cost)
				return GetEnduranceType.Available;

			float total = totalEndurance();

			if (total > cost)
				return GetEnduranceType.AvailableWithNewBar;
			else
				return GetEnduranceType.NotAvailable;

			float totalEndurance()
			{
				float t = 0;
				for (int i = 0; i < enduranceContainers.Count; i++)
					t += enduranceContainers[i];

				return t;
			}
		}
		private void UseEndurance(float amount)
		{
			usedEnduranceCooldown = PlayerSettings.EnduranceRegenDelayAfterUse;
			recentlyUsedEndurance = true;

			enduranceContainers[activeEnduranceContainerIndex] -= amount;
			if(enduranceContainers[activeEnduranceContainerIndex] <= 0)
			{
				activeEnduranceContainerIndex--;
				if(activeEnduranceContainerIndex < 0)
				{
					activeEnduranceContainerIndex = 0;
					enduranceContainers[0] = 0;
				}
				else
				{
					enduranceContainers[activeEnduranceContainerIndex] += enduranceContainers[activeEnduranceContainerIndex + 1];
					enduranceContainers[activeEnduranceContainerIndex + 1] = 0;
				}
			}
		}
		public void EnduranceRegen()
		{
			if(recentlyUsedEndurance)
			{
				usedEnduranceCooldown -= Time.deltaTime;
				if (usedEnduranceCooldown <= 0)
				{
					usedEnduranceCooldown = 0;
					recentlyUsedEndurance = false;
				}
			}

			if(activeEnduranceContainerIndex < totalEnduranceContainers - 1)
			{
				float perSecond =	PlayerSettings.EnduranceRegen *
									PlayerSettings.LockedEnduranceRegenMutliplier * 
									(curStates.IsInCombat ? PlayerSettings.EnduranceRegenInCombat : 1);

				lockedEndurance += Time.deltaTime * perSecond;

				if (lockedEndurance >= PlayerSettings.EndurancePerBar)
				{
					float add = PlayerSettings.EndurancePerBar - enduranceContainers[activeEnduranceContainerIndex];
					enduranceContainers[activeEnduranceContainerIndex] = PlayerSettings.EndurancePerBar;
					float rest = PlayerSettings.EndurancePerBar - add;

					activeEnduranceContainerIndex++;
					enduranceContainers[activeEnduranceContainerIndex] = rest;
					lockedEndurance = 0;
				}
			}
			else
			{
				lockedEndurance = 0;
			}

			if (!recentlyUsedEndurance && enduranceContainers[activeEnduranceContainerIndex] < PlayerSettings.EndurancePerBar)
			{
				enduranceContainers[activeEnduranceContainerIndex] += Time.deltaTime * PlayerSettings.EnduranceRegen * (curStates.IsInCombat ? PlayerSettings.EnduranceRegenInCombat : 1);
				if (enduranceContainers[activeEnduranceContainerIndex] > PlayerSettings.EndurancePerBar)
					enduranceContainers[activeEnduranceContainerIndex] = PlayerSettings.EndurancePerBar;
			}
		}
		#endregion
		#region Movement
		private void MovementControl()
		{
			JumpControl();
			PlayerMoveControl();
		}
		private void JumpControl()
		{
			if (InputController.Jump.Down && curStates.IsGrounded)
			{
				float cost = PlayerSettings.JumpEnduranceCost;
				if ((int)CanAffordEnduranceCost(cost) > 0) //0 == Not available; 1/2 == available
				{
					Jump();
					UseEndurance(cost);
				}
			}
		}
		private void PlayerMoveControl()
		{
			//Where is the player Pointing the Joystick at?
			Vector2 controllDirection = InputController.PlayerMove;

			bool moving = controllDirection != Vector2.zero;
			curStates.IsMoving = moving;

			//Is the player not trying to move? Then stop here
			if (!moving)
				return;

			//Check if the player intends to run and is able to
			bool running = curStates.IsRunning;
			if(running)
			{
				float runCost = PlayerSettings.RunEnduranceCost * Time.deltaTime;

				if ((int)CanAffordEnduranceCost(runCost) == 0) //0 == Not available; 1/2 == available
					running = curStates.IsRunning = false;
				else
					UseEndurance(runCost);
			}
			else if (InputController.Run.Down)
			{
				float runCost = PlayerSettings.RunEnduranceCost * Time.deltaTime;

				if ((int)CanAffordEnduranceCost(runCost) > 0) //0 == Not available; 1/2 == available
				{
					UseEndurance(runCost);
					running = curStates.IsRunning = true;
				}
			}

			float intendedControl = Mathf.Min(1, controllDirection.magnitude);
			intendedAcceleration = intendedControl * (running ? PlayerSettings.RunSpeedMultiplicator : 1);

			if(intendedControl > 0.5f)
				intendedRotation = Mathf.Atan2(controllDirection.y, controllDirection.x) * Mathf.Rad2Deg + 90 + PlayerCameraController.CurRotation.x;
		}
		private void CameraControl()
		{
			if (!targetedEntitie)
			{
				Vector2 newMoveDistance = InputController.CameraMove;
				newMoveDistance = new Vector2(newMoveDistance.x * selfSettings.CameraRotationPower.x, newMoveDistance.y * selfSettings.CameraRotationPower.y) * Time.deltaTime;
				PlayerCameraController.TargetRotation += newMoveDistance;
			}
		}
		private void InteractControl()
		{
			if (Interactable.MarkedInteractable && InputController.Attack.Down)
			{
				if (!pressedInteract)
				{
					Interactable.MarkedInteractable.Interact();
				}
				pressedInteract = true;
			}
			else
			{
				pressedInteract = false;
			}
		}
		private void TargetEnemyControl()
		{
			if (InputController.Aim.Down)
			{
				float lowestDist = 100000;
				int targetIndex = -1;
				for(int i = 0; i < AllEntities.Count; i++)
				{
					if (AllEntities[i] is Player)
						continue;
					float selfDist = (actuallWorldPosition - AllEntities[i].actuallWorldPosition).sqrMagnitude;
					if(selfDist < lowestDist)
					{
						lowestDist = selfDist;
						targetIndex = i;
					}
				}

				if(targetIndex != -1)
				{
					targetedEntitie = AllEntities[targetIndex];
				}
			}

			if (InputController.Aim.Up)
				targetedEntitie = null;
		}
		#endregion
		#region Physical Attack Control
		private void AttackControl()
		{
			if (InputController.Dodge.Down)
			{
				EffectUtils.ShakeScreen(0.25f, 0.25f, 5);
			}
			if (comboTimer.HasValue)
			{
				comboTimer -= Time.deltaTime;
				if (comboTimer.Value < 0)
				{
					comboTimer = null;
					comboIndex = 0;
				}
			}

			if (equipedWeapon == null || isCurrentlyAttacking || (!InputController.Attack.Down && !InputController.HeavyAttack.Down))
				return;

			int state = 0;
			state += InputController.Attack.Down ? 0 : 4; //If we are here either heavy attack or normal attack was pressed
			state += curStates.IsRunning ? 1 : 0; //Running => Sprint attack
			state += !curStates.IsGrounded ? 2 : 0; //In air => Jump attack

			StartCoroutine(BeginAttack((AttackState)state));
		}
		public void CancelAttackAnimation()
		{
			if (isCurrentlyAttacking)
				canceledAnimation = true;
		}
		private IEnumerator BeginAttack(AttackState state)
		{
			(Attack targetAttack, float? comboDelay) = PrepareTargetAttack(state);

			activeAttack = targetAttack;
			if (targetAttack != null)
			{ 
				AttackAnimation anim = targetAttack.animationInfo.animation;

				isCurrentlyAttacking = true;
				lastAttackType = state;

				(float animTime, float chargeDelay) = animationDelayLookup[anim];
				animationControl.SetBool("Attack", true);
				animationControl.SetTrigger(anim.ToString());

				bool applyForceAfterCustomDelay = !targetAttack.velocityEffect.applyForceAfterAnimationCharge;
				float forceApplyDelay = applyForceAfterCustomDelay ? targetAttack.velocityEffect.applyForceDelay : 0;

				if (chargeDelay == 0)
				{
					if(!applyForceAfterCustomDelay)
						ApplyAttackForces();
					heldWeapon.Active = true;
				}

				if(applyForceAfterCustomDelay && forceApplyDelay == 0)
				{
					ApplyAttackForces();
				}

				while (animTime > 0 || activeAttack.animationInfo.haltAnimationTillCancel)
				{
					yield return new WaitForEndOfFrame();
					animTime -= Time.deltaTime;

					if(chargeDelay > 0)
					{
						chargeDelay -= Time.deltaTime;

						if (chargeDelay <= 0)
						{
							if (targetAttack.velocityEffect.applyForceAfterAnimationCharge)
								ApplyAttackForces();
							heldWeapon.Active = true;
						}
					}

					if(applyForceAfterCustomDelay && forceApplyDelay > 0)
					{
						forceApplyDelay -= Time.deltaTime;
						if(forceApplyDelay <= 0)
						{
							ApplyAttackForces();
						}
					}

					if (canceledAnimation || CheckForCancelCondition())
					{
						canceledAnimation = false;
						break;
					}
				}

				isCurrentlyAttacking = false;
				heldWeapon.Active = false;
				animationControl.SetBool("Attack", false);

				//Combo setup
				comboTimer = comboDelay;
				comboIndex = comboDelay.HasValue ? comboIndex + 1 : 0;
			}
		}
		private (Attack, float?) PrepareTargetAttack(AttackState state)
		{
			int attackIndex = (int)state;
			int innerIndex = state != lastAttackType ? 0 : comboIndex;
			Attack style = null;
			float? comboDelay = null;

			style = equipedWeapon.weaponAttackStyle[attackIndex].attacks[innerIndex];
			if (!style.enabled)
			{
				style = null;
			}
			else if(equipedWeapon.weaponAttackStyle[attackIndex].attacks.Length > innerIndex + 1)
			{
				comboDelay = equipedWeapon.weaponAttackStyle[attackIndex].delays[innerIndex];
			}

			//Find out if the player can afford the attack
			if(style != null)
			{
				float cost = style.info.enduranceMultiplier * equipedWeapon.baseEnduranceDrain;
				if ((int)CanAffordEnduranceCost(cost) == 0) //0 == Not available; 1/2 == available
				{
					style = null;
					comboDelay = null;
				}
				else
					UseEndurance(cost);
			}

			return (style, comboDelay);
		}
		private bool CheckForCancelCondition()
		{
			int requiredGroundState = (int)activeAttack.animationInfo.cancelWhenOnGround - 1;
			bool groundStateAchieved = (curStates.IsGrounded ? 0 : 1) == requiredGroundState;

			if (groundStateAchieved && !activeAttack.animationInfo.bothStates)
				return true;

			if (!groundStateAchieved && activeAttack.animationInfo.bothStates)
				return false;

			int requiredSprintState = (int)activeAttack.animationInfo.cancelWhenSprinting - 1;
			bool sprintStateAchieved = (curStates.IsRunning ? 0 : 1) == requiredSprintState;

			return (sprintStateAchieved && groundStateAchieved) || (!activeAttack.animationInfo.bothStates && sprintStateAchieved);
		}
		private void ApplyAttackForces()
		{
			AttackVelocityEffect effect = activeAttack.velocityEffect;

			if (effect.velocityIntent == AttackVelocityIntent.Off)
				return;
			Vector3 velocity = Vector3.zero;

			if (effect.useRightValue)
			{
				velocity += effect.rightValue * transform.right;
			}
			if (effect.useUpValue)
			{
				velocity += effect.upValue * transform.up;
			}
			if (effect.useForwardValue)
			{
				velocity += effect.forwardValue * transform.forward;
			}

			if (effect.velocityIntent == AttackVelocityIntent.Set)
			{
				curAcceleration = 0;
				curMoveForce = Vector3.zero;

				if (effect.ignoreVerticalVelocity)
				{
					curExtraForce = new Vector3(velocity.x, curExtraForce.y, velocity.z);
					curJumpForce = new Vector3(0, curJumpForce.y, 0);
				}
				else
				{
					curExtraForce = new Vector3(velocity.x, 0, velocity.z);
					curJumpForce = new Vector3(0, velocity.y, 0);
				}
			}
			else
			{
				curAcceleration = 0;
				curMoveForce = Vector3.zero;

				if (effect.ignoreVerticalVelocity)
				{
					curExtraForce += new Vector3(velocity.x, 0, velocity.z);
				}
				else
				{
					curExtraForce += new Vector3(velocity.x, 0, velocity.z);
					curJumpForce += new Vector3(0, velocity.y, 0);
				}
			}

			body.velocity = curVelocity;
		}
		#endregion
		#region StateControl
		protected override void Death()
		{
			if (heldWeapon)
				Destroy(heldWeapon.gameObject);
			EffectUtils.BlurScreen(1, 100, 5);

			Alive = false;
			base.Death();
		}
		#endregion
	}
}