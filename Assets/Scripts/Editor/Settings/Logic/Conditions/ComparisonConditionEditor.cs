using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information.Logic
{
	[CustomEditor(typeof(ComparisonCondition), true), CanEditMultipleObjects]
	public class ComparisonConditionEditor : ObjectEditor
	{
		private static float currentTestValue = 0;
		private bool needsToCheckComparison = true;
		private bool comparisonMet = false;
		protected override void CustomInspector()
		{
			ComparisonCondition settings = target as ComparisonCondition;

			needsToCheckComparison |= BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Inverse))), ref settings.Inverse);
			LineBreak(new Color(0.25f, 0.25f, 0.25f, 0.75f));

			EnumField<ComparisonCondition.ComparisonTarget>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.comparisonTarget))), ref settings.comparisonTarget);
			needsToCheckComparison |= DrawValueComparer(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.firstComparer))), settings.firstComparer, serializedObject.FindProperty(nameof(settings.firstComparer)));

			needsToCheckComparison |= BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.useSecondComparer))), ref settings.useSecondComparer);
			if (settings.useSecondComparer)
			{
				needsToCheckComparison |= DrawValueComparer(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.secondComparer))), settings.secondComparer, serializedObject.FindProperty(nameof(settings.secondComparer)));
			}

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			if (needsToCheckComparison)
			{
				bool isMet = settings.firstComparer.ComparisonMet(currentTestValue) && (!settings.useSecondComparer || settings.secondComparer.ComparisonMet(currentTestValue));
				comparisonMet = settings.Inverse != isMet;
			}
			needsToCheckComparison |= FloatField(new GUIContent("Test Value"), ref currentTestValue);
			GUILayout.Label("Test Comparison is: " + comparisonMet + "!", EditorStyles.boldLabel);
		}
		private bool DrawValueComparer(GUIContent content, ComparisonCondition.ValueComparer settings, SerializedProperty property, int offSet = 0)
		{
			bool changed = false;
			Foldout(content, property, offSet);
			if (property.isExpanded)
			{
				changed |= EnumField<ComparisonCondition.ValueComparer.ComparisonStyle>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.compareStyle))), ref settings.compareStyle, offSet + 1);
				changed |= FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.compareValue))), ref settings.compareValue, offSet + 1);

				Header(settings.compareValue + " " + ComparisonStyleToString(settings.compareStyle) + " Value", 1);
			}
			return changed;
		}
		private string ComparisonStyleToString(ComparisonCondition.ValueComparer.ComparisonStyle style)
		{
			switch (style)
			{
				case ComparisonCondition.ValueComparer.ComparisonStyle.LowerEquals:
					return "<=";
				case ComparisonCondition.ValueComparer.ComparisonStyle.Lower:
					return "<";
				case ComparisonCondition.ValueComparer.ComparisonStyle.Equals:
					return "==";
				case ComparisonCondition.ValueComparer.ComparisonStyle.Higher:
					return ">";
				case ComparisonCondition.ValueComparer.ComparisonStyle.HigherEquals:
					return ">=";
			}

			return "??";
		}
	}
}