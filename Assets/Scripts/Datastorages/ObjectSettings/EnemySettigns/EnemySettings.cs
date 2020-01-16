using EoE.Combatery;

namespace EoE.Information
{
	public class EnemySettings : EntitySettings
	{
		//Combat
		public float AttackRange = 2;
		public float AttackKnockBack = 3;
		public float CritChance = 0;
		public ActivationEffect[] OnCombatTriggerEffect = new ActivationEffect[0];

		//Player search
		public float PlayerTrackSpeed = 4;
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