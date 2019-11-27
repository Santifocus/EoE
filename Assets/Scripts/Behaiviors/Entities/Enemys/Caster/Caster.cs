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
		private float castCooldown;
		private Vector3 aimPos;
		protected override void EntitieUpdate()
		{
			if (castCooldown > 0)
				castCooldown -= Time.deltaTime;
		}
		protected override void InRangeBehaivior()
		{
			if (!isChargingSpell && castCooldown <= 0 && curMana >= settings.ProjectileManaCost)
			{
				StartCoroutine(ChargeSpell());
				ChangeMana(new ChangeInfo(this, CauseType.Magic, TargetStat.Mana, settings.ProjectileManaCost));
			}
		}
		private IEnumerator ChargeSpell()
		{
			isChargingSpell = true;
			float spellChargeTime = 0;

			aimPos = Player.Alive ? player.actuallWorldPosition : (actuallWorldPosition + settings.ProjectileSpawnOffset + Random.insideUnitSphere * 3);
			for (int i = 0; i < settings.CastingAnnouncement.Length; i++)
			{
				FXManager.PlayFX(settings.CastingAnnouncement[i], transform, true);
			}
			while (spellChargeTime < settings.ProjectileChargeTime)
			{
				yield return new WaitForEndOfFrame();
				spellChargeTime += Time.deltaTime;
				if (Player.Alive)
					aimPos = Vector3.Lerp(aimPos, player.actuallWorldPosition, Time.deltaTime * 5);

				if(GameController.CurrentGameSettings.IsDebugEnabled)
					Debug.DrawLine(actuallWorldPosition + settings.ProjectileSpawnOffset, aimPos, Color.red, Time.deltaTime * 1.1f);
			}
			CreateProjectile();
			isChargingSpell = false;
		}
		private void CreateProjectile()
		{
			CasterProjectile projectile = Instantiate(settings.ProjectilePrefab, Storage.ProjectileStorage);
			projectile.transform.position = actuallWorldPosition + settings.ProjectileSpawnOffset;
			projectile.Setup(settings, this, (aimPos - (actuallWorldPosition + settings.ProjectileSpawnOffset)).normalized);
			castCooldown = settings.ProjectileSpellCooldown;

			FXManager.PlayFX(settings.ProjectileFlyParticles, projectile.transform, true);
		}
	}
}