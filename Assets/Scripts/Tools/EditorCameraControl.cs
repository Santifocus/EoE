using EoE.Behaviour.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Tools
{
	public class EditorCameraControl : MonoBehaviour
	{
		private const string PLAYER_OBJECTNAME = "PlayerEntity";
		private const string PLAYER_CAMERA_OBJECTNAME = "PlayerCameraAnchor";
		private const string GAME_UI_OBJECTNAME = "GameUI";

		private static readonly KeyCode ForwardKey = KeyCode.W;
		private static readonly KeyCode BackwardKey = KeyCode.S;
		private static readonly KeyCode RightKey = KeyCode.D;
		private static readonly KeyCode LeftKey = KeyCode.A;
		private static readonly KeyCode UpKey = KeyCode.Space;
		private static readonly KeyCode DownKey = KeyCode.LeftShift;

		private static readonly KeyCode CameraZPlus = KeyCode.E;
		private static readonly KeyCode CameraZMinus = KeyCode.Q;

		private static readonly KeyCode CameraZReset = KeyCode.T;

		private const string MOUSE_SCROLL_AXIS = "Mouse ScrollWheel";

		private const string MOUSE_X_AXIS = "Mouse X";
		private const string MOUSE_Y_AXIS = "Mouse Y";

		[SerializeField] private float moveStrength = 10;
		[SerializeField] private float scrollMoveMultiplier = 0.5f;
		[SerializeField] private float moveLerpSpeed = 5;

		[Space(6)]
		[SerializeField] private float rotateStrength = 90;
		[SerializeField] private float rotateLerpSpeed = 7;

		private float flightStrengthMultiplier = 1;
		private float curFlightStrength => moveStrength * flightStrengthMultiplier;

		private Vector3 moveVelocity;
		private Vector3 rotateVelocity;

		private void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;

			GameObject playerObject = GameObject.Find(PLAYER_OBJECTNAME);
			if (playerObject)
				playerObject.GetComponent<Player>().enabled = false;

			GameObject.Find(GAME_UI_OBJECTNAME)?.SetActive(false);
			GameObject.Find(PLAYER_CAMERA_OBJECTNAME)?.SetActive(false);
		}
		private void LateUpdate()
		{
			if (Time.deltaTime > 0.25f)
				return;

			Move();
			Rotate();
		}
		private void Move()
		{
			flightStrengthMultiplier = Mathf.Max(flightStrengthMultiplier + Input.GetAxis(MOUSE_SCROLL_AXIS) * scrollMoveMultiplier * 1000 * Time.deltaTime, 0.1f);

			if (Input.GetKey(ForwardKey))
				moveVelocity += transform.forward * curFlightStrength * Time.deltaTime;
			if (Input.GetKey(BackwardKey))
				moveVelocity += transform.forward * curFlightStrength * Time.deltaTime * -1;
			if (Input.GetKey(RightKey))
				moveVelocity += transform.right * curFlightStrength * Time.deltaTime;
			if (Input.GetKey(LeftKey))
				moveVelocity += transform.right * curFlightStrength * Time.deltaTime * -1;
			if (Input.GetKey(UpKey))
				moveVelocity += transform.up * curFlightStrength * Time.deltaTime;
			if (Input.GetKey(DownKey))
				moveVelocity += transform.up * curFlightStrength * Time.deltaTime * -1;

			LerpMove();
		}
		private void LerpMove()
		{
			Vector3 moveDelta = moveVelocity * Time.deltaTime * moveLerpSpeed;
			moveVelocity -= moveDelta;

			transform.position += moveDelta;
		}
		private void Rotate()
		{
			float zInput = Input.GetKey(CameraZPlus) ? 1 : (Input.GetKey(CameraZMinus) ? -1 : 0);
			if (Input.GetKey(CameraZReset))
				zInput = Mathf.DeltaAngle(rotateVelocity.z + transform.eulerAngles.z, 0);

			rotateVelocity += new Vector3(	-Input.GetAxis(MOUSE_Y_AXIS) * Time.deltaTime * rotateStrength, 
											Input.GetAxis(MOUSE_X_AXIS) * Time.deltaTime * rotateStrength,
											zInput * Time.deltaTime * rotateStrength
											);
			LerpRotate();
		}
		private void LerpRotate()
		{
			Vector3 rotateDelta = rotateVelocity * Time.deltaTime * rotateLerpSpeed;
			rotateVelocity -= rotateDelta;

			transform.eulerAngles = new Vector3(transform.eulerAngles.x + rotateDelta.x,
												transform.eulerAngles.y + rotateDelta.y,
												transform.eulerAngles.z + rotateDelta.z);
		}
	}
}