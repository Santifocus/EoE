using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(TutorialSettings)), CanEditMultipleObjects]
	public class TutorialSettingsEditor : ObjectEditor
	{
		protected override void CustomInspector()
		{
			TutorialSettings settings = target as TutorialSettings;

			DrawArray<TutorialPart>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Parts))), ref settings.Parts, serializedObject.FindProperty(nameof(settings.Parts)), DrawTutorialPart, PartContentGetter, 0, true);
		}
		private void DrawTutorialPart(GUIContent content, TutorialPart settings, SerializedProperty property, int offSet)
		{
			if(settings == null)
			{
				return;
			}
			Foldout(content, property, offSet);
			if (property.isExpanded)
			{
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DelayTime))), ref settings.DelayTime, offSet + 1);
				DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effects))), ref settings.Effects, property.FindPropertyRelative(nameof(settings.Effects)), DrawActivationEffect, new GUIContent(". Effect"), offSet + 1);

				LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.SpawnDummy))), ref settings.SpawnDummy, offSet + 1);
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DeleteDummyOnFinish))), ref settings.DeleteDummyOnFinish, offSet + 1);
			}
		}
		private GUIContent PartContentGetter(int index)
		{
			TutorialSettings settings = target as TutorialSettings;

			TutorialPart part = settings.Parts[index];

			if (part != null && 
				part.Effects?.Length > 0 &&
				part.Effects[0]?.FXObjects?.Length > 0 && 
				part.Effects[0].FXObjects[0]?.FX)
			{
				return new GUIContent(ObjectNames.NicifyVariableName(part.Effects[0].FXObjects[0].FX.name));
			}
			return new GUIContent((index + 1) + ". Part");
		}
	}
}