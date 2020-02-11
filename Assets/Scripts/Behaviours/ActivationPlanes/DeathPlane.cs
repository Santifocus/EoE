using System.Collections;
using System.Collections.Generic;
using EoE.Behaviour.Entities;
using UnityEngine;

namespace EoE.Behaviour
{
	public class DeathPlane : ContactActivationObject
	{
		protected override void OnContact(Entity targetEntity)
		{
			base.OnContact(targetEntity);
			targetEntity.BaseDeath(null);
		}
	}
}