using EoE.Entities;
using EoE.Information;
using UnityEngine;

namespace EoE
{
	public class PlayerCameraController : MonoBehaviour
	{
		private const float CAMERA_ROTATE_KILL_THRESHOLD = 0.001f;
		public static PlayerCameraController Instance { get; private set; }
		public static Vector2 TargetRotation;
		public static Vector2 CurRotation;
		private static float cameraFOV;
		public static float CameraFOV
		{
			get => cameraFOV;
			set
			{
				cameraFOV = Instance.playerCamera.fieldOfView = Instance.overlayCamera.fieldOfView = value;
			}
		}
		public static Camera PlayerCamera => Instance.playerCamera;

		[SerializeField] private Camera playerCamera = default;
		[SerializeField] private Camera overlayCamera = default;
		private Vector3 curOffset;

		private PlayerSettings playerSettigns => Player.Instance.SelfSettings as PlayerSettings;

		private void Start()
		{
			TargetRotation = CurRotation = new Vector2(transform.eulerAngles.y, transform.eulerAngles.x);
			Instance = this;
			playerCamera.transform.localPosition = new Vector3(0, 0, -playerSettigns.CameraToPlayerDistance);
			curOffset = GetOffset();
			CameraFOV = playerSettigns.CameraBaseFOV;
			AnchorToPlayer();
		}

		private void AnchorToPlayer()
		{
			if (!Player.Alive)
				return;

			if (Player.TargetedEntitie)
			{
				Vector3 dir = (Player.TargetedEntitie.actuallWorldPosition - transform.position).normalized;
				LookAtDirection(dir);
			}

			curOffset = Vector3.Lerp(curOffset, GetOffset(), Time.deltaTime * 3);
			transform.position = Player.Instance.transform.position + curOffset;
			transform.eulerAngles = new Vector3(CurRotation.y, CurRotation.x, 0);

			float camDist = GetCameraDistance();
			if (camDist > playerCamera.transform.localPosition.z)
			{
				playerCamera.transform.localPosition = new Vector3(0, 0, camDist);
			}
			else
			{
				playerCamera.transform.localPosition = new Vector3(0, 0, Mathf.Lerp(playerCamera.transform.localPosition.z, camDist, Time.deltaTime * 3));
			}
		}
		public void LookAtDirection(Vector3 direction)
		{
			float hAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg - 90;
			float vAngle = Mathf.Asin(direction.y) * Mathf.Rad2Deg;

			TargetRotation = -new Vector2(hAngle, vAngle);
		}
		private Vector3 GetOffset()
		{
			float sinPart = Mathf.Sin((CurRotation.x + 90) * Mathf.Deg2Rad);
			float cosPart = Mathf.Cos((CurRotation.x + 90) * Mathf.Deg2Rad);

			Vector3 baseOffset = Player.TargetedEntitie ? playerSettigns.CameraAnchorOffsetWhenTargeting : playerSettigns.CameraAnchorOffset;

			Vector3 offset = baseOffset.x * new Vector3(sinPart, 0, cosPart) +
				new Vector3(0, baseOffset.y, 0) +
				baseOffset.z * new Vector3(cosPart, 0, sinPart);

			return offset;
		}
		private float GetCameraDistance()
		{
			Vector3 rayDir = (playerCamera.transform.position - transform.position).normalized;
			RaycastHit hit;
			float distance;

			if (Physics.Raycast(transform.position, rayDir, out hit, playerSettigns.CameraToPlayerDistance, ConstantCollector.TERRAIN_LAYER_MASK))
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