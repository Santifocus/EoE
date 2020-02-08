﻿using EoE.Information;
using EoE.Information.Logic;
using UnityEngine;

namespace EoE.Entities
{
	public abstract class Interactable : MonoBehaviour
	{
		private const float FAIL_INTERACT_EFFECT_COOLDOWN = 0.2f;
		public static Interactable MarkedInteractable;
		protected bool isTarget { get; private set; }
		public bool IsInteractable => canBeInteracted;
		[SerializeField] protected LogicComponent interactCondition = default;
		[SerializeField] protected FXObject[] failedInteractEffects = default;

		protected bool canBeInteracted;
		private float failInteractEffectCooldown;
		public void MarkAsInteractTargetBase()
		{
			isTarget = true;
			MarkedInteractable = this;
			MarkAsInteractTarget();
		}
		public void StopMarkAsInteractableBase()
		{
			isTarget = false;
			if (MarkedInteractable == this)
				MarkedInteractable = null;
			StopMarkAsInteractable();
		}
		public bool TryInteract()
		{
			if (!canBeInteracted)
				return false;

			if(interactCondition && !interactCondition.True)
			{
				if(failInteractEffectCooldown <= 0)
				{
					failInteractEffectCooldown = FAIL_INTERACT_EFFECT_COOLDOWN;
					FXManager.ExecuteFX(failedInteractEffects, Player.Instance.transform, true);
				}
				return false;
			}

			Interact();
			return true;
		}
		private void Update()
		{
			if (failInteractEffectCooldown > 0) 
				failInteractEffectCooldown -= Time.deltaTime;
		}
		protected abstract void Interact();
		protected abstract void MarkAsInteractTarget();
		protected abstract void StopMarkAsInteractable();
	}
}