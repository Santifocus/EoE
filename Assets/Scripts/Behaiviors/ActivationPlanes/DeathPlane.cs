using System.Collections;
using System.Collections.Generic;
using EoE.Entities;
using UnityEngine;

namespace EoE.Behaivior
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