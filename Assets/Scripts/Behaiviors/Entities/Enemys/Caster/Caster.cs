using System.Collections;
using System.Collections.Generic;
using EoE.Information;
using EoE.Weapons;
using UnityEngine;

namespace EoE.Entities
{
	public class Caster : Enemy
	{
		public override EnemySettings enemySettings => settings;
		public CasterSettings settings;
		protected override void InRangeBehaivior()
		{
			LookAtPlayer();
			CastSpell(settings.CasterAttack);
		}
	}
}