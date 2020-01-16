using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(EnemySettings), true), CanEditMultipleObjects]
	public class EnemySettingsEditor : EntitySettingsEditor
	{
		protected static bool PlayerChasingSettingsOpen;
		protected static bool WanderSettingsOpen;
		protected static bool LookAroundSettingsgOpen;
		protected override void CustomInspector()
		{
			base.CustomInspector();
			DrawInFoldoutHeader("Player Chasing Settings", ref PlayerChasingSettingsOpen, PlayerChasingArea);
			DrawInFoldoutHeader("Wander Settings", ref WanderSettingsOpen, WanderSettingsArea);
			DrawInFoldoutHeader("Look Around Settings", ref LookAroundSettingsgOpen, LookAroundSettingsArea);
		}
		protected override void CombatSettings()
		{
			base.CombatSettings();

			EnemySettings settings = target as EnemySettings;
			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			FloatField(new GUIContent("Attack Range", "At which distance to the player should this Enemy change into its combat behavior?"), ref settings.AttackRange, 1);
			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OnCombatTriggerEffect))), ref settings.OnCombatTriggerEffect, serializedObject.FindProperty(nameof(settings.OnCombatTriggerEffect)), DrawActivationEffect, new GUIContent(". Effect"), 1);
		}
		protected virtual void PlayerChasingArea()
		{
			EnemySettings settings = target as EnemySettings;

			FloatField(new GUIContent("Player Track Speed"), ref settings.PlayerTrackSpeed, 1);
			FloatField(new GUIContent("Sight Range", "How far can this Enemy see? If the player is further then this it is impossible for the Enemy to see the player."), ref settings.SightRange, 1);
			FloatField(new GUIContent("Found Player Sight Range", "How far can this Enemy see, after it has spotted the player? If the player is further then this it is impossible for the Enemy to see the player."), ref settings.FoundPlayerSightRange, 1);
			FloatField(new GUIContent("Sight Angle", "What is the FOV of this enemy. (In Angles)"), ref settings.SightAngle, 1);
			FloatField(new GUIContent("Found Player Sight Angle", "When the Enemy found the player the Enemys FOV will be set to this."), ref settings.FoundPlayerSightAngle, 1);
			FloatField(new GUIContent("Chase Interest", "After this Enemy spotted the player how long will he try to chase him AFTER he lost sight of him. (In Seconds, Tries to go to the last seen position.)"), ref settings.ChaseInterest, 1);
			FloatField(new GUIContent("Look Around After Lost Player Time", "After losing sight of the Player how long should this Enemy look around in search of the player? (Goes back to wandering / Ideling afterwards)"), ref settings.InvestigationTime, 1);
		}
		protected virtual void WanderSettingsArea()
		{
			EnemySettings settings = target as EnemySettings;

			FloatField(new GUIContent("Wandering Factor", "How far does this Enemy wander around its spawn position. (In Unity-Unity, 0 == Stands still till Player is spotted)"), ref settings.WanderingFactor, 1);
			FloatField(new GUIContent("Wandering Delay Min", "After reaching a wandering point how long (MIN) should the enemy stand around for?"), ref settings.WanderingDelayMin, 1);
			FloatField(new GUIContent("Wandering Delay Max", "After reaching a wandering point how long (MAX) should the enemy stand around for"), ref settings.WanderingDelayMax, 1);
		}
		protected virtual void LookAroundSettingsArea()
		{
			EnemySettings settings = target as EnemySettings;

			FloatField(new GUIContent("Look Around Delay Min", "After finishing looking into a direction how long should the Enemy idle? (MIN)"), ref settings.LookAroundDelayMin, 1);
			FloatField(new GUIContent("Look Around Delay Max", "After finishing looking into a direction how long should the Enemy idle? (MAX)"), ref settings.LookAroundDelayMax, 1);
		}
	}
}
