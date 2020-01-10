using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(Spell), true), CanEditMultipleObjects]
	public class SpellEditor : ScriptableObjectEditor
	{
		private static bool BaseInfoOpen;
		private static bool CastInfoOpen;
		private static bool StartSettingsOpen;

		protected override void CustomInspector()
		{
			Spell settings = target as Spell;

			DrawInFoldoutHeader(new GUIContent("Base Settings"), ref BaseInfoOpen, BaseInfoArea);

			if (settings.HasSpellPart(SpellPart.Cast))
				DrawInFoldoutHeader(new GUIContent("Casting Settings"), ref CastInfoOpen, CastInfoArea);
			if (settings.HasSpellPart(SpellPart.Start))
				DrawInFoldoutHeader(new GUIContent("Start Settings"), ref StartSettingsOpen, StartInfoArea);

			if (settings.HasSpellPart(SpellPart.Projectile))
			{
				SerializedProperty projectileArrayProperty = serializedObject.FindProperty(nameof(settings.ProjectileInfos));
				DrawArray<ProjectileInfo>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ProjectileInfos))), ref settings.ProjectileInfos, projectileArrayProperty, DrawProjectileInfo, new GUIContent(". Projectile"), 0, true);
			}
		}

		private void BaseInfoArea()
		{
			Spell settings = target as Spell;

			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseDamage))), ref settings.BaseDamage, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseHealthCost))), ref settings.BaseHealthCost, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseManaCost))), ref settings.BaseManaCost, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseEnduranceCost))), ref settings.BaseEnduranceCost, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseKnockback))), ref settings.BaseKnockback, 1);
			SliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseCritChance))), ref settings.BaseCritChance, 0, 1, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.SpellCooldown))), ref settings.SpellCooldown, 1);

			EnumFlagField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ContainedParts))), ref settings.ContainedParts, 1);
			EnumFlagField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MovementRestrictions))), ref settings.MovementRestrictions, 1);
			EnumFlagField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.RotationRestrictions))), ref settings.RotationRestrictions, 1);
		}
		private void CastInfoArea()
		{
			Spell settings = target as Spell;

			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CastInfo.Duration))), ref settings.CastInfo.Duration, 1);
			SerializedProperty startEffectsProperty = serializedObject.FindProperty(nameof(settings.CastInfo)).FindPropertyRelative(nameof(settings.CastInfo.StartEffects));
			ObjectArrayField<EffectAOE>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CastInfo.StartEffects))), ref settings.CastInfo.StartEffects, startEffectsProperty, new GUIContent(". Effect"), 1);

			SerializedProperty whileEffectsProperty = serializedObject.FindProperty(nameof(settings.CastInfo)).FindPropertyRelative(nameof(settings.CastInfo.WhileEffects));
			ObjectArrayField<EffectAOE>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CastInfo.WhileEffects))), ref settings.CastInfo.WhileEffects, whileEffectsProperty, new GUIContent(". Effect"), 1);

			//Custom FX Objects
			SerializedProperty visualCastEffectsProperty = serializedObject.FindProperty(nameof(settings.CastInfo)).FindPropertyRelative(nameof(settings.CastInfo.VisualEffects));
			DrawArray<CustomFXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CastInfo.VisualEffects))), ref settings.StartInfo.VisualEffects, visualCastEffectsProperty, DrawCustomFXObject, new GUIContent(". Effect"), 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CastInfo.WhileTickTime))), ref settings.CastInfo.WhileTickTime, 1);
		}
		private void StartInfoArea()
		{
			Spell settings = target as Spell;

			SerializedProperty startEffectsProperty = serializedObject.FindProperty(nameof(settings.StartInfo)).FindPropertyRelative(nameof(settings.StartInfo.Effects));
			ObjectArrayField<EffectAOE>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartInfo.Effects))), ref settings.StartInfo.Effects, startEffectsProperty, new GUIContent(". Effect"), 1);

			SerializedProperty visualStartEffectProperty = serializedObject.FindProperty(nameof(settings.StartInfo)).FindPropertyRelative(nameof(settings.StartInfo.VisualEffects));
			DrawArray<CustomFXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartInfo.VisualEffects))), ref settings.StartInfo.VisualEffects, visualStartEffectProperty, DrawCustomFXObject, new GUIContent(". Effect"), 1);
		}
	}
}