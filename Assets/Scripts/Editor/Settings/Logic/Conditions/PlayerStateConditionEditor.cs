using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information.Logic
{
	[CustomEditor(typeof(PlayerStateCondition), true), CanEditMultipleObjects]
	public class PlayerStateConditionEditor : ObjectEditor
	{
		protected override void CustomInspector()
		{
			PlayerStateCondition settings = target as PlayerStateCondition;
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Inverse))), ref settings.Inverse);
			LineBreak(new Color(0.25f, 0.25f, 0.25f, 0.75f));
			EnumField<PlayerStateCondition.StateTarget>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.stateTarget))), ref settings.stateTarget);
		}
	}
}