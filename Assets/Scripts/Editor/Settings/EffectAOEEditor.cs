using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;
using EoE.Combatery;

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
		private static bool EffectsArrayOpen;
		protected override void CustomInspector()
		{
			BaseSettingsArea();
			DamageSettingsArea();
			KnockbackSettingsArea();
			BuffSettingsArea();

			EffectAOE settings = target as EffectAOE;
			DrawArray<CustomFXObject>(new GUIContent("Effects"), DrawCustomFXObject, ref settings.Effects, ref EffectsArrayOpen, 0, true);
		}

		private void BaseSettingsArea()
		{
			EffectAOE settings = target as EffectAOE;

			FoldoutHeader("Base Settings", ref BaseSettingsOpen);
			if (BaseSettingsOpen)
			{
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
			EndFoldoutHeader();
		}
		private void DamageSettingsArea()
		{
			EffectAOE settings = target as EffectAOE;

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
			EffectAOE settings = target as EffectAOE;

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
			EffectAOE settings = target as EffectAOE;
			FoldoutHeader("Buffs", ref BuffSettingsOpen);
			if (BuffSettingsOpen)
			{
				EnumField(new GUIContent("Buff Stack Style"), ref settings.BuffStackStyle, 1);
				ObjectArrayField<Buff>(new GUIContent("Buffs To Apply"), ref settings.BuffsToApply, ref BuffArrayOpen, new GUIContent("Buff "), 1);
			}
			EndFoldoutHeader();
		}
		private bool DrawCustomFXObject(int index, int offset, CustomFXObject[] parentArray)
		{
			bool changed = false;
			if (parentArray == null)
			{
				return false;
			}
			else if (parentArray[index] == null)
			{
				parentArray[index] = new CustomFXObject();
			}

			changed |= Foldout(new GUIContent("Effect " + index), ref parentArray[index].openInInspector, offset);
			if (parentArray[index].openInInspector)
			{
				changed |= ObjectField<FXObject>("Effect", ref parentArray[index].FX, offset + 1);
				changed |= NullableVector3Field(new GUIContent("Has Custom Offset"), new GUIContent("Custom Offset"), ref parentArray[index].CustomOffset, ref parentArray[index].HasCustomOffset, offset + 1);
				changed |= NullableVector3Field(new GUIContent("Has Rotation Offset"), new GUIContent("Custom Rotation Offset"), ref parentArray[index].CustomRotation, ref parentArray[index].HasCustomRotationOffset, offset + 1);
				changed |= NullableVector3Field(new GUIContent("Has Custom Scale"), new GUIContent("Custom Scale"), ref parentArray[index].CustomScale, ref parentArray[index].HasCustomScale, offset + 1);
			}

			return changed;
		}
	}
}
