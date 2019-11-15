using EoE.Information;
using System.Collections;
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
		protected override void InRangeBehaivior()
		{
			TryForBash();
		}
		protected override void PlayerJustEnteredAttackRange()
		{
			SetAgentState(false);
		}
		private void TryForBash()
		{
			if (!chargingBash)
			{
				bool allowedToBash = true;

				//Make sure the Crawler is looking at the player
				Vector2 xzDif = new Vector2(player.actuallWorldPosition.x, player.actuallWorldPosition.z) - new Vector2(actuallWorldPosition.x, actuallWorldPosition.z);
				float distance = xzDif.magnitude;
				Vector2 directon = xzDif / distance;
				float cosAngle = Vector2.Dot(new Vector2(transform.forward.x, transform.forward.z), directon);

				//Is the player behind the Crawler? Then just stop here
				if (cosAngle <= 0)
				{
					allowedToBash = false;
				}
				else
				{
					//Otherwise we calculate the distance between the player and the transform.forward of the Crawler
					float distToForward = (1 - cosAngle) * distance;

					//If the distance is greater then the widht of this crawler then we cant allow the dash
					if (distToForward > coll.bounds.extents.x * 0.95f)
						allowedToBash = false;
				}

				if (allowedToBash)
					StartCoroutine(ChargeUpBash());
				else
					LookAtPlayer();
			}
		}
		private IEnumerator ChargeUpBash()
		{
			chargingBash = true;
			behaviorSimpleStop = true;

			GameController.BeginDelayedCall(() => AnnounceBash(), settings.AttackSpeed + settings.BashAnnouncementDelay, TimeType.ScaledDeltaTime, new System.Func<bool>(() => this), OnDelayConditionNotMet.Cancel);

			float timer = 0;
			while (timer < settings.AttackSpeed)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;
			}

			//Disable and enable so OnCollisionEnter can be called with a fresh calculation,
			//this is needed for the situation in which the player is touching the enemy
			coll.enabled = false;
			coll.enabled = true;

			chargingBash = false;
			behaviorSimpleStop = false;

			StartCombat();
			bashing = true;
			appliedMoveStuns++;
			bashForce = entitieForceController.ApplyForce(transform.forward * settings.BashSpeed, settings.BashSpeed / settings.BashDistance, false, bashFinish);
		}
		private void AnnounceBash()
		{
			Utils.EffectUtils.PlayParticleEffect(settings.BashAnnouncementParticles, actuallWorldPosition);
		}
		private void FinishedBash()
		{
			bashing = false;
			appliedMoveStuns--;
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
						if (collision.GetContact(i).otherCollider == player.coll)
						{
							hitPoint = collision.GetContact(i).point;
							break;
						}
					}

					player.ChangeHealth(new ChangeInfo(this, CauseType.Physical, settings.EntitieElement, hitPoint, bashForce.Force / restForce, restForce / settings.BashSpeed * settings.BaseAttackDamage, Random.value < settings.CritChance, restForce * settings.ForceTranslationMultiplier));
					entitieForceController.ForceRemoveForce(bashForce);
					body.velocity = curVelocity;
				}
			}
		}
	}
}