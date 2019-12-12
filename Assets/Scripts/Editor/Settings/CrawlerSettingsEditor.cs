﻿using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(CrawlerSettings), true), CanEditMultipleObjects]
	public class CrawlerSettingsEditor : EnemySettingsEditor
	{
		private static bool AnnouncementFXOpen;
		protected override void CustomInspector()
		{
			base.CustomInspector();
			DrawInFoldoutHeader("FX Settings", ref VFXSettingsOpen, FXSettingsArea);
		}
		protected override void CombatSettings()
		{
			CrawlerSettings settings = target as CrawlerSettings;
			base.CombatSettings();

			FloatField(new GUIContent("Attack Speed"), ref settings.AttackSpeed);
			FloatField(new GUIContent("Bash Speed"), ref settings.BashSpeed);
			FloatField(new GUIContent("Bash Distance"), ref settings.BashDistance);
			FloatField(new GUIContent("ForceTranslationMultiplier", "When the Crawler hits the player he will give the current speed multiplied by this amount to the Player"), ref settings.ForceTranslationMultiplier);
		}
		private void FXSettingsArea()
		{
			CrawlerSettings settings = target as CrawlerSettings;
			FloatField(new GUIContent("Bash Announcement Delay", "If 'Bash Start' = T, then the BashAnnouncement will be played at 'T + Delay'"), ref settings.BashAnnouncementDelay);
			ObjectArrayField(new GUIContent("Bash Announcement", "The particles that will be shown when the BashAnnouncement is played."), ref settings.BashAnnouncement, ref AnnouncementFXOpen, new GUIContent("Effect "));
		}
	}
}