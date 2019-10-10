using System.Collections;
using System.Collections.Generic;
using EoE.Information;
using UnityEngine;

namespace EoE.Entities
{
	public class testentitie : Entitie
	{
		public override EntitieSettings SelfSettings => settings;
		public EntitieSettings settings;

		protected override void EntitieStart()
		{
			StartCoroutine(testDamage());
		}

		private IEnumerator testDamage()
		{
			yield return new WaitForSeconds(2);
			ChangeHealth(new InflictionInfo(this, CauseType.Physical, ElementType.None, transform.position + Vector3.right/2, 15, false));
			//StartCoroutine(testDamage());
		}
	}
}