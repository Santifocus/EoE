using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE
{
	public class PlayerCameraController : MonoBehaviour
	{
		private const float CAMERA_ROTATE_KILL_THRESHOLD = 0.001f;

		private static PlayerCameraController instance;
		public static PlayerCameraController Instance => instance;
		public static Vector2 ToRotate;
		public static Vector2 CurRotation;
		public static Camera PlayerCamera => instance.playerCamera;



		[SerializeField] private Camera playerCamera = default;
		[SerializeField] private Camera screenCapturerCamera = default;

		private PlayerSettings playerSettigns => Player.Instance.SelfSettings as PlayerSettings;

		private void Start()
		{
			instance = this;
			playerCamera.transform.localPosition = new Vector3(0, 0, -playerSettigns.CameraToPlayerDistance);
			AnchorToPlayer();
		}

		private void AnchorToPlayer()
		{
			if (!Player.Alive)
				return;

			transform.position = Player.Instance.transform.position + playerSettigns.CameraAnchorOffset;
			transform.eulerAngles = new Vector3(CurRotation.y, CurRotation.x, 0);

			float camDist = GetCameraDistance();
			if(camDist > playerCamera.transform.localPosition.z)
			{
				playerCamera.transform.localPosition = screenCapturerCamera.transform.localPosition = new Vector3(0, 0, camDist);
			}
			else
			{
				playerCamera.transform.localPosition = screenCapturerCamera.transform.localPosition = new Vector3(0, 0, Mathf.Lerp(playerCamera.transform.localPosition.z, camDist, Time.deltaTime * 3));
			}
		}

		private float GetCameraDistance()
		{
			Vector3 rayDir = (playerCamera.transform.position - transform.position).normalized;
			RaycastHit hit;
			float distance = 0;

			if(Physics.Raycast(transform.position, rayDir, out hit, playerSettigns.CameraToPlayerDistance, ConstantCollector.TERRAIN_LAYER))
			{
				distance = -(transform.position - hit.point).magnitude * 0.95f;
			}
			else
				distance = -playerSettigns.CameraToPlayerDistance * (1 + rayDir.y * playerSettigns.CameraExtraZoomOnVertical);

			return distance;
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