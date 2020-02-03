using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information.Logic
{
	[CustomEditor(typeof(PlayerItemCondition), true), CanEditMultipleObjects]
	public class PlayerItemConditionEditor : ObjectEditor
	{
		protected override void CustomInspector()
		{
			PlayerItemCondition settings = target as PlayerItemCondition;
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Inverse))), ref settings.Inverse);
			LineBreak(new Color(0.25f, 0.25f, 0.25f, 0.75f));

			ObjectField<Item>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.TargetItem))), ref settings.TargetItem);

			IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MinStackSize))), ref settings.MinStackSize);
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.HasMaxStackSize))), ref settings.HasMaxStackSize);
			if (settings.HasMaxStackSize)
			{
				IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MaxStackSize))), ref settings.MaxStackSize, 1);
			}
			if(settings.MaxStackSize < settings.MinStackSize)
			{
				settings.MaxStackSize = settings.MinStackSize;
				isDirty = true;
			}

			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.HasToBeEquipped))), ref settings.HasToBeEquipped);
		}
	}
}