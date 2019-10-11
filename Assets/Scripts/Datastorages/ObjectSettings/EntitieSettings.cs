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
		[HideInInspector] public float EntitieMass = 1;
		[HideInInspector] public Vector3 MassCenter = Vector3.zero;

		[HideInInspector] public ElementType EntitieElement = ElementType.None;
		[HideInInspector] public float Health = 10;
		[HideInInspector] public bool DoRegen = false;
		[HideInInspector] public float HealthRegen = 0;

		[HideInInspector] public float MoveSpeed = 1;
		[HideInInspector] public float BaseAcceleration = 0.1f;
		[HideInInspector] public float MoveAcceleration = 1;
		[HideInInspector] public float NoMoveDeceleration = 1;
		[HideInInspector] public float TurnSpeed = 0.35f;
#if UNITY_EDITOR
		[CustomEditor(typeof(EntitieSettings), true), CanEditMultipleObjects]
		protected class EntitieSettingsEditor : ObjectSettingEditor
		{
			protected override void CustomInspector()
			{
				EntitieSettings settings = target as EntitieSettings;

				//Body
				Header("Body Settings");
				FloatField(new GUIContent("Entitie Mass", "What is the mass of this Entitie? This will be used for Knockback calculation."), ref settings.EntitieMass);
				Vector3Field(new GUIContent("Mass Center", "Where is the Mass center of this Entitie? Based on this the Knockback direction will be calculated."), ref settings.MassCenter);

				//Stats
				Header("Stat Settings");
				FloatField(new GUIContent("Start Health", "With what health does this Entitie Spawn?"), ref settings.Health);
				BoolField(new GUIContent("Do Health Regen", "Should this entitie Regen over time?"), ref settings.DoRegen);
				if (settings.DoRegen)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Space(8);
					FloatField(new GUIContent("Regen Amount", "How much should this Entitie heal? (Per Second)"), ref settings.HealthRegen);
					GUILayout.EndHorizontal();
				}

				//Movement
				Header("Movement");
				FloatField(new GUIContent("Move Speed", "What is the maximum speed at which this Entitie can move?"), ref settings.MoveSpeed);
				FloatField(new GUIContent("Turn Speed", "How fast does the entitie to the target direction? Less == Faster"), ref settings.TurnSpeed);
				GUILayout.Space(2);
				FloatField(new GUIContent("Base Acceleration", "The starting / minimum acceleration of the entitie. (Should never be 0)"), ref settings.BaseAcceleration);
				FloatField(new GUIContent("Move Acceleration", "How fast can this Entitie get to its maximum speed? (In Seconds)"), ref settings.MoveAcceleration);
				FloatField(new GUIContent("No Move Deceleration", "How fast does this Entitie fall back to its base Acceleration? (In Seconds)"), ref settings.NoMoveDeceleration);
			}
		}
#endif
	}
}