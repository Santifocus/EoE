using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
    public class Shield : MonoBehaviour
    {
		private enum ShieldState { Destroyed = 0, OneQuarter = 1, TwoQuarter = 2, ThreeQuarter = 3, FourQuarter = 4 }
		[SerializeField] private SphereCollider coll = default;

        public Entity creator { get; private set; }
		public ShieldData info { get; private set; }

		private List<FXInstance> boundEffects;
		private bool shieldActive;
		private float curShieldResistance;
		private ShieldState curState;

		public static Shield CreateShield(ShieldData info, Entity creator, Vector3 spawnPosition = default)
		{
			Shield newShield = Instantiate(GameController.ShieldPrefab, Storage.ParticleStorage);
			newShield.transform.position = spawnPosition;

			newShield.info = info;
			newShield.creator = creator;
			newShield.coll.radius = info.ShieldSize;
			newShield.curShieldResistance = info.ShieldResistance;
			newShield.curState = ShieldState.FourQuarter;

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
				FXManager.FinishFX(ref boundEffects);
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
					BreakShield();
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
				FXInstance[] createdFXInstances = activationEffects[i].Activate(creator, info.BaseData, 1, transform);
				if (binding)
				{
					if (boundEffects == null)
						boundEffects = new List<FXInstance>(createdFXInstances);
					else
						boundEffects.AddRange(createdFXInstances);
				}
			}
		}
		private void BreakShield()
		{
			FXManager.FinishFX(ref boundEffects);
			if(info.EffectOnUserOnShieldBreak)
				info.EffectOnUserOnShieldBreak.Activate(creator, creator, info.BaseData, Vector3.zero, creator.actuallWorldPosition);
			ActivateActivationEffects(info.ShieldBreakEffects, false);

			shieldActive = coll.enabled = false;
		}
        private void OnDestroy()
		{
			FXManager.FinishFX(ref boundEffects);
		}
	}
}