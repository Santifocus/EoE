using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;
using EoE.Combatery;

namespace EoE.Information
{
	[CustomEditor(typeof(Ultimate)), CanEditMultipleObjects]
	public class UltimateEditor : ScriptableObjectEditor
	{
		protected override void CustomInspector()
		{
			Ultimate settings = target as Ultimate;
			SerializedProperty baseSettingsProperty = serializedObject.FindProperty(nameof(settings.BaseSettings));
			DrawInFoldoutHeader(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseSettings))), baseSettingsProperty, DrawBaseSettings);
		}

		protected virtual void DrawBaseSettings()
		{
			Ultimate settings = target as Ultimate;
			ObjectField<Sprite>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.UltimateIcon))), ref settings.UltimateIcon, 1);
			ObjectField<CombatObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseSettings))), ref settings.BaseSettings, 1);
		}
	}
}