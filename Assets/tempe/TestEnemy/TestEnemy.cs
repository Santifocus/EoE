using System.Collections;
using System.Collections.Generic;
using EoE.Information;
using UnityEngine;

namespace EoE.Entities
{
	public class TestEnemy : Enemy
	{
		public override EnemySettings enemySettings => settings;
		public EnemySettings settings;

		protected override void CombatBehavior()
		{
			curStates.IsMoving = false;
			UpdateAcceleration();
			TurnTo(chaseDirection);
		}
	}
}
