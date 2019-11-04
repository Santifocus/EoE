using EoE.Controlls;
using EoE.Information;
using EoE.Utils;
using EoE.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EoE.Events;

namespace EoE.Entities
{
	public class Player : Entitie
	{
		#region Fields
		//Constants
		private const float ATTACK_INTERACT_COOLDOWN = 0.75f;
		//Inspector variables
		[Space(10)]
		[SerializeField] private PlayerSettings selfSettings = default;
		[SerializeField] private Transform rightHand = default;
		[SerializeField] private Weapon equipedWeapon = default;
		[SerializeField] private PlayerBuffDisplay buffDisplay = default;
		[SerializeField] private TextMeshProUGUI soulCount = default;

		public TMPro.TextMeshProUGUI debugText = default;

		//Endurance
		[HideInInspector] public bool recentlyUsedEndurance;
		[HideInInspector] public List<float> enduranceContainers;
		[HideInInspector] public int totalEnduranceContainers;
		[HideInInspector] public int activeEnduranceContainerIndex;
		[HideInInspector] public float lockedEndurance;
		private float usedEnduranceCooldown;

		//Dodge
		[SerializeField] private Transform modelTransform;
		private float dodgeCooldown;
		private bool currentlyDodging;

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
		public static Entitie TargetedEntitie;
		#region Leveling
		public static Buff LevelingBaseBuff;
		public static Buff LevelingSkillPointsBuff;
		public static PlayerBuffDisplay BuffDisplay => instance.buffDisplay;
		public static int PlayerLevel { get; private set; }
		public static int TotalSoulCount { get; private set; }
		public static int RequiredSoulsForLevel { get; private set; }
		public static int AvailableSkillPoints;
		#endregion

		//Other
		private float attackInteractCooldown;

		#endregion
		#region Basic Monobehaivior
		protected override void EntitieStart()
		{
			Alive = true;
			instance = this;

			SetupLevelingControl();

			ChangeWeapon(equipedWeapon);
			SetupEndurance();
		}
		private void SetupLevelingControl()
		{
			LevelingBaseBuff = new Buff()
			{
				Name = "LevelingBase",
				Quality = BuffType.Positive,
				Icon = null,
				BuffTime = 0,
				Permanent = true,
				DOTs = new DOT[0]
			};
			LevelingSkillPointsBuff = new Buff()
			{
				Name = "LevelingSkillPoints",
				Quality = BuffType.Positive,
				Icon = null,
				BuffTime = 0,
				Permanent = true,
				DOTs = new DOT[0]
			};

			//Health, Mana, PhysicalDamage, MagicalDamage, Defense
			int incremtingStats = 5;

			LevelingBaseBuff.Effects = new Effect[incremtingStats];
			LevelingSkillPointsBuff.Effects = new Effect[incremtingStats];
			for (int i = 0; i < incremtingStats; i++) 
			{
				LevelingBaseBuff.Effects[i] = LevelingSkillPointsBuff.Effects[i] = 
					new Effect
					{
						Amount = 0,
						Percent = false,
						targetStat = (TargetStat)i
					};
			}

			AddBuff(LevelingBaseBuff, this);
			AddBuff(LevelingSkillPointsBuff, this);

			PlayerLevel = 0;
			RequiredSoulsForLevel = PlayerSettings.LevelSettings.curve.GetRequiredSouls(PlayerLevel);
			TotalSoulCount = 0;
			AvailableSkillPoints = 0;
			//Safest way to ensure everyhting is correct is by adding 0 Souls to our current count
			AddSouls(0);
		}
		protected override void EntitieUpdate()
		{
			EnduranceRegen();
			CameraControl();
			MovementControl();
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
			DodgeControl();
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
			Vector2 inputDirection = InputController.PlayerMove;

			bool moving = inputDirection != Vector2.zero;
			curStates.IsMoving = moving;

			if (TargetedEntitie)
				intendedRotation = PlayerCameraController.TargetRotation.x;

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
			//Check how fast the player wants to accelerate based on how far the movestick is moved
			float intendedControl = Mathf.Min(1, inputDirection.magnitude);
			intendedAcceleration = intendedControl * (running ? PlayerSettings.RunSpeedMultiplicator : 1);

			//Rotate the input direction base on the camera direction
			float cosDir = Mathf.Cos((-PlayerCameraController.CurRotation.x) * Mathf.Deg2Rad);
			float sinDir = Mathf.Sin((-PlayerCameraController.CurRotation.x) * Mathf.Deg2Rad);
			float newX = (inputDirection.x * cosDir) - (inputDirection.y * sinDir);
			float newZ = (inputDirection.x * sinDir) + (inputDirection.y * cosDir);
			//Now set the controll direction to the rotated direction, normalize by dividing with the intended speed
			controllDirection = new Vector3(newX, 0, newZ) / intendedControl;

			if (!TargetedEntitie)
				intendedRotation = -(Mathf.Atan2(controllDirection.Value.z, controllDirection.Value.x) * Mathf.Rad2Deg - 90);
		}
		private void DodgeControl()
		{
			if(dodgeCooldown > 0)
			{
				dodgeCooldown -= Time.deltaTime;
				return;
			}

			if(InputController.Dodge.Down && !currentlyDodging && CanAffordEnduranceCost(PlayerSettings.DodgeEnduranceCost) != GetEnduranceType.NotAvailable)
			{
				UseEndurance(PlayerSettings.DodgeEnduranceCost);
				StartCoroutine(DodgeCoroutine());
			}
		}
		private IEnumerator DodgeCoroutine()
		{
			currentlyDodging = true;
			float timer = 0;
			float targetAngle = intendedRotation;
			Vector2 controllDirection = InputController.PlayerMove;
			if (controllDirection != Vector2.zero)
			{
				targetAngle = Mathf.Atan2(controllDirection.y, controllDirection.x) * Mathf.Rad2Deg + 90 + PlayerCameraController.CurRotation.x;
			}

			Vector3 newForce = new Vector3(Mathf.Sin(targetAngle * Mathf.Deg2Rad), 0, Mathf.Cos(targetAngle * Mathf.Deg2Rad)) * PlayerSettings.DodgePower * curWalkSpeed;
			newForce.y = PlayerSettings.DodgeUpForce;

			entitieForceController.ApplyForce(newForce, 1/PlayerSettings.DodgeDuration);

			while(timer < PlayerSettings.DodgeDuration)
			{
				yield return new WaitForFixedUpdate();
				timer += Time.fixedDeltaTime;
			}
			dodgeCooldown = PlayerSettings.DodgeCooldown;
			currentlyDodging = false;
		}
		private void CameraControl()
		{
			if (!TargetedEntitie)
			{
				if (InputController.ResetCamera.Active || InputController.ResetCamera.Down)
				{
					PlayerCameraController.Instance.LookAtDirection(transform.forward);
					PlayerCameraController.TargetRotation = new Vector2(PlayerCameraController.TargetRotation.x, PlayerSettings.CameraVerticalAngleClamps.y / 2);
					return;
				}

				Vector2 newMoveDistance = InputController.CameraMove;
				newMoveDistance = new Vector2(newMoveDistance.x * selfSettings.CameraRotationPower.x, newMoveDistance.y * selfSettings.CameraRotationPower.y) * Time.deltaTime;
				PlayerCameraController.TargetRotation += newMoveDistance;
			}
		}
		private void TargetEnemyControl()
		{
			if (InputController.Aim.Down)
			{
				List<(Entitie, float) > possibleTargets = new List<(Entitie, float)>();

				for(int i = 0; i < AllEntities.Count; i++)
				{
					if (AllEntities[i] is Player) //We ignore the player
						continue;

					float distance = (AllEntities[i].actuallWorldPosition - PlayerCameraController.PlayerCamera.transform.position).magnitude;
					if (distance > PlayerSettings.MaxEnemyTargetingDistance)
						continue;

					Vector3 direction = (AllEntities[i].actuallWorldPosition - PlayerCameraController.PlayerCamera.transform.position) / distance;
					float cosAngle = Vector3.Dot(PlayerCameraController.PlayerCamera.transform.forward, direction);

					//We use the difference distance multipied with the inversed cosinus angle to find out how far from the view dir the entitie is
					//Then we add an extra amount to prefer targets that are closer to the camera even if their distance to the viewdirection is the same
					float distanceToViewDir = (1 - cosAngle) * distance + distance * 0.025f;

					//Now find out where the entitie is on the screen
					Vector3 screenPos = PlayerCameraController.PlayerCamera.WorldToScreenPoint(AllEntities[i].actuallWorldPosition);
					Vector2 normScreenPos = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
					bool notOnScreen = normScreenPos.x < 0 || normScreenPos.x > 1 || normScreenPos.y < 0 || normScreenPos.y > 1;

					if (screenPos.z < 0 || notOnScreen) //Behind the camera or not on screen? => We bail out
						continue;

					possibleTargets.Add((AllEntities[i], distanceToViewDir));
				}

				//Sort by distance low to high
				possibleTargets.Sort((x, y) => x.Item2.CompareTo(y.Item2));

				TargetedEntitie = null;
				if (possibleTargets.Count > 0)
				{
					//Now we have a list if possible targets sorted by lowest to highest distance
					//We try to find a target that we can see
					//So we start with the lowest distance (index 0) and keep going until we find one and take that one as new target
					//If there is none, then the targetEntitie will stay null
					for (int i = 0; i < possibleTargets.Count; i++)
					{
						if (CheckIfCanSeeEntitie(possibleTargets[i].Item1))
						{
							TargetedEntitie = possibleTargets[i].Item1;
							//We found a target so we stop here
							break;
						}
					}
				}
			}

			if (InputController.Aim.Up)
				TargetedEntitie = null;
		}
		#endregion
		#region Physical Attack Control
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

			if (attackInteractCooldown > 0)
				attackInteractCooldown -= Time.deltaTime;

			bool normalAttack = InputController.Attack.Down;
			if(normalAttack && Interactable.MarkedInteractable && !(attackInteractCooldown > 0))
			{
				if (Interactable.MarkedInteractable.TryInteract())
					return;
			}

			if (equipedWeapon == null || isCurrentlyAttacking || (!normalAttack && !InputController.HeavyAttack.Down))
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

				attackInteractCooldown = ATTACK_INTERACT_COOLDOWN;
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
					impactForce = new Vector3(velocity.x, impactForce.y, velocity.z);
				}
				else
				{
					impactForce = new Vector3(velocity.x, 0, velocity.z);
					body.velocity = new Vector3(body.velocity.x, velocity.y, body.velocity.z);
				}
			}
			else
			{
				curAcceleration = 0;
				curMoveForce = Vector3.zero;

				if (effect.ignoreVerticalVelocity)
				{
					impactForce += new Vector3(velocity.x, 0, velocity.z);
				}
				else
				{
					impactForce += new Vector3(velocity.x, 0, velocity.z);
					body.velocity += new Vector3(0, velocity.y, 0);
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
		public void AddSouls(int amount)
		{
			TotalSoulCount += amount;
			while(TotalSoulCount >= RequiredSoulsForLevel)
			{
				PlayerLevel++;
				AvailableSkillPoints++;
				RequiredSoulsForLevel = PlayerSettings.LevelSettings.curve.GetRequiredSouls(PlayerLevel);

				//Update buff
				for(int i = 0; i < LevelingBaseBuff.Effects.Length; i++)
				{
					LevelingBaseBuff.Effects[i].Amount += PlayerSettings.LevelSettings.baseIncrementPerLevel[i];
				}
				RecalculateBuffs();

				//Inform all script that need the information that the player leveled
				EventManager.PlayerLevelupInvoke();
			}

			soulCount.text = TotalSoulCount + " / " + RequiredSoulsForLevel;
		}
		#endregion
	}
}