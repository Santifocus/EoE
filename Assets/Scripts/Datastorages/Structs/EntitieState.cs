using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EoE.Entities
{
	public struct EntitieState
	{
		private byte state;

		private const byte GroundedReset = 254;
		private const byte MovingReset = 253;
		private const byte RunningReset = 251;
		private const byte BlockingReset = 247;
		private const byte CombatReset = 239;
		private const byte FallingReset = 223;
		//private const byte ? = 191;
		//private const byte ? = 127;

		public bool IsGrounded
		{
			get => ((state | 1) == state);
			set
			{
				state = (byte)(state & GroundedReset);
				if (value)
				{
					state |= 1;
				}
			}
		}
		public bool IsMoving
		{
			get => ((state | 2) == state);
			set
			{
				state = (byte)(state & MovingReset);
				if (value)
				{
					state |= 2;
				}
			}
		}
		public bool IsRunning
		{
			get => ((state | 4) == state);
			set
			{
				state = (byte)(state & RunningReset);
				if (value)
				{
					state |= 4;
				}
			}
		}
		public bool IsBlocking
		{
			get => ((state | 8) == state);
			set
			{
				state = (byte)(state & BlockingReset);
				if (value)
				{
					state |= 8;
				}
			}
		}
		public bool IsInCombat
		{
			get => ((state | 16) == state);
			set
			{
				state = (byte)(state & CombatReset);
				if (value)
				{
					state |= 16;
				}
			}
		}
		public bool IsFalling
		{
			get => ((state | 32) == state);
			set
			{
				state = (byte)(state & FallingReset);
				if (value)
				{
					state |= 32;
				}
			}
		}
	}
}