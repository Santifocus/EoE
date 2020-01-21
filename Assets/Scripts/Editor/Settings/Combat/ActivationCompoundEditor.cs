using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(ActivationCompound), true), CanEditMultipleObjects]
	public class ActivationCompoundEditor : ScriptableObjectEditor
	{
		private static bool BaseSettingsOpen;
		protected override void CustomInspector()
		{
			ActivationCompound settings = target as ActivationCompound;
			DrawInFoldoutHeader(new GUIContent("Base Settings"), ref BaseSettingsOpen, DrawBaseInfo);

			for (int i = 0; i < settings.Elements.Length; i++)
			{
				DrawActivationElement(new GUIContent(i + ". Element"), settings.Elements[i], serializedObject.FindProperty(nameof(settings.Elements)).GetArrayElementAtIndex(i));
			}
		}
		private void DrawBaseInfo()
		{
			ActivationCompound settings = target as ActivationCompound;
			DrawCombatObjectBase(settings, serializedObject, 1);
			if(settings.Elements.Length > 0)
				IntSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CostActivationIndex))), ref settings.CostActivationIndex, 0, settings.Elements.Length - 1, 1);

			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CausedCooldown))), ref settings.CausedCooldown, 1);

			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CancelFromStun))), ref settings.CancelFromStun, 1);
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CancelIfCostActivationIsImpossible)), "If the cost of the ActivationCompound was possible at the start of the activation but not when cost activation index is reached, should the Compound be stopped completly or the cost should be applied even if it is not fully paid."), ref settings.CancelIfCostActivationIsImpossible, 1);

			int newSize = settings.Elements.Length;
			DelayedIntField("Element Count", ref newSize, 1);
			
			if (settings.Elements.Length != newSize)
			{
				SerializedProperty arrayProperty = serializedObject.FindProperty(nameof(settings.Elements));
				isDirty = true;
				ActivationElement[] newArray = new ActivationElement[newSize];
				for (int i = 0; i < newSize; i++)
				{
					if (i < settings.Elements.Length)
						newArray[i] = settings.Elements[i];
					else
						break;
				}
				settings.Elements = newArray;
				arrayProperty.arraySize = newSize;
			}
		}
		private void DrawActivationElement(GUIContent content, ActivationElement settings, SerializedProperty property)
		{
			Foldout(content, property, 0, true);
			
			if (settings != null && property.isExpanded)
			{
				DrawRestrictionData(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Restrictions))), settings.Restrictions, property.FindPropertyRelative(nameof(settings.Restrictions)), 1);

				LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
				ObjectArrayField<ConditionObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.PauseConditions)), "While Effects will still be executed but the Duration will not be changed."), ref settings.PauseConditions, property.FindPropertyRelative(nameof(settings.PauseConditions)), new GUIContent(". Condition"), 1);
				ObjectArrayField<ConditionObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.YieldConditions)), "While Effects will NOT be executed and the Duration will not be changed."), ref settings.YieldConditions, property.FindPropertyRelative(nameof(settings.YieldConditions)), new GUIContent(". Condition"), 1);
				ObjectArrayField<ConditionObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StopConditions)), "If any of these conditions are true the compound will be canceled."), ref settings.StopConditions, property.FindPropertyRelative(nameof(settings.StopConditions)), new GUIContent(". Condition"), 1);

				LineBreak(new Color(0.25f, 0.5f, 0.25f, 1));
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ElementDuration))), ref settings.ElementDuration, 1);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WhileTickTime))), ref settings.WhileTickTime, 1);
				if(settings.WhileTickTime <= 0)
				{
					settings.WhileTickTime = 0.0001f;
					isDirty = true;
				}
				Header("Executed While Ticks: " + (long)((settings.ElementDuration - float.Epsilon) / settings.WhileTickTime), 1, false);

				LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
				DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartEffects))), ref settings.StartEffects, property.FindPropertyRelative(nameof(settings.StartEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
				DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WhileEffects))), ref settings.WhileEffects, property.FindPropertyRelative(nameof(settings.WhileEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);

				LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
				DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AtTargetStartEffects))), ref settings.AtTargetStartEffects, property.FindPropertyRelative(nameof(settings.AtTargetStartEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
				DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AtTargetWhileEffects))), ref settings.AtTargetWhileEffects, property.FindPropertyRelative(nameof(settings.AtTargetWhileEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			}
			EndFoldoutHeader();
		}
	}
}