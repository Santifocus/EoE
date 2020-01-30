using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(CrawlerSettings), true), CanEditMultipleObjects]
	public class CrawlerSettingsEditor : EnemySettingsEditor
	{
		protected static bool AnimationSettingsOpen;
		protected override void CustomInspector()
		{
			base.CustomInspector();
			DrawInFoldoutHeader("Effect Settings", ref EffectSettingsOpen, EffectSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Animation Settings"), ref AnimationSettingsOpen, DrawAnimationSettings);
		}
		protected override void CombatSettings()
		{
			CrawlerSettings settings = target as CrawlerSettings;
			base.CombatSettings();

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ForceTranslationMultiplier))), ref settings.ForceTranslationMultiplier, 1);

			Header("Bash", 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BashChargeSpeed))), ref settings.BashChargeSpeed, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BashSpeed))), ref settings.BashSpeed, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BashDistance))), ref settings.BashDistance, 1);

			Header("Trick Bash", 1);
			FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ChanceForTrickBash))), ref settings.ChanceForTrickBash, 0, 1, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.TrickBashSpeed))), ref settings.TrickBashSpeed, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.TrickBashDistance))), ref settings.TrickBashDistance, 1);
		}
		private void EffectSettingsArea()
		{
			CrawlerSettings settings = target as CrawlerSettings;

			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BashChargeStartEffects))), ref settings.BashChargeStartEffects, serializedObject.FindProperty(nameof
				(settings.BashChargeStartEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.TrickBashChargeStartEffects))), ref settings.TrickBashChargeStartEffects, serializedObject.FindProperty(nameof
				(settings.TrickBashChargeStartEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BashStartEffects))), ref settings.BashStartEffects, serializedObject.FindProperty(nameof
				(settings.BashStartEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BashHitTerrainEffects))), ref settings.BashHitTerrainEffects, serializedObject.FindProperty(nameof
				(settings.BashHitTerrainEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BashHitEntitieEffects))), ref settings.BashHitEntitieEffects, serializedObject.FindProperty(nameof
				(settings.BashHitEntitieEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
		}

		private void DrawAnimationSettings()
		{
			CrawlerSettings settings = target as CrawlerSettings;
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AnimationWalkSpeedDivider))), ref settings.AnimationWalkSpeedDivider, 1);
		}
	}
}