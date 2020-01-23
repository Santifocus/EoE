using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(TutorialSettings)), CanEditMultipleObjects]
	public class TutorialSettingsEditor : ScriptableObjectEditor
	{
		protected override void CustomInspector()
		{
			TutorialSettings settings = target as TutorialSettings;

			DrawArray<TutorialPart>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Parts))), ref settings.Parts, serializedObject.FindProperty(nameof(settings.Parts)), DrawTutorialPart, new GUIContent(". Part"), 0, true);
		}
		private void DrawTutorialPart(GUIContent content, TutorialPart settings, SerializedProperty property, int offSet)
		{
			Foldout(content, property, offSet);
			if (property.isExpanded)
			{
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DelayTime))), ref settings.DelayTime, offSet + 1);
				DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effects))), ref settings.Effects, property.FindPropertyRelative(nameof(settings.Effects)), DrawActivationEffect, new GUIContent(". Effect"), offSet + 1);

				LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.SpawnDummy))), ref settings.SpawnDummy, offSet + 1);
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DeleteDummyOnFinish))), ref settings.DeleteDummyOnFinish, offSet + 1);

				LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
				DrawArray<ItemGiveInfo>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ItemsToGive))), ref settings.ItemsToGive, property.FindPropertyRelative(nameof(settings.ItemsToGive)), DrawStartItem, new GUIContent(". Item"), offSet + 1);
			}
		}

		private void DrawStartItem(GUIContent content, ItemGiveInfo settings, SerializedProperty property, int offSet)
		{
			if (settings == null)
			{
				isDirty = true;
				settings = new ItemGiveInfo();
			}
			Foldout(content, property, offSet);

			if (property.isExpanded)
			{
				ObjectField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Item))), ref settings.Item, offSet + 1);
				IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ItemCount))), ref settings.ItemCount, offSet + 1);
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ForceEquip))), ref settings.ForceEquip, offSet + 1);
			}
		}
	}
}