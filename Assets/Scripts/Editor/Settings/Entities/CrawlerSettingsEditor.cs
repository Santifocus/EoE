using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(CrawlerSettings), true), CanEditMultipleObjects]
	public class CrawlerSettingsEditor : EnemySettingsEditor
	{
		protected override void CustomInspector()
		{
			base.CustomInspector();
			DrawInFoldoutHeader("FX Settings", ref VFXSettingsOpen, FXSettingsArea);
		}
		protected override void CombatSettings()
		{
			CrawlerSettings settings = target as CrawlerSettings;
			base.CombatSettings();

			FloatField(new GUIContent("Attack Speed"), ref settings.BashChargeSpeed, 1);
			FloatField(new GUIContent("Bash Speed"), ref settings.BashSpeed, 1);
			FloatField(new GUIContent("Bash Distance"), ref settings.BashDistance, 1);
			FloatField(new GUIContent("ForceTranslationMultiplier", "When the Crawler hits the player he will give the current speed multiplied by this amount to the Player"), ref settings.ForceTranslationMultiplier, 1);
		}
		private void FXSettingsArea()
		{
			CrawlerSettings settings = target as CrawlerSettings;

			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BashChargeStartEffects))), ref settings.BashChargeStartEffects, serializedObject.FindProperty(nameof
				(settings.BashChargeStartEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BashStartEffects))), ref settings.BashStartEffects, serializedObject.FindProperty(nameof
				(settings.BashStartEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BashHitTerrainEffects))), ref settings.BashHitTerrainEffects, serializedObject.FindProperty(nameof
				(settings.BashHitTerrainEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BashHitEntitieEffects))), ref settings.BashHitEntitieEffects, serializedObject.FindProperty(nameof
				(settings.BashHitEntitieEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
		}
	}
}