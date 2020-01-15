using EoE.Combatery;
using EoE.Information;
using System.Collections;
using UnityEngine;

namespace EoE.Entities
{
	public class Crawler : Enemy
	{
		//Inspector Variables
		[SerializeField] private CrawlerSettings settings = default;
		[SerializeField] private CrawlerHitbox[] crawlerHitboxes = default;

		//Attack
		private bool chargingBash;
		private bool bashing;
		private ForceController.SingleForce bashForce;

		//Getter helper
		public override EnemySettings enemySettings => settings;
		protected override void EntitieStart()
		{
			for (int i = 0; i < crawlerHitboxes.Length; i++)
			{
				crawlerHitboxes[i].Setup(this);
			}
		}
		protected override void InRangeBehaivior()
		{
			TryForBash();
		}
		private void TryForBash()
		{
			if (!chargingBash && !bashing)
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

					//If the distance is greater then the widht of this crawler then we cant allow the bash
					if (distToForward > coll.bounds.extents.x * 0.95f)
						allowedToBash = false;
				}

				if (allowedToBash)
				{
					SetAgentState(false);
					StartCoroutine(ChargeUpBash());
				}
				else
					LookAtTarget();
			}
		}
		private IEnumerator ChargeUpBash()
		{
			chargingBash = true;
			behaviorSimpleStop = true;

			ActivateActivationEffects(settings.BashChargeStartEffects);
			float timer = 0;
			while (timer < settings.BashChargeSpeed)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;
				if(IsStunned)
				{
					goto CanceledBash;
				}
			}

			StartCombat();
			bashing = true;
			MovementStops++;
			bashForce = entitieForceController.ApplyForce(transform.forward * settings.BashSpeed, settings.BashSpeed / settings.BashDistance, false, () => FinishedBash());
			SetBashColliderState(true);
			ActivateActivationEffects(settings.BashStartEffects);

		CanceledBash:;

			chargingBash = false;
			behaviorSimpleStop = false;
		}
		private void FinishedBash()
		{
			bashing = false;
			SetBashColliderState(false);
			MovementStops--;
		}
		private void SetBashColliderState(bool state)
		{
			for (int i = 0; i < crawlerHitboxes.Length; i++)
				crawlerHitboxes[i].CollisionActive = state;
		}
		public void HitCollider(Collider other, Collider self)
		{
			float restForce = bashForce.Force.magnitude;
			float normalizedRestForce = restForce / settings.BashSpeed;
			bool wasCrit = Utils.Chance01(settings.CritChance);
			float resultingDamage = normalizedRestForce * settings.BasePhysicalDamage;

			bool shouldRemoveForces = true;
			switch (other.gameObject.layer)
			{
				case ConstantCollector.ENTITIE_LAYER:
					{
						Entity hitEntity = other.gameObject.GetComponent<Entity>();
						if (!(hitEntity is Enemy))
						{
							hitEntity.ChangeHealth(new ChangeInfo(this, CauseType.Physical, settings.EntitieElement, TargetStat.Health, other.ClosestPoint(self.bounds.center), bashForce.Force / restForce, resultingDamage, wasCrit, restForce * settings.ForceTranslationMultiplier));

							ActivateActivationEffects(settings.BashHitEntitieEffects, normalizedRestForce);
						}
						else
						{
							shouldRemoveForces = false;
						}
					}
					break;
				case ConstantCollector.SHIELD_LAYER:
					{
						Shield hitShield = other.gameObject.GetComponent<Shield>();
						if(hitShield.creator != this)
						{
							hitShield.HitShield(resultingDamage * (wasCrit ? GameController.CurrentGameSettings.CritDamageMultiplier : 1));
						}
						else
						{
							shouldRemoveForces = false;
						}
					}
					break;
				case ConstantCollector.TERRAIN_LAYER:
					{
						ActivateActivationEffects(settings.BashHitTerrainEffects, normalizedRestForce);
					}
					break;
			}

			if (shouldRemoveForces)
			{
				entitieForceController.ForceRemoveForce(bashForce);
				body.velocity = CurVelocity;
			}
		}
	}
}