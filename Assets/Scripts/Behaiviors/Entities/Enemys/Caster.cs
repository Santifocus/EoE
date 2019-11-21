using System.Collections;
using System.Collections.Generic;
using EoE.Information;
using UnityEngine;

namespace EoE.Entities
{
	public class Caster : Enemy
	{
		public override EnemySettings enemySettings => settings;
		public CasterSettings settings;

		private float spellChargeTime;
		protected override void PlayerJustEnteredAttackRange()
		{

		}

		protected override void InRangeBehaivior()
		{
			spellChargeTime += Time.deltaTime;

		}
	}
}