using EoE.Combatery;
using EoE.Information;
using System.Collections;
using UnityEngine;

namespace EoE.Entities
{
	public class Crawler : Enemy
	{
		private const float BASH_END_VELOCITY_THRESHOLD = 2f;
		//Inspector Variables
		[SerializeField] private CrawlerSettings settings = default;
		[SerializeField] private CrawlerHitbox[] crawlerHitboxes = default;
		[SerializeField] private Animator animator = default;

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
			if (!chargingBash && !bashing)
			{
				TryForBash();
			}
		}
		protected override void EntitieUpdate()
		{
			AnimationControl();
		}
		private void TryForBash()
		{
			bool allowedToBash = true;

			//Make sure the Crawler is looking at the player
			Vector2 xzDif = new Vector2(Player.Instance.actuallWorldPosition.x, Player.Instance.actuallWorldPosition.z) - new Vector2(actuallWorldPosition.x, actuallWorldPosition.z);
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
				if (Utils.Chance01(settings.ChanceForTrickBash))
				{
					StartCoroutine(ChargeBashC(TrickBash, settings.TrickBashChargeStartEffects));
					
				}
				else
				{
					StartCoroutine(ChargeBashC(Bash, settings.BashChargeStartEffects));
				}
			}
		}
		private IEnumerator ChargeBashC(System.Action chargeFinishAction, ActivationEffect[] chargeEffects)
		{
			SetAgentState(false);
			StartCombat();

			chargingBash = true;
			behaviorSimpleStop = true;

			ActivateActivationEffects(chargeEffects);
			float timer = 0;
			while (timer < settings.BashChargeSpeed)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;
				if(IsStunned || !Alive)
				{
					goto CanceledBash;
				}
			}

			chargeFinishAction?.Invoke();

			CanceledBash:;
			chargingBash = false;
			behaviorSimpleStop = false;
		}
		private void AnimationControl()
		{
			float sqrVelocity = agent.velocity.sqrMagnitude;
			if (sqrVelocity > 0.1f)
			{
				animator.SetFloat("MoveSpeed", Mathf.Sqrt(sqrVelocity) / settings.AnimationWalkSpeedDivider);
				animator.SetBool("Moving", true);
			}
			else
			{
				animator.SetBool("Moving", false);
			}
		}
		protected override void Death()
		{
			animator.SetTrigger("Death");
			base.Death();
		}
		private void Bash()
		{
			StartCombat();
			bashing = true;
			MovementStops++;
			TurnStops++;

			animator.SetTrigger("Bash");
			bashForce = entitieForceController.ApplyForce((transform.forward * settings.BashSpeed) * (CurWalkSpeed / settings.WalkSpeed), settings.BashSpeed / settings.BashDistance, false, () => FinishedBash());

			GameController.BeginDelayedCall(() => animator.SetTrigger("BashEnd"), 0, TimeType.ScaledDeltaTime, () => bashForce == null || bashForce.Force.sqrMagnitude < BASH_END_VELOCITY_THRESHOLD);
			SetBashColliderState(true);
			ActivateActivationEffects(settings.BashStartEffects);
		}
		private void TrickBash()
		{
			StartCombat();
			bashing = true;
			MovementStops++;
			Vector3 bashDirection = transform.right;
			string animation = "BashR";

			if (Utils.Chance01(0.5f))
			{ 
				//Go left instead
				bashDirection *= -1;
				animation = "BashL";
			}

			animator.SetTrigger(animation);
			entitieForceController.ApplyForce((bashDirection * settings.TrickBashSpeed) * (CurWalkSpeed / settings.WalkSpeed), settings.TrickBashSpeed / settings.TrickBashDistance, true, () =>
			{
				animator.SetTrigger("BashEnd");
				MovementStops--; 
				Bash(); 
			}
			);
		}
		private void FinishedBash()
		{
			bashing = false;
			SetBashColliderState(false);
			MovementStops--;
			TurnStops--;
		}
		private void SetBashColliderState(bool state)
		{
			for (int i = 0; i < crawlerHitboxes.Length; i++)
				crawlerHitboxes[i].CollisionActive = state;
		}
		public void HitCollider(Collider other, Collider self)
		{
			if (!Alive)
				return;

			float restForce = bashForce.Force.magnitude;
			float normalizedRemainingForce = restForce / settings.BashSpeed;
			bool wasCrit = Utils.Chance01(settings.CritChance);
			float resultingDamage = normalizedRemainingForce * settings.BasePhysicalDamage;

			bool shouldRemoveForces = true;
			switch (other.gameObject.layer)
			{
				case ConstantCollector.ENTITIE_LAYER:
					{
						Entity hitEntity = other.gameObject.GetComponent<Entity>();
						if (!(hitEntity is Enemy))
						{
							hitEntity.ChangeHealth(new ChangeInfo(this, CauseType.Physical, settings.EntitieElement, TargetStat.Health, other.ClosestPoint(self.bounds.center), bashForce.Force / restForce, resultingDamage, wasCrit, restForce * settings.ForceTranslationMultiplier));

							ActivateActivationEffects(settings.BashHitEntitieEffects, normalizedRemainingForce);
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
						ActivateActivationEffects(settings.BashHitTerrainEffects, normalizedRemainingForce);
					}
					break;
				case ConstantCollector.TERRAIN_LAYER:
					{
						ActivateActivationEffects(settings.BashHitTerrainEffects, normalizedRemainingForce);
					}
					break;
			}

			if (shouldRemoveForces)
			{
				entitieForceController.ForceRemoveForce(bashForce);
				bashForce = null;
				body.velocity = CurVelocity;
			}
		}
	}
}