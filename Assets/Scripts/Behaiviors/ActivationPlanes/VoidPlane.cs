using System.Collections;
using System.Collections.Generic;
using EoE.Entities;
using EoE.Events;
using UnityEngine;

namespace EoE.Behaivior
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