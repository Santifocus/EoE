﻿using EoE.Information;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Combatery
{
	[CustomEditor(typeof(RemnantsData), true), CanEditMultipleObjects]
	public class RemenantsDataEditor : ObjectEditor
	{
		private static bool BaseSettingsOpen;
		protected override void CustomInspector()
		{
			DrawInFoldoutHeader(new GUIContent("Base Data"), ref BaseSettingsOpen, DrawBaseSettings);

			RemnantsData settings = target as RemnantsData;

			SerializedProperty startEffectsProperty = serializedObject.FindProperty(nameof(settings.StartEffects));
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartEffects))), ref settings.StartEffects, startEffectsProperty, DrawActivationEffect, new GUIContent(". Effect"), 0, true);

			SerializedProperty whileEffectsProperty = serializedObject.FindProperty(nameof(settings.WhileEffects));
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WhileEffects))), ref settings.WhileEffects, whileEffectsProperty, DrawActivationEffect, new GUIContent(". Effect"), 0, true);

			SerializedProperty onEndEffectsProperty = serializedObject.FindProperty(nameof(settings.OnEndEffects));
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OnEndEffects))), ref settings.OnEndEffects, onEndEffectsProperty, DrawActivationEffect, new GUIContent(". Effect"), 0, true);
		}
		private void DrawBaseSettings()
		{
			RemnantsData settings = target as RemnantsData;
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.TryGroundRemenants))), ref settings.TryGroundRemenants, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Duration))), ref settings.Duration, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WhileTickTime))), ref settings.WhileTickTime, 1);
			if (settings.WhileTickTime <= 0)
			{
				settings.WhileTickTime = 0.0001f;
				isDirty = true;
			}
			Header("Executed While Ticks: " + (long)((settings.Duration - float.Epsilon) / settings.WhileTickTime), 1, false);
		}
	}
}