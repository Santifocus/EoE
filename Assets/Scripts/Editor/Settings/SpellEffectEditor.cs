using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;
using EoE.Weapons;

namespace EoE.Information
{
	[CustomEditor(typeof(SpellEffect), true), CanEditMultipleObjects]
	public class SpellEffectEditor : ObjectSettingEditor
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

			SpellEffect settings = target as SpellEffect;
			ObjectArrayField<FXObject>(new GUIContent("Effects"), ref settings.Effects, ref EffectsArrayOpen, new GUIContent("Effect "));
		}

		private void BaseSettingsArea()
		{
			SpellEffect settings = target as SpellEffect;

			FoldoutHeader("Base Settings", ref BaseSettingsOpen);
			if (BaseSettingsOpen)
			{
				EnumFlagField(new GUIContent("Affected Targets"), ref settings.AffectedTargets, 1);

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
			EndFoldoutHeader();
		}
		private void DamageSettingsArea()
		{
			SpellEffect settings = target as SpellEffect;

			FoldoutHeader("Damage", ref DamageSettingsOpen);
			if (DamageSettingsOpen)
			{
				EnumField(new GUIContent("Damage Element"), ref settings.DamageElement, 1);
				FloatField(new GUIContent("Crit Chance"), ref settings.CritChance, 1);
				FloatField(new GUIContent("Base Damage Multiplier"), ref settings.DamageMultiplier, 1);
			}
			EndFoldoutHeader();
		}
		private void KnockbackSettingsArea()
		{
			SpellEffect settings = target as SpellEffect;

			FoldoutHeader("Knockback", ref KnockbackSettingsOpen);
			if (KnockbackSettingsOpen)
			{
				FloatField(new GUIContent("Base Knockback Multiplier"), ref settings.KnockbackMultiplier, 1);
				EnumField(new GUIContent("Knockback Origin"), ref settings.KnockbackOrigin, 1);
				if (settings.KnockbackOrigin != EffectiveDirection.Center)
					EnumField(new GUIContent("Knockback Direction"), ref settings.KnockbackDirection, 1);
			}
			EndFoldoutHeader();
		}
		private void BuffSettingsArea()
		{
			SpellEffect settings = target as SpellEffect;
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
