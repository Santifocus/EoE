using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
    public class Shield : MonoBehaviour
    {
		[SerializeField] private SphereCollider coll = default;

        public Entity creator { get; private set; }
		private List<FXInstance> boundEffects;
		public ShieldData info { get; private set; }

		private bool shieldActive;
		private float curShieldResistance;

		public static Shield CreateShield(ShieldData info, Entity creator, Vector3 spawnPosition = default)
		{
			Shield newShield = Instantiate(GameController.ShieldPrefab, Storage.ParticleStorage);
			newShield.transform.position = spawnPosition;

			newShield.info = info;
			newShield.creator = creator;
			newShield.coll.radius = info.ShieldSize;
			newShield.curShieldResistance = info.ShieldResistance;
			newShield.boundEffects = new List<FXInstance>();

			newShield.AnchorShield();
			newShield.coll.enabled = false;
			return newShield;
		}
		public void HitShield(float damage)
		{
			curShieldResistance = Mathf.Min(curShieldResistance - damage, info.ShieldResistance);
			ActivateActivationEffects(info.ShieldHitEffects, false);

			if (curShieldResistance <= 0 && shieldActive)
				BreakShield();
		}
		public void SetShieldState(bool state)
		{
			if (shieldActive == state)
				return;

			if (state)
			{
				float resistanceCost = Time.deltaTime * info.ShieldDrain;
				if (info.FirstActivationCost.CanActivate(creator, 1, 1, 1) && curShieldResistance >= resistanceCost)
				{
					info.FirstActivationCost.Activate(creator, 1, 1, 1);
					curShieldResistance -= resistanceCost;
				}
				else
				{
					ActivateActivationEffects(info.ShieldFailedStartEffects, false);
					return;
				}
			}

			shieldActive = state;
			coll.enabled = state;
			if (shieldActive)
			{
				ActivateActivationEffects(info.ShieldStartEffects, false);
				ActivateActivationEffects(info.ShieldActiveEffects, true);
			}
			else
			{
				StopBoundFX();
				ActivateActivationEffects(info.ShieldDisableEffects, false);
			}
		}
		private void LateUpdate()
		{
			AnchorShield();

			if (shieldActive)
			{
				float resistanceCost = Time.deltaTime * info.ShieldDrain;
				if ((resistanceCost <= curShieldResistance) && (info.BaseData.Cost.CanActivate(creator, Time.deltaTime, Time.deltaTime, Time.deltaTime))) 
				{
					info.BaseData.Cost.Activate(creator, Time.deltaTime, Time.deltaTime, Time.deltaTime);
					curShieldResistance -= resistanceCost;
				}
				else
				{
					SetShieldState(false);
				}
			}
			else
			{
				curShieldResistance += info.ShieldRegeneration * Time.deltaTime;
			}
		}
		private void FixedUpdate()
		{
			AnchorShield();
		}
		private void AnchorShield()
		{
			if (!creator)
				return;

			if (info.FollowOwner)
			{
				transform.position = creator.actuallWorldPosition + info.OffsetToOwner;
			}
			if (info.InheritOwnerRotation)
			{
				transform.eulerAngles = creator.transform.eulerAngles + info.RotationOffsetToOwner;
			}
		}
		private void ActivateActivationEffects(ActivationEffect[] activationEffects, bool binding)
		{
			for (int i = 0; i < activationEffects.Length; i++)
			{
				FXInstance[] fxInstances = activationEffects[i].Activate(creator, info.BaseData, 1, transform);
				if (binding)
					boundEffects.AddRange(fxInstances);
			}
		}
		private void StopBoundFX()
		{
			if (boundEffects.Count == 0)
				return;

			for (int i = 0; i < boundEffects.Count; i++)
			{
				if (boundEffects[i] != null)
					boundEffects[i].FinishFX();
			}
			boundEffects = new List<FXInstance>();
		}
		private void BreakShield()
		{
			StopBoundFX(); 
			if(info.EffectOnUserOnShieldBreak)
				info.EffectOnUserOnShieldBreak.Activate(creator, creator, info.BaseData, Vector3.zero, creator.actuallWorldPosition);
			ActivateActivationEffects(info.ShieldBreakEffects, false);
		}
        private void OnDestroy()
        {
			StopBoundFX();
		}
	}
}