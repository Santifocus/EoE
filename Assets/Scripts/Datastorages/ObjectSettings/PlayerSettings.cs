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
		[HideInInspector] public float Endurance;

		[HideInInspector] public float CameraToPlayerDistance = 10;
		[HideInInspector] public Vector3 CameraAnchorOffset = new Vector3(0, 4, 0);
		[HideInInspector] public Vector2 CameraRotationPower = new Vector2(1, 0.5f);
		[HideInInspector] public float CameraRotationSpeed = 1;
		[HideInInspector] public Vector2 CameraVerticalAngleClamps = new Vector2(-30, 60);

#if UNITY_EDITOR
		[CustomEditor(typeof(PlayerSettings), true), CanEditMultipleObjects]
		private class PlayerSettingsEditor : EntitieSettingsEditor
		{
			protected override void CustomInspector()
			{
				base.CustomInspector();
				PlayerSettings settings = target as PlayerSettings;
				FloatField("Endurance", ref settings.Endurance);

				Header("Camera Settings");
				FloatField(new GUIContent("Camera to Player Distance", "How far from the Player should the camera be?"), ref settings.CameraToPlayerDistance);
				Vector3Field(new GUIContent("Camera Anchor offset", "The camera will always look at the Camera Anchor."), ref settings.CameraAnchorOffset);
				Vector2Field(new GUIContent("Camera Rotation Power", "The amount of rotation that will be added when the player tries to rotate the camera."), ref settings.CameraRotationPower);
				FloatField(new GUIContent("Camera Rotation Speed", "How fast should the added rotation be interpolated. Higher value means less smooth, lower means that even after multiple seconds the camera might still slightly move."), ref settings.CameraRotationSpeed);
				Vector2Field(new GUIContent("Camera Vertical Angle Clamps", "How far around the X Axis can the player rotate the camera. (Up / Down)"), ref settings.CameraVerticalAngleClamps);
			}
		}
#endif
	}
}