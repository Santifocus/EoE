using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(EnemySettings), true), CanEditMultipleObjects]
	public class CrawlerSettingsEditor : EnemySettingsEditor
	{
		protected override void CombatSettings()
		{
			CrawlerSettings settings = target as CrawlerSettings;
			base.CombatSettings();

			FloatField(new GUIContent("Attack Speed"), ref settings.AttackSpeed);
			FloatField(new GUIContent("In Range Wait Time", "If the Player is in attack range, the Crawler wants to Bash him but if the player leaves his attack range in this amount if time the Crawler will cancel the attack."), ref settings.InRangeWaitTime);
			FloatField(new GUIContent("Bash Speed"), ref settings.BashSpeed);
			FloatField(new GUIContent("Bash Distance"), ref settings.BashDistance);
			FloatField(new GUIContent("ForceTranslationMultiplier", "When the Crawler hits the player he will give the current speed multiplied by this amount to the Player"), ref settings.ForceTranslationMultiplier);
		}
	}
}