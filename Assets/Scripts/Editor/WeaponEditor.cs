using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EoE.Weapons
{
	[CustomEditor(typeof(Weapon), true), CanEditMultipleObjects]
	public class WeaponEditor : Editor
	{
		[MenuItem("EoE/New Weapon")]
		public static void CreateAttackStyle()
		{
			Weapon asset = CreateInstance<Weapon>();

			AssetDatabase.CreateAsset(asset, "Assets/Settings/Weapons/New Weapon.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
			Debug.Log("Created: 'New Weapon' at: Assets/Settings/Weapons/...");
		}
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