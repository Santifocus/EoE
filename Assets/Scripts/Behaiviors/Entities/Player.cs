using EoE.Controlls;
using EoE.Events;
using EoE.Information;
using EoE.Utils;
using EoE.Weapons;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EoE.Entities
{
	public class Player : Entitie
	{
		#region Fields
		//Constants
		private const float ATTACK_INTERACT_COOLDOWN = 0.25f;
		private const float RUN_ANIM_THRESHOLD = 0.75f;
		private const float NON_TURNING_THRESHOLD = 60;
		private const float LERP_TURNING_AREA = 0.5f;

		//Inspector variables
		[Space(10)]
		[SerializeField] private PlayerSettings selfSettings = default;
		[SerializeField] private Transform rightHand = default;
		[SerializeField] private Weapon equipedWeapon = default;
		[SerializeField] private PlayerBuffDisplay buffDisplay = default;
		[SerializeField] private TextMeshProUGUI soulCount = default;
		[SerializeField] private TextMeshProUGUI levelDisplay = default;

		public TMPro.TextMeshProUGUI debugText = default;

		//Endurance
		public bool recentlyUsedEndurance { get; private set; }
		public float curEndurance { get; private set; }
		public float lockedEndurance { get; private set; }
		public int totalEnduranceContainers { get; private set; }
		[HideInInspector] public float trueEnduranceAmount;
		private float usedEnduranceCooldown;
		private float useableEndurance => totalEnduranceContainers * PlayerSettings.EndurancePerBar;

		//Dodge
		public Transform modelTransform = default;
		private float dodgeCooldown;
		private bool currentlyDodging;

		//Blocking
		private float blockingTimer;
		private bool isBlocking;
		private BuffInstance blockingBuff;

		//Velocity
		protected Vector3 curMoveForce;
		protected Vector3? controllDirection = null;
		protected float intendedAcceleration;
		protected float curAcceleration;

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
		public static PlayerBuffDisplay BuffDisplay => instance.buffDisplay;

		public static Inventory ItemInventory;
		public static Inventory WeaponInventory;
		public static Inventory ArmorInventory;
		public static Inventory SpellInventory;

		public static InventoryItem EquipedItem;
		public static InventoryItem EquipedWeapon;
		public static InventoryItem EquipedArmor;
		public static InventoryItem EquipedSpell;
		public static bool MagicSelected { get; private set; }
		#region Leveling
		public static Buff LevelingBaseBuff;
		public static Buff LevelingPointsBuff;
		public static int TotalSoulCount { get; private set; }
		public static int RequiredSoulsForLevel { get; private set; }
		public static int AvailableSkillPoints;
		public static int AvailableAtributePoints;
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
			SetupInventorys();
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
			LevelingPointsBuff = new Buff()
			{
				Name = "LevelingSkillPoints",
				Quality = BuffType.Positive,
				Icon = null,
				BuffTime = 0,
				Permanent = true,
				DOTs = new DOT[0]
			};

			//Health, Mana, Endurance, PhysicalDamage, MagicalDamage, Defense
			int incremtingStats = System.Enum.GetNames(typeof(TargetBaseStat)).Length;

			LevelingBaseBuff.Effects = new Effect[incremtingStats];
			LevelingPointsBuff.Effects = new Effect[incremtingStats];
			for (int i = 0; i < incremtingStats; i++)
			{
				LevelingBaseBuff.Effects[i] = LevelingPointsBuff.Effects[i] =
					new Effect
					{
						Amount = 0,
						Percent = false,
						TargetBaseStat = (TargetBaseStat)i
					};
			}

			AddBuff(LevelingBaseBuff, this);
			AddBuff(LevelingPointsBuff, this);

			EntitieLevel = 0;
			RequiredSoulsForLevel = PlayerSettings.LevelSettings.curve.GetRequiredSouls(EntitieLevel);
			TotalSoulCount = 0;
			AvailableSkillPoints = 0;
			AvailableAtributePoints = 0;
			//Safest way to ensure everyhting is correct is by adding 0 Souls to our current count
			AddSouls(0);
		}
		private void SetupInventorys()
		{
			MagicSelected = false;

			ItemInventory = new Inventory(24);
			WeaponInventory = new Inventory(8);
			ArmorInventory = new Inventory(8);
			SpellInventory = new Inventory(8);

			ItemInventory.InventoryChanged += UpdateEquipedItems;
			WeaponInventory.InventoryChanged += UpdateEquipedItems;
			ArmorInventory.InventoryChanged += UpdateEquipedItems;
			SpellInventory.InventoryChanged += UpdateEquipedItems;
		}
		protected override void EntitieUpdate()
		{
			if (GameController.GameIsPaused)
			{
				if (TargetedEntitie)
					TargetedEntitie = null;
				return;
			}

			RegenEndurance();
			CameraControl();
			MovementControl();
			AttackControl();
			TargetEnemyControl();
			PositionHeldWeapon();
			ItemUseControll();
			BlockControl();

			//Inventory Cooldowns
			ItemInventory.UpdateCooldown();
			WeaponInventory.UpdateCooldown();
			ArmorInventory.UpdateCooldown();
			SpellInventory.UpdateCooldown();
		}
		protected override void EntitieFixedUpdate()
		{
			PositionHeldWeapon();
			TurnControl();
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

			if (equipedWeapon != null)
			{
				heldWeapon = Instantiate(equipedWeapon.weaponPrefab);
				heldWeapon.Setup();
			}
		}
		private void SetupEndurance()
		{
			trueEnduranceAmount = PlayerSettings.EnduranceBars * PlayerSettings.EndurancePerBar;
			totalEnduranceContainers = PlayerSettings.EnduranceBars;
			curEndurance = useableEndurance;
		}
		protected void PositionHeldWeapon()
		{
			Vector3 worldOffset = equipedWeapon.weaponHandleOffset.x * rightHand.right +
										equipedWeapon.weaponHandleOffset.y * rightHand.up +
										equipedWeapon.weaponHandleOffset.z * rightHand.forward;
			heldWeapon.transform.position = rightHand.position + worldOffset;
			heldWeapon.transform.rotation = rightHand.rotation;
		}
		#endregion
		#region Endurance Control
		public void ChangeEndurance(ChangeInfo change)
		{
			ChangeInfo.ChangeResult changeResult = new ChangeInfo.ChangeResult(change, this, false);
			if (changeResult.finalChangeAmount > 0)
			{
				recentlyUsedEndurance = true;
				usedEnduranceCooldown = PlayerSettings.EnduranceRegenDelayAfterUse;
			}

			curEndurance -= changeResult.finalChangeAmount;
			curEndurance = Mathf.Clamp(curEndurance, 0, useableEndurance);
		}
		public void UpdateEnduranceStat()
		{
			totalEnduranceContainers = (int)(trueEnduranceAmount / PlayerSettings.EndurancePerBar);
			if (curEndurance > useableEndurance)
				curEndurance = useableEndurance;
		}
		private void RegenEndurance()
		{
			if (recentlyUsedEndurance)
			{
				usedEnduranceCooldown -= Time.deltaTime;
				if (usedEnduranceCooldown <= 0)
				{
					usedEnduranceCooldown = 0;
					recentlyUsedEndurance = false;
				}
			}

			if (curEndurance < useableEndurance - PlayerSettings.EndurancePerBar)
			{
				lockedEndurance += PlayerSettings.EnduranceRegen * PlayerSettings.LockedEnduranceRegenMutliplier * (curStates.Fighting ? PlayerSettings.EnduranceRegenInCombat : 1) * Time.deltaTime;
				if(lockedEndurance > PlayerSettings.EndurancePerBar)
				{
					curEndurance += PlayerSettings.EndurancePerBar;
					lockedEndurance -= PlayerSettings.EndurancePerBar;
				}
			}
			else
			{
				lockedEndurance = 0;
			}

			if (!recentlyUsedEndurance && curEndurance < useableEndurance)
			{
				curEndurance += PlayerSettings.EnduranceRegen * (curStates.Fighting ? PlayerSettings.EnduranceRegenInCombat : 1) * Time.deltaTime;
				if (curEndurance > useableEndurance)
					curEndurance = useableEndurance;
			}
		}
		#endregion
		#region Movement
		private void MovementControl()
		{
			UpdateAcceleration();
			UpdateMovementSpeed();

			if (IsStunned)
			{
				curAcceleration = 0;
				return;
			}

			JumpControl();
			PlayerMoveControl();
			DodgeControl();
		}
		private void UpdateAcceleration()
		{
			if (curStates.Moving && !curStates.Turning)
			{
				float clampedIntent = Mathf.Clamp01(intendedAcceleration);
				if (curAcceleration < clampedIntent)
				{
					if (SelfSettings.MoveAcceleration > 0)
						curAcceleration = Mathf.Min(clampedIntent, curAcceleration + intendedAcceleration * Time.fixedDeltaTime / SelfSettings.MoveAcceleration / SelfSettings.EntitieMass * (curStates.Grounded ? 1 : SelfSettings.InAirAccelerationMultiplier));
					else
						curAcceleration = clampedIntent;
				}
			}
			else //decelerate
			{
				if (curAcceleration > 0)
				{
					if (SelfSettings.NoMoveDeceleration > 0)
						curAcceleration = Mathf.Max(0, curAcceleration - Time.fixedDeltaTime / SelfSettings.NoMoveDeceleration / SelfSettings.EntitieMass * (curStates.Grounded ? 1 : SelfSettings.InAirAccelerationMultiplier));
					else
						curAcceleration = 0;
				}
			}
		}
		private void UpdateMovementSpeed()
		{
			bool turning = curStates.Turning;
			bool moving = curStates.Moving;
			bool running = curStates.Running;

			//If the Player doesnt move intentionally but is in run mode, then stop the run mode
			if (running && (!moving || isBlocking))
				curStates.Running = running = false;

			//Set the animation state to either Turning, Walking or Running
			animationControl.SetBool("Turning", turning);
			animationControl.SetBool("Walking", !turning && curAcceleration > 0 && !(running && curAcceleration > RUN_ANIM_THRESHOLD));
			animationControl.SetBool("Running", !turning && curAcceleration > 0 && (running && curAcceleration > RUN_ANIM_THRESHOLD));

			//We find out in which direction the Entitie should move according to its movement
			float baseTargetSpeed = curWalkSpeed * (curStates.Running ? SelfSettings.RunSpeedMultiplicator : 1) * curAcceleration;
			curMoveForce = baseTargetSpeed * (controllDirection.HasValue ? controllDirection.Value : transform.forward);

			animationControl.SetFloat("Runspeed", baseTargetSpeed);
			//Now combine those forces as the current speed
			body.velocity = curVelocity + curMoveForce;
		}
		private void TurnControl()
		{
			float turnAmount = Time.fixedDeltaTime * SelfSettings.TurnSpeed * (curStates.Grounded ? 1 : SelfSettings.InAirTurnSpeedMultiplier);
			float normalizedDif = Mathf.Abs(curRotation - intendedRotation) / NON_TURNING_THRESHOLD;
			turnAmount *= Mathf.Min(normalizedDif * LERP_TURNING_AREA, 1);
			curRotation = Mathf.MoveTowardsAngle(curRotation, intendedRotation, turnAmount);

			curStates.Turning = normalizedDif > 1;

			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
														curRotation,
														transform.localEulerAngles.z);
		}
		private void JumpControl()
		{
			if (InputController.Jump.Down && curStates.Grounded)
			{
				if (curEndurance >= PlayerSettings.JumpEnduranceCost)
				{
					Jump();
					ChangeEndurance(new ChangeInfo(this, CauseType.Magic, PlayerSettings.JumpEnduranceCost, false));
				}
			}
		}
		private void PlayerMoveControl()
		{
			//Where is the player Pointing the Joystick at?
			Vector2 inputDirection = InputController.PlayerMove;

			bool moving = inputDirection != Vector2.zero;
			curStates.Moving = moving;

			if (TargetedEntitie)
			{
				Vector3 direction = (TargetedEntitie.actuallWorldPosition - actuallWorldPosition).normalized;
				float hAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg -90;

				intendedRotation = -hAngle;
			}

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
					ChangeEndurance(new ChangeInfo(this, CauseType.Magic, runCost, false));
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
					ChangeEndurance(new ChangeInfo(this, CauseType.Magic, runCost, false));
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
				intendedRotation = -(Mathf.Atan2(controllDirection.Value.z, controllDirection.Value.x) * Mathf.Rad2Deg - 90);
		}
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
				ChangeEndurance(new ChangeInfo(this, CauseType.Magic, PlayerSettings.DodgeEnduranceCost, false));
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
				targetAngle = -(Mathf.Atan2(controllDirection.Value.z, controllDirection.Value.x) * Mathf.Rad2Deg - 90);
			}

			Vector3 newForce = new Vector3(Mathf.Sin(targetAngle * Mathf.Deg2Rad), 0, Mathf.Cos(targetAngle * Mathf.Deg2Rad)) * PlayerSettings.DodgePower * curWalkSpeed;

			entitieForceController.ApplyForce(newForce, 1 / PlayerSettings.DodgeDuration);
			EffectUtils.BlurScreen(0.8f, PlayerSettings.DodgeDuration * 1.6f, 6);

			//Create a copy of the player model
			Material dodgeMaterialInstance = Instantiate(PlayerSettings.DodgeModelMaterial);
			Transform modelCopy = Instantiate(modelTransform, Storage.ParticleStorage);
			WeaponController weaponCopy = Instantiate(heldWeapon, Storage.ParticleStorage);

			modelCopy.transform.position = modelTransform.position;
			modelCopy.transform.localScale = modelTransform.lossyScale;
			modelCopy.transform.rotation = modelTransform.rotation;

			Color baseColor = dodgeMaterialInstance.color;
			Color alphaPart = new Color(0, 0, 0, dodgeMaterialInstance.color.a);
			baseColor.a = 0;

			weaponCopy.enabled = false;
			for (int i = 0; i < weaponCopy.weaponHitboxes.Length; i++)
			{
				weaponCopy.weaponHitboxes[i].enabled = false;
			}

			ApplyMaterialToAllChildren(modelCopy);
			ApplyMaterialToAllChildren(weaponCopy.transform);

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
			Destroy(weaponCopy.gameObject);

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
				List<(Entitie, float)> possibleTargets = new List<(Entitie, float)>();

				for (int i = 0; i < AllEntities.Count; i++)
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
		#region ItemUseControl
		private void ItemUseControll()
		{
			if (InputController.UseItem.Down && EquipedItem != null)
			{
				if(EquipedItem.useCooldown <= 0)
					EquipedItem.data.Use(EquipedItem, this, ItemInventory);
			}
			if (InputController.PhysicalMagicSwap.Down)
			{
				MagicSelected = !MagicSelected;
			}
		}
		private void UpdateEquipedItems()
		{
			if (EquipedItem != null && EquipedItem.stackSize <= 0)
				EquipedItem = null;
			if (EquipedWeapon != null && EquipedWeapon.stackSize <= 0)
				EquipedWeapon = null;
			if (EquipedSpell != null && EquipedSpell.stackSize <= 0)
				EquipedSpell = null;
			if (EquipedArmor != null && EquipedArmor.stackSize <= 0)
				EquipedArmor = null;
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

			bool normalAttack = InputController.Attack.Down || Input.GetKeyDown(KeyCode.K);
			if (normalAttack && Interactable.MarkedInteractable && !(attackInteractCooldown > 0))
			{
				if (Interactable.MarkedInteractable.TryInteract())
					return;
			}

			if (equipedWeapon == null || isCurrentlyAttacking || (!normalAttack && !InputController.HeavyAttack.Down))
				return;

			int state = 0;
			state += InputController.Attack.Down ? 0 : 4; //If we are here either heavy attack or normal attack was pressed
			state += curStates.Running ? 1 : 0; //Running => Sprint attack
			state += !curStates.Grounded ? 2 : 0; //In air => Jump attack

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
					if (!applyForceAfterCustomDelay)
						ApplyAttackForces();
					heldWeapon.Active = true;
				}

				if (applyForceAfterCustomDelay && forceApplyDelay == 0)
				{
					ApplyAttackForces();
				}

				while (animTime > 0 || activeAttack.animationInfo.haltAnimationTillCancel)
				{
					yield return new WaitForEndOfFrame();
					animTime -= Time.deltaTime;

					if (chargeDelay > 0)
					{
						chargeDelay -= Time.deltaTime;

						if (chargeDelay <= 0)
						{
							if (targetAttack.velocityEffect.applyForceAfterAnimationCharge)
								ApplyAttackForces();
							heldWeapon.Active = true;
						}
					}

					if (applyForceAfterCustomDelay && forceApplyDelay > 0)
					{
						forceApplyDelay -= Time.deltaTime;
						if (forceApplyDelay <= 0)
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
			else if (equipedWeapon.weaponAttackStyle[attackIndex].attacks.Length > innerIndex + 1)
			{
				comboDelay = equipedWeapon.weaponAttackStyle[attackIndex].delays[innerIndex];
			}

			//Find out if the player can afford the attack
			if (style != null)
			{
				float cost = style.info.enduranceMultiplier * equipedWeapon.baseEnduranceDrain;
				if (curEndurance >= cost)
				{
					ChangeEndurance(new ChangeInfo(this, CauseType.Magic, cost, false));
				}
				else
				{
					style = null;
					comboDelay = null;
				}
			}

			return (style, comboDelay);
		}
		private bool CheckForCancelCondition()
		{
			int requiredGroundState = (int)activeAttack.animationInfo.cancelWhenOnGround - 1;
			bool groundStateAchieved = (curStates.Grounded ? 0 : 1) == requiredGroundState;

			if (groundStateAchieved && !activeAttack.animationInfo.bothStates)
				return true;

			if (!groundStateAchieved && activeAttack.animationInfo.bothStates)
				return false;

			int requiredSprintState = (int)activeAttack.animationInfo.cancelWhenSprinting - 1;
			bool sprintStateAchieved = (curStates.Running ? 0 : 1) == requiredSprintState;

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
			EffectUtils.BlurScreen(1, Mathf.Infinity, 5);
			Alive = false;
			if (TargetedEntitie)
				TargetedEntitie = null;
			base.Death();
		}
		#region IFrames
		protected override void ReceivedHealthDamage(ChangeInfo baseChange, ChangeInfo.ChangeResult resultInfo)
		{
			if(baseChange.attacker != this)
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
			const int startRotationAt = 1;
			const int enumOffset = (int)TargetBaseStat.PhysicalDamage;

			TotalSoulCount += amount;
			while (TotalSoulCount >= RequiredSoulsForLevel)
			{
				EntitieLevel++;
				RequiredSoulsForLevel = PlayerSettings.LevelSettings.curve.GetRequiredSouls(EntitieLevel);
				AvailableAtributePoints += PlayerSettings.LevelSettings.AttributePointsPerLevel;
				AvailableSkillPoints += PlayerSettings.LevelSettings.SkillPointsPerLevel;

				float baseRotationAmount = (EntitieLevel / 10) * PlayerSettings.LevelSettings.PerTenLevelsBasePoints + 1;
				int rotationIndex = (startRotationAt + EntitieLevel) % 3 + enumOffset;
				for (int i = enumOffset; i < enumOffset + 3; i++)
				{
					LevelingBaseBuff.Effects[i].Amount += baseRotationAmount;
					if (i == rotationIndex)
						LevelingBaseBuff.Effects[i].Amount += PlayerSettings.LevelSettings.RotationExtraPoints;
				}
				RecalculateBuffs();

				//Inform all script that need the information that the player leveled
				EventManager.PlayerLevelupInvoke();
			}
			levelDisplay.text = (EntitieLevel + 1).ToString();
			soulCount.text = TotalSoulCount + " / " + RequiredSoulsForLevel;
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
						value += PlayerSettings.EnduranceBars * PlayerSettings.EndurancePerBar;
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
				case TargetBaseStat.JumpHeight:
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