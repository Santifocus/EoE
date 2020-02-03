using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;
using EoE.Combatery;

namespace EoE.Information
{
	[CustomEditor(typeof(Ultimate)), CanEditMultipleObjects]
	public class UltimateEditor : ObjectEditor
	{
		private static bool BaseSettingsOpen;
		protected override void CustomInspector()
		{
			Ultimate settings = target as Ultimate;
			DrawInFoldoutHeader(new GUIContent("Base Settings"), ref BaseSettingsOpen, DrawBaseSettings);
		}

		protected virtual void DrawBaseSettings()
		{
			Ultimate settings = target as Ultimate;
			DrawCombatObjectBase(settings, serializedObject, 1);
			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			ObjectField<Sprite>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.UltimateIcon))), ref settings.UltimateIcon, 1);
		}
	}
}