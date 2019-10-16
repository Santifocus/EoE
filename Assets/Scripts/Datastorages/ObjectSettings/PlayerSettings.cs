using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class PlayerSettings : EntitieSettings
	{
		public float CameraToPlayerDistance = 10;
		public Vector3 CameraAnchorOffset = new Vector3(0, 4, 0);
		public Vector2 CameraRotationPower = new Vector2(1, 0.5f);
		public float CameraRotationSpeed = 1;
		public Vector2 CameraVerticalAngleClamps = new Vector2(-30, 60);

		//Endurance Settings
		public int EnduranceBars = 4;
		public float EndurancePerBar = 25;

		//UI
		public float StatTextUpdateSpeed = 1;
		public Gradient HealthTextColors = new Gradient();
		public Gradient ManaTextColors = new Gradient();

		public Color ReserveEnduranceBarColor = new Color(0.65f, 0.325f, 0, 1);
		public Color ActiveEnduranceBarColor = new Color(1, 0.5f, 0, 1);
		public Color ReloadingEnduranceBarColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
	}
}