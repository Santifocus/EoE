using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Information;
using EoE.Events;

namespace EoE.Entities
{
	public abstract class Entitie : MonoBehaviour
	{
		[SerializeField] protected Rigidbody body = default;
		[SerializeField] protected Collider coll = default;
		public abstract EntitieSettings SelfSettings { get; }
		private float regenTimer = 0;
		private float currentHealth;
		private float currentAcceleration;

		private void Start()
		{
			currentHealth = SelfSettings.Health;
			currentAcceleration = SelfSettings.BaseAcceleration;
			EntitieStart();
		}
		protected virtual void EntitieStart(){}

		private void Update()
		{
			Regen();
		}
		protected float UpdateAcceleration(bool accelerate, float factor = 1)
		{
			if (accelerate)
			{
				if(currentAcceleration < 1)
				{
					if (SelfSettings.MoveAcceleration > 0)
						currentAcceleration = Mathf.Min(1, currentAcceleration + Time.deltaTime / SelfSettings.MoveAcceleration * factor);
					else
						currentAcceleration = 1;
				}
			}
			else //decelerate
			{
				if (currentAcceleration > SelfSettings.BaseAcceleration)
				{
					if (SelfSettings.NoMoveDeceleration > 0)
						currentAcceleration = Mathf.Max(SelfSettings.BaseAcceleration, currentAcceleration - Time.deltaTime / SelfSettings.NoMoveDeceleration * factor);
					else
						currentAcceleration = SelfSettings.NoMoveDeceleration;
				}
			}
			return currentAcceleration;
		}
		protected virtual void Regen()
		{
			regenTimer += Time.deltaTime;
			if (regenTimer >= GameController.CurrentGameSettings.SecondsPerEntititeRegen)
			{
				regenTimer -= GameController.CurrentGameSettings.SecondsPerEntititeRegen;
				if (SelfSettings.DoRegen && currentHealth < SelfSettings.Health)
				{
					InflictionInfo basis = new InflictionInfo(this, CauseType.Heal, ElementType.None, transform.position + SelfSettings.MassCenter, -SelfSettings.HealthRegen * GameController.CurrentGameSettings.SecondsPerEntititeRegen, false);
					InflictionInfo.InflictionResult regenResult = new InflictionInfo.InflictionResult(basis, this, true, true);

					currentHealth = Mathf.Min(SelfSettings.Health, currentHealth - regenResult.finalDamage);
				}
			}
		}

		public void ChangeHealth(InflictionInfo causedDamage)
		{
			InflictionInfo.InflictionResult damageResult = new InflictionInfo.InflictionResult(causedDamage, this, true);

			currentHealth -= damageResult.finalDamage;
			body.velocity += damageResult.causedKnockback;
			currentHealth = Mathf.Min(SelfSettings.Health, currentHealth);

			if(currentHealth <= 0)
			{
				if(this is Player)
				{
					EventManager.PlayerDiedInvoke(causedDamage.attacker);
				}
				else
				{
					EventManager.EntitieDiedInvoke(this, causedDamage.attacker);
				}
				Death();
			}
		}

		protected virtual void Death()
		{
			Destroy(gameObject);
		}
	}
}