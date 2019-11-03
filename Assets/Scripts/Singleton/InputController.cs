﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Controlls
{
	public class InputController : MonoBehaviour
	{
		//Static
		public static InputController Instance { get; private set; }
		#region GameInput
		public static Vector2 PlayerMove => Instance.playerMove;
		public static Vector2 CameraMove => Instance.cameraMove;

		public static Button Jump;
		public static Button Dodge;
		public static Button Attack;
		public static Button HeavyAttack;

		public static Button Run;
		public static Button ResetCamera;

		public static Button Aim;
		public static Button Block;

		public static Button Pause;
		public static Button PlayerMenu;

		public static Button UseItem;
		public static Button PhysicalMagicSwap;

		public static Button MagicScrollUp;
		public static Button MagicScrollDown;
		public static Button ItemScrollUp;
		public static Button ItemScrollDown;

		private Vector2 playerMove;
		private Vector2 cameraMove;
		#endregion
		#region MenuInput
		public static Button MenuRight;
		public static Button MenuLeft;
		public static Button MenuUp;
		public static Button MenuDown;

		public static Button MenuEnter;
		public static Button MenuBack;

		public static Button MenuPause;
		public static Button MenuPlayerMenu;
		#endregion

		//Internal
		private PlayerInput playerInput;
		private PlayerInput.GameInputActions gameActions => playerInput.GameInput;
		private PlayerInput.MenuInputActions menuActions => playerInput.MenuInput;

		private void Start()
		{
			if (Instance)
				Destroy(Instance.gameObject);
			Instance = this;
			playerInput = new PlayerInput();
			playerInput.Enable();
			SetupInputs();
		}

		private void SetupInputs()
		{
			SetupGameInput();
			SetupMenuInput();
		}

		private void SetupGameInput()
		{
			//Player/Cam - Move
			gameActions.PlayerMove.performed += ctx => playerMove = ctx.ReadValue<Vector2>();
			gameActions.CameraMove.performed += ctx => cameraMove = ctx.ReadValue<Vector2>();
			gameActions.PlayerMove.canceled += ctx => playerMove = Vector2.zero;
			gameActions.CameraMove.canceled += ctx => cameraMove = Vector2.zero;

			//Jump
			Jump = new Button("Jump");
			gameActions.Jump.started += ctx => ButtonStarted(Jump);
			gameActions.Jump.canceled += ctx => ButtonEnded(Jump);

			//Dodge
			Dodge = new Button("Dodge");
			gameActions.Dodge.started += ctx => ButtonStarted(Dodge);
			gameActions.Dodge.canceled += ctx => ButtonEnded(Dodge);

			//Attack
			Attack = new Button("Attack");
			gameActions.NormalAttack.started += ctx => ButtonStarted(Attack);
			gameActions.NormalAttack.canceled += ctx => ButtonEnded(Attack);

			//HeavyAttack
			HeavyAttack = new Button("HeavyAttack");
			gameActions.HeavyAttack.started += ctx => ButtonStarted(HeavyAttack);
			gameActions.HeavyAttack.canceled += ctx => ButtonEnded(HeavyAttack);

			//Run
			Run = new Button("Run");
			gameActions.Run.started += ctx => ButtonStarted(Run);
			gameActions.Run.canceled += ctx => ButtonEnded(Run);

			//ResetCamera
			ResetCamera = new Button("ResetCamera");
			gameActions.ResetCamera.started += ctx => ButtonStarted(ResetCamera);
			gameActions.ResetCamera.canceled += ctx => ButtonEnded(ResetCamera);

			//Aim
			Aim = new Button("Aim");
			gameActions.Aim.started += ctx => ButtonStarted(Aim);
			gameActions.Aim.canceled += ctx => ButtonEnded(Aim);

			//Block
			Block = new Button("Block");
			gameActions.Block.started += ctx => ButtonStarted(Block);
			gameActions.Block.canceled += ctx => ButtonEnded(Block);

			//Pause
			Pause = new Button("Pause");
			gameActions.Pause.started += ctx => ButtonStarted(Pause);
			gameActions.Pause.canceled += ctx => ButtonEnded(Pause);

			//PlayerMenu
			Pause = new Button("PlayerMenu");
			gameActions.PlayerMenu.started += ctx => ButtonStarted(PlayerMenu);
			gameActions.PlayerMenu.canceled += ctx => ButtonEnded(PlayerMenu);

			//UseItem
			UseItem = new Button("UseItem");
			gameActions.UseItem.started += ctx => ButtonStarted(UseItem);
			gameActions.UseItem.canceled += ctx => ButtonEnded(UseItem);

			//PhysicalMagicSwap
			PhysicalMagicSwap = new Button("PhysicalMagicSwap");
			gameActions.PhysicalMagicSwap.started += ctx => ButtonStarted(PhysicalMagicSwap);
			gameActions.PhysicalMagicSwap.canceled += ctx => ButtonEnded(PhysicalMagicSwap);

			//MagicScrollUp
			MagicScrollUp = new Button("MagicScrollUp");
			gameActions.MagicScrollUp.started += ctx => ButtonStarted(MagicScrollUp);
			gameActions.MagicScrollUp.canceled += ctx => ButtonEnded(MagicScrollUp);

			//MagicScrollDown
			MagicScrollDown = new Button("MagicScrollDown");
			gameActions.MagicScrollDown.started += ctx => ButtonStarted(MagicScrollDown);
			gameActions.MagicScrollDown.canceled += ctx => ButtonEnded(MagicScrollDown);

			//ItemScrollUp
			ItemScrollUp = new Button("ItemScrollUp");
			gameActions.ItemScrollUp.started += ctx => ButtonStarted(ItemScrollUp);
			gameActions.ItemScrollUp.canceled += ctx => ButtonEnded(ItemScrollUp);

			//ItemScrollDown
			ItemScrollDown = new Button("ItemScrollDown");
			gameActions.ItemScrollDown.started += ctx => ButtonStarted(ItemScrollDown);
			gameActions.ItemScrollDown.canceled += ctx => ButtonEnded(ItemScrollDown);
		}
		private void SetupMenuInput()
		{
			//MenuRight
			MenuRight = new Button("MenuRight");
			menuActions.Right.started += ctx => ButtonStarted(MenuRight);
			menuActions.Right.canceled += ctx => ButtonEnded(MenuRight);

			//MenuLeft
			MenuLeft = new Button("MenuLeft");
			menuActions.Left.started += ctx => ButtonStarted(MenuLeft);
			menuActions.Left.canceled += ctx => ButtonEnded(MenuLeft);

			//MenuUp
			MenuUp = new Button("MenuUp");
			menuActions.Up.started += ctx => ButtonStarted(MenuUp);
			menuActions.Up.canceled += ctx => ButtonEnded(MenuUp);

			//MenuDown
			MenuDown = new Button("MenuDown");
			menuActions.Down.started += ctx => ButtonStarted(MenuDown);
			menuActions.Down.canceled += ctx => ButtonEnded(MenuDown);

			//MenuEnter
			MenuEnter = new Button("MenuEnter");
			menuActions.Enter.started += ctx => ButtonStarted(MenuEnter);
			menuActions.Enter.canceled += ctx => ButtonEnded(MenuEnter);

			//MenuBack
			MenuBack = new Button("MenuBack");
			menuActions.Back.started += ctx => ButtonStarted(MenuBack);
			menuActions.Back.canceled += ctx => ButtonEnded(MenuBack);

			//MenuPause
			MenuPause = new Button("MenuPause");
			menuActions.Pause.started += ctx => ButtonStarted(MenuDown);
			menuActions.Pause.canceled += ctx => ButtonEnded(MenuDown);

			//MenuPlayerMenu
			MenuPlayerMenu = new Button("MenuPlayerMenu");
			menuActions.PlayerMenu.started += ctx => ButtonStarted(MenuPlayerMenu);
			menuActions.PlayerMenu.canceled += ctx => ButtonEnded(MenuPlayerMenu);
		}

		private void OnDestroy()
		{
			playerInput.Disable();
		}

		private void ButtonStarted(Button target)
		{
			StartCoroutine(FrameDelayStart(target));
		}
		private IEnumerator FrameDelayStart(Button target)
		{
			target.Down = true;
			yield return new WaitForEndOfFrame();
			target.Down = false;
			target.Active = true;
		}
		private void ButtonEnded(Button target)
		{
			StartCoroutine(FrameDelayEnd(target));
		}
		private IEnumerator FrameDelayEnd(Button target)
		{
			//The player can theoretically press and release the button in the same frame which would cause target.Active to be true untill the button will be pressed again
			//so in case target.Down == true we wait a extra frame
			if(target.Down)
				yield return new WaitForEndOfFrame();

			target.Up = true;
			target.Active = false;
			yield return new WaitForEndOfFrame();
			target.Up = false;
		}

		public class Button
		{
			public readonly string ButtonName;
			public bool Down { get; internal set; }
			public bool Active { get; internal set; }
			public bool Up { get; internal set; }

			public Button(string ButtonName)
			{
				this.ButtonName = ButtonName;
				Down = false;
				Active = false;
				Up = false;
			}
		}
	}
}