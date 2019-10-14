using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EoE.Information
{
	public class EntitieSettings : ObjectSettings
	{
		private const float MIN_ENTITIE_MASS = 0.001f;

		[HideInInspector] public float EntitieMass = 1;
		[HideInInspector] public Vector3 MassCenter = Vector3.zero;

		[HideInInspector] public ElementType EntitieElement = ElementType.None;

		//Health
		[HideInInspector] public float Health = 10;
		[HideInInspector] public bool DoHealthRegen = false;
		[HideInInspector] public float HealthRegen = 0;
		[HideInInspector] public float HealthRegenInCombatFactor = 0.5f;
		//Endurance
		[HideInInspector] public float Endurance;
		[HideInInspector] public bool DoEnduranceRegen = false;
		[HideInInspector] public float EnduranceRegen = 0;
		[HideInInspector] public float EnduranceRegenInCombatFactor = 0.5f;
		//Mana
		[HideInInspector] public float Mana;
		[HideInInspector] public bool DoManaRegen = false;
		[HideInInspector] public float ManaRegen = 0;
		[HideInInspector] public float ManaRegenInCombatFactor = 0.5f;

		//Movement
		[HideInInspector] public float WalkSpeed = 1;
		[HideInInspector] public float RunSpeedMultiplicator = 1.5f;
		[HideInInspector] public Vector3 JumpPower = new Vector3(0, 10, 0);
		[HideInInspector] public float TurnSpeed = 0.35f;
		//Movement acceleration
		[HideInInspector] public float MoveAcceleration = 1;
		[HideInInspector] public float NoMoveDeceleration = 1;
		//Costs
		[HideInInspector] public float JumpEnduranceCost = 0;
		[HideInInspector] public float RunEnduranceCost = 0;

#if UNITY_EDITOR
		[CustomEditor(typeof(EntitieSettings), true), CanEditMultipleObjects]
		protected class EntitieSettingsEditor : ObjectSettingEditor
		{
			protected override void CustomInspector()
			{
				EntitieSettings settings = target as EntitieSettings;

				//Body
				Header("Body Settings");
				if (FloatField(new GUIContent("Entitie Mass", "What is the mass of this Entitie? This will be used for Knockback,- and Acceleration - calculations."), ref settings.EntitieMass) && settings.EntitieMass < MIN_ENTITIE_MASS)
					settings.EntitieMass = MIN_ENTITIE_MASS;
				Vector3Field(new GUIContent("Mass Center", "Where is the Mass center of this Entitie? Based on this the Knockback direction will be calculated."), ref settings.MassCenter);

				//Stats
				Header("Stat Settings");
				//Health
				FloatField(new GUIContent("Start Health", "With what health does this Entitie Spawn?"), ref settings.Health);
				BoolField(new GUIContent("Do Health Regen", "Should this Entitie Regen over time?"), ref settings.DoHealthRegen);
				if (settings.DoHealthRegen)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Space(8);
					FloatField(new GUIContent("Health Regen Amount", "How much Health should this Entitie regenerate? (Per Second)"), ref settings.HealthRegen);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.Space(8);
					FloatField(new GUIContent("Health Regen In Combat Factor", "In combat regeneration will be multiplied by this amount."), ref settings.HealthRegenInCombatFactor);
					GUILayout.EndHorizontal();
				}
				//Endurance
				GUILayout.Space(5);
				FloatField(new GUIContent("Endurance", "Attacks and movement use up endurance. When a Entitie is out of endurance he enters into a weakness state."), ref settings.Endurance);
				BoolField(new GUIContent("Do Endurance Regen", "Should this Entitie Regen over time?"), ref settings.DoEnduranceRegen);
				if (settings.DoEnduranceRegen)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Space(8);
					FloatField(new GUIContent("Endurance Regen Amount", "How much Endurance should this Entitie regenerate? (Per Second)"), ref settings.EnduranceRegen);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.Space(8);
					FloatField(new GUIContent("Endurance Regen In Combat Factor", "In combat regeneration will be multiplied by this amount."), ref settings.EnduranceRegenInCombatFactor);
					GUILayout.EndHorizontal();
				}
				//Mana
				GUILayout.Space(5);
				FloatField(new GUIContent("Mana", "Magic uses Mana. When a Entitie is out of Mana he cant use Magic anymore."), ref settings.Mana);
				BoolField(new GUIContent("Do Mana Regen", "Should this Entitie Regen over time?"), ref settings.DoManaRegen);
				if (settings.DoManaRegen)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Space(8);
					FloatField(new GUIContent("Mana Regen Amount", "How much Mana should this Entitie regenerate? (Per Second)"), ref settings.ManaRegen);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.Space(8);
					FloatField(new GUIContent("Mana Regen In Combat Factor", "In combat regeneration will be multiplied by this amount."), ref settings.ManaRegenInCombatFactor);
					GUILayout.EndHorizontal();
				}

				//Movement
				Header("Movement");
				FloatField(new GUIContent("Move Speed", "What is the maximum speed at which this Entitie walks?"), ref settings.WalkSpeed);
				FloatField(new GUIContent("Run Speed Multiplicator", "Multilpies the walkspeed when running."), ref settings.RunSpeedMultiplicator);
				Vector3Field(new GUIContent("Jump Power", "With which velocity does this Entitie jump? (X == Sideways, Y == Upward, Z == Foreward)"), ref settings.JumpPower);
				FloatField(new GUIContent("Turn Speed", "How fast does the Entitie to the target direction? Less == Faster"), ref settings.TurnSpeed);

				GUILayout.Space(4);
				FloatField(new GUIContent("Move Acceleration", "How fast can this Entitie get to its maximum speed? (In Seconds)"), ref settings.MoveAcceleration);
				FloatField(new GUIContent("No Move Deceleration", "How fast does this Entitie fall back to its base Acceleration? (In Seconds)"), ref settings.NoMoveDeceleration);
				GUILayout.Space(4);
				FloatField(new GUIContent("Jump Endurance Cost", "How much Endurance does a jump cost?"), ref settings.JumpEnduranceCost);
				FloatField(new GUIContent("Run Endurance Cost", "How much Endurance does running cost? (Per Second)"), ref settings.RunEnduranceCost);
			}
		}
#endif
	}
}