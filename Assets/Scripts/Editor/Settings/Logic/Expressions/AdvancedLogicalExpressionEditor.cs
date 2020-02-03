using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information.Logic
{
	[CustomEditor(typeof(AdvancedLogicalExpression), true), CanEditMultipleObjects]
	public class AdvancedLogicalExpressionEditor : ObjectEditor
	{
		private static bool BaseSettingsOpen;
		protected override void CustomInspector()
		{
			AdvancedLogicalExpression settings = target as AdvancedLogicalExpression;
			DrawInFoldoutHeader(new GUIContent("Base Settings"), ref BaseSettingsOpen, DrawBaseSettings);
			DrawArray<LogicalElement>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Elements))), ref settings.Elements, serializedObject.FindProperty(nameof(settings.Elements)), DrawLogicalElement, new GUIContent(". Element"), 0, true);
			EndFoldoutHeader();
		}
		private void DrawBaseSettings()
		{
			AdvancedLogicalExpression settings = target as AdvancedLogicalExpression;

			DrawHelpBoxFromBounds(settings.MinimumTrue, settings.MaximumTrue, settings.Elements.Length, settings.Inverse);
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Inverse))), ref settings.Inverse, 1);
			if (settings.Elements.Length > 0)
			{
				if (IntSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MinimumTrue))), ref settings.MinimumTrue, 0, settings.Elements.Length, 1))
				{
					if (settings.MaximumTrue < settings.MinimumTrue)
					{
						settings.MaximumTrue = settings.MinimumTrue;
						isDirty = true;
					}
				}
				if (IntSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MaximumTrue))), ref settings.MaximumTrue, 0, settings.Elements.Length, 1))
				{
					if (settings.MinimumTrue > settings.MaximumTrue)
					{
						settings.MinimumTrue = settings.MaximumTrue;
						isDirty = true;
					}
				}
			}
			else
			{
				if (settings.MinimumTrue > 0)
				{
					settings.MinimumTrue = 0;
					isDirty = true;
				}
				if (settings.MaximumTrue > 0)
				{
					settings.MaximumTrue = 0;
					isDirty = true;
				}
			}
		}
		private void DrawLogicalElement(GUIContent content, LogicalElement settings, SerializedProperty property, int offSet)
		{
			if(settings == null)
			{
				return;
			}

			Foldout(content, property, offSet);

			if (property.isExpanded)
			{
				DrawHelpBoxFromBounds(settings.MinimumTrue, settings.MaximumTrue, settings.LogicalConditions.Length, settings.Inverse);

				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Inverse))), ref settings.Inverse, offSet + 1);
				if (settings.LogicalConditions.Length > 0)
				{
					if (IntSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MinimumTrue))), ref settings.MinimumTrue, 0, settings.LogicalConditions.Length, offSet + 1))
					{
						if (settings.MaximumTrue < settings.MinimumTrue)
						{
							settings.MaximumTrue = settings.MinimumTrue;
							isDirty = true;
						}
					}
					if (IntSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MaximumTrue))), ref settings.MaximumTrue, 0, settings.LogicalConditions.Length, offSet + 1))
					{
						if (settings.MinimumTrue > settings.MaximumTrue)
						{
							settings.MinimumTrue = settings.MaximumTrue;
							isDirty = true;
						}
					}
				}
				else
				{
					if (settings.MinimumTrue > 0)
					{
						settings.MinimumTrue = 0;
						isDirty = true;
					}
					if (settings.MaximumTrue > 0)
					{
						settings.MaximumTrue = 0;
						isDirty = true;
					}
				}
				ObjectArrayField<LogicComponent>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.LogicalConditions))), ref settings.LogicalConditions, property.FindPropertyRelative(nameof(settings.LogicalConditions)), new GUIContent(". Component"), offSet + 1);
			}
		}
		private void DrawHelpBoxFromBounds(int lowerBound, int upperBound, int checkArea, bool inverse)
		{
			string allowedCounts = "";
			int lastIndex = 0;
			int indexCount = 0;
			for (int i = 0; i < checkArea + 1; i++)
			{
				bool inBound = (i >= lowerBound) && (i <= upperBound);
				if (inBound != inverse)
				{
					allowedCounts += i + ", ";
					lastIndex = i;
					indexCount++;
				}
			}

			string elementsSorP = (lastIndex == 1 && indexCount == 1) ? "Element is" : "Elements are";
			string isExact = (indexCount == 1) ? "exactly " : "";

			if(indexCount == (checkArea + 1))
			{
				EditorGUILayout.HelpBox("Always True.", MessageType.Warning);
			}
			else if (indexCount > 1)
			{
				allowedCounts = allowedCounts.Replace(", " + lastIndex + ", ", " or " + lastIndex);
				EditorGUILayout.HelpBox("True if " + isExact + allowedCounts + " " + elementsSorP + " True.", MessageType.Info);
			}
			else if (indexCount == 1)
			{
				EditorGUILayout.HelpBox("True if " + isExact + lastIndex.ToString() + " " + elementsSorP + " True.", MessageType.Info);
			}
			else if(indexCount == 0)
			{
				EditorGUILayout.HelpBox("Never True.", MessageType.Warning);
			}
		}
	}
}