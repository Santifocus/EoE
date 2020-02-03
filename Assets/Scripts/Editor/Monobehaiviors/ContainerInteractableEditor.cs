using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(ContainerInteractable)), CanEditMultipleObjects]
	public class ContainerInteractableEditor : ObjectEditor
	{
		protected override void CustomInspector()
		{
			DrawDefaultInspector();
			ContainerInteractable settings = target as ContainerInteractable;
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.effectsOnPlayerOnFailOpen))), ref settings.effectsOnPlayerOnFailOpen, serializedObject.FindProperty(nameof(settings.effectsOnPlayerOnFailOpen)), DrawActivationEffect, new GUIContent(". Effect"));
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.effectsOnPlayerOnOpen))), ref settings.effectsOnPlayerOnOpen, serializedObject.FindProperty(nameof(settings.effectsOnPlayerOnOpen)), DrawActivationEffect, new GUIContent(". Effect"));
		}
	}
}