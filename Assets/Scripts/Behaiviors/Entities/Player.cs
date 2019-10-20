using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Events;
using EoE.Information;
using EoE.Controlls;
using EoE.Utils;
using EoE.Weapons;

namespace EoE.Entities
{
	public class Player : Entitie
	{
		private static Player instance;
		public static Player Instance => instance;
		public override EntitieSettings SelfSettings => selfSettings;
		public static PlayerSettings PlayerSettings => instance.selfSettings;
		[SerializeField] private PlayerSettings selfSettings = default;
		public TMPro.TextMeshProUGUI debugText = default;

		private enum GetEnduranceType : byte { NotAvailable = 0, Available = 1, AvailableWithNewBar = 2 }
		[HideInInspector] public List<float> enduranceContainers;
		[HideInInspector] public int totalEnduranceContainers;
		[HideInInspector] public int activeEnduranceContainerIndex;
		[HideInInspector] public float lockedEndurance;

		[HideInInspector] public bool recentlyUsedEndurance;
		private float usedEnduranceCooldown;
		private Vector3 cameraDirection;

		//Attack
		public Weapon PlayerWeapon { get => equipedWeapon; set => UpdateWeapon(value); }
		[SerializeField] private Transform rightHand;
		[SerializeField] private Weapon equipedWeapon;
		[HideInInspector] public Attack activeAttack;
		private WeaponController heldWeapon;
		public enum AttackState : short { NormalStand = 0, NormalSprint = 1, NormalJump = 2, NormalSprintJump = 3, HeavyStand = 4, HeavySprint = 5, HeavyJump = 6, HeavySprintJump = 7 }
		private AttackState lastAttackType;
		private bool isCurrentlyAttacking;
		private bool canceledAnimation;

		private float? comboTimer;
		private int comboIndex;

		private Dictionary<AttackAnimation, (float, float)> animationDelayLookup = new Dictionary<AttackAnimation, (float, float)>()
		{
			{ AttackAnimation.Stab, (0.533f,0) },
			{ AttackAnimation.ToLeftSlash, (0.533f,0) },
			{ AttackAnimation.TopDownSlash, (0.533f,0) },
			{ AttackAnimation.ToRightSlash, (0.533f, 0.22f) },
			{ AttackAnimation.Uppercut, (0.533f,0) },
		};

		protected override void EntitieStart()
		{
			instance = this;
			UpdateWeapon(equipedWeapon);
			SetupEndurance();
		}

		private void UpdateWeapon(Weapon newWeapon)
		{
			equipedWeapon = newWeapon;
			if (heldWeapon)
				Destroy(heldWeapon);

			if(equipedWeapon != null)
			{
				heldWeapon = Instantiate(equipedWeapon.weaponPrefab, rightHand);
				heldWeapon.Setup();
			}
		}

		protected override void EntitieUpdate()
		{
			EnduranceRegen();
			CameraControl();
			Movement();
			AttackControl();
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
				lockedEndurance += Time.deltaTime * PlayerSettings.EnduranceRegen * (curStates.IsInCombat ? PlayerSettings.EnduranceRegenInCombatFactor : 1);

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
				enduranceContainers[activeEnduranceContainerIndex] += Time.deltaTime * PlayerSettings.EnduranceRegen * (curStates.IsInCombat ? PlayerSettings.EnduranceRegenInCombatFactor : 1);
				if (enduranceContainers[activeEnduranceContainerIndex] > PlayerSettings.EndurancePerBar)
					enduranceContainers[activeEnduranceContainerIndex] = PlayerSettings.EndurancePerBar;
			}
		}

		private void Movement()
		{
			JumpControl();
			Walk();
		}
		private void JumpControl()
		{
			if (PlayerControlls.Buttons.Jump.Down && curStates.IsGrounded)
			{
				float cost = PlayerSettings.JumpEnduranceCost;
				if ((int)CanAffordEnduranceCost(cost) > 0) //0 == Not available; 1/2 == available
				{
					Jump();
					UseEndurance(cost);
				}
			}
		}
		private void Walk()
		{
			//1.0.: Where is the player Pointing the Joystick at
			Vector3 controllDirection = PlayerControlls.GetPlayerMove();

			bool moving = controllDirection != Vector3.zero;
			curStates.IsMoving = moving;
			//1.1.: If there is no input, stop here
			if (!moving)
			{
				float curAcceleration = UpdateAcceleration();
				if (curAcceleration == 0)
				{
					animationControl.SetBool("Walking", false);
					animationControl.SetBool("Running", false);
				}
				return;
			}

			//Check if the player intends to run ans is able to
			bool running = curStates.IsRunning;
			if(running)
			{
				float cost = PlayerSettings.RunEnduranceCost * Time.deltaTime;
				if ((int)CanAffordEnduranceCost(cost) == 0) //0 == Not available; 1/2 == available
					running = curStates.IsRunning = false;
				else
					UseEndurance(cost);
			}
			else if (PlayerControlls.Buttons.Run.Active)
			{
				curStates.IsRunning = true;
				running = true;
			}

			//Set animations
			animationControl.SetBool("Walking", !running);
			animationControl.SetBool("Running", running);

			//1.2.: How fast does the player actually want to walk? (1,1) Would be greater then 1 * MoveSpeed => we map it back to 1
			float intendedMoveSpeed = Mathf.Min(1, controllDirection.magnitude) * (running ? PlayerSettings.RunSpeedMultiplicator : 1);

			//2.: Rotate the controlled direction based on where the camera is facing
			cameraDirection = new Vector3(Mathf.Sin((-PlayerCameraController.CurRotation.x) * Mathf.Deg2Rad), 0, Mathf.Cos((-PlayerCameraController.CurRotation.x) * Mathf.Deg2Rad));
			float newX = (controllDirection.x * cameraDirection.z) - (controllDirection.z * cameraDirection.x);
			float newZ = (controllDirection.z * cameraDirection.z) + (controllDirection.x * cameraDirection.x);
			controllDirection.x = newX;
			controllDirection.z = newZ;

			float turnFactor = TurnTo(controllDirection);
			UpdateAcceleration(intendedMoveSpeed * turnFactor);
		}

		private void CameraControl()
		{
			Vector2 newMoveDistance = PlayerControlls.CameraMove();
			newMoveDistance = new Vector2(newMoveDistance.x * selfSettings.CameraRotationPower.x, newMoveDistance.y * selfSettings.CameraRotationPower.y) * Time.deltaTime;
			PlayerCameraController.ToRotate += newMoveDistance;
		}

		private void AttackControl()
		{
			if (comboTimer.HasValue)
			{
				comboTimer -= Time.deltaTime;
				if (comboTimer.Value < 0)
				{
					comboTimer = null;
					comboIndex = 0;
				}
			}

			if (equipedWeapon == null || isCurrentlyAttacking || (!PlayerControlls.Buttons.Attack.Down && !PlayerControlls.Buttons.HeavyAttack.Down))
				return;

			int state = 0;
			state += PlayerControlls.Buttons.Attack.Down ? 0 : 4; //If we are here either heavy attack or normal attack was pressed
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

				if (chargeDelay == 0)
					heldWeapon.Active = true;

				while (animTime > 0 || activeAttack.animationInfo.haltAnimationTillCancel)
				{
					yield return new WaitForEndOfFrame();
					animTime -= Time.deltaTime;

					if(chargeDelay > 0)
					{
						chargeDelay -= Time.deltaTime;

						if (chargeDelay <= 0)
							heldWeapon.Active = true;
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
	}
}