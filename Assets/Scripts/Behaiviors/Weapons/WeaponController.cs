using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	public enum AttackAnimation { AttackDash = 1, Attack1 = 2, Attack2 = 4, Attack3 = 8 }
	public class WeaponController : MonoBehaviour
	{
		#region Fields
		//Static info
		private static readonly Dictionary<AttackAnimation, (float, float)> animationDelayLookup = new Dictionary<AttackAnimation, (float, float)>()
		{
			{ AttackAnimation.AttackDash, (1, 0) },
			{ AttackAnimation.Attack1, (0.666f, 0) },
			{ AttackAnimation.Attack2, (1, 0) },
			{ AttackAnimation.Attack3, (1.666f, 0) },
		};
		public static WeaponController PlayerWeaponController;

		//Getter helper
		private bool PlayerRunning => Player.Instance.curStates.Running;
		private bool PlayerInAir => !Player.Instance.charController.isGrounded;

		//Inspector variables
		[SerializeField] private WeaponHitbox[] weaponHitboxes = default;

		//Behaivior controll
		private bool curActive;
		public bool Active { get => curActive; set => ChangeWeaponState(value); }
		private Weapon weaponInfo;
		#endregion
		#region Setups
		public void Setup(Weapon weaponInfo)
		{
			for (int i = 0; i < weaponHitboxes.Length; i++)
			{
				weaponHitboxes[i].Setup(this);
			}
			PlayerWeaponController = this;
			this.weaponInfo = weaponInfo;
		}
		private void ChangeWeaponState(bool state)
		{
			curActive = state;
			for (int i = 0; i < weaponHitboxes.Length; i++)
			{
				weaponHitboxes[i].Active = state;
				if (!state)
					weaponHitboxes[i].ResetCollisionIgnores();
			}
		}
		#endregion
		#region FollowPlayer
		private void LateUpdate()
		{
			FollowPlayer();
		}
		private void FixedUpdate()
		{
			FollowPlayer();
		}
		private void FollowPlayer()
		{
			transform.position = Player.Instance.weaponHoldPoint.position + weaponInfo.WeaponPositionOffset;
			transform.eulerAngles = Player.Instance.weaponHoldPoint.eulerAngles + weaponInfo.WeaponRotationOffset;
		}
		#endregion
		public void StartAttack()
		{

		}
		public void HitObject(Vector3 hitPos, Collider hit)
		{

		}
	}
}