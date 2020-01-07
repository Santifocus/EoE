using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;
using EoE.Combatery;

namespace EoE.Information
{
	[CustomEditor(typeof(BasicUltimate), true), CanEditMultipleObjects]
	public class BasicUltimateEditor : UltimateEditor
	{
		protected override void CustomInspector()
		{
			base.CustomInspector();

			BasicUltimate settings = (target as BasicUltimate);
			SerializedProperty activationEffectsProperty = serializedObject.FindProperty(nameof(settings.ActivationEffects));
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ActivationEffects))), ref settings.ActivationEffects, activationEffectsProperty, DrawActivationEffect, new GUIContent(". Activation Effect"), 0, true);
		}

	}
}