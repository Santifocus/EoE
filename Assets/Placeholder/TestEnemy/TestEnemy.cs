using System.Collections;
using System.Collections.Generic;
using EoE.Information;
using UnityEngine;

namespace EoE.Entities
{
	public class TestEnemy : Enemy
	{
		//Inspector variables
		public EnemySettings settings;

		//Getter Helpers
		public override EnemySettings enemySettings => settings;

		protected override void CombatBehavior(float distance)
		{
			curStates.IsMoving = false;
		}
	}
}
