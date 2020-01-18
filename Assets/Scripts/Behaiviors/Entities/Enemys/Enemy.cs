using EoE.Events;
using EoE.Information;
using UnityEngine;
using UnityEngine.AI;

namespace EoE.Entities
{
	public abstract class Enemy : Entity
	{
		#region Fields
		//Constants
		private const float CLOSEST_NAVMESH_POINT_DIST = 20;
		private const float REACH_DESTINATION_MIN = 0.2f;

		private const float REACHED_LOOK_ANGLE_THRESHOLD = 1;

		//Inspector Variables
		[SerializeField] protected Rigidbody body = default;
		public NavMeshAgent agent = default;

		protected Vector3 originalSpawnPosition;
		protected NavMeshPath curPath;

		//Chasing
		private bool chasingPlayer;
		private float normalCosSightCone;
		private float foundPlayerCosSightCone;
		private float remainingInvestigationTime;

		//Wandering
		private float wanderingCooldown;

		//Lookaround
		private float lookAroundCooldown;

		//Last infos
		protected float lastSeenPlayer;
		protected Vector3 lastPlayerSpeed;
		protected Vector3 lastConfirmedPlayerPos;
		protected Vector3 lostChaseInterestPos;

		//Getter Helpers
		public override Vector3 CurVelocity => new Vector3(impactForce.x, 0, impactForce.y) + entitieForceController.currentTotalForce;
		public override EntitySettings SelfSettings => enemySettings;
		public abstract EnemySettings enemySettings { get; }
		public bool PlayerInAttackRange { get; private set; }
		protected Vector3 GuessedPlayerPosition => lastConfirmedPlayerPos + new Vector3(lastPlayerSpeed.x, 0, lastPlayerSpeed.z) * lastSeenPlayer;

		//Behaivior
		protected bool behaviorSimpleStop;
		protected bool isIdle => !(chasingPlayer || remainingInvestigationTime > 0);
		protected Vector3? overrideTargetPosition;
		protected Vector3? pointOfInterest;
		#endregion
		#region Basic Monobehaivior
		protected override void Start()
		{
			base.Start();
			EventManager.PlayerDiedEvent += PlayerDied;
		}
		protected override void FullEntitieReset()
		{
			base.FullEntitieReset();
			SetupNavMeshAgent();

			originalSpawnPosition = actuallWorldPosition;
			normalCosSightCone = Mathf.Cos(Mathf.Min(360, enemySettings.SightAngle) * Mathf.Deg2Rad);
			foundPlayerCosSightCone = Mathf.Cos(Mathf.Min(360, enemySettings.FoundPlayerSightAngle) * Mathf.Deg2Rad);

			wanderingCooldown = Random.Range(enemySettings.WanderingDelayMin, enemySettings.WanderingDelayMax);
			lookAroundCooldown = Random.Range(enemySettings.LookAroundDelayMin, enemySettings.LookAroundDelayMax);
		}
		private void SetupNavMeshAgent()
		{
			curPath = new NavMeshPath();
			agent.radius = selfColliderType == ColliderType.Box ? Mathf.Max(coll.bounds.extents.x, coll.bounds.extents.z) : (selfColliderType == ColliderType.Capsule ? (coll as CapsuleCollider).radius : (coll as SphereCollider).radius);

			agent.angularSpeed = enemySettings.TurnSpeed * GameController.CurrentGameSettings.IdleMovementUrgency;
			agent.acceleration = enemySettings.MoveAcceleration * GameController.CurrentGameSettings.IdleMovementUrgency;
			agent.speed = enemySettings.WalkSpeed * GameController.CurrentGameSettings.IdleMovementUrgency;

			agent.stoppingDistance = REACH_DESTINATION_MIN;
		}
		protected override void Update()
		{
			base.Update();
			DecideOnBehaivior();
			EnforceKnockback();
		}
		#endregion
		#region Behaivior
		private void DecideOnBehaivior()
		{
			if (Player.Existant)
			{
				bool prevInRange = PlayerInAttackRange;
				float sqrPlayerDist = (Player.Instance.actuallWorldPosition - actuallWorldPosition).sqrMagnitude;
				PlayerInAttackRange = sqrPlayerDist < (enemySettings.AttackRange * enemySettings.AttackRange);

				if (curStates.Fighting)
				{
					chasingPlayer = true;
					if (CheckIfCanSeeEntitie(transform, Player.Instance, true))
					{
						RefreshDataOnPlayer(false);

						if (!prevInRange && PlayerInAttackRange)
						{
							PlayerJustEnteredAttackRangeBase();
						}
					}
				}

				if (chasingPlayer)
				{
					targetPosition = Vector3.Lerp(targetPosition ?? GuessedPlayerPosition, GuessedPlayerPosition, Time.deltaTime * enemySettings.PlayerTrackSpeed);
					
					if (!prevInRange && PlayerInAttackRange)
					{
						PlayerJustEnteredAttackRangeBase();
					}
				}
			}
			else
			{
				chasingPlayer = false;
				PlayerInAttackRange = false;
				targetPosition = null;
			}
			UpdateAgentSettings();

			if (PlayerInAttackRange && chasingPlayer && !IsStunned)
				InRangeBehaiviorBase();

			bool cantMove = IsStunned || IsMovementStopped;
			if (cantMove)
			{
				body.isKinematic = false;
				body.velocity = CurVelocity;
			}

			if(cantMove || behaviorSimpleStop)
			{
				SetAgentState(false);
				if(!IsStunned)
					LookAtTarget();
				return;
			}

			body.isKinematic = true;
			SetAgentState(true);

			if (CanSeePlayer())
			{
				RefreshDataOnPlayer(false);

				//Find a path
				if (TryWalkToTargetPosition() && (agent.remainingDistance < agent.stoppingDistance))
				{
					LookAtTarget();
				}
			}
			else if (chasingPlayer)
			{
				lastSeenPlayer += Time.deltaTime;

				if(!TryWalkToTargetPosition())
				{
					//We cant reach the player so we force interest loss on chasing
					lastSeenPlayer = enemySettings.ChaseInterest;
				}

				//The Enemy didnt see the player for too long, therefore it will stop here and investigate the area
				//If it doesnt find the player it will return back to its original spawn
				if (lastSeenPlayer >= enemySettings.ChaseInterest || agent.isStopped)
				{
					chasingPlayer = false;
					lostChaseInterestPos = actuallWorldPosition;
					remainingInvestigationTime = enemySettings.InvestigationTime;

					//Set Cooldowns for wander / look
					wanderingCooldown = Random.Range(enemySettings.WanderingDelayMin, enemySettings.WanderingDelayMax);
				}
			}
			else
			{
				if (remainingInvestigationTime > 0)
					remainingInvestigationTime -= Time.deltaTime;

				//Idle / Investigation behaivior
				if (!WanderAround())
				{
					LookAroundArea();
				}
			}

		}
		private void RefreshDataOnPlayer(bool fromCombatTrigger)
		{
			if(!fromCombatTrigger)
				StartCombat();
			chasingPlayer = true;
			lastConfirmedPlayerPos = Player.Instance.actuallWorldPosition;
			lastPlayerSpeed = Player.Instance.CurVelocity;
			lastSeenPlayer = 0;
		}
		private bool TryWalkToTargetPosition()
		{
			//Prefers override if both are not null, if override is null we goto to the standard target, if both are null we dont do anything
			Vector3? pos = overrideTargetPosition ?? targetPosition; 
			if (pos.HasValue)
			{
				Vector3? destination = GetClosestPointOnNavmesh(pos.Value);
				if (destination.HasValue && agent.CalculatePath(destination.Value, curPath) && curPath.status != NavMeshPathStatus.PathInvalid)
				{
					agent.SetPath(curPath);
					agent.stoppingDistance = REACH_DESTINATION_MIN;

					bool reached = agent.remainingDistance < agent.stoppingDistance;
					SetAgentState(!reached);
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
		protected void EnforceKnockback()
		{
			agent.nextPosition += CurVelocity * Time.deltaTime;
		}
		private void UpdateAgentSettings()
		{
			if (isIdle)
			{
				agent.angularSpeed = enemySettings.TurnSpeed * GameController.CurrentGameSettings.IdleMovementUrgency;
				agent.acceleration = enemySettings.MoveAcceleration * GameController.CurrentGameSettings.IdleMovementUrgency;
				agent.speed = curWalkSpeed * GameController.CurrentGameSettings.IdleMovementUrgency;
			}
			else
			{
				agent.angularSpeed = enemySettings.TurnSpeed;
				agent.acceleration = enemySettings.MoveAcceleration;
				agent.speed = curWalkSpeed;
			}
		}
		private bool CanSeePlayer()
		{
			if (!Player.Existant)
				return false;

			Vector3 dif = Player.Instance.actuallWorldPosition - actuallWorldPosition;
			float sqrDist = dif.sqrMagnitude;
			float sqrSightDist = chasingPlayer ? enemySettings.FoundPlayerSightRange : enemySettings.SightRange;
			sqrSightDist *= sqrSightDist;

			//First check is the distance because it is the least performance costing
			if (sqrDist > sqrSightDist)
				return false;

			Vector3 direction = dif / Mathf.Sqrt(sqrDist);
			float cosAngle = Vector3.Dot(transform.forward, direction);

			//Then we check the dot product with precalculated cos angles
			if (cosAngle < (chasingPlayer ? foundPlayerCosSightCone : normalCosSightCone))
				return false;

			//Lastly we do a Low priority check if this entitie can see any part of the player
			return CheckIfCanSeeEntitie(transform, Player.Instance, true);
		}
		protected void SetAgentState(bool state)
		{
			bool stopped = !state; //Better readability
			if (stopped)
				agent.velocity = Vector3.zero;
			if (agent.enabled && agent.isStopped != stopped)
				agent.isStopped = stopped;
		}
		private bool WanderAround()
		{
			if (wanderingCooldown > 0)
			{
				wanderingCooldown -= Time.deltaTime;
				if (wanderingCooldown <= 0)
				{
					return CreateNewWanderPath();
				}
				return false;
			}

			bool reached = agent.remainingDistance < agent.stoppingDistance;
			SetAgentState(!reached);
			if (reached)
			{
				lookAroundCooldown = Random.Range(enemySettings.LookAroundDelayMin, enemySettings.LookAroundDelayMax);
				wanderingCooldown = lookAroundCooldown + Random.Range(enemySettings.WanderingDelayMin, enemySettings.WanderingDelayMax);
				return false;
			}

			return true;
		}
		private bool CreateNewWanderPath()
		{
			bool investigating = remainingInvestigationTime > 0;
			float wanderMaxRadius = investigating ? Mathf.Max(enemySettings.WanderingFactor, GameController.CurrentGameSettings.EnemyMinimumInvestigationArea) : enemySettings.WanderingFactor;
			Vector3 wanderOrigin = investigating ? lostChaseInterestPos : originalSpawnPosition;

			Vector3? destination = GetRandomNavmeshPoint(wanderMaxRadius, wanderOrigin);
			if (destination.HasValue && agent.CalculatePath(destination.Value, curPath) && curPath.status != NavMeshPathStatus.PathInvalid)
			{
				agent.SetPath(curPath);
				return true;
			}
			return false;
		}
		private void LookAroundArea()
		{
			if (IsRotationStopped)
				return;

			if (lookAroundCooldown > 0)
			{
				lookAroundCooldown -= Time.deltaTime;
				if (lookAroundCooldown <= 0)
				{
					GetNewLookAngle();
				}
				return;
			}

			//Turn the Enemy
			curRotation = Mathf.LerpAngle(curRotation, intendedRotation, Time.deltaTime * enemySettings.TurnSpeed / 180);
			transform.localEulerAngles = new Vector3(0, curRotation, 0);

			if (Mathf.Abs(Mathf.DeltaAngle(curRotation, intendedRotation)) < REACHED_LOOK_ANGLE_THRESHOLD)
			{
				lookAroundCooldown = Random.Range(enemySettings.LookAroundDelayMin, enemySettings.LookAroundDelayMax);
			}
		}
		private void GetNewLookAngle()
		{
			intendedRotation = Random.value * 360;
			curRotation = transform.localEulerAngles.y;
		}
		private void PlayerDied(Entity killer)
		{
			curStates.Fighting = false;
			chasingPlayer = false;
			EventManager.PlayerDiedEvent -= PlayerDied;
		}
		private void PlayerJustEnteredAttackRangeBase()
		{
			StartCombat();
			PlayerJustEnteredAttackRange();
		}
		protected virtual void PlayerJustEnteredAttackRange() { }
		private void InRangeBehaiviorBase()
		{
			StartCombat();
			InRangeBehaivior();
		}
		protected virtual void InRangeBehaivior() { }
		#endregion
		#region Helper Functions
		protected Vector3? GetRandomNavmeshPoint(float radius, Vector3 origin)
		{
			Vector3 worldPos = Random.insideUnitSphere * radius + origin;
			return GetClosestPointOnNavmesh(worldPos, radius);
		}
		protected Vector3? GetClosestPointOnNavmesh(Vector3 point, float radius = CLOSEST_NAVMESH_POINT_DIST)
		{
			NavMeshHit hit;
			if (NavMesh.SamplePosition(point, out hit, Mathf.Max(radius, coll.bounds.extents.y * 1.5f), 1))
			{
				return hit.position;
			}
			else
				return null;
		}
		protected void LookAtTarget()
		{
			if (IsRotationStopped || IsStunned)
				return;

			Vector3? aimPos = pointOfInterest ?? (overrideTargetPosition ?? targetPosition);
			if(!aimPos.HasValue)
				return;

			Vector2 direction = new Vector2(aimPos.Value.x - actuallWorldPosition.x, aimPos.Value.z - actuallWorldPosition.z).normalized;
			float rotation = -Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

			transform.localEulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(transform.localEulerAngles.y, rotation, Time.deltaTime * enemySettings.TurnSpeed), 0);
		}
		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
				return;

			if (GameController.CurrentGameSettings.IsDebugEnabled)
			{
				if (targetPosition.HasValue)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawLine(actuallWorldPosition, targetPosition.Value);
				}
				if (overrideTargetPosition.HasValue)
				{
					Gizmos.color = Color.yellow;
					Gizmos.DrawLine(actuallWorldPosition, overrideTargetPosition.Value);
				}
			}
		}
		public override void StartCombat()
		{
			if (!curStates.Fighting)
			{
				ActivateActivationEffects(enemySettings.OnCombatTriggerEffect);
			}
			RefreshDataOnPlayer(true);
			base.StartCombat();
		}
		#endregion
	}
}