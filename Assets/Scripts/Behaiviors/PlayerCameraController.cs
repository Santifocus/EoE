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
		public static Vector2 TargetRotation;
		public static Vector2 CurRotation;
		public static Camera PlayerCamera => instance.playerCamera;

		[SerializeField] private Camera playerCamera = default;
		[SerializeField] private Camera screenCapturerCamera = default;

		private PlayerSettings playerSettigns => Player.Instance.SelfSettings as PlayerSettings;

		private void Start()
		{
			TargetRotation = CurRotation = new Vector2(transform.eulerAngles.y, transform.eulerAngles.x);
			instance = this;
			playerCamera.transform.localPosition = new Vector3(0, 0, -playerSettigns.CameraToPlayerDistance);
			AnchorToPlayer();
		}

		private void AnchorToPlayer()
		{
			if (!Player.Alive)
				return;

			if(Player.TargetedEntitie)
			{
				Vector3 dir = (Player.TargetedEntitie.actuallWorldPosition - transform.position).normalized;
				LookAtDirection(dir);
			}

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
		public void LookAtDirection(Vector3 direction)
		{
			float hAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg - 90;
			float vAngle = Mathf.Asin(direction.y) * Mathf.Rad2Deg;

			TargetRotation = -new Vector2(hAngle, vAngle);
		}
		private float GetCameraDistance()
		{
			Vector3 rayDir = (playerCamera.transform.position - transform.position).normalized;
			RaycastHit hit;
			float distance;

			if(Physics.Raycast(transform.position, rayDir, out hit, playerSettigns.CameraToPlayerDistance, ConstantCollector.TERRAIN_LAYER))
			{
				distance = -(transform.position - hit.point).magnitude;
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
			bool targeting = Player.TargetedEntitie;
			CurRotation.x %= 360;
			TargetRotation.y = Mathf.Clamp(TargetRotation.y, targeting ? playerSettigns.CameraClampsWhenTargeting.x : playerSettigns.CameraVerticalAngleClamps.x, targeting ? playerSettigns.CameraClampsWhenTargeting.y : playerSettigns.CameraVerticalAngleClamps.y);
			CurRotation = new Vector2(Mathf.LerpAngle(CurRotation.x, TargetRotation.x, Time.deltaTime * playerSettigns.CameraRotationSpeed), Mathf.LerpAngle(CurRotation.y, TargetRotation.y, Time.deltaTime * playerSettigns.CameraRotationSpeed));
		}
	}
}