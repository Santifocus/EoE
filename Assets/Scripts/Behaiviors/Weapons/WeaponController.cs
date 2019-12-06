using EoE.Entities;
using EoE.Information;
using EoE.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Weapons
{
	public class WeaponController : MonoBehaviour
	{
		public WeaponHitbox[] weaponHitboxes;
		private bool curActive;
		public bool Active { get => curActive; set => ChangeWeaponState(value); }
		public void Setup()
		{
			for (int i = 0; i < weaponHitboxes.Length; i++)
			{
				weaponHitboxes[i].Setup(this);
			}
		}
		private void ChangeWeaponState(bool state)
		{
			if(!state)
			{
				for(int i = 0; i < weaponHitboxes.Length; i++)
				{
					weaponHitboxes[i].ResetCollisionIgnores();
				}
			}
			curActive = state;
			for (int i = 0; i < weaponHitboxes.Length; i++)
			{
				weaponHitboxes[i].Active = state;
			}
		}
		public void HitObject(Vector3 hitPos, Collider hit)
		{
			for(int i = 0; i < weaponHitboxes.Length; i++)
			{
				weaponHitboxes[i].IgnoreCollider(hit);
			}

			if (hit.gameObject.layer == ConstantCollector.TERRAIN_LAYER)
			{
				if (!Player.Instance.activeAttack.animationInfo.penetrateTerrain)
					Player.Instance.CancelAttackAnimation();

				//TODO: Some form of VFX on the hit terrain
				return;
			}

			Entitie hitEntitie = hit.GetComponent<Entitie>();
			if (hitEntitie is Player)
				return;

			bool wasCrit = BaseUtils.Chance01(Player.Instance.PlayerWeapon.baseCritChance * Player.Instance.activeAttack.info.critChanceMultiplier);
			float damageAmount = Player.Instance.PlayerWeapon.baseAttackDamage * Player.Instance.activeAttack.info.damageMutliplier + Player.Instance.curPhysicalDamage;
			float? knockBackAmount = Player.Instance.PlayerWeapon.baseKnockbackAmount * Player.Instance.activeAttack.info.knockbackMutliplier;
			if (knockBackAmount == 0)
				knockBackAmount = null;
			Vector3 impactDirection = (hitEntitie.actuallWorldPosition - Player.Instance.transform.position).normalized;

			hitEntitie.ChangeHealth(new ChangeInfo(Player.Instance, CauseType.Physical, Player.Instance.PlayerWeapon.element, TargetStat.Health, hitPos, impactDirection, damageAmount, wasCrit, knockBackAmount));

			if (!Player.Instance.activeAttack.animationInfo.penetrateEntities)
				Player.Instance.CancelAttackAnimation();
		}
	}
}