using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE
{
	public class PlayerCameraController : MonoBehaviour
	{
		private const float CAMERA_ROTATE_KILL_THRESHOLD = 1;

		private static PlayerCameraController instance;
		public static PlayerCameraController Instance => instance;
		public static Vector2 ToRotate;
		public static Vector2 CurRotation;
		public static Camera PlayerCamera => PlayerCamera;

		[SerializeField] private Camera playerCamera = default;

		private PlayerSettings playerSettigns => Player.Instance.SelfSettings as PlayerSettings;

		private void Start()
		{
			instance = this;
			playerCamera.transform.localPosition = new Vector3(0, 0, -playerSettigns.CameraToPlayerDistance);
			AnchorToPlayer();
		}

		private void AnchorToPlayer()
		{
			transform.position = Player.Instance.transform.position + playerSettigns.CameraAnchorOffset;
			transform.eulerAngles = new Vector3(CurRotation.y, CurRotation.x, 0);
		}

		private void Update()
		{
			AnchorToPlayer();
			RotateCamera();
		}

		private void RotateCamera()
		{
			if (ToRotate == Vector2.zero)
				return;

			Vector2 rotateFraction = ToRotate * Mathf.Min(1, playerSettigns.CameraRotationSpeed * Time.deltaTime);
			CurRotation = new Vector2((CurRotation.x + rotateFraction.x) % 360, Mathf.Clamp(CurRotation.y + rotateFraction.y, playerSettigns.CameraVerticalAngleClamps.x, playerSettigns.CameraVerticalAngleClamps.y));

			ToRotate -= rotateFraction;
			if (ToRotate.sqrMagnitude < CAMERA_ROTATE_KILL_THRESHOLD)
				ToRotate = Vector2.zero;
		}

		public void RotateCameraToDirection(Vector2 direction)
		{

		}
	}
}