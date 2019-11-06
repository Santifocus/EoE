using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class EnemySettings : EntitieSettings
	{
		//Player search
		public float AttackRange = 2;
		public float SightRange = 10;
		public float FoundPlayerSightRange = 30;
		public float SightAngle = 60;
		public float FoundPlayerSightAngle = 120;
		public float ChaseInterest = 4;
		public float InvestigationTime = 8;

		//Wandering
		public float WanderingFactor = 5;
		public float WanderingDelayMin = 0.5f;
		public float WanderingDelayMax = 5;

		//Lookaround
		public float LookAroundDelayMin = 0.25f;
		public float LookAroundDelayMax = 2;
	}
}