using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information.Logic
{
	[CustomEditor(typeof(StandardLogicalExpression), true), CanEditMultipleObjects]
	public class StandardLogicalExpressionEditor : ObjectEditor
	{
		private static bool BaseSettingsOpen;
		protected override void CustomInspector()
		{
			StandardLogicalExpression settings = target as StandardLogicalExpression;
			DrawInFoldoutHeader(new GUIContent("Settings"), ref BaseSettingsOpen, DrawBaseSettings);
		}
		private void DrawBaseSettings()
		{
			StandardLogicalExpression settings = target as StandardLogicalExpression;

			EditorGUILayout.HelpBox("True if " + (settings.Inverse ? "0 Condtions are" : "at least 1 Condtion is") + " met.", MessageType.Info);

			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Inverse))), ref settings.Inverse, 1);
			ObjectArrayField<LogicComponent>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.LogicalComponents))), ref settings.LogicalComponents, serializedObject.FindProperty(nameof(settings.LogicalComponents)), new GUIContent(". Component"), 1);
		}
	}
}