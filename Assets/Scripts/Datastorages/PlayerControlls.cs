using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Controlls
{
	public static class PlayerControlls
	{
		private const float INPUT_ACCEPTANCE_THRESHOLD =	0.35f;

		private const string MOVE_X =						"HorizontalMove";
		private const string MOVE_Z =						"VerticalMove";
		private const string CAMERA_MOVE_Y =				"CameraMoveVertical";
		private const string CAMERA_MOVE_Z =				"CameraMoveSide";
		
		private const string AIM_BLOCK =					"AimAndBlock";

		private const string ITEM_SWAP =					"ItemSwap";
		private const string MAGIC_SWAP =					"MagicSwap";

		public static Vector3 GetPlayerMove()
		{
			Vector3 axes = new Vector3(Input.GetAxis(MOVE_X), 0, Input.GetAxis(MOVE_Z) * -1);
			axes.x = Mathf.Abs(axes.x) > INPUT_ACCEPTANCE_THRESHOLD ? axes.x : 0;
			axes.z = Mathf.Abs(axes.z) > INPUT_ACCEPTANCE_THRESHOLD ? axes.z : 0;
			return axes;
		}

		public static Vector2 CameraMove()
		{
			Vector2 axes = new Vector2(Input.GetAxis(CAMERA_MOVE_Z), Input.GetAxis(CAMERA_MOVE_Y) * -1);
			axes.x = Mathf.Abs(axes.x) > INPUT_ACCEPTANCE_THRESHOLD ? axes.x : 0;
			axes.y = Mathf.Abs(axes.y) > INPUT_ACCEPTANCE_THRESHOLD ? axes.y : 0;
			return axes;
		}
		
		public static (bool, bool) AimingOrBlocking()
		{
			float point = Input.GetAxis(AIM_BLOCK);
			return (point < -INPUT_ACCEPTANCE_THRESHOLD, point > INPUT_ACCEPTANCE_THRESHOLD);
		}

		public static int ItemSelectChange()
		{
			float state = Input.GetAxis(ITEM_SWAP);

			if (state < 0)
				return -1;
			else if (state > 0)
				return 1;
			else
				return 0;
		}
		public static int MagicSelectChange()
		{
			float state = Input.GetAxis(MAGIC_SWAP);

			if (state < 0)
				return -1;
			else if (state > 0)
				return 1;
			else
				return 0;
		}

		public static class Buttons
		{
			private const KeyCode JUMP =					KeyCode.Joystick1Button0;
			private const KeyCode DODGE =					KeyCode.Joystick1Button1;
			private const KeyCode NORMAL_ATTACK =			KeyCode.Joystick1Button2;
			private const KeyCode HEAVY_ATTACK =			KeyCode.Joystick1Button3;
			private const KeyCode USE_ITEM =				KeyCode.Joystick1Button4;
			private const KeyCode WEAPON_MAGIC_SWAP =		KeyCode.Joystick1Button5;
			private const KeyCode BACK =					KeyCode.Joystick1Button6;
			private const KeyCode PAUSE =					KeyCode.Joystick1Button7;

			public static ButtonInput Jump =				new ButtonInput("A", JUMP);
			public static ButtonInput Dodge =				new ButtonInput("B", DODGE);
			public static ButtonInput Attack =				new ButtonInput("X", NORMAL_ATTACK);
			public static ButtonInput HeavyAttack =			new ButtonInput("Y", HEAVY_ATTACK);
			public static ButtonInput UseItem =				new ButtonInput("LB", USE_ITEM);
			public static ButtonInput WeaponMagicSwap =		new ButtonInput("RB", WEAPON_MAGIC_SWAP);
			public static ButtonInput Back =				new ButtonInput("Back", BACK);
			public static ButtonInput Pause =				new ButtonInput("Pause", PAUSE);

			public class ButtonInput
			{
				public readonly string ButtonName;
				private KeyCode targetKey;

				public bool Down => IsDown();
				public bool Active => IsActive();
				public bool Up => IsUp();
				
				public ButtonInput(string ButtonName, KeyCode targetKey)
				{
					this.ButtonName = ButtonName;
					this.targetKey = targetKey;
				}

				private bool IsDown()
				{
					return Input.GetKeyDown(targetKey);
				}
				private bool IsActive()
				{
					return Input.GetKey(targetKey);
				}
				private bool IsUp()
				{
					return Input.GetKeyUp(targetKey);
				}
			}
		}
	}
}