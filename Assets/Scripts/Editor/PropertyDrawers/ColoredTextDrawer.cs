using EoE.UI;
using UnityEditor;
using UnityEngine;

namespace EoE
{
	[CustomPropertyDrawer(typeof(ColoredText))]
	public class ColoredTextDrawer : PropertyDrawer
	{
		private static ColoredText exampleInstance = default;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			float lineHeight = EditorGUIUtility.singleLineHeight;
			float curIndentOff = System.Math.Max(1, EditorGUI.indentLevel) * 15;
			float colorFieldWidht = 100;
			float textFieldWidht = position.width - curIndentOff - colorFieldWidht;

			SerializedProperty curTextProperty = property.FindPropertyRelative(nameof(exampleInstance.text));
			Rect textRect = new Rect(curIndentOff, position.y, textFieldWidht, lineHeight);
			curTextProperty.stringValue = EditorGUI.TextField(textRect, label, curTextProperty.stringValue);

			SerializedProperty curColorProperty = property.FindPropertyRelative(nameof(exampleInstance.textColor));
			Rect colorRect = new Rect(curIndentOff + textFieldWidht, position.y, colorFieldWidht, lineHeight);
			curColorProperty.colorValue = EditorGUI.ColorField(colorRect, curColorProperty.colorValue);

			EditorGUI.EndProperty();
		}
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}
	}
}