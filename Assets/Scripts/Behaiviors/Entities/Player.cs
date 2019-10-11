using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Events;
using EoE.Information;
using EoE.Controlls;
using EoE.Utils;

namespace EoE.Entities
{
	public class Player : Entitie
	{
		private static Player instance;
		public static Player Instance => instance;
		public override EntitieSettings SelfSettings => selfSettings;
		[SerializeField] private PlayerSettings selfSettings = default;


		private float currentEndurance;

		protected override void EntitieStart()
		{
			currentEndurance = selfSettings.Endurance;
			instance = this;
		}

		private void Update()
		{
			Movement();
			CameraControl();
			Attack();
		}

		private void Movement()
		{
			//1.0.: Where is the player Pointing the Joystick at
			Vector3 controllDirection = PlayerControlls.GetPlayerMove();

			//1.1.:If there is no input, stop here
			if (controllDirection == Vector3.zero)
			{
				UpdateAcceleration(false);
				return;
			}

			//1.2.: How fast does the player actually want to walk? (1,1) Would be greater then 1 * MoveSpeed => we map it back to 1
			float intendedMoveSpeed = Mathf.Min(1, controllDirection.magnitude);

			//2.: Rotate the controlled direction based on where the camera is facing
			float camXDir = Mathf.Sin((-PlayerCameraController.CurRotation.x) * Mathf.Deg2Rad);
			float camZDir = Mathf.Cos((-PlayerCameraController.CurRotation.x) * Mathf.Deg2Rad);
			float newX = (controllDirection.x * camZDir) - (controllDirection.z * camXDir);
			float newZ = (controllDirection.z * camZDir) + (controllDirection.x * camXDir);
			controllDirection.x = newX;
			controllDirection.z = newZ;

			//3.: Now we know where the player wants to walk, but if the model is not aligned yet
			//we need to interpolate between the targeted direction and the current direction
			float turningFactor = GameController.CurrentGameSettings.TurnSpeedCurve.Evaluate(1 - ((transform.forward - controllDirection).magnitude / 2));
			float fraction = Mathf.Min(1, Time.deltaTime / SelfSettings.TurnSpeed);
			transform.forward = ((1 - fraction) * transform.forward + fraction * controllDirection).normalized;

			body.velocity = transform.forward * intendedMoveSpeed * SelfSettings.MoveSpeed * turningFactor * UpdateAcceleration(true, intendedMoveSpeed);
		}
		private void CameraControl()
		{
			Vector2 newMoveDistance = PlayerControlls.CameraMove() * Time.deltaTime;
			newMoveDistance = new Vector2(newMoveDistance.x * selfSettings.CameraRotationPower.x, newMoveDistance.y * selfSettings.CameraRotationPower.y);
			PlayerCameraController.ToRotate += newMoveDistance;
		}

		private void Attack()
		{

		}
	}
}