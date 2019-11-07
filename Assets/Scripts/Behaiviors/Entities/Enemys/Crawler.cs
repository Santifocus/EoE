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
			StartCoroutine(ChargeUpBash());
		}
		private IEnumerator ChargeUpBash()
		{
			stopBaseBehaivior = true;
			float timer = 0;
			bool canceled = false;
			while(timer < settings.AttackSpeed)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;

				if(timer < settings.InRangeWaitTime && !PlayerInAttackRange)
				{
					canceled = true;
					break;
				}
			}

			if (!canceled)
			{
				bashing = true;
				bashForce = entitieForceController.ApplyForce(transform.forward * settings.BashSpeed, settings.BashSpeed / settings.BashDistance, false, bashFinish);
			}
			else
			{
				stopBaseBehaivior = false;
			}
		}
		private void FinishedBash()
		{
			bashing = stopBaseBehaivior = false;
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