using EoE.Information;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Combatery
{
	[CustomEditor(typeof(ComboSet), true), CanEditMultipleObjects]
	public class ComboSetEditor : ScriptableObjectEditor
	{
		private static bool BaseSettingsOpen;
		protected override void CustomInspector()
		{
			ComboSet settings = target as ComboSet;
			DrawInFoldoutHeader(new GUIContent("Base Settings"), ref BaseSettingsOpen, BaseSettingsArea);

			SerializedProperty comboDataProperty = serializedObject.FindProperty(nameof(settings.ComboData));
			DrawArray<ComboInfo>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ComboData))), ref settings.ComboData, comboDataProperty, DrawComboInfo, new GUIContent(". Combo Info"), 0, true);
			
			//Check if the array needs to be resorted
			if (isDirty)
			{
				bool needsResort = false;
				int lastComboCount = -1;
				for (int i = 0; i < settings.ComboData.Length; i++)
				{
					if (lastComboCount > settings.ComboData[i].RequiredComboCount)
					{
						needsResort = true;
						break;
					}
					lastComboCount = settings.ComboData[i].RequiredComboCount;
				}

				if (needsResort)
				{
					List<ComboInfo> comboList = new List<ComboInfo>(settings.ComboData);

					comboList.Sort((x, y) => x.RequiredComboCount.CompareTo(y.RequiredComboCount));
					settings.ComboData = comboList.ToArray();
				}
			}
		}
		private void BaseSettingsArea()
		{
			ComboSet settings = target as ComboSet;

			GradientField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StandardTextColor))), ref settings.StandardTextColor, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ColorScrollSpeed))), ref settings.ColorScrollSpeed, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));

			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.TextPunch))), ref settings.TextPunch, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.PunchResetSpeed))), ref settings.PunchResetSpeed, 1);
		}
		private void DrawComboInfo(GUIContent content, ComboInfo settings, SerializedProperty property, int offSet)
		{
			if(settings == null)
			{
				settings = new ComboInfo();
				isDirty = true;
			}

			Foldout(content, property, offSet);
			if (property.isExpanded)
			{
				DelayedIntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.RequiredComboCount))), ref settings.RequiredComboCount, offSet + 1);
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));

				//Color override
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.OverrideTextColor))), ref settings.Effect.OverrideTextColor, offSet + 1);
				if (settings.Effect.OverrideTextColor)
				{
					GUILayout.Space(3);
					GradientField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.TextColor))), ref settings.Effect.TextColor, offSet + 2);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.ColorScrollSpeed))), ref settings.Effect.ColorScrollSpeed, offSet + 2);
				}

				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				//Text punch override
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.OverrideTextPunch))), ref settings.Effect.OverrideTextPunch, offSet + 1);
				if (settings.Effect.OverrideTextPunch)
				{
					GUILayout.Space(3);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.TextPunch))), ref settings.Effect.TextPunch, offSet + 2);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.PunchResetSpeed))), ref settings.Effect.PunchResetSpeed, offSet + 2);
				}
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));

				SerializedProperty effectProperty = property.FindPropertyRelative(nameof(settings.Effect));
				///Effects
				ObjectField<EffectSingle>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.EffectOnTarget))), ref settings.Effect.EffectOnTarget, offSet + 1);
				ObjectField<EffectAOE>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.EffectAOE))), ref settings.Effect.EffectAOE, offSet + 1);

				//Heal effects
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				SerializedProperty healEffectsProperty = effectProperty.FindPropertyRelative(nameof(settings.Effect.HealEffects));
				DrawArray<HealTargetInfo>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.HealEffects))), ref settings.Effect.HealEffects, healEffectsProperty, DrawHealTargetInfo, new GUIContent(". Heal Effect"), offSet + 1);
				
				//FX
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				SerializedProperty comboFXProperty = effectProperty.FindPropertyRelative(nameof(settings.Effect.EffectsTillComboEnds));
				DrawArray<CustomFXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.EffectsTillComboEnds))), ref settings.Effect.EffectsTillComboEnds, comboFXProperty, DrawCustomFXObject, new GUIContent(". Effect"), offSet + 1);

				//Buffs
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				SerializedProperty buffsProperty = effectProperty.FindPropertyRelative(nameof(settings.Effect.BuffsTillComboEnds));
				ObjectArrayField<Buff>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.BuffsTillComboEnds))), ref settings.Effect.BuffsTillComboEnds, buffsProperty, new GUIContent(". Buff"), offSet + 1);

				LineBreak(new Color(0.65f, 0.25f, 0.25f, 1));
			}
		}
	}
}