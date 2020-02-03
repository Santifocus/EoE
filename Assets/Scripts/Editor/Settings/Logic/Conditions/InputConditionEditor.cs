using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information.Logic
{
	[CustomEditor(typeof(InputCondition), true), CanEditMultipleObjects]
	public class InputConditionConditionEditor : ObjectEditor
	{
		protected override void CustomInspector()
		{
			InputCondition settings = target as InputCondition;
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Inverse))), ref settings.Inverse);
			LineBreak(new Color(0.25f, 0.25f, 0.25f, 0.75f));
			EnumField<InputCondition.InputTarget>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.inputTarget))), ref settings.inputTarget);
			if (settings.inputTarget < InputCondition.InputTarget.MovingCamera)
				EnumField<InputCondition.InputCheckStyle>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.inputCheckStyle))), ref settings.inputCheckStyle);
		}
	}
}