using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Controlls
{
	public static class PlayerControlls
	{
		public delegate void ScreenResolutionUpdate();
		public static ScreenResolutionUpdate UpdateScreenRes;
	}
	public class PlayerInput
	{
		private KeyCode targetKey;
		public KeyCode TargetKey
		{
			get => targetKey;
			set
			{
				ChangeTargetKey(value);
			}
		}
		public string KeyName;

		public bool Down => IsKeyDown();
		public bool Active => IsKeyActive();
		public bool Up => IsKeyUp();

		private void ChangeTargetKey(KeyCode newTargetKey)
		{
			this.targetKey = newTargetKey;
			KeyName = targetKey.ToString();
		}
		public PlayerInput(KeyCode targetKey)
		{
			this.targetKey = targetKey;
			KeyName = targetKey.ToString();
		}
		private bool IsKeyDown()
		{
			return Input.GetKeyDown(targetKey);
		}
		private bool IsKeyActive()
		{
			return Input.GetKey(targetKey);
		}
		private bool IsKeyUp()
		{
			return Input.GetKeyUp(targetKey);
		}
	}
}