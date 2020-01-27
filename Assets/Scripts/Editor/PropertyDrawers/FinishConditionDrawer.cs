using UnityEditor;
using UnityEngine;

namespace EoE.Information
{
	[CustomPropertyDrawer(typeof(FinishConditions))]
	public class FinishConditionDrawer : PropertyDrawer
	{
		private bool TimeOpen(SerializedProperty property) => property.FindPropertyRelative("OnTimeout").boolValue;
		private bool ConditionMetOpen(SerializedProperty property) => property.FindPropertyRelative("OnConditionMet").boolValue;
		private const float INDENT_WIDHT = 15;
		private float curIndentOff => EditorGUI.indentLevel * INDENT_WIDHT;
		private static FinishConditions exampleInstance = new FinishConditions();
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
				EditorGUI.PropertyField(onParentDeathRect, property.FindPropertyRelative(nameof(exampleInstance.OnParentDeath)));

				Rect onTimeoutRect = new Rect(curIndentOff, position.y + linePos * line++, position.width - curIndentOff, lineHeight);
				EditorGUI.PropertyField(onTimeoutRect, property.FindPropertyRelative(nameof(exampleInstance.OnTimeout)));

				if (TimeOpen(property))
				{
					EditorGUI.indentLevel++;
					Rect finishTimeRect = new Rect(curIndentOff, position.y + linePos * line++, position.width - curIndentOff + 15, lineHeight);
					EditorGUI.PropertyField(finishTimeRect, property.FindPropertyRelative(nameof(exampleInstance.TimeStay)));
					EditorGUI.indentLevel--;
				}

				Rect onConditionMetRect = new Rect(curIndentOff, position.y + linePos * line++, position.width - curIndentOff, lineHeight);
				EditorGUI.PropertyField(onConditionMetRect, property.FindPropertyRelative(nameof(exampleInstance.OnConditionMet)));

				if (ConditionMetOpen(property))
				{
					EditorGUI.indentLevel++;
					Rect conditionRect = new Rect(curIndentOff, position.y + linePos * line++, position.width - curIndentOff + 15, lineHeight);
					EditorGUI.PropertyField(conditionRect, property.FindPropertyRelative(nameof(exampleInstance.Condition)), true);
					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}

			EditorGUI.EndProperty();
		}
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float mul = property.isExpanded ? 4 : 1;
			if (property.isExpanded)
			{
				mul += TimeOpen(property) ? 1 : 0;
				mul += ConditionMetOpen(property) ? 1 : 0;
			}
			return EditorGUIUtility.singleLineHeight * mul + EditorGUIUtility.standardVerticalSpacing * (mul - 1);
		}
	}
}