using EoE.Events;
using EoE.Information;
using UnityEngine;
using UnityEngine.AI;

namespace EoE.Entities
{
	public abstract class Enemy : Entitie
	{
		#region Fields
		//Constants
		private const float CLOSEST_NAVMESH_POINT_DIST = 20;
		private const float REACH_DESTINATION_MIN = 0.2f;

		private const float REACHED_LOOK_ANGLE_THRESHOLD = 1;

		//Inspector Variables
		[SerializeField] protected Rigidbody body = default;
		[SerializeField] protected NavMeshAgent agent = default;

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
		protected Player player => Player.Instance;
		public override Vector3 curVelocity => new Vector3(impactForce.x, 0, impactForce.y) + entitieForceController.currentTotalForce;
		public override EntitieSettings SelfSettings => enemySettings;
		public abstract EnemySettings enemySettings { get; }
		public bool PlayerInAttackRange { get; private set; }
		protected Vector3 GuessedPlayerPosition => lastConfirmedPlayerPos + new Vector3(lastPlayerSpeed.x, 0, lastPlayerSpeed.z) * lastSeenPlayer;

		//Behaivior
		protected bool behaviorSimpleStop;
		protected bool isIdle => !(chasingPlayer || remainingInvestigationTime > 0);
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
			if (Player.Instance.Alive)
			{
				bool prevInRange = PlayerInAttackRange;
				float sqrPlayerDist = (player.actuallWorldPosition - actuallWorldPosition).sqrMagnitude;
				PlayerInAttackRange = sqrPlayerDist < (enemySettings.AttackRange * enemySettings.AttackRange);

				if (curStates.Fighting)
				{
					chasingPlayer = true;
					if (CheckIfCanSeeEntitie(transform, player, true))
					{
						lastConfirmedPlayerPos = player.actuallWorldPosition;
						lastPlayerSpeed = player.curVelocity;
						lastSeenPlayer = 0;

						if (!prevInRange && PlayerInAttackRange)
						{
							PlayerJustEnteredAttackRangeBase();
						}
					}
				}

				if (chasingPlayer)
				{
					targetPosition = Vector3.Lerp(targetPosition ?? GuessedPlayerPosition, GuessedPlayerPosition, Time.deltaTime * enemySettings.PlayerTrackSpeed);
					if (GameController.CurrentGameSettings.IsDebugEnabled)
						Debug.DrawLine(actuallWorldPosition, targetPosition.Value, Color.red, Time.deltaTime * 1.1f);

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

			if (PlayerInAttackRange && chasingPlayer)
				InRangeBehaiviorBase();

			if (IsStunned)
			{
				body.isKinematic = false;
				body.velocity = curVelocity;
				SetAgentState(false);
				return;
			}
			else if (behaviorSimpleStop)
			{
				SetAgentState(false);
				return;
			}

			body.isKinematic = true;
			SetAgentState(true);

			if (CheckForPlayer())
			{
				//Update information on the player
				chasingPlayer = true;
				lastConfirmedPlayerPos = player.actuallWorldPosition;
				lastPlayerSpeed = player.curVelocity;
				lastSeenPlayer = 0;

				//Find a path
				Vector3? destination = GetClosestPointOnNavmesh(lastConfirmedPlayerPos);

				if (destination.HasValue && agent.enabled && agent.CalculatePath(destination.Value, curPath) && curPath.status != NavMeshPathStatus.PathInvalid)
				{
					agent.SetPath(curPath);
					agent.stoppingDistance = enemySettings.AttackRange * 0.95f;
					bool reachedDestination = ReachedDestination(true);

					if (reachedDestination)
					{
						LookAtPlayer();
					}
				}
			}
			else if (chasingPlayer)
			{
				lastSeenPlayer += Time.deltaTime;
				Vector3? destination = GetClosestPointOnNavmesh(GuessedPlayerPosition);

				if (destination.HasValue && agent.CalculatePath(destination.Value, curPath) && curPath.status != NavMeshPathStatus.PathInvalid)
				{
					agent.SetPath(curPath);
					agent.stoppingDistance = REACH_DESTINATION_MIN;
					ReachedDestination(true);
				}
				else
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
		protected void EnforceKnockback()
		{
			agent.nextPosition += curVelocity * Time.deltaTime;
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
		private bool CheckForPlayer()
		{
			if (!Player.Instance.Alive)
				return false;

			Vector3 dif = player.actuallWorldPosition - actuallWorldPosition;
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
			return CheckIfCanSeeEntitie(transform, player, true);
		}
		protected void SetAgentState(bool state)
		{
			if (!state)
				agent.velocity = Vector3.zero;
			if (agent.enabled && agent.isStopped == state)
				agent.isStopped = !state;
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

			if (ReachedDestination(true))
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
		private void PlayerDied(Entitie killer)
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
		protected bool ReachedDestination(bool setAgentState)
		{
			bool reached = agent.remainingDistance < agent.stoppingDistance;
			if (setAgentState)
				SetAgentState(!reached);

			return reached;
		}
		protected void LookAtPlayer()
		{
			Vector3 plyPos = Player.Instance.actuallWorldPosition;
			Vector2 direction = new Vector2(plyPos.x - actuallWorldPosition.x, plyPos.z - actuallWorldPosition.z).normalized;
			float rotation = -Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

			transform.localEulerAngles = new Vector3(0, Mathf.LerpAngle(transform.localEulerAngles.y, rotation, Time.deltaTime * enemySettings.TurnSpeed / 90), 0);
		}
		#endregion
	}
}