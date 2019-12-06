using UnityEditor;
using UnityEngine;

namespace EoE.Combatery
{
	[CustomEditor(typeof(Weapon), true), CanEditMultipleObjects]
	public class WeaponEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			Weapon t = target as Weapon;
			if (!t.WeaponPrefab)
			{
				EditorGUILayout.HelpBox("Weapon has no Weapon Prefab selected.", MessageType.Error);
			}
		}
	}
}