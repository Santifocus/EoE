using UnityEditor;
using UnityEngine;

namespace EoE.Weapons
{
	[CustomEditor(typeof(Weapon), true), CanEditMultipleObjects]
	public class WeaponEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			Weapon t = target as Weapon;
			if (!t.weaponAttackStyle)
			{
				EditorGUILayout.HelpBox("Weapon has no Attack-Style selected.", MessageType.Error);
			}
			if (!t.weaponPrefab)
			{
				EditorGUILayout.HelpBox("Weapon has no Weapon Prefab selected.", MessageType.Error);
			}
		}
	}
}