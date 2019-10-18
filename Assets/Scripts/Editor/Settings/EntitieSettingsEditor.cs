using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(EntitieSettings), true), CanEditMultipleObjects]
	public class EntitieSettingsEditor : ObjectSettingEditor
	{
		protected override void CustomInspector()
		{
			EntitieSettings settings = target as EntitieSettings;

			//Body
			EoEEditor.Header("Body Settings");
			if (EoEEditor.FloatField(new GUIContent("Entitie Mass", "What is the mass of this Entitie? This will be used for Knockback,- and Acceleration - calculations."), ref settings.EntitieMass) && settings.EntitieMass < EntitieSettings.MIN_ENTITIE_MASS)
				settings.EntitieMass = EntitieSettings.MIN_ENTITIE_MASS;
			EoEEditor.Vector3Field(new GUIContent("Mass Center", "Where is the Mass center of this Entitie? Based on this the Knockback direction will be calculated."), ref settings.MassCenter);

			//Stats
			EoEEditor.Header("Stat Settings");
			//Health
			EoEEditor.FloatField(new GUIContent("Start Health", "With what health does this Entitie Spawn?"), ref settings.Health);
			EoEEditor.BoolField(new GUIContent("Do Health Regen", "Should this Entitie Regen over time?"), ref settings.DoHealthRegen);
			if (settings.DoHealthRegen)
			{
				EoEEditor.FloatField(new GUIContent("Health Regen Amount", "How much Health should this Entitie regenerate? (Per Second)"), ref settings.HealthRegen, 1);
				EoEEditor.FloatField(new GUIContent("Health Regen In Combat Factor", "In combat regeneration will be multiplied by this amount."), ref settings.HealthRegenInCombatFactor, 1);
			}
			//Endurance
			GUILayout.Space(5);
			EoEEditor.FloatField(new GUIContent("Endurance", "Attacks and movement use up endurance. When a Entitie is out of endurance he enters into a weakness state."), ref settings.Endurance);
			EoEEditor.BoolField(new GUIContent("Do Endurance Regen", "Should this Entitie Regen over time?"), ref settings.DoEnduranceRegen);
			if (settings.DoEnduranceRegen)
			{
				EoEEditor.FloatField(new GUIContent("Endurance Regen Amount", "How much Endurance should this Entitie regenerate? (Per Second)"), ref settings.EnduranceRegen, 1);
				EoEEditor.FloatField(new GUIContent("Endurance Regen In Combat Factor", "In combat regeneration will be multiplied by this amount."), ref settings.EnduranceRegenInCombatFactor, 1);
			}
			//Mana
			GUILayout.Space(5);
			EoEEditor.FloatField(new GUIContent("Mana", "Magic uses Mana. When a Entitie is out of Mana he cant use Magic anymore."), ref settings.Mana);
			EoEEditor.BoolField(new GUIContent("Do Mana Regen", "Should this Entitie Regen over time?"), ref settings.DoManaRegen);
			if (settings.DoManaRegen)
			{
				EoEEditor.FloatField(new GUIContent("Mana Regen Amount", "How much Mana should this Entitie regenerate? (Per Second)"), ref settings.ManaRegen, 1);
				EoEEditor.FloatField(new GUIContent("Mana Regen In Combat Factor", "In combat regeneration will be multiplied by this amount."), ref settings.ManaRegenInCombatFactor, 1);
			}

			//Movement
			EoEEditor.Header("Movement");
			EoEEditor.FloatField(new GUIContent("Move Speed", "What is the maximum speed at which this Entitie walks?"), ref settings.WalkSpeed);
			EoEEditor.FloatField(new GUIContent("Run Speed Multiplicator", "Multilpies the walkspeed when running."), ref settings.RunSpeedMultiplicator);
			EoEEditor.Vector3Field(new GUIContent("Jump Power", "With which velocity does this Entitie jump? (X == Sideways, Y == Upward, Z == Foreward)"), ref settings.JumpPower);
			EoEEditor.FloatField(new GUIContent("Turn Speed", "How fast does the Entitie to the target direction? Less == Faster"), ref settings.TurnSpeed);

			GUILayout.Space(4);
			EoEEditor.FloatField(new GUIContent("Move Acceleration", "How fast can this Entitie get to its maximum speed? (In Seconds)"), ref settings.MoveAcceleration);
			EoEEditor.FloatField(new GUIContent("No Move Deceleration", "How fast does this Entitie Decelerate? (In Seconds)"), ref settings.NoMoveDeceleration);
			GUILayout.Space(4);
			EoEEditor.FloatField(new GUIContent("Jump Endurance Cost", "How much Endurance does a jump cost?"), ref settings.JumpEnduranceCost);
			EoEEditor.FloatField(new GUIContent("Run Endurance Cost", "How much Endurance does running cost? (Per Second)"), ref settings.RunEnduranceCost);
		}
	}
}