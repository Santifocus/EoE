using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(EffectAOE), true), CanEditMultipleObjects]
	public class EffectAOEEditor : ObjectSettingEditor
	{
		private static bool BaseSettingsOpen;
		private static bool DamageSettingsOpen;
		private static bool KnockbackSettingsOpen;
		private static bool BuffSettingsOpen;
		private static bool BuffArrayOpen;
		protected override void CustomInspector()
		{
			DrawInFoldoutHeader("Base Settings", ref BaseSettingsOpen, BaseSettingsArea);
			DrawInFoldoutHeader("Damage Settings", ref DamageSettingsOpen, DamageSettingsArea);
			DrawInFoldoutHeader("Knockback Settings", ref KnockbackSettingsOpen, KnockbackSettingsArea);
			DrawInFoldoutHeader("Buff Settings", ref BuffSettingsOpen, BuffSettingsArea);

			EffectAOE settings = target as EffectAOE;
			DrawArray<CustomFXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Effects))), ref settings.Effects, serializedObject.FindProperty(nameof(settings.Effects)), DrawCustomFXObject, 0, null, true);
		}

		private void BaseSettingsArea()
		{
			EffectAOE settings = target as EffectAOE;

			EnumFlagField(new GUIContent("Affected Targets"), ref settings.AffectedTargets, 1);
			EnumField(new GUIContent("Damage Element"), ref settings.DamageElement, 1);
			EnumField(new GUIContent("Cause Type"), ref settings.CauseType, 1);

			FloatField(new GUIContent("Base Effect Radius"), ref settings.BaseEffectRadius, 1);
			if (FloatField(new GUIContent("Zero Out Distance"), ref settings.ZeroOutDistance, 1))
			{
				settings.ZeroOutDistance = Mathf.Max(settings.BaseEffectRadius, settings.ZeroOutDistance, 1);
			}

			BoolField(new GUIContent("Has Maximum Hits"), ref settings.HasMaximumHits, 1);
			if (settings.HasMaximumHits)
			{
				IntField(new GUIContent("Maximum Hits"), ref settings.MaximumHits, 2);
			}
		}
		private void DamageSettingsArea()
		{
			EffectAOE settings = target as EffectAOE;
			FloatField(new GUIContent("Crit Chance Multiplier"), ref settings.CritChanceMultiplier, 1);
			FloatField(new GUIContent("Base Damage Multiplier"), ref settings.DamageMultiplier, 1);
		}
		private void KnockbackSettingsArea()
		{
			EffectAOE settings = target as EffectAOE;

			FloatField(new GUIContent("Base Knockback Multiplier"), ref settings.KnockbackMultiplier, 1);
			EnumField(new GUIContent("Knockback Origin"), ref settings.KnockbackOrigin, 1);
			if (settings.KnockbackOrigin != EffectiveDirection.Center)
				EnumField(new GUIContent("Knockback Direction"), ref settings.KnockbackDirection, 1);
			Vector3Field(new GUIContent("Knockback Axis Multiplier"), ref settings.KnockbackAxisMultiplier);
		}
		private void BuffSettingsArea()
		{
			EffectAOE settings = target as EffectAOE;

			EnumField(new GUIContent("Buff Stack Style"), ref settings.BuffStackStyle, 1);
			ObjectArrayField<Buff>(new GUIContent("Buffs To Apply"), ref settings.BuffsToApply, ref BuffArrayOpen, new GUIContent(". Buff"), 1);
		}
	}
}
