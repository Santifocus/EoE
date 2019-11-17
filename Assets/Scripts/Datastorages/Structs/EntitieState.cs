namespace EoE.Entities
{
	public struct EntitieState
	{
		private byte state;

		private const byte GroundedReset = 254;
		private const byte MovingReset = 253;
		private const byte RunningReset = 251;
		private const byte CombatReset = 247;
		private const byte FallingReset = 239;
		private const byte TurningReset = 223;

		public bool Grounded
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
		public bool Moving
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
		public bool Running
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
		public bool Fighting
		{
			get => ((state | 8) == state);
			set
			{
				state = (byte)(state & CombatReset);
				if (value)
				{
					state |= 8;
				}
			}
		}
		public bool Falling
		{
			get => ((state | 16) == state);
			set
			{
				state = (byte)(state & FallingReset);
				if (value)
				{
					state |= 16;
				}
			}
		}
		public bool Turning
		{
			get => ((state | 32) == state);
			set
			{
				state = (byte)(state & TurningReset);
				if (value)
				{
					state |= 32;
				}
			}
		}
	}
}