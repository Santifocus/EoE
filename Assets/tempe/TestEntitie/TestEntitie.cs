using System.Collections;
using System.Collections.Generic;
using EoE.Information;
using UnityEngine;

namespace EoE.Entities
{
	public class TestEntitie : Entitie
	{
		public override EntitieSettings SelfSettings => settings;
		public EntitieSettings settings;
	}
}