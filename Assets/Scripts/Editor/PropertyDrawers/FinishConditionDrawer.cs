﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EoE.Information
{
	[CustomPropertyDrawer(typeof(FinishConditions))]
	public class FinishConditionDrawer : PropertyDrawer
	{
		private bool TimeOpen(SerializedProperty property) => property.FindPropertyRelative("OnTimeout").boolValue;
		private const float INDENT_WIDHT = 15;
		private float curIndentOff => EditorGUI.indentLevel * INDENT_WIDHT;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			Rect foldOutRect = position;
			if (property.isExpanded)
				foldOutRect.height = EditorGUIUtility.singleLineHeight;

			property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, label, true);

			if (property.isExpanded)
			{
				EditorGUI.indentLevel++;

				float lineHeight = EditorGUIUtility.singleLineHeight;
				float linePos = lineHeight + EditorGUIUtility.standardVerticalSpacing;
				int line = 1;

				Rect onParentDeathRect = new Rect(curIndentOff, position.y + linePos * line++, position.width - curIndentOff, lineHeight);
				EditorGUI.PropertyField(onParentDeathRect, property.FindPropertyRelative("OnParentDeath"));

				Rect onTimeoutRect = new Rect(curIndentOff, position.y + linePos * line++, position.width - curIndentOff, lineHeight);
				EditorGUI.PropertyField(onTimeoutRect, property.FindPropertyRelative("OnTimeout"));

				if (TimeOpen(property))
				{
					EditorGUI.indentLevel++;
					Rect finishTimeRect = new Rect(curIndentOff, position.y + linePos * line++, position.width - curIndentOff + 15, lineHeight);
					EditorGUI.PropertyField(finishTimeRect, property.FindPropertyRelative("TimeStay"));
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
			float mul = property.isExpanded ? 4 : 1;
			mul += TimeOpen(property) && property.isExpanded ? 1 : 0;
			return EditorGUIUtility.singleLineHeight * mul + EditorGUIUtility.standardVerticalSpacing * (mul -1);
		}
	}
}