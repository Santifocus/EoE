using EoE.Information;
using UnityEngine;

namespace EoE.Weapons
{
	public class Weapon : ScriptableObject
	{
		public string Name;
		public AttackStyle weaponAttackStyle;
		public WeaponController weaponPrefab;
		public Vector3 weaponHandleOffset;
		public ElementType element;
		public float baseAttackDamage;
		public float baseEnduranceDrain;
		public float baseKnockbackAmount;
		public float baseCritChance;
	}
}