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
			}
		}
	}
}