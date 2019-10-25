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
			BodySettingsArea();
			StatSettingsArea();
			MovementSettingsArea();
		}

		protected virtual void BodySettingsArea()
		{
			EntitieSettings settings = target as EntitieSettings;

			System.Enum element = settings.EntitieElement;
			bool changed = EoEEditor.EnumField(new GUIContent("Entitie Element", "Which element is this Entitie? (Will be used for damage calculations)"), ref element);
			if (changed)
				settings.EntitieElement = (ElementType)element;


			EoEEditor.Header("Body Settings");
			if (EoEEditor.FloatField(new GUIContent("Entitie Mass", "What is the mass of this Entitie? This will be used for Knockback,- and Acceleration - calculations."), ref settings.EntitieMass) && settings.EntitieMass < EntitieSettings.MIN_ENTITIE_MASS)
				settings.EntitieMass = EntitieSettings.MIN_ENTITIE_MASS;
			EoEEditor.Vector3Field(new GUIContent("Mass Center", "Where is the Mass center of this Entitie? Based on this the Knockback direction will be calculated."), ref settings.MassCenter);
		}

		protected virtual void StatSettingsArea()
		{
			EntitieSettings settings = target as EntitieSettings;

			EoEEditor.Header("Stat Settings");
			//Health
			EoEEditor.FloatField(new GUIContent("Start Health", "With what health does this Entitie Spawn?"), ref settings.Health);
			EoEEditor.BoolField(new GUIContent("Do Health Regen", "Should this Entitie Regen over time?"), ref settings.DoHealthRegen);
			if (settings.DoHealthRegen)
			{
				EoEEditor.FloatField(new GUIContent("Health Regen Amount", "How much Health should this Entitie regenerate? (Per Second)"), ref settings.HealthRegen, 1);
				EoEEditor.FloatField(new GUIContent("Health Regen In Combat Factor", "In combat regeneration will be multiplied by this amount."), ref settings.HealthRegenInCombatMultiplier, 1);
			}

			//Mana
			GUILayout.Space(5);
			EoEEditor.FloatField(new GUIContent("Mana", "Magic uses Mana. When a Entitie is out of Mana he cant use Magic anymore."), ref settings.Mana);
			EoEEditor.BoolField(new GUIContent("Do Mana Regen", "Should this Entitie Regen over time?"), ref settings.DoManaRegen);
			if (settings.DoManaRegen)
			{
				EoEEditor.FloatField(new GUIContent("Mana Regen Amount", "How much Mana should this Entitie regenerate? (Per Second)"), ref settings.ManaRegen, 1);
				EoEEditor.FloatField(new GUIContent("Mana Regen In Combat Factor", "In combat regeneration will be multiplied by this amount."), ref settings.ManaRegenInCombatMultiplier, 1);
			}
		}

		protected virtual void MovementSettingsArea()
		{
			EntitieSettings settings = target as EntitieSettings;

			EoEEditor.Header("Movement Settings");
			EoEEditor.FloatField(new GUIContent("Move Speed", "What is the maximum speed at which this Entitie walks?"), ref settings.WalkSpeed);
			EoEEditor.FloatField(new GUIContent("Run Speed Multiplicator", "Multilpies the walkspeed when running."), ref settings.RunSpeedMultiplicator);
			EoEEditor.Vector3Field(new GUIContent("Jump Power", "With which velocity does this Entitie jump? (X == Sideways, Y == Upward, Z == Foreward)"), ref settings.JumpPower);
			EoEEditor.FloatField(new GUIContent("Turn Speed", "How fast does the Entitie to the target direction in degree? More == Faster"), ref settings.TurnSpeed);

			GUILayout.Space(4);
			EoEEditor.FloatField(new GUIContent("Move Acceleration", "How fast can this Entitie get to its maximum speed? (In Seconds)"), ref settings.MoveAcceleration);
			EoEEditor.FloatField(new GUIContent("No Move Deceleration", "How fast does this Entitie Decelerate? (In Seconds)"), ref settings.NoMoveDeceleration);
		}
	}
}