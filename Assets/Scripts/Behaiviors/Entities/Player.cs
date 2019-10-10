using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Events;
using EoE.Information;
using EoE.Controlls;

namespace EoE.Entities
{
	public class Player : Entitie
	{
		public override EntitieSettings SelfSettings => selfSettings;
		[SerializeField] private PlayerSettings selfSettings = default;
		private float currentEndurance;

		private void Start()
		{
			currentEndurance = selfSettings.endurance;
		}
	}
}