using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EoE.Information
{
	[CustomPropertyDrawer(typeof(FinishConditions))]
	public class FinishConditionDrawer : PropertyDrawer
	{
		private static bool open;
		private bool timeOpen;
		private const float indentSize = 15;
		private float curIndentOff => EditorGUI.indentLevel * indentSize;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			//position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			Rect foldOutRect = position;
			if (open)
				foldOutRect.height = EditorGUIUtility.singleLineHeight;

			open = EditorGUI.Foldout(foldOutRect, open, label, true);
			timeOpen = property.FindPropertyRelative("OnTimeout").boolValue;

			if (open)
			{
				EditorGUI.indentLevel++;

				float lineHeight = EditorGUIUtility.singleLineHeight;
				float linePos = lineHeight + EditorGUIUtility.standardVerticalSpacing;
				int line = 1;

				Rect onParentDeathRect = new Rect(curIndentOff, position.y + linePos * line++, position.width - curIndentOff, lineHeight);
				EditorGUI.PropertyField(onParentDeathRect, property.FindPropertyRelative("OnParentDeath"), new GUIContent("OnParentDeath"));

				Rect onTimeoutRect = new Rect(curIndentOff, position.y + linePos * line++, position.width - curIndentOff, lineHeight);
				EditorGUI.PropertyField(onTimeoutRect, property.FindPropertyRelative("OnTimeout"), new GUIContent("OnTimeout"));

				if (timeOpen)
				{
					EditorGUI.indentLevel++;
					Rect finishTimeRect = new Rect(curIndentOff, position.y + linePos * line++, position.width - curIndentOff + 15, lineHeight);
					EditorGUI.PropertyField(finishTimeRect, property.FindPropertyRelative("TimeStay"), new GUIContent("TimeStay"));
					EditorGUI.indentLevel--;
				}

				Rect internalCancelRect = new Rect(curIndentOff, position.y + linePos * line++, position.width - curIndentOff, lineHeight);
				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.Toggle(internalCancelRect, new GUIContent("Internal Cancel"), true);
				EditorGUI.EndDisabledGroup();

				EditorGUI.indentLevel--;
			}

			EditorGUI.EndProperty();
		}
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float mul = open ? 4 : 1;
			mul += timeOpen && open ? 1 : 0;
			return EditorGUIUtility.singleLineHeight * mul + EditorGUIUtility.standardVerticalSpacing * (mul -1);
		}
	}
}