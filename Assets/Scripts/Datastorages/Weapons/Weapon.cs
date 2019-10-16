using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Weapons
{
	public class Weapon : ScriptableObject
	{
		public string Name;
		public AttackStyle weaponAttackStyle;
		public WeaponController weaponPrefab;
		public float baseAttackDamage;
		public float baseEnduranceDrain;
	}
}