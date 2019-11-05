using System.Collections;
using System.Collections.Generic;
using EoE.Information;
using EoE.Utils;
using EoE.Events;
using EoE.UI;
using UnityEngine;
using UnityEngine.AI;

namespace EoE.Entities
{
	public abstract class Enemy : Entitie
	{
		#region Fields
		//Constants
		private const float RE_CHECK_FREQUENCY = 0.25f;
		private const float CLOSEST_NAVMESH_POINT_DIST = 20;
		private const float REACH_DESTINATION_MIN = 0.2f;

		//Inspector Variables
		[SerializeField] private NavMeshAgent agent = default;

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
		public override EntitieSettings SelfSettings => enemySettings;
		public abstract EnemySettings enemySettings { get; }
		protected Vector3 GuessedPlayerPosition => lastConfirmedPlayerPos + new Vector3(lastPlayerSpeed.x, 0, lastPlayerSpeed.z) * lastSeenPlayer;
		#endregion
		#region Basic Monobehaivior
		protected override void Start()
		{
			base.Start();
			originalSpawnPosition = actuallWorldPosition;
			EventManager.PlayerDiedEvent += PlayerDied;

			normalCosSightCone = Mathf.Cos(Mathf.Min(360, enemySettings.SightAngle) * Mathf.Deg2Rad);
			foundPlayerCosSightCone = Mathf.Cos(Mathf.Min(360, enemySettings.FoundPlayerSightAngle) * Mathf.Deg2Rad);
			SetupNavMeshAgent();
		}
		private void SetupNavMeshAgent()
		{
			curPath = new NavMeshPath();
			agent.radius = Mathf.Max(coll.bounds.extents.x, coll.bounds.extents.y);
			agent.angularSpeed = enemySettings.TurnSpeed;
			agent.acceleration = 1 / Mathf.Max(0.0001f, enemySettings.MoveAcceleration);
			agent.speed = enemySettings.WalkSpeed;
			agent.stoppingDistance = REACH_DESTINATION_MIN;
		}
		private void PlayerDied(Entitie killer)
		{
			curStates.IsInCombat = false;
			EventManager.PlayerDiedEvent -= PlayerDied;
		}
		protected override void Update()
		{
			base.Update();
			DecideOnBehaivior();
		}
		private void DecideOnBehaivior()
		{
			body.velocity = Vector2.zero;
			if (CheckForPlayer())
			{
				//Update information on the player
				lastConfirmedPlayerPos = Player.Instance.actuallWorldPosition;
				lastPlayerSpeed = Player.Instance.curVelocity;
				lastSeenPlayer = 0;

				//Find a path
				//TODO Give a natural behaivior for when the Enemy cant reach the player but sees him
				Vector3? destination = GetClosestPointOnNavmesh(lastConfirmedPlayerPos);

				if(destination.HasValue && agent.CalculatePath(destination.Value, curPath) && curPath.status != NavMeshPathStatus.PathInvalid)
				{
					agent.SetPath(curPath);
					agent.stoppingDistance = enemySettings.AttackRange;
					agent.isStopped = ReachedDestination();
				}

				//Inform the Entitie that it is now chasing the Player
				chasingPlayer = true;
			}
			else if(chasingPlayer)
			{
				lastSeenPlayer += Time.deltaTime; 
				Vector3? destination = GetClosestPointOnNavmesh(GuessedPlayerPosition);

				if (destination.HasValue && agent.CalculatePath(destination.Value, curPath) && curPath.status != NavMeshPathStatus.PathInvalid)
				{
					agent.SetPath(curPath);
					agent.stoppingDistance = REACH_DESTINATION_MIN;
					agent.isStopped = ReachedDestination();
				}
				else
				{
					//We cant reach the player so we force interest loss on chasing
					lastSeenPlayer = enemySettings.ChaseInterest;
				}

				//The Enemy didnt see the player for too long, therefore it will stop here and investigate the area
				//If it doesnt find the player it will return back to its original spawn
				if(lastSeenPlayer >= enemySettings.ChaseInterest || agent.isStopped)
				{
					chasingPlayer = false;
					lostChaseInterestPos = actuallWorldPosition;
					remainingInvestigationTime = enemySettings.InvestigationTime;
				}
			}
			else
			{
				//Idle behaivior
				if (!WanderAround())
				{
					LookAroundArea();
				}
			}
		}
		private bool CheckForPlayer()
		{
			Vector3 dif = Player.Instance.actuallWorldPosition - actuallWorldPosition;
			float sqrDist = dif.sqrMagnitude;

			if (sqrDist > enemySettings.SightRange * enemySettings.SightRange)
				return false;

			Vector3 direction = dif / Mathf.Sqrt(sqrDist);
			float cosAngle = Vector3.Dot(transform.forward, direction);

			if (cosAngle < (chasingPlayer ? foundPlayerCosSightCone : normalCosSightCone))
				return false;

			//Low priority check if this entitie can see any part of the player
			return CheckIfCanSeeEntitie(Player.Instance, true);
		}
		private bool ReachedDestination()
		{
			return agent.remainingDistance < agent.stoppingDistance;
		}
		private bool WanderAround()
		{
			return false;
		}
		private void LookAroundArea()
		{

		}
		private void ChasePlayer()
		{

		}
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
			if (NavMesh.SamplePosition(point, out hit, radius, 1))
			{
				return hit.position;
			}
			else
				return null;
		}
		#endregion
	}
}