using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(CasterSettings), true), CanEditMultipleObjects]
	public class CasterSettingsEditor : EnemySettingsEditor
	{
		protected static bool CastingAnnouncementOpen;
		protected override void CombatSettings()
		{
			CasterSettings settings = target as CasterSettings;
			base.CombatSettings();

			ObjectField<Spell>(new GUIContent("Spell"), ref settings.CasterAttack);
		}
	}
}