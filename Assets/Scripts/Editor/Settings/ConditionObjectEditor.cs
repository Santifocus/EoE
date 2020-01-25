using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(ConditionObject), true), CanEditMultipleObjects]
	public class ConditionObjectEditor : ScriptableObjectEditor
	{
		private static float currentTestValue = 0;
		private bool needsToCheckComparison = true;
		private bool comparisonMet = false;
		protected override void CustomInspector()
		{
			ConditionObject settings = target as ConditionObject;

			needsToCheckComparison |= BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Inverse))), ref settings.Inverse);
			EnumField<ConditionObject.ConditionType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.conditionType))), ref settings.conditionType);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));

			if (settings.conditionType == ConditionObject.ConditionType.Comparison)
			{
				EnumField<ConditionObject.ComparisonTarget>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.comparisonTarget))), ref settings.comparisonTarget);
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
			else if (settings.conditionType == ConditionObject.ConditionType.State)
			{
				EnumField<ConditionObject.StateTarget>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.stateTarget))), ref settings.stateTarget);
			}
			else
			{
				EnumField<ConditionObject.InputTarget>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.inputTarget))), ref settings.inputTarget);
				if(settings.inputTarget < ConditionObject.InputTarget.MovingCamera)
					EnumField<ConditionObject.InputCheckStyle>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.inputCheckStyle))), ref settings.inputCheckStyle);
			}
		}
		private bool DrawValueComparer(GUIContent content, ConditionObject.ValueComparer settings, SerializedProperty property, int offSet = 0)
		{
			bool changed = false;
			Foldout(content, property, offSet);
			if (property.isExpanded)
			{
				changed |= EnumField<ConditionObject.ValueComparer.ComparisonStyle>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.compareStyle))), ref settings.compareStyle, offSet + 1);
				changed |= FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.compareValue))), ref settings.compareValue, offSet + 1);

				Header(settings.compareValue + " " + ComparisonStyleToString(settings.compareStyle) + " Value", 1);
			}
			return changed;
		}
		private string ComparisonStyleToString(ConditionObject.ValueComparer.ComparisonStyle style)
		{
			switch (style)
			{
				case ConditionObject.ValueComparer.ComparisonStyle.LowerEquals:
					return "<=";
				case ConditionObject.ValueComparer.ComparisonStyle.Lower:
					return "<";
				case ConditionObject.ValueComparer.ComparisonStyle.Equals:
					return "==";
				case ConditionObject.ValueComparer.ComparisonStyle.Higher:
					return ">";
				case ConditionObject.ValueComparer.ComparisonStyle.HigherEquals:
					return ">=";
			}

			return "??";
		}
	}
}