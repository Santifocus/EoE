using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Behaivior
{
	public class ContactActivationObject : MonoBehaviour
	{
		[SerializeField] private bool onlyActivateForPlayer = true;
		[SerializeField] private FXObject[] effectsOnContact = default;
		[SerializeField] private bool triggerOnlyOnce = default;

		private bool wasTriggered;
        protected virtual void OnContact(Entity targetEntity)
		{
			FXManager.ExecuteFX(effectsOnContact, targetEntity.transform, targetEntity is Player);
		}
		private void OnTriggerEnter(Collider other)
		{
			if (wasTriggered && triggerOnlyOnce)
				return;

			if (other.gameObject.layer != ConstantCollector.ENTITIE_LAYER)
				return;

			Entity targetEntity = other.gameObject.GetComponent<Entity>();
			if (onlyActivateForPlayer && !(targetEntity is Player))
				return;

			wasTriggered = true;
			OnContact(targetEntity);
		}
	}
}