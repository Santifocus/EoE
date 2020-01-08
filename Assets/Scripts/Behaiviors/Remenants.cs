using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Combatery
{
	public class Remenants : MonoBehaviour
	{
		[SerializeField] private Rigidbody body = default;

		private Entitie creator;
		private CombatObject baseData;
		private RemenantsData info;
		private List<FXInstance> boundEffects = new List<FXInstance>();

		private float delayToWhileCast;
		private float remainingLifeTime;
		public static Remenants CreateRemenants(CombatObject baseData, RemenantsData info, Entitie creator, Vector3 spawnPos)
		{
			Remenants newRemenants = Instantiate(GameController.RemenantsPrefab, Storage.ProjectileStorage);
			newRemenants.baseData = baseData;
			newRemenants.info = info;
			newRemenants.creator = creator;

			newRemenants.transform.position = spawnPos;
			newRemenants.delayToWhileCast = info.WhileTickTime;

			newRemenants.ActivateAOEEffects(info.StartEffects);
			newRemenants.CreateFX(info.VisualEffects, true);

			newRemenants.remainingLifeTime = info.Duration;

			newRemenants.body.useGravity = info.TryGroundRemenants;

			return newRemenants;
		}
		private void Update()
		{
			delayToWhileCast -= Time.deltaTime;
			remainingLifeTime -= Time.deltaTime;

			if (delayToWhileCast <= 0)
			{
				delayToWhileCast += info.WhileTickTime;
				ActivateAOEEffects(info.WhileEffects);
			}

			if (remainingLifeTime < 0)
			{
				Destroy(gameObject);
			}
		}
		private void ActivateAOEEffects(EffectAOE[] effects)
		{
			for (int i = 0; i < effects.Length; i++)
				effects[i].Activate(creator, transform, baseData);
		}
		private void CreateFX(CustomFXObject[] effects, bool bind = false)
		{
			for (int i = 0; i < effects.Length; i++)
			{
				FXInstance instance = FXManager.PlayFX(effects[i], transform, false, 1);
				if (bind)
					boundEffects.Add(instance);
			}
		}
		private void StopBoundFX()
		{
			for (int i = 0; i < boundEffects.Count; i++)
			{
				boundEffects[i].FinishFX();
			}
			boundEffects = new List<FXInstance>();
		}
		private void OnDestroy()
		{
			StopBoundFX();
		}
	}
}