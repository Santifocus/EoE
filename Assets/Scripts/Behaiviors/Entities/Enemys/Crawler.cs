using System.Collections;
using System.Collections.Generic;
using EoE.Information;
using UnityEngine;

namespace EoE.Entities
{
	public class Crawler : Enemy
	{
		//Inspector Variables
		[SerializeField] private CrawlerSettings settings = default;

		//Attack
		private ForceController.OnForceDelete bashFinish;
		private bool chargingBash;
		private bool bashing;
		private ForceController.SingleForce bashForce;

		//Getter helper
		public override EnemySettings enemySettings => settings;
		protected override void EntitieStart()
		{
			bashFinish += FinishedBash;
		}
		protected override void PlayerJustEnteredAttackRange()
		{
			base.PlayerJustEnteredAttackRange();
			if(!chargingBash)
				StartCoroutine(ChargeUpBash());
		}
		private IEnumerator ChargeUpBash()
		{
			chargingBash = true;
			behaviorSimpleStop = true;

			float timer = 0;
			bool canceled = false;
			while(timer < settings.AttackSpeed)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;
			}

			//diable and enable so OnCollisionEnter can be called with a fresh calculation,
			//this is needed for the situation in which the player is touching the enemy
			coll.enabled = false;
			coll.enabled = true;
			chargingBash = false;

			if (!canceled)
			{
				behaviorSimpleStop = false;
				agentFullStopBehaivior = true;
				bashing = true;
				bashForce = entitieForceController.ApplyForce(transform.forward * settings.BashSpeed, settings.BashSpeed / settings.BashDistance, false, bashFinish);
			}
			else
			{
				behaviorSimpleStop = false;
			}
		}
		private void FinishedBash()
		{
			bashing = agentFullStopBehaivior = false;
			if(PlayerInAttackRange)
				StartCoroutine(ChargeUpBash());
		}
		private void OnCollisionEnter(Collision collision)
		{
			if (!bashing)
				return;

			if (collision.gameObject.layer == 9)
			{
				Player hitPlayer = collision.gameObject.GetComponent<Player>();
				if (hitPlayer)
				{
					float restForce = bashForce.Force.magnitude;
					Vector3 hitPoint = Vector3.zero;

					for (int i = 0; i < collision.contactCount; i++)
					{
						if(collision.GetContact(i).otherCollider == player.coll)
						{
							hitPoint = collision.GetContact(i).point;
							break;
						}
					}

					player.ChangeHealth(new InflictionInfo(this, CauseType.Physical, settings.EntitieElement, hitPoint, bashForce.Force / restForce, restForce / settings.BashSpeed * settings.BaseAttackDamage, Random.value < settings.CritChance, true, restForce * settings.ForceTranslationMultiplier));
					entitieForceController.ForceRemoveForce(bashForce);
					body.velocity = curVelocity;
				}
			}
		}
	}
}