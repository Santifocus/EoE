using System.Collections;
using System.Collections.Generic;
using EoE.Behaviour.Entities;
using EoE.Events;
using UnityEngine;

namespace EoE.Behaviour
{
	public class VoidPlane : ContactActivationObject
	{
		protected override void OnContact(Entity targetEntity)
		{
			base.OnContact(targetEntity);
			EventManager.PlayerDiedInvoke(null);
		}
	}
}