using EoE.Information;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Combatery
{
	[CustomEditor(typeof(ComboSet), true), CanEditMultipleObjects]
	public class ComboSetEditor : ObjectSettingEditor
	{
		private static bool BaseSettingsOpen;
		protected override void CustomInspector()
		{
			ComboSet settings = target as ComboSet;
			DrawInFoldoutHeader(new GUIContent("Base Settings"), ref BaseSettingsOpen, BaseSettingsArea);

			SerializedProperty comboDataProperty = serializedObject.FindProperty(nameof(settings.ComboData));
			FoldoutFromSerializedProperty(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ComboData))), comboDataProperty, 0, true);
			if (comboDataProperty.isExpanded)
			{
				int newSize = settings.ComboData.Length;
				DelayedIntField("Size", ref newSize, 1);

				for (int i = 0; i < settings.ComboData.Length; i++)
				{
					DrawComboInfo(settings.ComboData[i], comboDataProperty.GetArrayElementAtIndex(i), i, 1);
				}

				if (settings.ComboData.Length != newSize)
				{
					isDirty = true;
					ComboInfo[] newArray = new ComboInfo[newSize];
					for (int i = 0; i < newSize; i++)
					{
						if (i < settings.ComboData.Length)
							newArray[i] = settings.ComboData[i];
						else
							newArray[i] = new ComboInfo();
					}
					settings.ComboData = newArray;
					comboDataProperty.arraySize = newSize;
				}

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
		private void DrawComboInfo(ComboInfo settings, SerializedProperty comboInfoProperty, int index, int offSet)
		{
			FoldoutFromSerializedProperty(new GUIContent((index + 1) + ". ComboEffect"), comboInfoProperty, offSet);
			if (comboInfoProperty.isExpanded)
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

				///Effects
				ObjectField<EffectSingle>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.EffectOnTarget))), ref settings.Effect.EffectOnTarget, offSet + 1);
				ObjectField<EffectAOE>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.EffectAOE))), ref settings.Effect.EffectAOE, offSet + 1);

				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				//Heal effects
				SerializedProperty healEffectsProperty = comboInfoProperty.FindPropertyRelative(nameof(settings.Effect)).FindPropertyRelative(nameof(settings.Effect.HealEffects));
				FoldoutFromSerializedProperty(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.HealEffects))), healEffectsProperty, offSet + 1);
				if (healEffectsProperty.isExpanded)
				{
					int newSize = settings.Effect.HealEffects.Length;
					DelayedIntField("Size", ref newSize, offSet + 2);

					for (int i = 0; i < settings.Effect.HealEffects.Length; i++)
					{
						DrawHealTargetInfo(settings.Effect.HealEffects[i], healEffectsProperty.GetArrayElementAtIndex(i), i, offSet + 2);
					}

					if (settings.Effect.HealEffects.Length != newSize)
					{
						isDirty = true;
						HealTargetInfo[] newArray = new HealTargetInfo[newSize];
						for (int i = 0; i < newSize; i++)
						{
							if (i < settings.Effect.HealEffects.Length)
								newArray[i] = settings.Effect.HealEffects[i];
							else
								newArray[i] = new HealTargetInfo();
						}
						settings.Effect.HealEffects = newArray;
						healEffectsProperty.arraySize = newSize;
					}
				}

				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				//FX
				SerializedProperty comboFXProperty = comboInfoProperty.FindPropertyRelative(nameof(settings.Effect)).FindPropertyRelative(nameof(settings.Effect.EffectsTillComboEnds));
				DrawCustomFXObjectArray(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.EffectsTillComboEnds))), ref settings.Effect.EffectsTillComboEnds, comboFXProperty, offSet + 1);

				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				//Buffs
				SerializedProperty buffsProperty = comboInfoProperty.FindPropertyRelative(nameof(settings.Effect)).FindPropertyRelative(nameof(settings.Effect.BuffsTillComboEnds));
				bool buffsOpen = buffsProperty.isExpanded;
				ObjectArrayField<Buff>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effect.BuffsTillComboEnds))), ref settings.Effect.BuffsTillComboEnds, ref buffsOpen, new GUIContent(". Buff"), offSet + 1);
				if (buffsOpen != buffsProperty.isExpanded)
					buffsProperty.isExpanded = buffsOpen;

				LineBreak(new Color(0.65f, 0.25f, 0.25f, 1));
			}
		}
		private void DrawHealTargetInfo(HealTargetInfo settings, SerializedProperty healTargetProperty, int index, int offSet)
		{
			FoldoutFromSerializedProperty((index + 1) + ". HealEffect", healTargetProperty, offSet);
			if (healTargetProperty.isExpanded)
			{
				EnumField<TargetStat>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.HealType))), ref settings.HealType, offSet + 1);
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Percent))), ref settings.Percent, offSet + 1);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Amount))), ref settings.Amount, offSet + 1);
			}
		}
	}
}