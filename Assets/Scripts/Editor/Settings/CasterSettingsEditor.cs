using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(CasterSettings), true), CanEditMultipleObjects]
	public class CasterSettingsEditor : EnemySettingsEditor
	{
		protected static bool CastingAnnouncementOpen;
		protected override void CustomInspector()
		{
			base.CustomInspector();

			FoldoutHeader("FX Settings", ref VFXSettingsOpen);
			if (VFXSettingsOpen)
			{
				FXSettingsArea();
			}
			EndFoldoutHeader();
		}
		protected override void CombatSettings()
		{
			CasterSettings settings = target as CasterSettings;
			base.CombatSettings();

			FloatField(new GUIContent("ProjectileChargeTime"), ref settings.ProjectileChargeTime);
			FloatField(new GUIContent("ProjectileDamageMultiplier"), ref settings.ProjectileDamageMultiplier);

			System.Enum ele = settings.ProjectileElement;
			if(EnumField(new GUIContent("Projectile Element"), ref ele))
			{
				settings.ProjectileElement = (ElementType)ele;
			}

			FloatField(new GUIContent("ProjectileKnockback"), ref settings.ProjectileKnockback);
			FloatField(new GUIContent("ProjectileCritChance"), ref settings.ProjectileCritChance);
			FloatField(new GUIContent("ProjectileManaCost"), ref settings.ProjectileManaCost);
			FloatField(new GUIContent("ProjectileFlightSpeed"), ref settings.ProjectileFlightSpeed);
			FloatField(new GUIContent("ProjectileSpellCooldown"), ref settings.ProjectileSpellCooldown);
			GUILayout.Space(3);
			ObjectField(new GUIContent("Projectile Prefab"), ref settings.ProjectilePrefab);
		}
		private void FXSettingsArea()
		{
			CasterSettings settings = target as CasterSettings;
			ObjectArrayField(new GUIContent("Casting Announcement"), ref settings.CastingAnnouncement, ref CastingAnnouncementOpen, new GUIContent("Effect "));
			ObjectField(new GUIContent("Projectile Fly Particles"), ref settings.ProjectileFlyParticles);
			ObjectField(new GUIContent("Projectile Destroy Particles"), ref settings.ProjectileDestroyParticles);
		}
	}
}