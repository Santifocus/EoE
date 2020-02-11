using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EoE.Behaviour.Entities;
using EoE.Information;
using EoE.Information.Logic;

namespace EoE.Behaviour
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
				return false; //Not interactable => return false as if we did not try to interact with it

			if(interactCondition && !interactCondition.True)
			{
				if(failInteractEffectCooldown <= 0)
				{
					failInteractEffectCooldown = FAIL_INTERACT_EFFECT_COOLDOWN;
					FXManager.ExecuteFX(failedInteractEffects, Player.Instance.transform, true);
				}
				return true; //We did try to interact but the condition failed so we still return true
			}

			Interact();
			return true; //Was able to itneract and the condition was true => let the invoker know it was successfull
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