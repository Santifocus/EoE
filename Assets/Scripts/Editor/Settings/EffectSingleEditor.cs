using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;
using EoE.Combatery;

namespace EoE.Information
{
	[CustomEditor(typeof(EffectSingle), true), CanEditMultipleObjects]
	public class EffectSingleEditor : ObjectSettingEditor
	{
		private static bool BaseSettingsOpen;
		private static bool DamageSettingsOpen;
		private static bool KnockbackSettingsOpen;
		private static bool BuffSettingsOpen;
		private static bool BuffArrayOpen;
		private static bool EffectsArrayOpen;
		protected override void CustomInspector()
		{
			BaseSettingsArea();
			DamageSettingsArea();
			KnockbackSettingsArea();
			BuffSettingsArea();

			EffectSingle settings = target as EffectSingle;
			ObjectArrayField<FXObject>(new GUIContent("Effects"), ref settings.Effects, ref EffectsArrayOpen, new GUIContent("Effect "));
		}

		private void BaseSettingsArea()
		{
			EffectSingle settings = target as EffectSingle;

			FoldoutHeader("Base Settings", ref BaseSettingsOpen);
			if (BaseSettingsOpen)
			{
				EnumFlagField(new GUIContent("Affected Targets"), ref settings.AffectedTargets, 1);
				EnumField(new GUIContent("Damage Element"), ref settings.DamageElement, 1);
				EnumField(new GUIContent("Cause Type"), ref settings.CauseType, 1);
			}
			EndFoldoutHeader();
		}
		private void DamageSettingsArea()
		{
			EffectSingle settings = target as EffectSingle;

			FoldoutHeader("Damage", ref DamageSettingsOpen);
			if (DamageSettingsOpen)
			{
				FloatField(new GUIContent("Crit Chance Multiplier"), ref settings.CritChanceMultiplier, 1);
				FloatField(new GUIContent("Base Damage Multiplier"), ref settings.DamageMultiplier, 1);
			}
			EndFoldoutHeader();
		}
		private void KnockbackSettingsArea()
		{
			EffectSingle settings = target as EffectSingle;

			FoldoutHeader("Knockback", ref KnockbackSettingsOpen);
			if (KnockbackSettingsOpen)
			{
				FloatField(new GUIContent("Base Knockback Multiplier"), ref settings.KnockbackMultiplier, 1);
				EnumField(new GUIContent("Knockback Origin"), ref settings.KnockbackOrigin, 1);
				if (settings.KnockbackOrigin != EffectiveDirection.Center)
					EnumField(new GUIContent("Knockback Direction"), ref settings.KnockbackDirection, 1);
				Vector3Field(new GUIContent("Knockback Axis Multiplier"), ref settings.KnockbackAxisMultiplier);
			}
			EndFoldoutHeader();
		}
		private void BuffSettingsArea()
		{
			EffectSingle settings = target as EffectSingle;
			FoldoutHeader("Buffs", ref BuffSettingsOpen);
			if (BuffSettingsOpen)
			{
				EnumField(new GUIContent("Buff Stack Style"), ref settings.BuffStackStyle, 1);
				ObjectArrayField<Buff>(new GUIContent("Buffs To Apply"), ref settings.BuffsToApply, ref BuffArrayOpen, new GUIContent("Buff "), 1);
			}
			EndFoldoutHeader();
		}
	}
}
