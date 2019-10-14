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
		[CustomEditor(typeof(ObjectSettings), true), CanEditMultipleObjects]
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
					EditorUtility.SetDirty(target);
				}
			}
			protected virtual void CustomInspector() { }

			protected void Header(string content) => Header(new GUIContent(content));
			protected void Header(GUIContent content)
			{
				GUILayout.Space(8);
				GUILayout.Label(content, EditorStyles.boldLabel);
				GUILayout.Space(4);
			}
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
			protected bool Vector2Field(string content, ref Vector2 curValue) => Vector2Field(new GUIContent(content), ref curValue);
			protected bool Vector2Field(GUIContent content, ref Vector2 curValue)
			{
				Vector2 newValue = EditorGUILayout.Vector2Field(content, curValue);
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
			protected bool ColorField(string content, ref Color curValue) => ColorField(new GUIContent(content), ref curValue);
			protected bool ColorField(GUIContent content, ref Color curValue)
			{
				Color newValue = EditorGUILayout.ColorField(content, curValue);
				if (newValue != curValue)
				{
					isDirty = true;
					curValue = newValue;
					return true;
				}
				return false;
			}
			protected bool GradientField(string content, ref Gradient curValue) => GradientField(new GUIContent(content), ref curValue);
			protected bool GradientField(GUIContent content, ref Gradient curValue)
			{
				//Create a copy so we can compare them afterwards
				Gradient newValue = new Gradient();
				newValue.colorKeys = curValue.colorKeys;
				newValue.alphaKeys = curValue.alphaKeys;

				newValue = EditorGUILayout.GradientField(content, newValue);

				if (!newValue.Equals(curValue))
				{
					isDirty = true;
					curValue = newValue;
					return true;
				}
				return false;
			}
			protected bool AnimationCurveField(string content, ref AnimationCurve curValue) => AnimationCurveField(new GUIContent(content), ref curValue);
			protected bool AnimationCurveField(GUIContent content, ref AnimationCurve curValue)
			{
				//Create a copy so we can compare them afterwards
				AnimationCurve newValue = new AnimationCurve(curValue.keys);
				newValue = EditorGUILayout.CurveField(content, newValue);

				if (!newValue.Equals(curValue))
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