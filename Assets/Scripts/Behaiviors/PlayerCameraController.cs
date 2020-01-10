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

		private PlayerSettings playerSettigns => Player.Instance.SelfSettings as PlayerSettings;

		private void Start()
		{
			TargetRotation = CurRotation = new Vector2(transform.eulerAngles.y, transform.eulerAngles.x);
			Instance = this;
			playerCamera.transform.localPosition = new Vector3(0, 0, -playerSettigns.CameraToPlayerDistance);
			curOffset = GetOffset();
			transform.position = Player.Instance.transform.position + curOffset;
			CameraFOV = playerSettigns.CameraBaseFOV;

			AnchorToPlayer();
		}
		private void FixedUpdate()
		{
			AnchorToPlayer();
			RotateCamera();
		}

		private void AnchorToPlayer()
		{
			if (!Player.Instance)
				return;

			curOffset = Vector3.Lerp(curOffset, GetOffset(), Time.fixedDeltaTime * 5);
			Vector3 targetPos = Player.Instance.transform.position + curOffset;
			transform.position = new Vector3(	targetPos.x,
												Utils.SpringLerp(transform.position.y, targetPos.y, ref curLerpAcceleration, playerSettigns.CameraYLerpSringStiffness, Time.fixedDeltaTime *										playerSettigns.CameraYLerpSpeed),
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
				playerCamera.transform.localPosition = new Vector3(0, 0, Mathf.Lerp(playerCamera.transform.localPosition.z, camDist, Time.fixedDeltaTime * 3));
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

			Vector3 baseOffset = Player.Instance.TargetedEntitie ? playerSettigns.CameraAnchorOffsetWhenTargeting : playerSettigns.CameraAnchorOffset;
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

			if (Physics.Raycast(transform.position, rayDir, out hit, playerSettigns.CameraToPlayerDistance, ConstantCollector.TERRAIN_LAYER_MASK))
			{
				distance = -(transform.position - hit.point).magnitude;
			}
			else
				distance = -playerSettigns.CameraToPlayerDistance * (1 + rayDir.y * playerSettigns.CameraExtraZoomOnVertical);

			return distance;
		}
		private void RotateCamera()
		{
			bool targeting = Player.Instance.TargetedEntitie;
			CurRotation.x %= 360;
			TargetRotation.y = Mathf.Clamp(TargetRotation.y, targeting ? playerSettigns.CameraClampsWhenTargeting.x : playerSettigns.CameraVerticalAngleClamps.x, targeting ? playerSettigns.CameraClampsWhenTargeting.y : playerSettigns.CameraVerticalAngleClamps.y);
			CurRotation = new Vector2(Mathf.LerpAngle(CurRotation.x, TargetRotation.x, Time.fixedDeltaTime * playerSettigns.CameraRotationSpeed), Mathf.LerpAngle(CurRotation.y, TargetRotation.y, Time.fixedDeltaTime * playerSettigns.CameraRotationSpeed));
		}
	}
}