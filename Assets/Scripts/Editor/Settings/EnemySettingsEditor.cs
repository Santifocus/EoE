using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(EnemySettings), true), CanEditMultipleObjects]
	public class EnemySettingsEditor : EntitieSettingsEditor
	{
		protected static bool PlayerChasingSettingsOpen;
		protected static bool WanderSettingsOpen;
		protected static bool LookAroundSettingsgOpen;
		protected override void CustomInspector()
		{
			base.CustomInspector();
			FoldoutHeader("Player Chasing Settings", ref PlayerChasingSettingsOpen);
			if (PlayerChasingSettingsOpen)
			{
				PlayerChasingArea();
			}
			EndFoldoutHeader();

			FoldoutHeader("Wander Settings", ref WanderSettingsOpen);
			if (WanderSettingsOpen)
			{
				WanderSettingsArea();
			}
			EndFoldoutHeader();

			FoldoutHeader("Look Around Settings", ref LookAroundSettingsgOpen);
			if (LookAroundSettingsgOpen)
			{
				LookAroundSettingsArea();
			}
			EndFoldoutHeader();
		}
		protected override void CombatSettings()
		{
			base.CombatSettings();
			FloatField(new GUIContent("Attack Range", "At which distance to the player should this Enemy change into its combat behavior?"), ref (target as EnemySettings).AttackRange);
		}
		protected virtual void PlayerChasingArea()
		{
			EnemySettings settings = target as EnemySettings;

			FloatField(new GUIContent("Sight Range", "How far can this Enemy see? If the player is further then this it is impossible for the Enemy to see the player."), ref settings.SightRange);
			FloatField(new GUIContent("Sight Angle", "What is the FOV of this enemy. (In Angles)"), ref settings.SightAngle);
			FloatField(new GUIContent("Found Player Sight Angle", "When the Enemy found the player the Enemys FOV will be set to this."), ref settings.FoundPlayerSightAngle);
			FloatField(new GUIContent("Chase Interest", "After this Enemy spotted the player how long will he try to chase him AFTER he lost sight of him. (In Seconds, Tries to go to the last seen position.)"), ref settings.ChaseInterest);
			FloatField(new GUIContent("Look Around After Lost Player Time", "After losing sight of the Player how long should this Enemy look around in search of the player? (Goes back to wandering / Ideling afterwards)"), ref settings.InvestigationTime);
		}
		protected virtual void WanderSettingsArea()
		{
			EnemySettings settings = target as EnemySettings;

			FloatField(new GUIContent("Wandering Factor", "How far does this Enemy wander around its spawn position. (In Unity-Unity, 0 == Stands still till Player is spotted)"), ref settings.WanderingFactor);
			FloatField(new GUIContent("Wandering Delay Min", "After reaching a wandering point how long (MIN) should the enemy stand around for?"), ref settings.WanderingDelayMin);
			FloatField(new GUIContent("Wandering Delay Max", "After reaching a wandering point how long (MAX) should the enemy stand around for"), ref settings.WanderingDelayMax);
		}
		protected virtual void LookAroundSettingsArea()
		{
			EnemySettings settings = target as EnemySettings;

			FloatField(new GUIContent("Look Around Delay Min", "After finishing looking into a direction how long should the Enemy idle? (MIN)"), ref settings.LookAroundDelayMin);
			FloatField(new GUIContent("Look Around Delay Max", "After finishing looking into a direction how long should the Enemy idle? (MAX)"), ref settings.LookAroundDelayMax);
		}
	}
}
