using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Combatery;
using EoE.Controlls;
using EoE.Events;
using EoE.Information;
using EoE.UI;
using EoE.Sounds;

namespace EoE.Entities
{
	public class Player : Entity
	{
		#region Fields
		//Constants
		private const float MIN_WALK_ACCELERATION = 0.45f;
		private const float NON_TURNING_THRESHOLD = 30;
		private const float LERP_TURNING_AREA = 0.5f;
		private const float JUMP_COOLDOWN = 0.2f;
		private const float FALLDAMAGE_COOLDOWN = 0.2f;
		private const float IS_FALLING_THRESHOLD = -1;
		private const float LANDED_VELOCITY_THRESHOLD = 0.15f;
		private const float SWITCH_TARGET_COOLDOWN = 0.25f;
		private const int SELECTABLE_ITEMS_AMOUNT = 4;

		//Inspector variables
		[Space(10)]
		public CharacterController charController = default;
		public Transform weaponHoldPoint = default;
		public Animator animationControl = default;

		[SerializeField] private PlayerSettings playerSettings = default;
		[SerializeField] private int combatMusicIndex = 1;
		public PlayerBuffDisplay buffDisplay = default;

		[SerializeField] private Transform head = default;
		[SerializeField] private Transform torso = default;

		//Endurance
		public float curEndurance { get; set; }
		public float curMaxEndurance { get; set; }
		private float usedEnduranceCooldown;

		//Dashing
		public Transform modelTransform = default;
		private float dashCooldown;
		private bool currentlyDashing;

		//Shielding
		private Shield playerShield;

		//Velocity
		private float jumpCooldown;
		public Vector3 curMoveVelocity => controllDirection * curWalkSpeed * (curStates.Running ? SelfSettings.RunSpeedMultiplicator : 1) * curAcceleration;
		private Vector3 controllDirection;
		private float intendedAcceleration;
		private float curAcceleration;
		private bool lastOnGroundState = true;
		private float lastFallVelocity;

		//Feedback
		private Vector2 curModelTilt;
		private Vector2 curSpringLerpAcceleration;
		private Vector2 curAnimationDirection;

		private FXInstance[] HealthBelowThresholdBoundEffects;
		private FXInstance[] PlayerWalkingBoundEffects;
		private FXInstance[] PlayerRunningBoundEffects;
		private FXInstance[] CombatBoundEffects;

		private MusicInstance combatMusic;

		//Targeting
		public Entity TargetedEntitie { get; private set; }
		private float switchTargetTimer;

		//Getter Helpers
		public static bool Existant => Instance && Instance.Alive;
		public float JumpVelocity { get; private set; }
		public float FallDamageCooldown { get; set; }
		public float VerticalVelocity { get; private set; }
		public float TotalVerticalVelocity => JumpVelocity * PlayerSettings.JumpImpulsePower + VerticalVelocity;
		public override Vector3 CurVelocity => base.CurVelocity + new Vector3(0, TotalVerticalVelocity, 0);
		public static Player Instance { get; private set; }
		public override EntitySettings SelfSettings => playerSettings;
		public static PlayerSettings PlayerSettings => Instance.playerSettings;
		public int TotalExperience { get; private set; }
		public int CurrentCurrencyAmount { get; private set; }
		#region Items
		public Inventory Inventory;

		//Weapon
		public InventoryItem EquipedWeapon;

		//Use Items
		public InventoryItem[] SelectableItems;
		public int selectedItemIndex { get; private set; }
		public InventoryItem EquipedItem => SelectableItems[selectedItemIndex];

		//Spell Items
		public InventoryItem[] SelectableActivationCompoundItems;
		public int selectedSpellItemIndex { get; private set; }
		public InventoryItem EquipedSpell => SelectableActivationCompoundItems[selectedSpellItemIndex];
		#endregion
		#region Leveling
		public Buff LevelingPointsBuff { get; private set; }
		public int RequiredExperienceForLevel { get; private set; }
		public int AvailableSkillPoints { get; set; }
		public int AvailableAtributePoints { get; set; }
		#endregion

		#endregion
		#region Basic Monobehaivior
		protected override void Start()
		{
			Instance = this;
			base.Start();
		}
		protected override void EntitieStart()
		{
			SetupInventory();
			combatMusic = new MusicInstance(0, 5, combatMusicIndex);
		}
		protected override void EntitieUpdate()
		{
			if (ContextControl())
				return;

			if (GameController.GameIsPaused)
			{
				if (TargetedEntitie)
					TargetedEntitie = null;
				return;
			}

			MovementControl();
			TargetEnemyControl();
			ItemControl();
			ShieldControl();

			animationControl.SetBool("InCombat", curStates.Fighting);
			if (Input.GetKeyDown(KeyCode.K))
				ChangeHealth(new ChangeInfo(this, CauseType.Magic, TargetStat.Health, 10000));
		}
		protected override void EntitieFixedUpdate()
		{
			CheckForLanding();
			ApplyForces();
			CheckForFalling();
			UpdateAcceleration();

			if (IsStunned || IsTurnStopped)
			{
				curStates.Turning = false;
			}
			else
			{
				TurnControl();
			}
		}
		private void LateUpdate()
		{
			if (!Alive)
				return;
			ProceduralAnimationControl();
		}
		#endregion
		#region Setups
		protected override void ResetStats()
		{
			base.ResetStats();
			curMaxEndurance = playerSettings.Endurance;
		}
		protected override void ResetStatValues()
		{
			base.ResetStatValues();
			curEndurance = curMaxEndurance;
			playerShield = Shield.CreateShield(playerSettings.ShieldSettings, this);
		}
		protected override void LevelSetup()
		{
			base.LevelSetup();
			LevelingPointsBuff = ScriptableObject.CreateInstance<Buff>();
			{
				LevelingPointsBuff.Name = "LevelingSkillPoints";
				LevelingPointsBuff.Quality = BuffType.Positive;
				LevelingPointsBuff.Icon = null;
				LevelingPointsBuff.FinishConditions = new FinishConditions()
				{
					OnTimeout = false
				};
				LevelingPointsBuff.DOTs = new DOT[0];
			}

			//Health, Mana, Endurance, PhysicalDamage, MagicalDamage, Defense
			int incremtingStats = System.Enum.GetNames(typeof(TargetBaseStat)).Length;
			LevelingPointsBuff.Effects = new Effect[incremtingStats];
			for (int i = 0; i < incremtingStats; i++)
			{
				LevelingPointsBuff.Effects[i] =
					new Effect
					{
						Amount = 0,
						Percent = false,
						TargetBaseStat = (TargetBaseStat)i
					};
			}

			Buff.ApplyBuff(LevelingPointsBuff, this, this);
			RequiredExperienceForLevel = PlayerSettings.LevelSettings.curve.GetRequiredExperience(EntitieLevel);
		}
		private void SetupInventory()
		{
			Inventory = new Inventory(PlayerSettings.InventorySize);
			Inventory.InventoryChanged += UpdateEquipedItems;

			EquipedWeapon = null;
			EquipedArmor = null;

			selectedItemIndex = 0;
			SelectableItems = new InventoryItem[SELECTABLE_ITEMS_AMOUNT];
			selectedSpellItemIndex = 0;
			SelectableActivationCompoundItems = new InventoryItem[SELECTABLE_ITEMS_AMOUNT];

			for (int i = 0; i < PlayerSettings.StartItems.Length; i++)
			{
				if (!PlayerSettings.StartItems[i].Item)
					continue;
				//Add it to the inventory
				List<int> targetSlots = Inventory.AddItem(new InventoryItem(PlayerSettings.StartItems[i].Item, PlayerSettings.StartItems[i].ItemCount));

				//Force equipp
				if (PlayerSettings.StartItems[i].ForceEquip)
				{
					InventoryItem targetItem = Inventory[targetSlots[targetSlots.Count - 1]];
					targetItem.isEquiped = true;
					//Find out what slot it belongs to, if it is a spell / normal item we try
					//to put it in a open slot, if all slots are filled we put it in the first
					if (PlayerSettings.StartItems[i].Item is WeaponItem)
					{
						EquipedWeapon = targetItem;
						targetItem.isEquiped = true;
						targetItem.data.Equip(targetItem, this);
					}
					else if (PlayerSettings.StartItems[i].Item is ArmorItem)
					{
						EquipedArmor = targetItem;
						targetItem.isEquiped = true;
						targetItem.data.Equip(targetItem, this);
					}
					else if (PlayerSettings.StartItems[i].Item is ActivationCompoundItem)
					{
						bool added = false;

						//Try to find a slot that is null and put the item there
						for (int j = 0; j < SelectableActivationCompoundItems.Length; j++)
						{
							if (SelectableActivationCompoundItems[j] == null)
							{
								SelectableActivationCompoundItems[j] = targetItem;
								SelectableActivationCompoundItems[j].isEquiped = true;
								added = true;
								if (j == 0)
								{
									SelectableActivationCompoundItems[j].data.Equip(targetItem, this);
								}
								break;
							}
						}
						//couldnt find a null slot, put it in the first one, (just a fallback)
						if (!added && SelectableActivationCompoundItems.Length > 0)
						{
							SelectableActivationCompoundItems[0] = targetItem;
							SelectableActivationCompoundItems[0].isEquiped = true;
							SelectableActivationCompoundItems[0].data.Equip(targetItem, this);
						}
					}
					else
					{
						bool added = false;

						//Try to find a slot that is null and put the item there
						for (int j = 0; j < SelectableItems.Length; j++)
						{
							if (SelectableItems[j] == null)
							{
								SelectableItems[j] = targetItem;
								SelectableItems[j].isEquiped = true;
								added = true;
								if (j == 0)
								{
									SelectableItems[j].data.Equip(targetItem, this);
								}
								break;
							}
						}
						//couldnt find a null slot, put it in the first one, (just a fallback)
						if (!added && SelectableItems.Length > 0)
						{
							SelectableItems[0] = targetItem;
							SelectableItems[0].isEquiped = true;
							SelectableItems[0].data.Equip(targetItem, this);
						}
					}
				}
			}
		}
		#endregion
		#region Movement
		private void MovementControl()
		{
			CameraControl();
			PlayerFeedbackControl();

			if (IsStunned || IsMovementStopped)
			{
				curStates.Moving = curStates.Running = false;
				if (IsStunned)
					curAcceleration = 0;
				return;
			}

			PlayerMoveControl();
			DashControl();
			JumpControl();
		}
		private void CameraControl()
		{
			if (!TargetedEntitie)
			{
				if (InputController.ResetCamera.Down)
				{
					PlayerCameraController.Instance.LookInDirection(transform.forward);
					PlayerCameraController.TargetRotation = new Vector2(PlayerCameraController.TargetRotation.x, PlayerSettings.CameraVerticalAngleClamps.y / 2);
					return;
				}

				Vector2 newMoveDistance = InputController.CameraMove;
				newMoveDistance = new Vector2(newMoveDistance.x * playerSettings.CameraRotationPower.x, newMoveDistance.y * playerSettings.CameraRotationPower.y) * Time.deltaTime;
				PlayerCameraController.TargetRotation += newMoveDistance;
			}
		}
		private void PlayerFeedbackControl()
		{
			bool turning = curStates.Turning;
			bool moving = curStates.Moving && curAcceleration >= MIN_WALK_ACCELERATION && intendedAcceleration >= MIN_WALK_ACCELERATION;
			bool running = curStates.Running;

			//If the Player doesnt move intentionally but is in run mode, then stop the run mode
			if (running && !moving)
				curStates.Running = false;

			//Set all the animation states
			animationControl.SetBool("Walking", !turning && curAcceleration > 0);

			//Model tilt
			Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
			Vector2 right = new Vector2(transform.right.x, transform.right.z);
			Vector3 moveVelocity = curMoveVelocity;

			float velocity = new Vector2(moveVelocity.x, moveVelocity.z).magnitude;
			float maxVelocity = curWalkSpeed * SelfSettings.RunSpeedMultiplicator;
			Vector2 velocityDirection = (velocity > 0) ? (new Vector2(moveVelocity.x, moveVelocity.z) / velocity) : (new Vector2(transform.forward.x, transform.forward.z));
			float normalizedMoveVelocity = (velocity / maxVelocity);

			float targetTiltAngle = normalizedMoveVelocity * PlayerSettings.MaxModelTilt;
			Vector2 targetTilt = velocityDirection * targetTiltAngle;
			targetTilt = targetTilt.x * right * -1 + targetTilt.y * forward;

			curModelTilt = new Vector2(Utils.SpringLerp(curModelTilt.x, targetTilt.x, ref curSpringLerpAcceleration.x, PlayerSettings.SideTurnLerpSpringStiffness, PlayerSettings.SideTurnSpringLerpSpeed * Time.deltaTime),
										Utils.SpringLerp(curModelTilt.y, targetTilt.y, ref curSpringLerpAcceleration.y, PlayerSettings.SideTurnLerpSpringStiffness, PlayerSettings.SideTurnSpringLerpSpeed * Time.deltaTime)
										);
			modelTransform.localEulerAngles = new Vector3(curModelTilt.y, 0, curModelTilt.x);

			//Move direction
			float forwardValue = Vector2.Dot(velocityDirection, forward);
			int xMultiplier = Vector2.Dot(velocityDirection, right) > 0 ? 1 : -1;

			curAnimationDirection = Vector2.Lerp(curAnimationDirection, new Vector2(forwardValue * normalizedMoveVelocity, (1 - Mathf.Abs(forwardValue)) * xMultiplier * normalizedMoveVelocity), Time.deltaTime * PlayerSettings.WalkAnimationLerpSpeed);

			animationControl.SetFloat("ZMove", curAnimationDirection.x);
			animationControl.SetFloat("XMove", curAnimationDirection.y);
			animationControl.SetFloat("CurWalkSpeed", normalizedMoveVelocity);

			///Control FX
			//Health critical effects
			bool curBelowHealthThreshold = HealthBelowThresholdBoundEffects != null;
			bool newBelowHealthThresholdState = (curHealth / curMaxHealth) < PlayerSettings.EffectsHealthThreshold;
			if (newBelowHealthThresholdState != curBelowHealthThreshold)
			{
				//Health fell below threshold
				if (newBelowHealthThresholdState)
				{
					FXManager.ExecuteFX(PlayerSettings.EffectsWhileHealthBelowThreshold, transform, true, out HealthBelowThresholdBoundEffects);
				}
				else //Health went above threshold
				{
					FXManager.FinishFX(ref HealthBelowThresholdBoundEffects);
				}
			}

			//Walk effects
			bool curWalkingEffectsOn = PlayerWalkingBoundEffects != null;
			bool newWalkingEffectsOn = (!turning && !running && moving) && (charController.isGrounded);
			if (curWalkingEffectsOn != newWalkingEffectsOn)
			{
				//Player started walking or stopped running and kept walking
				if (newWalkingEffectsOn)
				{
					FXManager.ExecuteFX(PlayerSettings.EffectsWhileWalk, transform, true, out PlayerWalkingBoundEffects);
				}
				else //Player stopped walking or started running
				{
					FXManager.FinishFX(ref PlayerWalkingBoundEffects);
				}
			}

			//Run effects
			bool curRunningEffectsOn = PlayerRunningBoundEffects != null;
			bool newRunningEffectsOn = (running) && (charController.isGrounded);
			if (curRunningEffectsOn != newRunningEffectsOn)
			{
				//Player started running
				if (newRunningEffectsOn)
				{
					FXManager.ExecuteFX(PlayerSettings.EffectsWhileRun, transform, true, out PlayerRunningBoundEffects);
				}
				else //Player stopped running
				{
					FXManager.FinishFX(ref PlayerRunningBoundEffects);
				}
			}
		}
		private float curTosoTurn;
		private float bodyLookSpringAcceleration;

		private Vector2 curHeadTurn;
		private float headHLookSpringAcceleration;
		private float headVLookSpringAcceleration;
		private void ProceduralAnimationControl()
		{
			return;
			Vector3 targetLookDirection;
			if (TargetedEntitie)
			{
				targetLookDirection = (TargetedEntitie.actuallWorldPosition - head.position).normalized;
			}
			else if(curAcceleration > MIN_WALK_ACCELERATION)
			{
				targetLookDirection = transform.forward;
			}
			else
			{
				targetLookDirection = PlayerCameraController.PlayerCamera.transform.forward;
			}


			float hAngle = -(Mathf.Atan2(targetLookDirection.z, targetLookDirection.x) * Mathf.Rad2Deg + 90);
			float vAngle = Mathf.Asin(targetLookDirection.y) * Mathf.Rad2Deg;

			float targetTorsoAngle = Mathf.Clamp(hAngle * playerSettings.BodyTurnWeight, -playerSettings.BodyTurnHorizontalClamp, playerSettings.BodyTurnHorizontalClamp);
			curTosoTurn = Utils.SpringLerp(curTosoTurn, targetTorsoAngle, ref bodyLookSpringAcceleration, playerSettings.LookLerpSpringStiffness, Time.deltaTime * playerSettings.LookLerpSpeed);

			torso.localEulerAngles = new Vector3(torso.localEulerAngles.x, curTosoTurn, torso.localEulerAngles.z);

			Vector2 targetHeadAngle = new Vector2(	Mathf.Clamp(hAngle - curTosoTurn, -playerSettings.HeadLookAngleClamps.z + curTosoTurn, playerSettings.HeadLookAngleClamps.x + curTosoTurn),
													Mathf.Clamp(vAngle, -playerSettings.HeadLookAngleClamps.w, playerSettings.HeadLookAngleClamps.y));

			curHeadTurn = new Vector2(	Utils.SpringLerp(curHeadTurn.x, targetHeadAngle.x, ref headHLookSpringAcceleration, playerSettings.LookLerpSpringStiffness, Time.deltaTime * playerSettings.LookLerpSpeed),
										Utils.SpringLerp(curHeadTurn.y, targetHeadAngle.y, ref headVLookSpringAcceleration, playerSettings.LookLerpSpringStiffness, Time.deltaTime * playerSettings.LookLerpSpeed));

			float vAngleSinPart = Mathf.Sin(curHeadTurn.x * Mathf.Deg2Rad);
			float vAngleCosPart = Mathf.Cos(curHeadTurn.x * Mathf.Deg2Rad);

			head.eulerAngles = new Vector3(curHeadTurn.y * vAngleSinPart, curHeadTurn.x, curHeadTurn.y * vAngleCosPart);
		}
		#region Walking
		private void TurnControl()
		{
			float turnAmount = Time.fixedDeltaTime * SelfSettings.TurnSpeed * (charController.isGrounded ? 1 : SelfSettings.InAirTurnSpeedMultiplier);
			float normalizedDif = Mathf.Abs(Mathf.DeltaAngle(curRotation, intendedRotation)) / NON_TURNING_THRESHOLD;
			turnAmount *= Mathf.Min(normalizedDif * LERP_TURNING_AREA, 1);
			curRotation = Mathf.MoveTowardsAngle(curRotation, intendedRotation, turnAmount);

			curStates.Turning = normalizedDif > 1;

			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
														curRotation,
														transform.localEulerAngles.z);
		}
		private void PlayerMoveControl()
		{
			//Where is the player Pointing the Joystick at?
			Vector2 inputDirection = InputController.PlayerMove;

			bool moving = inputDirection != Vector2.zero;
			curStates.Moving = moving;

			//Is the player not trying to move? Then stop here
			if (!moving)
			{
				controllDirection = Vector3.zero;
				return;
			}

			//Check if the player intends to run and is able to
			bool running = curStates.Running;
			if (running)
			{
				float runCost = PlayerSettings.RunEnduranceCost * Time.deltaTime;

				if (curEndurance >= runCost)
				{
					ChangeEndurance(new ChangeInfo(this, CauseType.Magic, TargetStat.Endurance, runCost));
				}
				else
				{
					running = curStates.Running = false;
				}
			}
			else if (InputController.Run.Down)
			{
				float runCost = PlayerSettings.RunEnduranceCost * Time.deltaTime;

				if (curEndurance >= runCost)
				{
					ChangeEndurance(new ChangeInfo(this, CauseType.Magic, TargetStat.Endurance, runCost));
					running = curStates.Running = true;
				}
			}
			//Check how fast the player wants to accelerate based on how far the movestick is moved
			float intendedControl = Mathf.Min(1, inputDirection.magnitude);
			intendedAcceleration = intendedControl * (running ? PlayerSettings.RunSpeedMultiplicator : 1);

			//Make the movement relative to the camera
			Vector3 camForward = PlayerCameraController.Instance.transform.forward;
			camForward.y = 0;
			camForward = camForward.normalized;

			Vector3 camRight = PlayerCameraController.Instance.transform.right;
			camRight.y = 0;
			camRight = camRight.normalized;

			controllDirection = inputDirection.y * camForward + inputDirection.x * camRight;

			if ((!TargetedEntitie) && (intendedControl > MIN_WALK_ACCELERATION * 0.95f))
				intendedRotation = -(Mathf.Atan2(controllDirection.z, controllDirection.x) * Mathf.Rad2Deg - 90);
		}
		#endregion
		#region Jumping
		private void JumpControl()
		{
			bool jumpPressed = InputController.Jump.Down;
			if (jumpPressed && Interactable.MarkedInteractable)
			{
				if (Interactable.MarkedInteractable.TryInteract())
					return;
			}

			bool disallowJump = false;
			if (jumpCooldown > 0)
			{
				jumpCooldown -= Time.deltaTime;
				disallowJump = true;
			}

			if(FallDamageCooldown > 0)
			{
				FallDamageCooldown -= Time.deltaTime;
				disallowJump = true;
			}

			if (!disallowJump && (jumpPressed && charController.isGrounded))
			{
				if (curEndurance >= PlayerSettings.JumpEnduranceCost)
				{
					Jump();
					ChangeEndurance(new ChangeInfo(this, CauseType.Magic, TargetStat.Endurance, PlayerSettings.JumpEnduranceCost));
				}
			}
		}
		protected void Jump()
		{
			float directionOffset = Vector3.Dot(controllDirection, transform.forward);
			float forwardMultiplier = Mathf.Lerp(PlayerSettings.JumpBackwardMultiplier, 1, (directionOffset + 1) / 2) * (directionOffset > 0 ? 1 : -1);

			JumpVelocity = Mathf.Min(PlayerSettings.JumpPower.y * curJumpPowerMultiplier, PlayerSettings.JumpPower.y * curJumpPowerMultiplier);
			Vector3 addedExtraForce = PlayerSettings.JumpPower.x * transform.right * curJumpPowerMultiplier + PlayerSettings.JumpPower.z * transform.forward * curJumpPowerMultiplier * (curStates.Running ? PlayerSettings.RunSpeedMultiplicator : 1) * forwardMultiplier;
			impactForce += new Vector2(addedExtraForce.x, addedExtraForce.z) * curAcceleration;
			VerticalVelocity = 0;

			FallDamageCooldown = FALLDAMAGE_COOLDOWN;
			animationControl.SetTrigger("Jump");

			FXManager.ExecuteFX(PlayerSettings.EffectsOnJump, transform, true);
		}
		private void CheckForFalling()
		{
			//Find out wether the entitie is falling or not
			bool playerWantsToFall = curStates.Falling || !InputController.Jump.Held;
			bool falling = !charController.isGrounded && (TotalVerticalVelocity < IS_FALLING_THRESHOLD || playerWantsToFall);

			//If so: we enable the falling animation and add extra velocity for a better looking fallcurve
			curStates.Falling = falling;
			animationControl.SetBool("Fall", falling);
			if (falling)
			{
				ApplyGravity(GameController.CurrentGameSettings.WhenFallingExtraGravity);
			}
		}
		private void CheckForLanding()
		{
			bool newOnGroundState = charController.isGrounded;
			float velDif = CurVelocity.y - lastFallVelocity;
			//Check if the grounded state changed
			if ((newOnGroundState != lastOnGroundState))
			{
				lastOnGroundState = newOnGroundState;
				//Did the ground state change from false to true?
				//Then if we reached a certain velocity we count this is landing
				if (newOnGroundState)
				{
					jumpCooldown = JUMP_COOLDOWN;
					if(velDif > LANDED_VELOCITY_THRESHOLD)
						Landed(velDif);
				}
			}
			lastFallVelocity = CurVelocity.y;
		}
		private void Landed(float velDif)
		{
			//Create FX
			if (velDif > PlayerSettings.PlayerLandingVelocityThreshold)
			{
				FXManager.ExecuteFX(PlayerSettings.EffectsOnPlayerLanding, transform, true);
			}

			float velocityMultiplier = 0;
			if (velDif > GameController.CurrentGameSettings.GroundHitVelocityLossMinThreshold)
			{
				if (velDif >= GameController.CurrentGameSettings.GroundHitVelocityLossMaxThreshold)
					velocityMultiplier = GameController.CurrentGameSettings.GroundHitVelocityLoss;
				else
					velocityMultiplier = (velDif - GameController.CurrentGameSettings.GroundHitVelocityLossMinThreshold) / (GameController.CurrentGameSettings.GroundHitVelocityLossMaxThreshold - GameController.CurrentGameSettings.GroundHitVelocityLossMinThreshold) * GameController.CurrentGameSettings.GroundHitVelocityLoss;
			}
			curAcceleration *= 1 - velocityMultiplier;
			impactForce *= 1 - velocityMultiplier;

			if (FallDamageCooldown <= 0)
			{
				//Check if there is any fall damage to give
				float damageAmount = GameController.CurrentGameSettings.FallDamageCurve.Evaluate(velDif);

				if (damageAmount > 0)
					ChangeHealth(new ChangeInfo(this, CauseType.Physical, TargetStat.Health, damageAmount));
			}
		}
		#endregion
		#endregion
		#region ForceControl
		private void UpdateAcceleration()
		{
			float clampedIntent = Mathf.Clamp01(intendedAcceleration);
			if (curStates.Moving && !curStates.Turning && intendedAcceleration >= MIN_WALK_ACCELERATION)
			{
				if (curAcceleration < clampedIntent)
				{
					if (SelfSettings.MoveAcceleration > clampedIntent)
						curAcceleration = Mathf.Min(clampedIntent, curAcceleration + intendedAcceleration * Time.fixedDeltaTime * SelfSettings.MoveAcceleration / SelfSettings.EntitieMass * (charController.isGrounded ? 1 : SelfSettings.InAirAccelerationMultiplier));
					else
						curAcceleration = clampedIntent;
				}
			}
			else //decelerate
			{
				if (curAcceleration > 0)
				{
					if (SelfSettings.NoMoveDeceleration > 0)
						curAcceleration = Mathf.Max(0, curAcceleration - Time.fixedDeltaTime * SelfSettings.NoMoveDeceleration / SelfSettings.EntitieMass * (charController.isGrounded ? 1 : SelfSettings.InAirAccelerationMultiplier));
					else
						curAcceleration = 0;
				}
			}
		}
		protected override void ApplyKnockback(Vector3 causedKnockback)
		{
			base.ApplyKnockback(causedKnockback);
			VerticalVelocity += causedKnockback.y;
		}
		private void ApplyForces()
		{
			Vector3 appliedForce = curMoveVelocity + CurVelocity;

			charController.Move(appliedForce * Time.fixedDeltaTime);
			animationControl.SetBool("OnGround", charController.isGrounded);
			ApplyGravity();
		}
		private void ApplyGravity(float scale = 1)
		{
			float lowerFallThreshold = Physics.gravity.y / 2;
			float force = scale * Physics.gravity.y * Time.fixedDeltaTime;

			if (!charController.isGrounded || TotalVerticalVelocity > 0)
			{
				if (JumpVelocity > 0)
				{
					JumpVelocity += force * PlayerSettings.JumpImpulsePower;
					if (JumpVelocity < 0)
					{
						VerticalVelocity += JumpVelocity;
						JumpVelocity = 0;
					}
				}
				else
				{
					VerticalVelocity += force;
					JumpVelocity = 0;
				}
			}
			else
			{
				JumpVelocity = 0;
				VerticalVelocity = lowerFallThreshold;
			}
		}
		#endregion
		#region DashControl
		private void DashControl()
		{
			if (dashCooldown > 0)
			{
				dashCooldown -= Time.deltaTime;
				return;
			}

			if (InputController.Dodge.Down && !currentlyDashing && curEndurance >= PlayerSettings.DashEnduranceCost)
			{
				EventManager.PlayerDashInvoke();
				ChangeEndurance(new ChangeInfo(this, CauseType.Magic, TargetStat.Endurance, PlayerSettings.DashEnduranceCost));
				StartCoroutine(DashCoroutine());
			}
		}
		private IEnumerator DashCoroutine()
		{
			currentlyDashing = true;
			float timer = 0;
			float targetAngle = intendedRotation;
			if (InputController.PlayerMove != Vector2.zero)
			{
				targetAngle = -(Mathf.Atan2(controllDirection.z, controllDirection.x) * Mathf.Rad2Deg - 90);
			}

			//FX
			FXManager.ExecuteFX(PlayerSettings.EffectsOnPlayerDash, transform, true, out FXInstance[] dashEffects);

			Vector3 newForce = new Vector3(Mathf.Sin(targetAngle * Mathf.Deg2Rad), 0, Mathf.Cos(targetAngle * Mathf.Deg2Rad)) * PlayerSettings.DashPower * curWalkSpeed;

			entitieForceController.ApplyForce(newForce, 1 / PlayerSettings.DashDuration);

			//Create a copy of the player model
			Material dashMaterialInstance = Instantiate(PlayerSettings.DashModelMaterial);
			Transform modelCopy = Instantiate(modelTransform, Storage.ParticleStorage);
			Transform weaponCopy = null;
			if (WeaponController.Instance)
				weaponCopy = WeaponController.Instance.CloneModel().transform;

			modelCopy.transform.position = modelTransform.position;
			modelCopy.transform.localScale = modelTransform.lossyScale;
			modelCopy.transform.rotation = modelTransform.rotation;

			Color baseColor = dashMaterialInstance.color;
			Color alphaPart = new Color(0, 0, 0, dashMaterialInstance.color.a);
			baseColor.a = 0;

			ApplyMaterialToAllChildren(modelCopy);
			if (weaponCopy)
				ApplyMaterialToAllChildren(weaponCopy);

			Invincibilities++;
			while (timer < PlayerSettings.DashModelExistTime)
			{
				yield return new WaitForFixedUpdate();
				timer += Time.fixedDeltaTime;
				dashMaterialInstance.color = baseColor + alphaPart * (1 - timer / PlayerSettings.DashModelExistTime);
			}
			dashCooldown = PlayerSettings.DashCooldown;
			currentlyDashing = false;
			Invincibilities--;

			Destroy(modelCopy.gameObject);
			if (weaponCopy)
				Destroy(weaponCopy.gameObject);

			FXManager.FinishFX(ref dashEffects);

			void ApplyMaterialToAllChildren(Transform parent)
			{
				//Apply to parent
				{
					parent.gameObject.layer = 0;
					Renderer rend = parent.GetComponent<Renderer>();
					if (rend)
					{
						rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
						rend.receiveShadows = false;
						Material[] newMats = new Material[rend.materials.Length];
						for (int i = 0; i < newMats.Length; i++)
						{
							newMats[i] = dashMaterialInstance;
						}
						rend.materials = newMats;
					}
				}

				//Apply to children
				int c = parent.childCount;
				for (int i = 0; i < c; i++)
				{
					parent.GetChild(i).gameObject.layer = 0;
					//Apply to children of child
					if (parent.GetChild(i).childCount > 0)
					{
						ApplyMaterialToAllChildren(parent.GetChild(i));
						continue;
					}

					Renderer rend = parent.GetChild(i).GetComponent<Renderer>();
					if (!rend)
						continue;

					rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					rend.receiveShadows = false;
					Material[] newMats = new Material[rend.materials.Length];
					for (int j = 0; j < newMats.Length; j++)
					{
						newMats[j] = dashMaterialInstance;
					}
					rend.materials = newMats;
				}
			}
		}
		#endregion
		#region Endurance Control
		public void ChangeEndurance(ChangeInfo change)
		{
			ChangeInfo.ChangeResult changeResult = new ChangeInfo.ChangeResult(change, this, false);

			if (changeResult.finalChangeAmount > 0)
			{
				usedEnduranceCooldown = PlayerSettings.EnduranceAfterUseCooldown;
			}

			curEndurance -= changeResult.finalChangeAmount;
			ClampEndurance();
		}
		public void ClampEndurance() => curEndurance = Mathf.Clamp(curEndurance, 0, curMaxEndurance);
		protected override void Regen()
		{
			base.Regen();

			if (PlayerSettings.DoEnduranceRegen && curEndurance < curMaxEndurance)
			{
				float enduranceRegenMultiplier = curStates.Fighting ? PlayerSettings.EnduranceRegenInCombatMultiplier : 1;
				if (usedEnduranceCooldown > 0)
				{
					usedEnduranceCooldown -= Time.deltaTime;
					enduranceRegenMultiplier *= PlayerSettings.EnduranceRegenAfterUseMultiplier;
				}
				curEndurance += PlayerSettings.EnduranceRegen * enduranceRegenMultiplier * Time.deltaTime;
				curEndurance = Mathf.Min(curEndurance, curMaxEndurance);
			}
		}
		#endregion
		#region EnemyTargeting
		private void TargetEnemyControl()
		{
			if (TargetedEntitie && !TargetedEntitie.gameObject.activeSelf)
				TargetedEntitie = null;

			if (TargetedEntitie)
			{
				Vector3 direction = (TargetedEntitie.actuallWorldPosition - actuallWorldPosition).normalized;
				float hAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg - 90;

				intendedRotation = -hAngle;
			}

			if (switchTargetTimer > 0)
				switchTargetTimer -= Time.deltaTime;

			targetPosition = TargetedEntitie ? (Vector3?)TargetedEntitie.actuallWorldPosition : null;

			if (InputController.Aim.Down)
			{
				List<(Entity, float)> possibleTargets = GetPossibleTargets();
				TargetedEntitie = null;
				if (possibleTargets.Count > 0)
				{
					//Now we have a list of possible targets sorted by lowest to highest distance
					//We try to find a target that we can see
					//So we start with the lowest distance (index 0) and keep going until we find one and take that one as new target
					//If there is none, then the targetEntitie will stay null
					for (int i = 0; i < possibleTargets.Count; i++)
					{
						if (CheckIfCanSeeEntitie(PlayerCameraController.PlayerCamera.transform, possibleTargets[i].Item1))
						{
							TargetedEntitie = possibleTargets[i].Item1;
							//We found a target so we stop here
							break;
						}
					}
				}
			}
			else if (InputController.Aim.Held && InputController.CameraMove.sqrMagnitude > 0.25f)
			{
				if (switchTargetTimer > 0 || TargetedEntitie == null)
					return;
				switchTargetTimer = SWITCH_TARGET_COOLDOWN;

				List<(Entity, float)> possibleTargets = GetPossibleTargets();

				if (possibleTargets.Count > 0)
				{
					List<(Entity, float)> screenTargets = new List<(Entity, float)>();
					Vector2 actuallInput = new Vector2(InputController.CameraMove.x * (SettingsData.ActiveInvertCameraX ? -1 : 1), InputController.CameraMove.y * (SettingsData.ActiveInvertCameraY ? -1 : 1));
					Vector2 clickedDirection = actuallInput.normalized;
					Vector2 curMiddle = PlayerCameraController.PlayerCamera.WorldToScreenPoint(TargetedEntitie.actuallWorldPosition);

					for (int i = 0; i < possibleTargets.Count; i++)
					{
						if (possibleTargets[i].Item1 == TargetedEntitie)
							continue;

						Vector2 entitieOnScreen = PlayerCameraController.PlayerCamera.WorldToScreenPoint(possibleTargets[i].Item1.actuallWorldPosition);
						Vector2 dif = (entitieOnScreen - curMiddle);
						float distance = dif.magnitude;

						float cosAngle = Vector3.Dot(clickedDirection, dif / distance);
						float distanceToClickDir = (1 - cosAngle) * distance + distance * 0.025f;

						screenTargets.Add((possibleTargets[i].Item1, distanceToClickDir));
					}

					//Sort by distance low to high
					screenTargets.Sort((x, y) => x.Item2.CompareTo(y.Item2));

					for (int i = 0; i < screenTargets.Count; i++)
					{
						if (CheckIfCanSeeEntitie(PlayerCameraController.PlayerCamera.transform, screenTargets[i].Item1))
						{
							TargetedEntitie = screenTargets[i].Item1;
							//We found a target so we stop here
							break;
						}
					}
				}
			}
			else if (InputController.Aim.Up)
				TargetedEntitie = null;
		}
		private List<(Entity, float)> GetPossibleTargets()
		{
			List<(Entity, float)> possibleTargets = new List<(Entity, float)>();

			for (int i = 0; i < AllEntities.Count; i++)
			{
				if (AllEntities[i] is Player) //We ignore the player
					continue;

				float distance = (AllEntities[i].actuallWorldPosition - PlayerCameraController.PlayerCamera.transform.position).sqrMagnitude;
				if (distance > PlayerSettings.MaxEnemyTargetingDistance * PlayerSettings.MaxEnemyTargetingDistance)
					continue;

				distance = Mathf.Sqrt(distance);

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
			return possibleTargets;
		}
		#endregion
		#region ItemControl
		private void ItemControl()
		{
			UpdateItemCooldowns();
			ScrollItemControll();
			ItemUseControl();
		}
		private void UpdateItemCooldowns()
		{
			for (int i = 0; i < GameController.ItemCollection.Data.Length; i++)
			{
				if (GameController.ItemCollection.Data[i].curCooldown > 0)
					GameController.ItemCollection.Data[i].curCooldown -= Time.deltaTime;
			}
		}
		private void ScrollItemControll()
		{
			//Consumable Scrolling
			int selectedItemIndexChange = InputController.ItemScrollUp.Down ? 1 : (InputController.ItemScrollDown.Down ? -1 : 0);
			if (selectedItemIndexChange != 0)
			{
				int t = 0;
				for (int i = 0; i < (SELECTABLE_ITEMS_AMOUNT - 1); i++)
				{
					t += selectedItemIndexChange;
					int valIndex = ValidatedID(t + selectedItemIndex);
					if (SelectableItems[valIndex] != null)
					{
						if (EquipedItem != null)
							EquipedItem.data.UnEquip(EquipedItem, this);

						selectedItemIndex = valIndex;
						EquipedItem.data.Equip(EquipedItem, this);
						break;
					}
				}
			}

			//ActivationCompound Scrolling
			InventoryItem selectedItem = SelectableActivationCompoundItems[selectedSpellItemIndex];
			ActivationCompoundItem selectedAC = selectedItem == null ? null : (selectedItem.data as ActivationCompoundItem);
			bool cannotScrollMagic = false;
			if (selectedAC == null)
			{
				cannotScrollMagic = true;
			}
			else if (!selectedAC.TargetCompound.NoDefinedActionType)
			{
				cannotScrollMagic = (selectedAC.TargetCompound.ActionType == ActionType.Attacking) ? IsAttackStopped : IsCastingStopped;
			}

			int selectedSpellIndexChange = cannotScrollMagic ? 0 : (InputController.MagicScrollUp.Down ? 1 : (InputController.MagicScrollDown.Down ? -1 : 0));
			if (selectedSpellIndexChange != 0)
			{
				int t = 0;
				for (int i = 0; i < (SELECTABLE_ITEMS_AMOUNT - 1); i++)
				{
					t += selectedSpellIndexChange;
					int valIndex = ValidatedID(t + selectedSpellItemIndex);
					if (SelectableActivationCompoundItems[valIndex] != null)
					{
						if (EquipedSpell != null)
							EquipedSpell.data.UnEquip(EquipedSpell, this);

						selectedSpellItemIndex = valIndex;
						EquipedSpell.data.Equip(EquipedSpell, this);
						break;
					}
				}
			}

			int ValidatedID(int id)
			{
				while (id < 0)
					id += SELECTABLE_ITEMS_AMOUNT;
				while (id >= SELECTABLE_ITEMS_AMOUNT)
					id -= SELECTABLE_ITEMS_AMOUNT;

				return id;
			}
		}
		private void ItemUseControl()
		{
			if (IsStunned)
				return;
			if (EquipedWeapon != null && (InputController.Attack.Down || (EquipedWeapon.data.AllowHoldUse && InputController.Attack.Pressed)))
			{
				if (EquipedWeapon.data.curCooldown <= 0)
					EquipedWeapon.data.Use(EquipedWeapon, this, Inventory);
			}
			else if (EquipedSpell != null && (InputController.MagicCast.Down || (EquipedSpell.data.AllowHoldUse && InputController.MagicCast.Pressed)))
			{
				if (EquipedSpell.data.curCooldown <= 0)
					EquipedSpell.data.Use(EquipedSpell, this, Inventory);
			}
			else if (EquipedItem != null && (InputController.UseItem.Down || (EquipedItem.data.AllowHoldUse && InputController.UseItem.Pressed)))
			{
				if (EquipedItem.data.curCooldown <= 0)
					EquipedItem.data.Use(EquipedItem, this, Inventory);
			}
		}
		private void UpdateEquipedItems()
		{
			if (EquipedWeapon != null && EquipedWeapon.stackSize <= 0)
				EquipedWeapon = null;
			if (EquipedArmor != null && EquipedArmor.stackSize <= 0)
				EquipedArmor = null;

			bool selectablesChanged = false;
			for (int i = 0; i < SELECTABLE_ITEMS_AMOUNT; i++)
			{
				if (SelectableItems[i] != null && SelectableItems[i].stackSize <= 0)
				{
					selectablesChanged = true;
					SelectableItems[i] = null;
				}
				if (SelectableActivationCompoundItems[i] != null && SelectableActivationCompoundItems[i].stackSize <= 0)
				{
					selectablesChanged = true;
					SelectableActivationCompoundItems[i] = null;
				}
			}
			if (selectablesChanged)
				SelectectableItemsChanged();
		}
		public void SelectectableItemsChanged()
		{
			//If either the spell or Item selectables changed we want to check if we currently have nothing equipped
			//if so, then we try to find item in the corrosponding array and equip, by doing this we can assure that when the player
			//has 0 items in the selectable list he can have nothing equipped but the moment he puts one item into the array it will be equipped
			//this is also used in case of the player using up one item, if that happens this function tries to jump to the next item

			if (EquipedItem == null)
			{
				for (int i = 0; i < SELECTABLE_ITEMS_AMOUNT; i++)
				{
					int valIndex = ValidatedID(i + selectedItemIndex);
					if (SelectableItems[valIndex] != null)
					{
						selectedItemIndex = valIndex;
						EquipedItem.data.Equip(EquipedItem, this);
						break;
					}
				}
			}

			if (EquipedSpell == null)
			{
				for (int i = 0; i < SELECTABLE_ITEMS_AMOUNT; i++)
				{
					int valIndex = ValidatedID(i + selectedSpellItemIndex);
					if (SelectableActivationCompoundItems[valIndex] != null)
					{
						selectedSpellItemIndex = valIndex;
						EquipedSpell.data.Equip(EquipedSpell, this);
						break;
					}
				}
			}

			int ValidatedID(int id)
			{
				while (id < 0)
					id += SELECTABLE_ITEMS_AMOUNT;
				while (id >= SELECTABLE_ITEMS_AMOUNT)
					id -= SELECTABLE_ITEMS_AMOUNT;

				return id;
			}
		}
		#endregion
		#region ShieldControl
		private void ShieldControl()
		{
			if (IsStunned)
				return;
			if (InputController.Block.Down)
				playerShield.SetShieldState(true);
			else if(InputController.Block.Up)
				playerShield.SetShieldState(false);
		}
		#endregion
		#region StateControl
		protected override void Death()
		{
			PauseMenuController.Instance.ToggleState(false);
			CharacterMenuController.Instance.ToggleState(false);
			FinishAliveBoundFX();

			if (TargetedEntitie)
				TargetedEntitie = null;
			Alive = false;

			animationControl.SetTrigger("Death");
		}
		public override void StartCombat()
		{
			if(!curStates.Fighting)
				FXManager.ExecuteFX(playerSettings.EffectsOnCombatStart, transform, true, out CombatBoundEffects);

			base.StartCombat();
			if (!combatMusic.IsAdded)
			{
				combatMusic.WantsToPlay = true;
				MusicController.Instance.AddMusicInstance(combatMusic);
			}
		}
		protected override void CombatEnd()
		{
			combatMusic.WantsToPlay = false;
			FXManager.FinishFX(ref CombatBoundEffects);
		}
		private void FinishAliveBoundFX()
		{
			FXManager.FinishFX(ref HealthBelowThresholdBoundEffects);
			FXManager.FinishFX(ref PlayerWalkingBoundEffects);
			FXManager.FinishFX(ref PlayerRunningBoundEffects);
			FXManager.FinishFX(ref CombatBoundEffects);
		}
		#region IFrames
		protected override void OnHealthChange(ChangeInfo baseChange, ChangeInfo.ChangeResult resultInfo)
		{
			if (resultInfo.finalChangeAmount < 0)
				return;

			if (baseChange.attacker != this && baseChange.cause != CauseType.DOT)
			{
				if (PlayerSettings.InvincibleAfterHit)
				{
					if (!remainingInvincibleTime.HasValue)
					{
						remainingInvincibleTime = PlayerSettings.InvincibleAfterHitTime;
						StartCoroutine(OnHitInvincibleCoroutine());
					}
					remainingInvincibleTime = PlayerSettings.InvincibleAfterHitTime;
				}
			}
		}
		private float? remainingInvincibleTime = null;
		private IEnumerator OnHitInvincibleCoroutine()
		{
			Invincibilities++;
			float flashTimer = 0;
			bool flashOn = false;
			Renderer[] targetRenderers = GetComponentsInChildren<Renderer>();
			Color[] baseColors = new Color[targetRenderers.Length];

			for (int i = 0; i < targetRenderers.Length; i++)
			{
				baseColors[i] = targetRenderers[i].material.color;
			}

			while ((remainingInvincibleTime ?? 0) > 0)
			{
				yield return new WaitForEndOfFrame();
				remainingInvincibleTime -= Time.deltaTime;
				flashTimer += Time.deltaTime;

				if (flashTimer > PlayerSettings.InvincibleModelFlashDelay)
				{
					if (!flashOn)
					{
						FlashMaterials();
					}
					if (flashTimer > PlayerSettings.InvincibleModelFlashDelay + PlayerSettings.InvincibleModelFlashTime)
					{
						UnFlashMaterials();
						flashTimer -= PlayerSettings.InvincibleModelFlashDelay + PlayerSettings.InvincibleModelFlashTime;
					}
				}
			}
			if (flashOn)
				UnFlashMaterials();

			remainingInvincibleTime = null;
			Invincibilities--;
			void FlashMaterials()
			{
				flashOn = true;
				for (int i = 0; i < targetRenderers.Length; i++)
					targetRenderers[i].material.color = PlayerSettings.InvincibleModelFlashColor;
			}
			void UnFlashMaterials()
			{
				flashOn = false;
				for (int i = 0; i < targetRenderers.Length; i++)
					targetRenderers[i].material.color = baseColors[i];
			}
		}
		#endregion
		public void AddExperience(int amount)
		{
			TotalExperience += amount;
			EventManager.PlayerExperienceChangedInvoke();
			while (TotalExperience >= RequiredExperienceForLevel)
			{
				LevelUpEntitie();
			}
		}
		public void ChangeCurrency(int change)
		{
			CurrentCurrencyAmount += change;
			EventManager.PlayerCurrencyChangedInvoke();
		}
		protected override void LevelUpEntitie()
		{
			base.LevelUpEntitie();
			TotalExperience = System.Math.Max(TotalExperience, PlayerSettings.LevelSettings.curve.GetRequiredExperience(EntitieLevel - 1));
			RequiredExperienceForLevel = PlayerSettings.LevelSettings.curve.GetRequiredExperience(EntitieLevel);
			AvailableAtributePoints += PlayerSettings.LevelSettings.AttributePointsPerLevel;
			AvailableSkillPoints += PlayerSettings.LevelSettings.SkillPointsPerLevel;

			//Inform all script that need the information that the player leveled
			EventManager.PlayerLevelupInvoke();
		}
		public float GetLeveledValue(TargetBaseStat stat)
		{
			float value = 0;
			for (int i = 0; i < LevelingBaseBuff.Effects.Length; i++)
			{
				if (LevelingBaseBuff.Effects[i].TargetBaseStat == stat)
					value += LevelingBaseBuff.Effects[i].Amount;
			}
			for (int i = 0; i < LevelingPointsBuff.Effects.Length; i++)
			{
				if (LevelingPointsBuff.Effects[i].TargetBaseStat == stat)
					value += LevelingPointsBuff.Effects[i].Amount;
			}
			switch (stat)
			{
				case TargetBaseStat.Health:
					{
						value += PlayerSettings.Health;
						break;
					}
				case TargetBaseStat.Mana:
					{
						value += PlayerSettings.Mana;
						break;
					}
				case TargetBaseStat.Endurance:
					{
						value += PlayerSettings.Endurance;
						break;
					}
				case TargetBaseStat.PhysicalDamage:
					{
						value += PlayerSettings.BasePhysicalDamage;
						break;
					}
				case TargetBaseStat.MagicalDamage:
					{
						value += PlayerSettings.BaseMagicalDamage;
						break;
					}
				case TargetBaseStat.Defense:
					{
						value += PlayerSettings.BaseDefense;
						break;
					}
				case TargetBaseStat.MoveSpeed:
					{
						value += PlayerSettings.WalkSpeed;
						break;
					}
				case TargetBaseStat.JumpHeightMultiplier:
					{
						value += PlayerSettings.JumpPower.y;
						break;
					}
			}

			return value;
		}
		#endregion
		#region ContextControl
		private bool ContextControl()
		{
			//Both for the pause and character menu the same logic applies:
			//1. Open if the relating pause button is pressed (Start or Select)
			//2. Close when the relating pause button is pressed or MenuBack
			//3. If the other is open we ignore all input (Paused + SelfNotOpen)
			if ((InputController.Pause.Down || (PauseMenuController.Instance.PauseMenuOpen && InputController.MenuBack.Down)) && 
				!(GameController.GameIsPaused && !PauseMenuController.Instance.PauseMenuOpen))
			{
				PauseMenuController.Instance.ToggleState(null);
				return true;
			}
			else if((InputController.PlayerMenu.Down || (CharacterMenuController.Instance.CharacterMenuOpen && InputController.MenuBack.Down)) && 
				!(GameController.GameIsPaused && !CharacterMenuController.Instance.CharacterMenuOpen))
			{
				CharacterMenuController.Instance.ToggleState(null);
				return true;
			}
			return false;
		}
		#endregion
		#region  Debug
		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
				return;

			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, transform.position + controllDirection * 2);

			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(modelTransform.position, modelTransform.position + modelTransform.up * 2.5f);
		}
		#endregion
	}
}