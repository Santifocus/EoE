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
		public ShieldData info { get; private set; }

		private bool shieldActive;
		private float curShieldResistance;
		private FXInstance[][] shieldLevelBoundEffects;

		public static Shield CreateShield(ShieldData info, Entity creator, Vector3 spawnPosition = default)
		{
			Shield newShield = Instantiate(GameController.ShieldPrefab, Storage.ParticleStorage);
			newShield.transform.position = spawnPosition;

			newShield.info = info;
			newShield.creator = creator;
			newShield.coll.radius = info.ShieldSize;
			newShield.curShieldResistance = info.ShieldResistance;
			newShield.shieldLevelBoundEffects = new FXInstance[info.ShieldLevelBasedEffects.Length][];

			newShield.AnchorShield();
			newShield.coll.enabled = false;
			return newShield;
		}
		public void HitShield(float damage)
		{
			curShieldResistance = Mathf.Min(curShieldResistance - damage, info.ShieldResistance);
			ActivateActivationEffects(info.ShieldHitEffects);

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
				if (info.FirstActivationCost.CanAfford(creator, 1, 1, 1) && curShieldResistance >= resistanceCost)
				{
					info.FirstActivationCost.PayCost(creator, 1, 1, 1);
					curShieldResistance -= resistanceCost;
				}
				else
				{
					ActivateActivationEffects(info.ShieldFailedStartEffects);
					return;
				}
			}

			shieldActive = state;
			coll.enabled = state;
			if (shieldActive)
			{
				ActivateActivationEffects(info.ShieldStartEffects);
				UpdateActiveEffects();
			}
			else
			{
				ActivateActivationEffects(info.ShieldDisableEffects);
				FinishLevelBoundFX();
			}
		}
		private void LateUpdate()
		{
			AnchorShield();

			if (shieldActive)
			{
				float resistanceCost = Time.deltaTime * info.ShieldDrain;
				if ((resistanceCost <= curShieldResistance) && (info.BaseData.CanActivate(creator, Time.deltaTime, Time.deltaTime, Time.deltaTime))) 
				{
					info.BaseData.Cost.PayCost(creator, Time.deltaTime, Time.deltaTime, Time.deltaTime);
					curShieldResistance -= resistanceCost;
					UpdateActiveEffects();
				}
				else
				{
					BreakShield();
				}
			}
			else
			{
				curShieldResistance = Mathf.Clamp(curShieldResistance + info.ShieldRegeneration * Time.deltaTime, 0, info.ShieldResistance);
			}
		}
		private void UpdateActiveEffects()
		{
			float normalizedResistance = curShieldResistance / info.ShieldResistance;
			for(int i = 0; i < info.ShieldLevelBasedEffects.Length; i++)
			{
				//Check if this level should be played currently
				bool shouldBePlayed = (normalizedResistance > info.ShieldLevelBasedEffects[i].MinShieldLevel) && (normalizedResistance < info.ShieldLevelBasedEffects[i].MaxShieldLevel);

				//Then check if that matches up by checking if the corrosponding array index is null / notnull
				if (shouldBePlayed == (shieldLevelBoundEffects[i] == null))
				{
					if (shouldBePlayed)
					{
						//First find out how big the array needs to be
						int requiredCapacity = 0;
						for(int j = 0; j < info.ShieldLevelBasedEffects[i].Effects.Length; j++)
						{
							requiredCapacity += info.ShieldLevelBasedEffects[i].Effects[j].FXObjects.Length;
						}

						//Now add the fx
						shieldLevelBoundEffects[i] = new FXInstance[requiredCapacity];
						int addedInstances = 0;
						for (int j = 0; j < info.ShieldLevelBasedEffects[i].Effects.Length; j++)
						{
							FXInstance[] createdFXInstances = info.ShieldLevelBasedEffects[i].Effects[j].Activate(creator, info.BaseData, 1, transform);
							for (int k = 0; k < createdFXInstances.Length; k++, addedInstances++)
							{
								shieldLevelBoundEffects[i][addedInstances] = createdFXInstances[k];
							}
						}
					}
					else
					{
						FXManager.FinishFX(ref shieldLevelBoundEffects[i]);
					}
				}
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
		private void ActivateActivationEffects(ActivationEffect[] activationEffects)
		{
			for (int i = 0; i < activationEffects.Length; i++)
			{
				activationEffects[i].Activate(creator, info.BaseData, 1, transform);
			}
		}
		private void BreakShield()
		{
			if(info.EffectOnUserOnShieldBreak)
				info.EffectOnUserOnShieldBreak.Activate(creator, creator, info.BaseData, Vector3.zero, creator.actuallWorldPosition);

			ActivateActivationEffects(info.ShieldBreakEffects);
			FinishLevelBoundFX();

			shieldActive = coll.enabled = false;
		}
		private void FinishLevelBoundFX()
		{
			for (int i = 0; i < info.ShieldLevelBasedEffects.Length; i++)
			{
				FXManager.FinishFX(ref shieldLevelBoundEffects[i]);
			}
		}
        private void OnDestroy()
		{
			FinishLevelBoundFX();
		}
	}
}