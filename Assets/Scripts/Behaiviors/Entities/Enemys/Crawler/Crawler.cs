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

					//If the distance is greater then the widht of this crawler then we cant allow the dash
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

			Coroutine bashAnnouncement = GameController.BeginDelayedCall(() => BashStart(), settings.BashChargeSpeed + settings.BashAnnouncementDelay, TimeType.ScaledDeltaTime, new System.Func<bool>(() => this), OnDelayConditionNotMet.Cancel);

			float timer = 0;
			while (timer < settings.BashChargeSpeed)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;
				if(IsStunned)
				{
					GameController.Instance.StopCoroutine(bashAnnouncement);
					goto CanceledBash;
				}
			}

			StartCombat();
			bashing = true;
			MovementStops++;
			bashForce = entitieForceController.ApplyForce(transform.forward * settings.BashSpeed, settings.BashSpeed / settings.BashDistance, false, () => FinishedBash());
			SetBashColliderState(true);

		CanceledBash:;

			chargingBash = false;
			behaviorSimpleStop = false;
		}
		private void BashStart()
		{
			for (int i = 0; i < settings.BashStartEffects.Length; i++)
			{
				settings.BashStartEffects[i].Activate(this, null);
			}
		}
		private void FinishedBash()
		{
			bashing = false;
			SetBashColliderState(false);
			MovementStops--;
		}
		private void SetBashColliderState(bool state)
		{

		}
		public void HitCollider(Collider other, Collider self)
		{
			float restForce = bashForce.Force.magnitude;
			bool wasCrit = Utils.Chance01(settings.CritChance);
			float resultingDamage = restForce / settings.BashSpeed * settings.BaseAttackDamage;

			switch (other.gameObject.layer)
			{
				case ConstantCollector.ENTITIE_LAYER:
					{
						Entity hitEntity = other.gameObject.GetComponent<Entity>();
						if (!(hitEntity is Enemy))
						{

							hitEntity.ChangeHealth(new ChangeInfo(this, CauseType.Physical, settings.EntitieElement, TargetStat.Health, other.ClosestPoint(self.bounds.center), bashForce.Force / restForce, resultingDamage, wasCrit, restForce * settings.ForceTranslationMultiplier));
							entitieForceController.ForceRemoveForce(bashForce);
							body.velocity = CurVelocity;
						}
					}
					break;
				case ConstantCollector.SHIELD_LAYER:
					{

					}
					break;
				case ConstantCollector.TERRAIN_LAYER:
					{

					}
					break;
			}
		}
		private void OnCollisionEnter(Collision collision)
		{
			if (!bashing)
				return;

			if (collision.gameObject.layer == ConstantCollector.ENTITIE_LAYER)
			{
				Entity hitEntity = collision.gameObject.GetComponent<Entity>();
				if (!(hitEntity is Enemy))
				{
					float restForce = bashForce.Force.magnitude;
					Vector3 hitPoint = Vector3.zero;

					for (int i = 0; i < collision.contactCount; i++)
					{
						if (collision.GetContact(i).otherCollider == hitEntity.coll)
						{
							hitPoint = collision.GetContact(i).point;
							break;
						}
					}

					hitEntity.ChangeHealth(new ChangeInfo(this, CauseType.Physical, settings.EntitieElement, TargetStat.Health, hitPoint, bashForce.Force / restForce, restForce / settings.BashSpeed * settings.BaseAttackDamage, Utils.Chance01(settings.CritChance), restForce * settings.ForceTranslationMultiplier));
					entitieForceController.ForceRemoveForce(bashForce);
					body.velocity = CurVelocity;
				}
			}
		}
	}
}