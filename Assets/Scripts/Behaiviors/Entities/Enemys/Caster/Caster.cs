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
		private bool isChargingSpell;

		protected override void InRangeBehaivior()
		{
			if (!isChargingSpell && curMana >= settings.ProjectileManaCost)
			{
				StartCoroutine(ChargeSpell());
				ChangeMana(new ChangeInfo(this, CauseType.Magic, settings.ProjectileManaCost, false));
			}
		}
		private IEnumerator ChargeSpell()
		{
			isChargingSpell = true;
			float spellChargeTime = 0;
			for(int i = 0; i < settings.CastingAnnouncement.Length; i++)
			{
				FXManager.PlayFX(settings.CastingAnnouncement[i], transform);
			}
			while (spellChargeTime < settings.ProjectileChargeTime)
			{
				yield return new WaitForEndOfFrame();
				spellChargeTime += Time.deltaTime;
			}
			CreateProjectile();
			isChargingSpell = false;
		}
		private void CreateProjectile()
		{
			CasterProjectile projectile = Instantiate(settings.ProjectilePrefab, Storage.ProjectileStorage);

			projectile.Setup(settings, this);
			
			FXManager.PlayFX(settings.ProjectileFlyParticles, projectile.transform);
		}
	}
}