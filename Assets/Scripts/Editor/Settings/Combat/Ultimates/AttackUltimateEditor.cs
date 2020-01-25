using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor.AttackSequenceDrawer;

namespace EoE.Information
{
	[CustomEditor(typeof(AttackUltimate), true), CanEditMultipleObjects]
	public class AttackUltimateEditor : BasicUltimateEditor
	{
		protected override void CustomInspector()
		{
			base.CustomInspector();

			AttackUltimate settings = (target as AttackUltimate);
			SerializedProperty standAttackProperty = serializedObject.FindProperty(nameof(settings.AttackData));
			DrawAttackSequence(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AttackData))), settings.AttackData, null, standAttackProperty, 0, true);
		}
	}
}