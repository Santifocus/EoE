using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(EffectSingle), true), CanEditMultipleObjects]
	public class EffectSingleEditor : ObjectEditor
	{
		private static bool BaseSettingsOpen;
		private static bool DamageSettingsOpen;
		private static bool KnockbackSettingsOpen;
		private static bool BuffSettingsOpen;

		protected override void CustomInspector()
		{
			DrawInFoldoutHeader(new GUIContent("Base Settings"), ref BaseSettingsOpen, BaseSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Damage Settings"), ref DamageSettingsOpen, DamageSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Knockback Settings"), ref KnockbackSettingsOpen, KnockbackSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Buff Settings"), ref BuffSettingsOpen, BuffSettingsArea);

			EffectSingle settings = target as EffectSingle;
			DrawArray<CustomFXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effects))), ref settings.Effects, serializedObject.FindProperty(nameof(settings.Effects)), DrawCustomFXObject, new GUIContent(". Effect"), 0, true);
		}

		private void BaseSettingsArea()
		{
			EffectSingle settings = target as EffectSingle;

			EnumFlagField(new GUIContent("Affected Targets"), ref settings.AffectedTargets, 1);
			EnumField(new GUIContent("Damage Element"), ref settings.DamageElement, 1);
			EnumField(new GUIContent("Cause Type"), ref settings.CauseType, 1);
		}
		private void DamageSettingsArea()
		{
			EffectSingle settings = target as EffectSingle;
			FloatField(new GUIContent("Crit Chance Multiplier"), ref settings.CritChanceMultiplier, 1);
			FloatField(new GUIContent("Base Damage Multiplier"), ref settings.DamageMultiplier, 1);
		}
		private void KnockbackSettingsArea()
		{
			EffectSingle settings = target as EffectSingle;
			EditorGUILayout.HelpBox("Note: In some cases the direction will be overriden. For example Physicall combat Weapons apply knockback in the direction the Weapon was swung.", MessageType.Info);
			FloatField(new GUIContent("Base Knockback Multiplier"), ref settings.KnockbackMultiplier, 1);
			EnumField(new GUIContent("Knockback Origin"), ref settings.KnockbackOrigin, 1);
			if (settings.KnockbackOrigin != EffectiveDirection.Center)
				EnumField(new GUIContent("Knockback Direction"), ref settings.KnockbackDirection, 1);
			Vector3Field(new GUIContent("Knockback Axis Multiplier"), ref settings.KnockbackAxisMultiplier, 1);
		}
		private void BuffSettingsArea()
		{
			EffectSingle settings = target as EffectSingle;
			EnumField(new GUIContent("Buff Stack Style"), ref settings.BuffStackStyle, 1);
			SerializedProperty buffsArrayProperty = serializedObject.FindProperty(nameof(settings.BuffsToApply));
			ObjectArrayField<Buff>(new GUIContent("Buffs To Apply"), ref settings.BuffsToApply, buffsArrayProperty, new GUIContent(". Buff"), 1);
		}
	}
}
