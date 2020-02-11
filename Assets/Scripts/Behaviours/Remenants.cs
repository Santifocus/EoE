using EoE.Behaviour.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	public class Remenants : MonoBehaviour
	{
		[SerializeField] private Rigidbody body = default;

		private Entity creator;
		private CombatObject baseData;
		private RemnantsData info;
		private List<FXInstance> boundEffects;

		private float delayToWhileCast;
		private float remainingLifeTime;
		private float effectMultiplier;
		private Vector3 spawnPos;
		public static Remenants CreateRemenants(CombatObject baseData, RemnantsData info, Entity creator, Vector3 spawnPos, float effectMultiplier = 1)
		{
			Remenants newRemenants = Instantiate(GameController.RemenantsPrefab, Storage.ProjectileStorage);
			newRemenants.baseData = baseData;
			newRemenants.info = info;
			newRemenants.creator = creator;

			newRemenants.transform.position = newRemenants.spawnPos = spawnPos;
			newRemenants.delayToWhileCast = info.WhileTickTime;

			newRemenants.remainingLifeTime = info.Duration;
			newRemenants.effectMultiplier = effectMultiplier;

			newRemenants.body.useGravity = info.TryGroundRemenants;
			if (info.TryGroundRemenants)
			{
				newRemenants.body.AddForce(Physics.gravity * 10, ForceMode.Impulse);
			}
			
			return newRemenants;
		}
		private void Start()
		{
			ActivateActivationEffects(info.StartEffects, true);
		}
		private void Update()
		{
			delayToWhileCast -= Time.deltaTime;
			remainingLifeTime -= Time.deltaTime;

			if (delayToWhileCast <= 0)
			{
				delayToWhileCast += info.WhileTickTime;
				ActivateActivationEffects(info.WhileEffects, true);
			}

			if (remainingLifeTime < 0)
			{
				ActivateActivationEffects(info.OnEndEffects, false);
				Destroy(gameObject);
			}
		}
		private void ActivateActivationEffects(ActivationEffect[] activationEffects, bool binding)
		{
			for (int i = 0; i < activationEffects.Length; i++)
			{
				FXInstance[] createdFXInstances = activationEffects[i].Activate(creator, baseData, effectMultiplier, transform);
				if (binding)
				{
					if (boundEffects == null)
						boundEffects = new List<FXInstance>(createdFXInstances);
					else
						boundEffects.AddRange(createdFXInstances);
				}
			}
		}
		private void OnDestroy()
		{
			FXManager.FinishFX(ref boundEffects);
		}
		#region Gizmos
		private void OnDrawGizmos()
		{
			if (info.Duration - remainingLifeTime < 0.5f)
				DrawEffectSpheres(info.StartEffects, spawnPos);

			DrawEffectSpheres(info.WhileEffects, transform.position);
		}
		private void DrawEffectSpheres(ActivationEffect[] arrayTarget, Vector3 targetPos)
		{
			for (int i = 0; i < arrayTarget.Length; i++)
			{
				if (arrayTarget[i].HasMaskFlag(EffectType.AOE))
				{
					for (int j = 0; j < arrayTarget[i].AOEEffects.Length; j++)
					{
						Gizmos.color = new Color(0, 1, 0, 0.3f);
						Gizmos.DrawSphere(targetPos, arrayTarget[i].AOEEffects[j].BaseEffectRadius);
						if (arrayTarget[i].AOEEffects[j].ZeroOutDistance > arrayTarget[i].AOEEffects[j].BaseEffectRadius)
						{
							Gizmos.color = Color.red;
							Gizmos.DrawWireSphere(targetPos, arrayTarget[i].AOEEffects[j].ZeroOutDistance);
						}
					}
				}
			}
		}
		#endregion
	}
}