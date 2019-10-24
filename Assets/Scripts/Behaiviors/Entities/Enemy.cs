using System.Collections;
using System.Collections.Generic;
using EoE.Information;
using EoE.Utils;
using EoE.Events;
using EoE.UI;
using UnityEngine;

namespace EoE.Entities
{
	public abstract class Enemy : Entitie
	{
		#region Fields
		//Constants
		private const float REACHED_WANDERPOINT_THRESHOLD	= 0.5f;
		private const float REACHED_LOOKAROUND_THRESHOLD	= 0.05f;
		private const float STUCK_THRESHOLD					= 0.25f;
		private const float FORWARD_CAN_STAND_DISTANCE		= 0.25f;

		//Enemy States
		protected bool cantReachPlayer;
		protected float stuckSince;
		protected float wanderWait;
		protected float lookAroundWait;
		protected float chaseInterestAmount;
		protected float lostVisualsOnPlayerLookaroundTimer;

		//Chasing
		protected bool chasingPlayer;
		protected Vector3 lastConfirmedPlayerPos;
		protected Vector3 chaseDirection;

		//Wandering
		protected bool reachedWanderPoint;
		protected Vector3 originalSpawnPosition;
		protected Vector2 wanderPosition;
		protected Vector3 wanderDirection;

		//Look around
		protected bool reachedLookAroundDir;
		protected Vector3 lookAroundTargetDir;

		//Other
		private EnemyHealthBar healthBar;

		//Getter Helpers
		protected Player player => Player.Instance;
		public override EntitieSettings SelfSettings => enemySettings;
		public abstract EnemySettings enemySettings { get; }
		#endregion
		#region Basic Monobehaivior
		protected override void Start()
		{
			base.Start();
			originalSpawnPosition = actuallWorldPosition;
			GetNewWanderPosition();
			GetNewLookDirection();
			EventManager.PlayerDiedEvent += PlayerDied;

			healthBar = Instantiate(GameController.CurrentGameSettings.EnemyHealthBarPrefab, GameController.Instance.enemyHealthBarStorage);
			UpdateHealthBar();
		}
		private void PlayerDied(Entitie killer)
		{
			curStates.IsInCombat = false;
			chasingPlayer = false;
		}
		protected override void Update()
		{
			base.Update();
			DecideOnBehavior();
			UpdateHealthBar();
		}
		private void DecideOnBehavior()
		{
			SearchForPlayer();
			if (curStates.IsInCombat && !cantReachPlayer)
			{
				ChasePlayer();
			}
			else
			{
				if (!chasingPlayer)
				{
					if (lostVisualsOnPlayerLookaroundTimer > 0)
					{
						lostVisualsOnPlayerLookaroundTimer -= Time.deltaTime;
						LookAround();
					}
					else if (enemySettings.WanderingFactor > 0 || (transform.position - originalSpawnPosition).sqrMagnitude > REACHED_WANDERPOINT_THRESHOLD)
						WanderAround();
					else
						LookAround();
				}
				else
				{
					if (!ChasePlayer())
						return;

					chaseInterestAmount -= Time.deltaTime;
					if (chaseInterestAmount <= 0)
					{
						lostVisualsOnPlayerLookaroundTimer = enemySettings.LookAroundAfterLostPlayerTime;
						EffectUtils.DisplayInfoText(transform.position, GameController.CurrentGameSettings.StandardTextColor, Vector3.up, "?", 2);
						chasingPlayer = false;
					}
				}
			}
		}
		#endregion
		#region Idle Behaivior
		private void SearchForPlayer()
		{
			if (!Player.Alive)
				return;

			float sqrDistance = (player.transform.position - transform.position).sqrMagnitude;
			if (sqrDistance > (enemySettings.SightRange * enemySettings.SightRange))
				return;

			Vector3 playerDir = (player.transform.position - transform.position).normalized;
			bool terrainBlocksSight = Physics.Raycast(transform.position, playerDir, Mathf.Sqrt(sqrDistance), ConstantCollector.TERRAIN_LAYER);

			if (terrainBlocksSight)
				return;

			float angleToPlayer = Mathf.Acos(Vector3.Dot(playerDir, transform.forward)) * Mathf.Rad2Deg;
			
			if(angleToPlayer < (chasingPlayer ? enemySettings.FoundPlayerSightAngle : enemySettings.SightAngle))
			{
				if(!chasingPlayer)
					EffectUtils.DisplayInfoText(transform.position, GameController.CurrentGameSettings.StandardTextColor, Vector3.up, "!", 2);

				chasingPlayer = true;
				cantReachPlayer = false;
				chaseInterestAmount = enemySettings.ChaseInterest;
				lastConfirmedPlayerPos = player.transform.position;
			}
		}
		#region LookAround
		private void LookAround()
		{
			curStates.IsMoving = false;
			UpdateAcceleration();
			if (reachedLookAroundDir)
			{
				lookAroundWait -= Time.deltaTime;
				if(lookAroundWait <= 0)
				{
					GetNewLookDirection();
				}
			}
			else
			{
				float turnDistance = TurnTo(lookAroundTargetDir).Item2;
				if (turnDistance < REACHED_LOOKAROUND_THRESHOLD)
				{
					reachedLookAroundDir = true;
					lookAroundWait = Random.Range(enemySettings.LookAroundDelayMin, enemySettings.LookAroundDelayMax);
				}
			}
		}
		private void GetNewLookDirection()
		{
			float angle = Random.value * 360 * Mathf.Deg2Rad;
			lookAroundTargetDir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
			reachedLookAroundDir = false;
		}
		#endregion
		#region WanderAround
		private void WanderAround()
		{
			curStates.IsMoving = !reachedWanderPoint;
			if (reachedWanderPoint)
			{
				UpdateAcceleration();
				wanderWait -= Time.deltaTime;
				if(wanderWait <= 0)
				{
					GetNewWanderPosition();
				}
				LookAround();
				return;
			}
			else
			{
				wanderDirection = new Vector3(wanderPosition.x - transform.position.x, 0, wanderPosition.y - transform.position.z).normalized;
				TurnTo(wanderDirection);
				UpdateAcceleration(GameController.CurrentGameSettings.EnemyWanderingUrgency);

				if (IsStuck())
				{
					if (CanJumpUp())
					{
						if (curStates.IsGrounded)
						{
							stuckSince = 0;
							Jump();
						}
					}
					else
					{
						reachedWanderPoint = true;
					}
				}

				Vector2 planePos = new Vector2(transform.position.x, transform.position.z);

				if((planePos - wanderPosition).sqrMagnitude < REACHED_WANDERPOINT_THRESHOLD)
				{
					reachedWanderPoint = true;
					wanderWait = Random.Range(enemySettings.WanderingDelayMin, enemySettings.WanderingDelayMax);
					GetNewLookDirection();
				}
			}
		}
		private void GetNewWanderPosition()
		{
			Vector3 target = originalSpawnPosition + Random.insideUnitSphere * enemySettings.WanderingFactor;
			wanderPosition = new Vector2(target.x, target.z);
			reachedWanderPoint = false;
		}
		#endregion
		#region Resolve Issues
		private bool IsStuck()
		{
			if (CheckBoxAtHeight(0.05f))
				stuckSince += Time.deltaTime;
			else if(stuckSince > 0)
				stuckSince = 0;

			return stuckSince >= STUCK_THRESHOLD;
		}
		private bool CanJumpUp()
		{
			float maxJump = ((enemySettings.JumpPower.y * enemySettings.JumpPower.y) / (-2 * Physics.gravity.y)) * 0.9f;
			return !CheckBoxAtHeight(maxJump + lowestPos);
		}
		private bool CheckBoxAtHeight(float verticalPos)
		{
			Vector3 testPos = coll.bounds.center + new Vector3(0, verticalPos, 0) + coll.transform.forward * FORWARD_CAN_STAND_DISTANCE;
			return Physics.CheckBox(testPos, coll.bounds.extents, transform.rotation, ConstantCollector.TERRAIN_LAYER);
		}
		#endregion
		#endregion
		#region Aggresive Behavior
		private bool ChasePlayer()
		{
			float distance = (lastConfirmedPlayerPos - transform.position).magnitude;
			Vector3 playerDir = (lastConfirmedPlayerPos - transform.position)/ distance;
			chaseDirection = new Vector3(playerDir.x, 0, playerDir.z).normalized;

			if (distance > enemySettings.AttackRange)
			{
				curStates.IsMoving = true;
				TurnTo(chaseDirection);
				UpdateAcceleration();

				if (IsStuck())
				{
					if (CanJumpUp())
					{
						if (curStates.IsGrounded)
						{
							Jump();
							stuckSince = 0;
						}
					}
					else
					{
						cantReachPlayer = true;
						chaseInterestAmount = 0;
						stuckSince = 0;
						lostVisualsOnPlayerLookaroundTimer = enemySettings.LookAroundAfterLostPlayerTime;
						EffectUtils.DisplayInfoText(transform.position, GameController.CurrentGameSettings.StandardTextColor, Vector3.up, "...", 1.5f);
						chasingPlayer = false;

						return false;
					}
				}
			}
			else
			{
				CombatBehavior(distance);
			}
			return true;
		}
		protected abstract void CombatBehavior(float distance);
		#endregion
		#region Other
		protected virtual void UpdateHealthBar()
		{
			if (curStates.IsInCombat)
			{
				if(!healthBar.gameObject.activeInHierarchy)
					healthBar.gameObject.SetActive(true);

				healthBar.Value = curHealth / curMaxHealth;
				healthBar.Position = PlayerCameraController.PlayerCamera.WorldToScreenPoint(new Vector3(coll.bounds.center.x, highestPos, coll.bounds.center.z)) + new Vector3(0,healthBar.Height);
			}
			else
			{
				if (healthBar.gameObject.activeInHierarchy)
					healthBar.gameObject.SetActive(false);
			}
		}
		protected override void Death()
		{
			Destroy(healthBar.gameObject);
			base.Death();
		}
		#endregion
	}
}