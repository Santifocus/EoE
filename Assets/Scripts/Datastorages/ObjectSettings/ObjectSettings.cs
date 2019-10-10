using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EoE.Information
{
	public class ObjectSettings : ScriptableObject
	{
#if UNITY_EDITOR
		[CustomEditor(typeof(ObjectSettings)), CanEditMultipleObjects]
		protected class ObjectSettingEditor : Editor
		{
			private bool isDirty = false;
			public override void OnInspectorGUI()
			{
				DrawDefaultInspector();
				CustomInspector();
				if (isDirty)
				{
					isDirty = false;
					EditorUtility.SetDirty(this);
				}
			}
			protected virtual void CustomInspector() { }

			protected bool FloatField(string content, ref float curValue) => FloatField(new GUIContent(content), ref curValue);
			protected bool FloatField(GUIContent content, ref float curValue)
			{
				float newValue = EditorGUILayout.FloatField(content, curValue);
				if (newValue != curValue)
				{
					isDirty = true;
					curValue = newValue;
					return true;
				}
				return false;
			}
			protected bool IntField(string content, ref int curValue) => IntField(new GUIContent(content), ref curValue);
			protected bool IntField(GUIContent content, ref int curValue)
			{
				int newValue = EditorGUILayout.IntField(content, curValue);
				if (newValue != curValue)
				{
					isDirty = true;
					curValue = newValue;
					return true;
				}
				return false;
			}
			protected bool BoolField(string content, ref bool curValue) => BoolField(new GUIContent(content), ref curValue);
			protected bool BoolField(GUIContent content, ref bool curValue)
			{
				bool newValue = EditorGUILayout.Toggle(content, curValue);
				if (newValue != curValue)
				{
					isDirty = true;
					curValue = newValue;
					return true;
				}
				return false;
			}
			protected bool Vector3Field(string content, ref Vector3 curValue) => Vector3Field(new GUIContent(content), ref curValue);
			protected bool Vector3Field(GUIContent content, ref Vector3 curValue)
			{
				Vector3 newValue = EditorGUILayout.Vector3Field(content, curValue);
				if (newValue != curValue)
				{
					isDirty = true;
					curValue = newValue;
					return true;
				}
				return false;
			}
		}
#endif
	}
}