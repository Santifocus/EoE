﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EoE.Controlls;
using EoE.Events;
using EoE.Information;
using EoE.Combatery;

namespace EoE.Entities
{
	public class Player : Entitie
	{
		#region Fields
		//Constants
		private const float RUN_ANIM_THRESHOLD = 0.75f;
		private const float NON_TURNING_THRESHOLD = 60;
		private const float LERP_TURNING_AREA = 0.5f;
		private const float JUMP_GROUND_COOLDOWN = 0.2f;
		private const float IS_FALLING_THRESHOLD = -1;
		private const float LANDED_VELOCITY_THRESHOLD = 0.5f;
		private const float SWITCH_TARGET_COOLDOWN = 0.25f;
		private const int SELECTABLE_ITEMS_AMOUNT = 4;

		//Inspector variables
		[Space(10)]
		public CharacterController charController = default;
		public TextMeshProUGUI debugText = default;
		public Transform weaponHoldPoint = default;
		public Animator animationControl = default;

		[SerializeField] private PlayerSettings selfSettings = default;
		[SerializeField] private PlayerBuffDisplay buffDisplay = default;

		//Endurance
		public float curEndurance { get; set; }
		public float curMaxEndurance { get; set; }
		private float usedEnduranceCooldown;

		//Dodge
		public Transform modelTransform = default;
		private float dodgeCooldown;
		private bool currentlyDodging;

		//Blocking
		private float blockingTimer;
		private bool isBlocking;
		private BuffInstance blockingBuff;

		//Velocity
		private Vector3 controllDirection;
		private float intendedAcceleration;
		private float curAcceleration;
		private float jumpGroundCooldown;
		private float lastFallVelocity;

		//Targeting
		private float switchTargetTimer;

		//Getter Helpers
		public float jumpVelocity { get; private set; }
		public float verticalVelocity { get; private set; }
		public float totalVerticalVelocity => jumpVelocity * PlayerSettings.JumpImpulsePower + verticalVelocity;
		public override Vector3 curVelocity => base.curVelocity + new Vector3(0, totalVerticalVelocity, 0);
		public enum AttackState : short { NormalStand = 0, NormalSprint = 1, NormalJump = 2, NormalSprintJump = 3, HeavyStand = 4, HeavySprint = 5, HeavyJump = 6, HeavySprintJump = 7 }
		public static Player Instance { get; private set; }
		public override EntitieSettings SelfSettings => selfSettings;
		public static PlayerSettings PlayerSettings => Instance.selfSettings;
		public Entitie TargetedEntitie;
		public PlayerBuffDisplay BuffDisplay => Instance.buffDisplay;

		#region Items
		public Inventory Inventory;

		//Weapon
		public InventoryItem EquipedWeapon;

		//Use Items
		public InventoryItem[] SelectableItems;
		public int selectedItemIndex { get; private set; }
		public InventoryItem EquipedItem => SelectableItems[selectedItemIndex];

		//Spell Items
		public InventoryItem[] SelectableSpellItems;
		public int selectedSpellItemIndex { get; private set; }
		public InventoryItem EquipedSpell => SelectableSpellItems[selectedSpellItemIndex];
		#endregion

		#region Leveling
		public Buff LevelingPointsBuff { get; private set; }
		public int TotalSoulCount { get; private set; }
		public int RequiredSoulsForLevel { get; private set; }
		public int AvailableSkillPoints;
		public int AvailableAtributePoints;
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
		}
		protected override void EntitieUpdate()
		{
			if (GameController.GameIsPaused)
			{
				if (TargetedEntitie)
					TargetedEntitie = null;
				return;
			}

			MovementControl();
			TargetEnemyControl();
			ItemControl();
			BlockControl();
			JumpControl();

			animationControl.SetBool("InCombat", curStates.Fighting);
		}
		protected override void EntitieFixedUpdate()
		{
			CheckForLanding();
			ApplyForces();
			CheckForFalling();
			UpdateAcceleration();

			if (!IsStunned || IsCasting)
				TurnControl();
		}
		#endregion
		#region Setups
		protected override void ResetStats()
		{
			base.ResetStats();
			curMaxEndurance = PlayerSettings.Endurance;
		}
		protected override void ResetStatValues()
		{
			base.ResetStatValues();
			curEndurance = curMaxEndurance;
		}
		protected override void LevelSetup()
		{
			base.LevelSetup();
			LevelingPointsBuff = ScriptableObject.CreateInstance<Buff>();
			{
				LevelingPointsBuff.Name = "LevelingSkillPoints";
				LevelingPointsBuff.Quality = BuffType.Positive;
				LevelingPointsBuff.Icon = null;
				LevelingPointsBuff.BuffTime = 0;
				LevelingPointsBuff.Permanent = true;
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

			AddBuff(LevelingPointsBuff, this);
			RequiredSoulsForLevel = PlayerSettings.LevelSettings.curve.GetRequiredSouls(EntitieLevel);
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
			SelectableSpellItems = new InventoryItem[SELECTABLE_ITEMS_AMOUNT];

			for (int i = 0; i < PlayerSettings.StartItems.Length; i++)
			{
				//Add it to the inventory
				List<int> targetSlots = Inventory.AddItem(new InventoryItem(PlayerSettings.StartItems[i].Item, PlayerSettings.StartItems[i].ItemCount));

				//Force equipp
				if (PlayerSettings.StartItems[i].ForceEquip)
				{
					InventoryItem targetItem = Inventory[targetSlots[targetSlots.Count - 1]];
					targetItem.isEquiped = true;
					//Find out what slot it belongs to, if it is a spell / normal item we try
					//to put it in a open slot, if all slots are filled we put it in the first
					if(PlayerSettings.StartItems[i].Item is WeaponItem)
					{
						EquipedWeapon = targetItem;
						targetItem.isEquiped = true;
						targetItem.data.Equip(targetItem, this);
					}
					else if(PlayerSettings.StartItems[i].Item is ArmorItem)
					{
						EquipedArmor = targetItem;
						targetItem.isEquiped = true;
						targetItem.data.Equip(targetItem, this);
					}
					else if(PlayerSettings.StartItems[i].Item is SpellItem)
					{
						bool added = false;

						//Try to find a slot that is null and put the item there
						for(int j = 0; j < SelectableSpellItems.Length; j++)
						{
							if(SelectableSpellItems[j] == null)
							{
								SelectableSpellItems[j] = targetItem;
								SelectableSpellItems[j].isEquiped = true;
								added = true;
								if (j == 0)
								{
									SelectableSpellItems[j].data.Equip(targetItem, this);
								}
								break;
							}
						}
						//couldnt find a null slot, put it in the first one, (just a fallback)
						if (!added && SelectableSpellItems.Length > 0)
						{
							SelectableSpellItems[0] = targetItem;
							SelectableSpellItems[0].isEquiped = true;
							SelectableSpellItems[0].data.Equip(targetItem, this);
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
		#region Movement
		private void MovementControl()
		{
			CameraControl();
			UpdateWalkingAnimations();

			if (IsStunned)
			{
				curAcceleration = 0;
				curStates.Moving = curStates.Running = false;
				curStates.Turning = IsCasting ? curStates.Turning : false;
				return;
			}

			PlayerMoveControl();
			DodgeControl();
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
		#region Walking
		private void UpdateWalkingAnimations()
		{
			bool turning = curStates.Turning;
			bool moving = curStates.Moving;
			bool running = curStates.Running;

			//If the Player doesnt move intentionally but is in run mode, then stop the run mode
			if (running && (!moving || isBlocking))
				curStates.Running = false;

			//Set all the animation states
			float directionOffset = Vector3.Dot(controllDirection, transform.forward);
			animationControl.SetBool("Walking", !turning && curAcceleration > 0);
			animationControl.SetFloat("WalkDirection", directionOffset < 0 ? -1 : 1); 
			animationControl.SetFloat("WalkSpeed", (curStates.Running ? SelfSettings.RunSpeedMultiplicator : 1) * curAcceleration);

			float newSideTurn = Mathf.LerpAngle(modelTransform.localEulerAngles.z, (1 - Mathf.Abs(directionOffset)) * PlayerSettings.MaxSideTurn * (InputController.PlayerMove.x < 0 ? 1 : -1) * curAcceleration, Time.deltaTime * PlayerSettings.SideTurnLerpSpeed);
			modelTransform.localEulerAngles = new Vector3(0, 0, newSideTurn);
		}
		private void TurnControl()
		{
			float turnAmount = Time.fixedDeltaTime * SelfSettings.TurnSpeed * (charController.isGrounded ? 1 : SelfSettings.InAirTurnSpeedMultiplier);
			float normalizedDif = Mathf.Abs(curRotation - intendedRotation) / NON_TURNING_THRESHOLD;
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
				return;

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

			//Rotate the input direction base on the camera direction
			float cosDir = Mathf.Cos((-PlayerCameraController.CurRotation.x) * Mathf.Deg2Rad);
			float sinDir = Mathf.Sin((-PlayerCameraController.CurRotation.x) * Mathf.Deg2Rad);
			float newX = (inputDirection.x * cosDir) - (inputDirection.y * sinDir);
			float newZ = (inputDirection.x * sinDir) + (inputDirection.y * cosDir);
			//Now set the controll direction to the rotated direction, normalize by dividing with the intended speed
			controllDirection = new Vector3(newX, 0, newZ) / intendedControl;

			if (!TargetedEntitie)
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

			if (jumpGroundCooldown > 0)
			{
				jumpGroundCooldown -= Time.deltaTime;
				return;
			}

			if (jumpPressed && charController.isGrounded)
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

			jumpVelocity = Mathf.Min(PlayerSettings.JumpPower.y * curJumpPowerMultiplier, PlayerSettings.JumpPower.y * curJumpPowerMultiplier);
			Vector3 addedExtraForce = PlayerSettings.JumpPower.x * transform.right * curJumpPowerMultiplier + PlayerSettings.JumpPower.z * transform.forward * curJumpPowerMultiplier * (curStates.Running ? PlayerSettings.RunSpeedMultiplicator : 1) * forwardMultiplier;
			impactForce += new Vector2(addedExtraForce.x, addedExtraForce.z) * curAcceleration;
			verticalVelocity = 0;

			jumpGroundCooldown = JUMP_GROUND_COOLDOWN;
			animationControl.SetTrigger("Jump");
		}
		private void CheckForFalling()
		{
			//Find out wether the entitie is falling or not
			bool playerWantsToFall = curStates.Falling || !InputController.Jump.Active;
			bool falling = !charController.isGrounded && (totalVerticalVelocity < IS_FALLING_THRESHOLD || playerWantsToFall);

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
			//Check if we landed
			float velDif = curVelocity.y - lastFallVelocity;
			if (velDif > LANDED_VELOCITY_THRESHOLD && jumpGroundCooldown <= 0) //We stopped falling for a certain amount, and we didnt change velocity because we just jumped
			{
				Landed(velDif);
			}
			lastFallVelocity = curVelocity.y;
		}
		private void Landed(float velDif)
		{
			EventManager.PlayerLandedInvoke(velDif);

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

			//Check if there is any fall damage to give
			float damageAmount = GameController.CurrentGameSettings.FallDamageCurve.Evaluate(velDif);

			if (damageAmount > 0)
				ChangeHealth(new ChangeInfo(null, CauseType.Physical, ElementType.None, TargetStat.Health, actuallWorldPosition, Vector3.up, damageAmount, false));
		}
		#endregion
		#endregion
		#region ForceControl
		private void UpdateAcceleration()
		{
			if (curStates.Moving && !curStates.Turning)
			{
				float clampedIntent = Mathf.Clamp01(intendedAcceleration);
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
			verticalVelocity += causedKnockback.y;
		}
		private void ApplyForces()
		{
			Vector3 appliedForce = controllDirection * curWalkSpeed * (curStates.Running ? SelfSettings.RunSpeedMultiplicator : 1) * curAcceleration + curVelocity;

			charController.Move(appliedForce * Time.fixedDeltaTime);
			animationControl.SetBool("OnGround", charController.isGrounded);
			ApplyGravity();
		}
		private void ApplyGravity(float scale = 1)
		{
			float lowerFallThreshold = Physics.gravity.y / 2;
			float force = scale * Physics.gravity.y * Time.fixedDeltaTime;

			if (!charController.isGrounded || totalVerticalVelocity > 0)
			{
				if (jumpVelocity > 0)
				{
					jumpVelocity += force * PlayerSettings.JumpImpulsePower;
					if (jumpVelocity < 0)
					{
						verticalVelocity += jumpVelocity;
						jumpVelocity = 0;
					}
				}
				else
				{
					verticalVelocity += force;
					jumpVelocity = 0;
				}
			}
			else
			{
				jumpVelocity = 0;
				verticalVelocity = lowerFallThreshold;
			}
		}
		#endregion
		#region Dodging
		private void DodgeControl()
		{
			if (dodgeCooldown > 0)
			{
				dodgeCooldown -= Time.deltaTime;
				return;
			}

			if (InputController.Dodge.Down && !currentlyDodging && curEndurance >= PlayerSettings.DodgeEnduranceCost)
			{
				EventManager.PlayerDodgeInvoke();
				ChangeEndurance(new ChangeInfo(this, CauseType.Magic, TargetStat.Endurance, PlayerSettings.DodgeEnduranceCost));
				StartCoroutine(DodgeCoroutine());
			}
		}
		private IEnumerator DodgeCoroutine()
		{
			currentlyDodging = true;
			float timer = 0;
			float targetAngle = intendedRotation;
			if (InputController.PlayerMove != Vector2.zero)
			{
				targetAngle = -(Mathf.Atan2(controllDirection.z, controllDirection.x) * Mathf.Rad2Deg - 90);
			}

			//Play FX
			FXInstance[] dodgeEffects = new FXInstance[PlayerSettings.EffectsOnPlayerDodge.Length];
			for (int i = 0; i < PlayerSettings.EffectsOnPlayerDodge.Length; i++)
			{
				dodgeEffects[i] = FXManager.PlayFX(PlayerSettings.EffectsOnPlayerDodge[i], transform, true);
			}

			Vector3 newForce = new Vector3(Mathf.Sin(targetAngle * Mathf.Deg2Rad), 0, Mathf.Cos(targetAngle * Mathf.Deg2Rad)) * PlayerSettings.DodgePower * curWalkSpeed;

			entitieForceController.ApplyForce(newForce, 1 / PlayerSettings.DodgeDuration);

			//Create a copy of the player model
			Material dodgeMaterialInstance = Instantiate(PlayerSettings.DodgeModelMaterial);
			Transform modelCopy = Instantiate(modelTransform, Storage.ParticleStorage);

			modelCopy.transform.position = modelTransform.position;
			modelCopy.transform.localScale = modelTransform.lossyScale;
			modelCopy.transform.rotation = modelTransform.rotation;

			Color baseColor = dodgeMaterialInstance.color;
			Color alphaPart = new Color(0, 0, 0, dodgeMaterialInstance.color.a);
			baseColor.a = 0;

			ApplyMaterialToAllChildren(modelCopy);

			invincible++;
			while (timer < PlayerSettings.DodgeModelExistTime)
			{
				yield return new WaitForFixedUpdate();
				timer += Time.fixedDeltaTime;
				dodgeMaterialInstance.color = baseColor + alphaPart * (1 - timer / PlayerSettings.DodgeModelExistTime);
			}
			dodgeCooldown = PlayerSettings.DodgeCooldown;
			currentlyDodging = false;
			invincible--;

			Destroy(modelCopy.gameObject);

			for(int i = 0; i < dodgeEffects.Length; i++)
			{
				dodgeEffects[i].FinishFX();
			}

			void ApplyMaterialToAllChildren(Transform parent)
			{
				//Apply to parent
				{
					parent.gameObject.layer = 0;
					Renderer rend = parent.GetComponent<Renderer>();
					if (rend)
					{
						Material[] newMats = new Material[rend.materials.Length];
						for (int i = 0; i < newMats.Length; i++)
						{
							newMats[i] = dodgeMaterialInstance;
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

					Material[] newMats = new Material[rend.materials.Length];
					for (int j = 0; j < newMats.Length; j++)
					{
						newMats[j] = dodgeMaterialInstance;
					}
					rend.materials = newMats;

				}
			}
		}
		#endregion
		#region EnemyTargeting
		private void TargetEnemyControl()
		{
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
				List<(Entitie, float)> possibleTargets = GetPossibleTargets();
				TargetedEntitie = null;
				if (possibleTargets.Count > 0)
				{
					//Now we have a list of possible targets sorted by lowest to highest distance
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
			else if (InputController.Aim.Active && InputController.CameraMove.sqrMagnitude > 0.25f)
			{
				if (switchTargetTimer > 0 || TargetedEntitie == null)
					return;
				switchTargetTimer = SWITCH_TARGET_COOLDOWN;

				List<(Entitie, float)> possibleTargets = GetPossibleTargets();

				if (possibleTargets.Count > 0)
				{
					List<(Entitie, float)> screenTargets = new List<(Entitie, float)>();
					Vector2 clickedDirection = InputController.CameraMove.normalized;
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
						if (CheckIfCanSeeEntitie(screenTargets[i].Item1))
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
		private List<(Entitie, float)> GetPossibleTargets()
		{
			List<(Entitie, float)> possibleTargets = new List<(Entitie, float)>();

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
			for(int i = 0; i < GameController.ItemCollection.Data.Length; i++)
			{
				if (GameController.ItemCollection.Data[i].curCooldown > 0)
					GameController.ItemCollection.Data[i].curCooldown -= Time.deltaTime;
			}
		}
		private void ScrollItemControll()
		{
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

			int selectedSpellIndexChange = InputController.MagicScrollUp.Down ? 1 : (InputController.MagicScrollDown.Down ? -1 : 0);
			if (selectedSpellIndexChange != 0)
			{
				int t = 0;
				for (int i = 0; i < (SELECTABLE_ITEMS_AMOUNT - 1); i++)
				{
					t += selectedSpellIndexChange;
					int valIndex = ValidatedID(t + selectedSpellItemIndex);
					if (SelectableSpellItems[valIndex] != null)
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
			if (InputController.Attack.Down && EquipedWeapon != null)
			{
				if (EquipedWeapon.data.curCooldown <= 0)
					EquipedWeapon.data.Use(EquipedWeapon, this, Inventory);
			}
			else if (InputController.MagicCast.Down && EquipedSpell != null)
			{
				if (EquipedSpell.data.curCooldown <= 0)
					EquipedSpell.data.Use(EquipedSpell, this, Inventory);
			}
			else if (InputController.UseItem.Down && EquipedItem != null)
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
			for(int i = 0; i < SELECTABLE_ITEMS_AMOUNT; i++)
			{
				if (SelectableItems[i] != null && SelectableItems[i].stackSize <= 0)
				{
					selectablesChanged = true;
					SelectableItems[i] = null;
				}
				if (SelectableSpellItems[i] != null && SelectableSpellItems[i].stackSize <= 0)
				{
					selectablesChanged = true;
					SelectableSpellItems[i] = null;
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
					if (SelectableSpellItems[valIndex] != null)
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
		#region BlockControl
		private void BlockControl()
		{
			float manaCost = 0;
			float enduranceCost = 0;
			for(int i = 0; i < PlayerSettings.BlockingBuff.DOTs.Length; i++)
			{
				if (PlayerSettings.BlockingBuff.DOTs[i].TargetStat == TargetStat.Mana)
					manaCost += PlayerSettings.BlockingBuff.DOTs[i].BaseDamage;
				else if(PlayerSettings.BlockingBuff.DOTs[i].TargetStat == TargetStat.Endurance)
					enduranceCost += PlayerSettings.BlockingBuff.DOTs[i].BaseDamage;
			}
			manaCost *= Time.deltaTime;
			enduranceCost *= Time.deltaTime;

			if ((InputController.Block.Down || InputController.Block.Active) && (curMana >= manaCost && curEndurance >= enduranceCost))
			{
				if (!isBlocking)
				{
					blockingTimer += Time.deltaTime;
					if (blockingTimer >= PlayerSettings.StartBlockingInertia)
					{
						isBlocking = true;
						blockingBuff = AddBuff(PlayerSettings.BlockingBuff, this);
					}
				}
			}
			else if(isBlocking)
			{
				blockingTimer -= Time.deltaTime;
				if (blockingTimer < PlayerSettings.StartBlockingInertia - PlayerSettings.StopBlockingInertia)
				{
					isBlocking = false;
					blockingTimer = 0;
					RemoveBuff(blockingBuff);
				}
			}
		}
		#endregion
		#region StateControl
		protected override void Death()
		{
			if (TargetedEntitie)
				TargetedEntitie = null;
			base.Death();
		}
		#region IFrames
		protected override void ReceivedHealthDamage(ChangeInfo baseChange, ChangeInfo.ChangeResult resultInfo)
		{
			if(baseChange.attacker != this && baseChange.cause != CauseType.DOT)
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
			invincible++;
			float flashTimer = 0;
			bool flashOn = false;
			Renderer[] targetRenderers = GetComponentsInChildren<Renderer>();
			Color[] baseColors = new Color[targetRenderers.Length];

			for (int i = 0; i < targetRenderers.Length; i++)
			{
				baseColors[i] = targetRenderers[i].material.color;
			}

			while((remainingInvincibleTime ?? 0) > 0)
			{
				yield return new WaitForEndOfFrame();
				remainingInvincibleTime -= Time.deltaTime;
				flashTimer += Time.deltaTime;

				if(flashTimer > PlayerSettings.InvincibleModelFlashDelay)
				{
					if (!flashOn)
					{
						FlashMaterials();
					}
					if(flashTimer > PlayerSettings.InvincibleModelFlashDelay + PlayerSettings.InvincibleModelFlashTime)
					{
						UnFlashMaterials();
						flashTimer -= PlayerSettings.InvincibleModelFlashDelay + PlayerSettings.InvincibleModelFlashTime;
					}
				}
			}
			if (flashOn)
				UnFlashMaterials();

			remainingInvincibleTime = null;
			invincible--;
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
		public void AddSouls(int amount)
		{
			TotalSoulCount += amount;
			EventManager.PlayerSoulCountChangedInvoke();
			while (TotalSoulCount >= RequiredSoulsForLevel)
			{
				LevelUpEntitie();
			}
		}
		protected override void LevelUpEntitie()
		{
			base.LevelUpEntitie();
			TotalSoulCount = System.Math.Max(TotalSoulCount, PlayerSettings.LevelSettings.curve.GetRequiredSouls(EntitieLevel - 1));
			RequiredSoulsForLevel = PlayerSettings.LevelSettings.curve.GetRequiredSouls(EntitieLevel);
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
						value += PlayerSettings.BaseAttackDamage;
						break;
					}
				case TargetBaseStat.MagicalDamage:
					{
						value += PlayerSettings.BaseMagicDamage;
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
	}
}