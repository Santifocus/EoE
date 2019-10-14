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
		public static PlayerSettings PlayerSettings => instance.selfSettings;
		[SerializeField] private PlayerSettings selfSettings = default;
		[SerializeField] private TMPro.TextMeshProUGUI debugText = default;

		private List<EnduranceBar> enduranceBars;
		private int totalEnduranceBars;
		private int reservedEnduranceBars;
		private float usableEndurance;
		private float lockedEndurance;

		private bool recentlyUsedEndurance;

		protected override void EntitieStart()
		{
			instance = this;
			SetupEndurance();
		}

		private void SetupEndurance()
		{
			totalEnduranceBars = PlayerSettings.EnduranceBars;
			usableEndurance = PlayerSettings.EndurancePerBar;

			enduranceBars = new List<EnduranceBar>(totalEnduranceBars);
			for(int i = 0; i < totalEnduranceBars; i++)
			{
				enduranceBars.Add(new EnduranceBar(PlayerSettings.EndurancePerBar));
			}
		}

		protected override void EntitieUpdate()
		{
			EnduranceRegen();
			Movement();
			CameraControl();
			Attack();
		}

		public void EnduranceRegen()
		{

		}

		private void Movement()
		{
			JumpControl();
			Walk();
		}
		private void JumpControl()
		{
			if (PlayerControlls.Buttons.Jump.Down && curStates.IsGrounded)
			{
				Jump();
			}
		}
		private void Walk()
		{
			//1.0.: Where is the player Pointing the Joystick at
			Vector3 controllDirection = PlayerControlls.GetPlayerMove();

			bool moving = controllDirection != Vector3.zero;
			curStates.IsMoving = moving;
			//1.1.:If there is no input, stop here
			if (!moving)
			{
				float curAcceleration = UpdateAcceleration();
				if (curAcceleration == 0)
				{
					animationControl.SetBool("Walking", false);
				}
				return;
			}
			animationControl.SetBool("Walking", true);

			//1.2.: How fast does the player actually want to walk? (1,1) Would be greater then 1 * MoveSpeed => we map it back to 1
			float intendedMoveSpeed = Mathf.Min(1, controllDirection.magnitude);

			//2.: Rotate the controlled direction based on where the camera is facing
			float camXDir = Mathf.Sin((-PlayerCameraController.CurRotation.x) * Mathf.Deg2Rad);
			float camZDir = Mathf.Cos((-PlayerCameraController.CurRotation.x) * Mathf.Deg2Rad);
			float newX = (controllDirection.x * camZDir) - (controllDirection.z * camXDir);
			float newZ = (controllDirection.z * camZDir) + (controllDirection.x * camXDir);
			controllDirection.x = newX;
			controllDirection.z = newZ;

			TurnTo(controllDirection);
			UpdateAcceleration(intendedMoveSpeed);
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

		private struct EnduranceBar
		{
			public bool lockedBar;
			public float fillAmount;
			public EnduranceBar(float fillAmount)
			{
				lockedBar = false;
				this.fillAmount = fillAmount;
			}
		}
	}
}