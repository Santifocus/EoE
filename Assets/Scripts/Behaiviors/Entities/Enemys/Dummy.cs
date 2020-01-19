using System.Collections;
using System.Collections.Generic;
using EoE.Information;
using UnityEngine;

namespace EoE.Entities
{
	public class Dummy : Entity
	{
		public override EntitySettings SelfSettings => DummySettings;
		[SerializeField] private EntitySettings DummySettings = default;
	}
}