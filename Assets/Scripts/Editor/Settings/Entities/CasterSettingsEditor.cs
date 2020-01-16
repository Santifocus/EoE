using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(CasterSettings), true), CanEditMultipleObjects]
	public class CasterSettingsEditor : EnemySettingsEditor
	{
		protected static bool CastingAnnouncementOpen;
		protected override void CombatSettings()
		{
			CasterSettings settings = target as CasterSettings;
			base.CombatSettings();

			DrawArray<CasterSettings.CasterBehaiviorPattern>(	new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BehaiviorPatterns))),
																ref settings.BehaiviorPatterns,
																serializedObject.FindProperty(nameof(settings.BehaiviorPatterns)),
																DrawCasterBehaiviorPattern,
																new GUIContent(". Pattern"),
																1);

			Header("Panic Mode", 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.PanicModeThreshold))), ref settings.PanicModeThreshold, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.PanicModeAlliedSearchRange))), ref settings.PanicModeAlliedSearchRange, 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.PanicModeEffects))), ref settings.PanicModeEffects, serializedObject.FindProperty(nameof(settings.PanicModeEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.PanicModeFinishedEffects))), ref settings.PanicModeFinishedEffects, serializedObject.FindProperty(nameof(settings.PanicModeFinishedEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);

		}
		private void DrawCasterBehaiviorPattern(GUIContent content, CasterSettings.CasterBehaiviorPattern settings, SerializedProperty property, int offSet)
		{
			Foldout(content, property, offSet);
			if (property.isExpanded)
			{
				ObjectField<Spell>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.TargetSpell))), ref settings.TargetSpell, offSet + 1);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MinRange))), ref settings.MinRange, offSet + 1);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MaxRange))), ref settings.MaxRange, offSet + 1);

				if (settings.MinRange < 0)
				{
					settings.MinRange = 0;
					isDirty = true;
				}

				if (settings.MaxRange < settings.MinRange)
				{
					settings.MaxRange = settings.MinRange;
					isDirty = true;
				}
			}
		}
	}
}