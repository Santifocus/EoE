using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EoE.Information
{
	public class PlayerSettings : EntitieSettings
	{
		[HideInInspector] public float CameraToPlayerDistance = 10;
		[HideInInspector] public Vector3 CameraAnchorOffset = new Vector3(0, 4, 0);
		[HideInInspector] public Vector2 CameraRotationPower = new Vector2(1, 0.5f);
		[HideInInspector] public float CameraRotationSpeed = 1;
		[HideInInspector] public Vector2 CameraVerticalAngleClamps = new Vector2(-30, 60);

		//Endurance Settings
		[HideInInspector] public int EnduranceBars = 4;
		[HideInInspector] public float EndurancePerBar = 25;

		//UI
		[HideInInspector] public float StatTextUpdateSpeed = 1;
		[HideInInspector] public Gradient HealthTextColors = new Gradient();
		[HideInInspector] public Gradient ManaTextColors = new Gradient();

		[HideInInspector] public Color ReserveEnduranceBarColor = new Color(0.65f, 0.325f, 0, 1);
		[HideInInspector] public Color ActiveEnduranceBarColor = new Color(1, 0.5f, 0, 1);
		[HideInInspector] public Color ReloadingEnduranceBarColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);

#if UNITY_EDITOR
		[CustomEditor(typeof(PlayerSettings), true), CanEditMultipleObjects]
		private class PlayerSettingsEditor : EntitieSettingsEditor
		{
			protected override void CustomInspector()
			{
				base.CustomInspector();
				PlayerSettings settings = target as PlayerSettings;

				Header("Camera Settings");
				FloatField(new GUIContent("Camera to Player Distance", "How far from the Player should the camera be?"), ref settings.CameraToPlayerDistance);
				Vector3Field(new GUIContent("Camera Anchor offset", "The camera will always look at the Camera Anchor."), ref settings.CameraAnchorOffset);
				Vector2Field(new GUIContent("Camera Rotation Power", "The amount of rotation that will be added when the player tries to rotate the camera."), ref settings.CameraRotationPower);
				FloatField(new GUIContent("Camera Rotation Speed", "How fast should the added rotation be interpolated. Higher value means less smooth, lower means that even after multiple seconds the camera might still slightly move."), ref settings.CameraRotationSpeed);
				Vector2Field(new GUIContent("Camera Vertical Angle Clamps", "How far around the X Axis can the player rotate the camera. (Up / Down)"), ref settings.CameraVerticalAngleClamps);

				Header("Endurance Settings");
				IntField(new GUIContent("Endurance Bars", "How many endurance reserve bars does the player have?"), ref settings.EnduranceBars);
				FloatField(new GUIContent("Endurance per Bar", "How much endurance points are stored per small bar. The player endurance can be calculated by multiplying this value times 'EnduranceBars'"), ref settings.EndurancePerBar);

				Header("HUD Settings");
				FloatField(new GUIContent("Stat Text Update Speed", "How fast should the Stat text display update its number? Depending on the current difference between actuall health and displayed health the update speed increases."), ref settings.StatTextUpdateSpeed);
				GradientField(new GUIContent("Health Text Colors", "Gradient Start == No Health, Gradient End == Full Health"), ref settings.HealthTextColors);
				GradientField(new GUIContent("Mana Text Colors", "Gradient Start == No Mana, Gradient End == Full Mana"), ref settings.ManaTextColors);

				GUILayout.Space(5);
				ColorField("Reserved Endurance Bar Color", ref settings.ReserveEnduranceBarColor);
				ColorField("Active Endurance Bar Color", ref settings.ActiveEnduranceBarColor);
				ColorField("Reloading Endurance Bar Color", ref settings.ReloadingEnduranceBarColor);
			}
				
		}
#endif
	}
}