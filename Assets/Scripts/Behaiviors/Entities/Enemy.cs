using System.Collections;
using System.Collections.Generic;
using EoE.Information;
using UnityEngine;

namespace EoE.Entities
{
	public abstract class Enemy : Entitie
	{
		private const float REACHED_WANDERPOINT_THRESHOLD = 0.05f;
		private const float REACHED_LOOKAROUND_THRESHOLD = 0.05f;
		private const float STUCK_THRESHOLD = 0.25f;

		private const float FORWARD_CAN_STAND_DISTANCE = 0.5f;

		public override EntitieSettings SelfSettings => enemySettings;
		public abstract EnemySettings enemySettings { get; }

		private float stuckSince;
		private bool cantReachPlayer;

		private Vector3 lastConfirmedPlayerPos;
		protected Vector3 chaseDirection;
		private bool chasingPlayer;
		private float chaseInterestAmount;

		//Wandering
		private Vector3 originalSpawnPosition;
		private Vector2 wanderPosition;
		private Vector3 wanderDirection;
		private float wanderWait;
		private bool reachedWanderPoint;

		//Look around
		private bool reachedLookAroundDir;
		private float lookAroundWait;
		private float lostVisualsOnPlayerLookaroundTimer;
		private Vector3 lookAroundTargetDir;

		protected Player player => Player.Instance;
		protected override void Start()
		{
			base.Start();
			originalSpawnPosition = actuallWorldPosition;
			GetNewWanderPosition();
			GetNewLookDirection();
		}
		protected override void EntitieUpdate()
		{
			base.EntitieUpdate();
			if (curStates.IsInCombat && !cantReachPlayer)
			{
				ChasePlayer();
			}
			else 
			{ 
				SearchForPlayer();
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
					ChasePlayer();
					chaseInterestAmount -= Time.deltaTime;
					if (chaseInterestAmount <= 0)
					{
						lostVisualsOnPlayerLookaroundTimer = enemySettings.LookAroundAfterLostPlayerTime;
						GameController.Instance.DisplayInfoText(transform.position, GameController.CurrentGameSettings.StandardTextColor, Vector3.up, "?", 2);
						chasingPlayer = false;
					}
				}
			}
		}
		private void SearchForPlayer()
		{
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
					GameController.Instance.DisplayInfoText(transform.position, GameController.CurrentGameSettings.StandardTextColor, Vector3.up, "!", 2);

				chasingPlayer = true;
				cantReachPlayer = true;
				chaseInterestAmount = enemySettings.ChaseInterest;
				lastConfirmedPlayerPos = player.transform.position;
			}
		}
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
					if (curEndurance >= enemySettings.JumpEnduranceCost && CanJumpUp())
					{
						if (curStates.IsGrounded)
						{
							stuckSince = 0;
							curEndurance -= enemySettings.JumpEnduranceCost;
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
		private bool IsStuck()
		{
			float intendedSpeed = curWalkSpeed * (curStates.IsRunning ? SelfSettings.RunSpeedMultiplicator : 1) * curAcceleration;
			intendedSpeed *= intendedSpeed;
			float actuallSpeed = body.velocity.sqrMagnitude;

			if (!CouldStand(0.01f))
				stuckSince += Time.deltaTime;
			else if(stuckSince > 0)
				stuckSince = 0;

			return stuckSince >= STUCK_THRESHOLD;
		}
		private bool CanJumpUp()
		{
			if (curEndurance < enemySettings.JumpEnduranceCost)
				return false;
			float maxJump = ((enemySettings.JumpPower.y * enemySettings.JumpPower.y) / (-2 * Physics.gravity.y)) * 0.9f;
			return CouldStand(maxJump + lowestPos);
		}
		private bool CouldStand(float verticalPos)
		{
			Vector3 testPos = coll.bounds.center + new Vector3(0, verticalPos, 0) + coll.transform.forward * FORWARD_CAN_STAND_DISTANCE;
			return !Physics.CheckBox(testPos, coll.bounds.extents, transform.rotation, ConstantCollector.TERRAIN_LAYER);
		}
		protected virtual void ChasePlayer()
		{
			float distance = (player.transform.position - transform.position).magnitude;
			Vector3 playerDir = (player.transform.position - transform.position)/ distance;
			chaseDirection = new Vector3(playerDir.x, 0, playerDir.z).normalized;

			if (distance > enemySettings.AttackRange)
			{
				curStates.IsMoving = true;
				TurnTo(chaseDirection);
				UpdateAcceleration();

				if (IsStuck())
				{
					if (curEndurance >= enemySettings.JumpEnduranceCost && CanJumpUp())
					{
						if (curStates.IsGrounded)
						{
							curEndurance -= enemySettings.JumpEnduranceCost;
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
						GameController.Instance.DisplayInfoText(transform.position, GameController.CurrentGameSettings.StandardTextColor, Vector3.up, "...", 1.5f);
						chasingPlayer = false;
					}
				}
			}
			else
			{
				CombatBehavior();
			}
		}
		protected abstract void CombatBehavior();
	}
}