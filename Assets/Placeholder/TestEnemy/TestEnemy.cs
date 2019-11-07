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
		private float attackCooldown;
		protected override void PlayerJustEnteredAttackRange()
		{
			base.PlayerJustEnteredAttackRange();
			attackCooldown = 1;
		}
		protected override void InRangeBehaivior()
		{
			if (attackCooldown > 0)
			{
				attackCooldown -= Time.deltaTime;
				if(attackCooldown < 0)
				{
					attackCooldown += 1;

					RaycastHit[] hits = Physics.RaycastAll(actuallWorldPosition, transform.forward, enemySettings.AttackRange, ConstantCollector.ENTITIE_LAYER);
					for(int i = 0; i < hits.Length; i++)
					{
						if(hits[i].collider.GetComponent<Entitie>() is Player)
							player.ChangeHealth(new InflictionInfo(this, CauseType.Physical, enemySettings.EntitieElement, hits[i].point, (player.actuallWorldPosition - hits[i].point).normalized, enemySettings.BaseAttackDamage, false, true, 4));
					}
				}
			}
		}
	}
}
