using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Controlls
{
	public class InputController : MonoBehaviour
	{
		//Static
		public static InputController Instance { get; private set; }
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

		public static Button UseItem;
		public static Button PhysicalMagicSwap;

		public static Button MagicScrollUp;
		public static Button MagicScrollDown;
		public static Button ItemScrollUp;
		public static Button ItemScrollDown;

		//Internal
		private PlayerInput inputSystem;
		private PlayerInput.GameInputActions actions => inputSystem.GameInput;

		private Vector2 playerMove;
		private Vector2 cameraMove;

		private void Start()
		{
			if (Instance)
				Destroy(Instance.gameObject);
			Instance = this;
			inputSystem = new PlayerInput();
			inputSystem.Enable();
			SetupInputs();
		}

		private void SetupInputs()
		{
			//Player/Cam - Move
			actions.PlayerMove.performed += ctx => playerMove = ctx.ReadValue<Vector2>();
			actions.CameraMove.performed += ctx => cameraMove = ctx.ReadValue<Vector2>();
			actions.PlayerMove.canceled += ctx => playerMove = Vector2.zero;
			actions.CameraMove.canceled += ctx => cameraMove = Vector2.zero;

			//Jump
			Jump = new Button("Jump");
			actions.Jump.started += ctx => ButtonStarted(Jump);
			actions.Jump.canceled += ctx => ButtonEnded(Jump);

			//Dodge
			Dodge = new Button("Dodge");
			actions.Dodge.started += ctx => ButtonStarted(Dodge);
			actions.Dodge.canceled += ctx => ButtonEnded(Dodge);

			//Attack
			Attack = new Button("Attack");
			actions.NormalAttack.started += ctx => ButtonStarted(Attack);
			actions.NormalAttack.canceled += ctx => ButtonEnded(Attack);

			//HeavyAttack
			HeavyAttack = new Button("HeavyAttack");
			actions.HeavyAttack.started += ctx => ButtonStarted(HeavyAttack);
			actions.HeavyAttack.canceled += ctx => ButtonEnded(HeavyAttack);

			//Run
			Run = new Button("Run");
			actions.Run.started += ctx => ButtonStarted(Run);
			actions.Run.canceled += ctx => ButtonEnded(Run);

			//ResetCamera
			ResetCamera = new Button("ResetCamera");
			actions.ResetCamera.started += ctx => ButtonStarted(ResetCamera);
			actions.ResetCamera.canceled += ctx => ButtonEnded(ResetCamera);

			//Aim
			Aim = new Button("Aim");
			actions.Aim.started += ctx => ButtonStarted(Aim);
			actions.Aim.canceled += ctx => ButtonEnded(Aim);

			//Block
			Block = new Button("Block");
			actions.Block.started += ctx => ButtonStarted(Block);
			actions.Block.canceled += ctx => ButtonEnded(Block);

			//Pause
			Pause = new Button("Pause");
			actions.Pause.started += ctx => ButtonStarted(Pause);
			actions.Pause.canceled += ctx => ButtonEnded(Pause);

			//UseItem
			UseItem = new Button("UseItem");
			actions.UseItem.started += ctx => ButtonStarted(UseItem);
			actions.UseItem.canceled += ctx => ButtonEnded(UseItem);

			//PhysicalMagicSwap
			PhysicalMagicSwap = new Button("PhysicalMagicSwap");
			actions.PhysicalMagicSwap.started += ctx => ButtonStarted(PhysicalMagicSwap);
			actions.PhysicalMagicSwap.canceled += ctx => ButtonEnded(PhysicalMagicSwap);

			//MagicScrollUp
			MagicScrollUp = new Button("MagicScrollUp");
			actions.MagicScrollUp.started += ctx => ButtonStarted(MagicScrollUp);
			actions.MagicScrollUp.canceled += ctx => ButtonEnded(MagicScrollUp);

			//MagicScrollDown
			MagicScrollDown = new Button("MagicScrollDown");
			actions.MagicScrollDown.started += ctx => ButtonStarted(MagicScrollDown);
			actions.MagicScrollDown.canceled += ctx => ButtonEnded(MagicScrollDown);

			//ItemScrollUp
			ItemScrollUp = new Button("ItemScrollUp");
			actions.ItemScrollUp.started += ctx => ButtonStarted(ItemScrollUp);
			actions.ItemScrollUp.canceled += ctx => ButtonEnded(ItemScrollUp);

			//ItemScrollDown
			ItemScrollDown = new Button("ItemScrollDown");
			actions.ItemScrollDown.started += ctx => ButtonStarted(ItemScrollDown);
			actions.ItemScrollDown.canceled += ctx => ButtonEnded(ItemScrollDown);
		}

		private void OnDestroy()
		{
			inputSystem.Disable();
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