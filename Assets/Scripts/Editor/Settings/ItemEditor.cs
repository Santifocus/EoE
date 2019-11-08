using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(Item), true), CanEditMultipleObjects]
	public class ItemEditor : Editor
	{
		[MenuItem("EoE/Item")]
		public static void CreateItem()
		{
			Item asset = CreateInstance<Item>();

			AssetDatabase.CreateAsset(asset, "Assets/Settings/Items/New Item.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
			Debug.Log("Created: 'New Item' at: Assets/Settings/Items/...");
		}

		private static bool GivenFlagsOpen;
		public override void OnInspectorGUI()
		{
			CustomInspector();
			if (isDirty)
			{
				isDirty = false;
				EditorUtility.SetDirty(target);
			}
		}
		protected virtual void CustomInspector()
		{
			Item item = target as Item;

			StringField(new GUIContent("Item Name"), ref item.ItemName);
			Foldout(new GUIContent("Item Flags"), ref GivenFlagsOpen);
			if (GivenFlagsOpen)
			{
				BoolField(new GUIContent("Usable"), ref item.GivenFlags.Useable, 1);
				BoolField(new GUIContent("Weapon"), ref item.GivenFlags.Weapon, 1);
				BoolField(new GUIContent("Armor"), ref item.GivenFlags.Armor, 1);
			}
			ObjectField(new GUIContent("Item Model"), ref item.ItemModel);
			IntField(new GUIContent("Max Stack"), ref item.MaxStack);
			ObjectField(new GUIContent("Item Icon"), ref item.ItemIcon);
		}
	}
}