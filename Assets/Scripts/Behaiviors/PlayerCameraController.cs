using EoE.Entities;
using EoE.Information;
using UnityEngine;

namespace EoE
{
	public class PlayerCameraController : MonoBehaviour
	{
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
		private float curLerpAcceleration;

		private void Start()
		{
			Instance = this;
			CameraFOV = Player.PlayerSettings.CameraBaseFOV;

			curOffset = GetOffset();
			playerCamera.transform.localPosition = new Vector3(0, 0, -Player.PlayerSettings.CameraToPlayerDistance);
			transform.position = Player.Instance.transform.position + curOffset;
			TargetRotation = CurRotation = new Vector2(Player.Instance.transform.eulerAngles.y, Player.PlayerSettings.CameraVerticalAngleClamps.y / 2);

			AnchorToPlayer();
		}
		private void LateUpdate()
		{
			AnchorToPlayer();
			RotateCamera();
		}
		private void AnchorToPlayer()
		{
			if (!Player.Existant)
				return;

			curOffset = Vector3.Lerp(curOffset, GetOffset(), Time.unscaledDeltaTime * 5);
			Vector3 targetPos = Player.Instance.transform.position + curOffset;
			transform.position = new Vector3(	targetPos.x,
												Utils.SpringLerp(transform.position.y, targetPos.y, ref curLerpAcceleration, Player.PlayerSettings.CameraYLerpSringStiffness, Time.unscaledDeltaTime * Player.PlayerSettings.CameraYLerpSpeed),
												targetPos.z
												);

			transform.eulerAngles = new Vector3(CurRotation.y, CurRotation.x, 0);

			float camDist = GetCameraDistance();
			if (camDist > playerCamera.transform.localPosition.z)
			{
				playerCamera.transform.localPosition = new Vector3(0, 0, camDist);
			}
			else
			{
				playerCamera.transform.localPosition = new Vector3(0, 0, Mathf.Lerp(playerCamera.transform.localPosition.z, camDist, Time.unscaledDeltaTime * 3));
			}

			if (Player.Instance.TargetedEntitie)
			{
				Vector3 dir = (Player.Instance.TargetedEntitie.actuallWorldPosition - playerCamera.transform.position).normalized;
				LookInDirection(dir);
			}
		}
		public void LookInDirection(Vector3 direction)
		{
			float hAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg - 90;
			float vAngle = Mathf.Asin(direction.y) * Mathf.Rad2Deg;

			TargetRotation = -new Vector2(hAngle, vAngle);
		}
		private Vector3 GetOffset()
		{
			float sinPart = Mathf.Sin((CurRotation.x + 90) * Mathf.Deg2Rad);
			float cosPart = Mathf.Cos((CurRotation.x + 90) * Mathf.Deg2Rad);

			Vector3 baseOffset = Player.Instance.TargetedEntitie ? Player.PlayerSettings.CameraAnchorOffsetWhenTargeting : Player.PlayerSettings.CameraAnchorOffset;
			Vector3 offset =	baseOffset.x	*	new Vector3(sinPart, 0, cosPart) +
								1				*	new Vector3(0, baseOffset.y, 0) +
								baseOffset.z	*	new Vector3(cosPart, 0, sinPart);

			return offset;
		}
		private float GetCameraDistance()
		{
			Vector3 rayDir = (playerCamera.transform.position - transform.position).normalized;
			RaycastHit hit;
			float distance;

			if (Physics.Raycast(transform.position, rayDir, out hit, Player.PlayerSettings.CameraToPlayerDistance, ConstantCollector.TERRAIN_LAYER_MASK))
			{
				distance = -(transform.position - hit.point).magnitude;
			}
			else
				distance = -Player.PlayerSettings.CameraToPlayerDistance * (1 + rayDir.y * Player.PlayerSettings.CameraExtraZoomOnVertical);

			return distance;
		}
		private void RotateCamera()
		{
			bool targeting = Player.Instance.TargetedEntitie;
			CurRotation.x %= 360;
			TargetRotation.y = Mathf.Clamp(TargetRotation.y, targeting ? Player.PlayerSettings.CameraClampsWhenTargeting.x : Player.PlayerSettings.CameraVerticalAngleClamps.x, targeting ? Player.PlayerSettings.CameraClampsWhenTargeting.y : Player.PlayerSettings.CameraVerticalAngleClamps.y);
			CurRotation = new Vector2(Mathf.LerpAngle(CurRotation.x, TargetRotation.x, Time.unscaledDeltaTime * Player.PlayerSettings.CameraRotationSpeed), Mathf.LerpAngle(CurRotation.y, TargetRotation.y, Time.unscaledDeltaTime * Player.PlayerSettings.CameraRotationSpeed));
		}
	}
}