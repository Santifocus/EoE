using EoE.Information;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;
using static EoE.EoEEditor.AttackSequenceDrawer;

namespace EoE.Combatery
{
	[CustomEditor(typeof(Weapon), true), CanEditMultipleObjects]
	public class WeaponEditor : ScriptableObjectEditor
	{
		private static bool BaseDataOpen;
		protected override void CustomInspector()
		{
			Weapon settings = target as Weapon;
			if (!settings.WeaponPrefab)
			{
				EditorGUILayout.HelpBox("Weapon has no Weapon Prefab selected.", MessageType.Error);
			}
			DrawInFoldoutHeader("Base Data", ref BaseDataOpen, BaseDataArea);
			GUILayout.Space(1);

			if (settings.HasMaskFlag(AttackStylePart.StandAttack))
			{
				SerializedProperty standAttackProperty = serializedObject.FindProperty(nameof(settings.StandAttackSequence));
				DrawAttackSequence(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StandAttackSequence))), settings.StandAttackSequence, standAttackProperty, 0, true);
				GUILayout.Space(1);
			}
			if (settings.HasMaskFlag(AttackStylePart.RunAttack))
			{
				SerializedProperty runAttackProperty = serializedObject.FindProperty(nameof(settings.RunAttackSequence));
				DrawAttackSequence(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.RunAttackSequence))), settings.RunAttackSequence, runAttackProperty, 0, true);
				GUILayout.Space(1);
			}
			if (settings.HasMaskFlag(AttackStylePart.JumpAttack))
			{
				SerializedProperty jumpAttackProperty = serializedObject.FindProperty(nameof(settings.JumpAttackSequence));
				DrawAttackSequence(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.JumpAttackSequence))), settings.JumpAttackSequence, jumpAttackProperty, 0, true);
				GUILayout.Space(1);
			}
			if (settings.HasMaskFlag(AttackStylePart.RunJumpAttack))
			{
				SerializedProperty runJumpAttackProperty = serializedObject.FindProperty(nameof(settings.RunJumpAttackSequence));
				DrawAttackSequence(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.RunJumpAttackSequence))), settings.RunJumpAttackSequence, runJumpAttackProperty, 0, true);
				GUILayout.Space(1);
			}

			if (settings.HasUltimate)
			{
				SerializedProperty ultimateSettingsProperty = serializedObject.FindProperty(nameof(settings.UltimateSettings));
				DrawInFoldoutHeader(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.UltimateSettings))), ultimateSettingsProperty, () => DrawUltimateSettings(settings.UltimateSettings, ultimateSettingsProperty));
				GUILayout.Space(1);
			}
		}

		private void BaseDataArea()
		{
			Weapon settings = target as Weapon;

			DrawCombatObjectBase(settings, serializedObject, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			ObjectField<WeaponController>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WeaponPrefab))), ref settings.WeaponPrefab, 1);
			EnumField<ElementType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WeaponElement))), ref settings.WeaponElement, 1);
			EnumFlagField<CauseType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WeaponCauseType))), ref settings.WeaponCauseType, 1);
			EnumFlagField<AttackStylePart>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ContainedParts))), ref settings.ContainedParts, 1);
			EnumFlagField<AttackStylePartFallback>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.FallBackPart))), ref settings.FallBackPart, 1);

			if (!settings.HasMaskFlag((AttackStylePart)settings.FallBackPart))
			{
				settings.FallBackPart = AttackStylePartFallback.None;
				isDirty = true;
			}

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			ObjectField<ComboSet>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ComboEffects))), ref settings.ComboEffects, 1);
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.HasUltimate))), ref settings.HasUltimate, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			Vector3Field(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WeaponPositionOffset))), ref settings.WeaponPositionOffset, 1);
			Vector3Field(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WeaponRotationOffset))), ref settings.WeaponRotationOffset, 1);
		}
		private void DrawUltimateSettings(WeaponUltimate settings, SerializedProperty property)
		{

			ObjectField<Ultimate>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Ultimate))), ref settings.Ultimate, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.TotalRequiredCharge))), ref settings.TotalRequiredCharge, 1);
			SliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OnUseChargeRemove))), ref settings.OnUseChargeRemove, 0, 1, 1);

			DrawArray<CustomFXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OnUltimateFullChargeEffects))), ref settings.OnUltimateFullChargeEffects, property.FindPropertyRelative(nameof(settings.OnUltimateFullChargeEffects)), DrawCustomFXObject, new GUIContent(". Effect"), 1);

			LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OnHitCharge))), ref settings.OnHitCharge, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OnCritHitCharge))), ref settings.OnCritHitCharge, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OnKillCharge))), ref settings.OnKillCharge, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.PerComboPointCharge))), ref settings.PerComboPointCharge, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ChargeOverTimeOnCombat))), ref settings.ChargeOverTimeOnCombat, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OutOfCombatDecrease))), ref settings.OutOfCombatDecrease, 1);
		}
	}
}