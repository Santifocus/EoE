using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Information;
using EoE.Events;

namespace EoE.Entities
{
	public abstract class Entitie : MonoBehaviour
	{
		[SerializeField] private Rigidbody body;
		[SerializeField] private Collider coll;
		public abstract EntitieSettings SelfSettings { get; }
		private float currentHealth;

		private float regenTimer = 0;
		private void Start()
		{
			currentHealth = SelfSettings.Health;
			EntitieStart();
		}
		protected virtual void EntitieStart(){}

		private void Update()
		{
			Regen();
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