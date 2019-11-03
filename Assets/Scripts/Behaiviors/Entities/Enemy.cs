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
		private float NormalCosSightCone;
		private float FoundPlayerCosSightCone;
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

			NormalCosSightCone = Mathf.Cos(enemySettings.SightAngle * Mathf.Deg2Rad);
			FoundPlayerCosSightCone = Mathf.Cos(enemySettings.FoundPlayerSightAngle * Mathf.Deg2Rad);
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

			if (Vector3.Dot(playerDir, transform.forward) > (chasingPlayer ? FoundPlayerCosSightCone : NormalCosSightCone))
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
				if (!curStates.IsTurning)
				{
					reachedLookAroundDir = true;
					lookAroundWait = Random.Range(enemySettings.LookAroundDelayMin, enemySettings.LookAroundDelayMax);
				}
			}
		}
		private void GetNewLookDirection()
		{
			intendedRotation = Random.value * 360;
			reachedLookAroundDir = false;
		}
		#endregion
		#region WanderAround
		private void WanderAround()
		{
			curStates.IsMoving = !reachedWanderPoint;
			if (reachedWanderPoint)
			{
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
				TargetPosition(new Vector3(wanderPosition.x, 0, wanderPosition.y));
				intendedAcceleration = GameController.CurrentGameSettings.EnemyWanderingUrgency;

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

				Vector2 planePos = new Vector2(actuallWorldPosition.x, actuallWorldPosition.z);

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
			TargetPosition(lastConfirmedPlayerPos);
			
			if (distance > enemySettings.AttackRange)
			{
				curStates.IsMoving = true;
				intendedAcceleration = 1;

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
	}
}