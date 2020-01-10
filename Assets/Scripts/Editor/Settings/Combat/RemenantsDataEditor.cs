using EoE.Information;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Combatery
{
	[CustomEditor(typeof(RemenantsData), true), CanEditMultipleObjects]
	public class RemenantsDataEditor : ScriptableObjectEditor
	{
		private static bool BaseSettingsOpen;
		protected override void CustomInspector()
		{
			DrawInFoldoutHeader(new GUIContent("Base Data"), ref BaseSettingsOpen, DrawBaseSettings);

			RemenantsData settings = target as RemenantsData;

			SerializedProperty startEffectsProperty = serializedObject.FindProperty(nameof(settings.StartEffects));
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartEffects))), ref settings.StartEffects, startEffectsProperty, DrawActivationEffect, new GUIContent(". Effect"), 0, true);

			SerializedProperty whileEffectsProperty = serializedObject.FindProperty(nameof(settings.WhileEffects));
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WhileEffects))), ref settings.WhileEffects, whileEffectsProperty, DrawActivationEffect, new GUIContent(". Effect"), 0, true);

			SerializedProperty onEndEffectsProperty = serializedObject.FindProperty(nameof(settings.OnEndEffects));
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OnEndEffects))), ref settings.OnEndEffects, onEndEffectsProperty, DrawActivationEffect, new GUIContent(". Effect"), 0, true);
		}
		private void DrawBaseSettings()
		{
			RemenantsData settings = target as RemenantsData;
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.TryGroundRemenants))), ref settings.TryGroundRemenants, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Duration))), ref settings.Duration, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WhileTickTime))), ref settings.WhileTickTime, 1);
		}
	}
}