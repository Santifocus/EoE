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

		[HideInInspector] public ElementType entitieElement = ElementType.None;
		[HideInInspector] public float Health = 10;
		[HideInInspector] public bool DoRegen = false;
		[HideInInspector] public float HealthRegen = 0;

		[HideInInspector] public float MoveSpeed = 1;
		[HideInInspector] public float MoveAcceleration = 1;
#if UNITY_EDITOR
		[CustomEditor(typeof(EntitieSettings)), CanEditMultipleObjects]
		private class EntitieSettingsEditor : ObjectSettingEditor
		{
			protected override void CustomInspector()
			{
				EntitieSettings settings = target as EntitieSettings;

				//Body
				FloatField(new GUIContent("Entitie Mass", "What is the mass of this Entitie? This will be used for Knockback calculation."), ref settings.EntitieMass);
				Vector3Field(new GUIContent("Mass Center", "Where is the Mass center of this Entitie? Based on this the Knockback direction will be calculated."), ref settings.MassCenter);

				//Health
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
			}
		}
#endif
	}
}