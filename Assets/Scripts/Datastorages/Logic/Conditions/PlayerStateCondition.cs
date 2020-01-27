using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information.Logic
{
	public class PlayerStateCondition : LogicComponent
	{
		public enum StateTarget
		{
			PlayerIsMoving = 1,
			PlayerIsRunning = 2,
			PlayerOnGround = 3,
			PlayerInCombat = 4,
			PlayerIsTurning = 5,
			PlayerIsFalling = 6,
			PlayerHasTarget = 7,
			PlayerIsStunned = 8,
			PlayerIsMovementStopped = 9,
			PlayerIsRotationStopped = 10,
			PlayerIsInvincible = 11,
		}
		protected override bool InternalTrue => StateCheck();
		public StateTarget stateTarget = StateTarget.PlayerOnGround;
		private bool StateCheck()
		{
			bool wasMet = false;
			switch (stateTarget)
			{
				case StateTarget.PlayerIsMoving:
					wasMet = Player.Instance ? Player.Instance.curStates.Moving : false;
					break;
				case StateTarget.PlayerIsRunning:
					wasMet = Player.Instance ? Player.Instance.curStates.Running : false;
					break;
				case StateTarget.PlayerOnGround:
					wasMet = Player.Instance ? Player.Instance.charController.isGrounded : false;
					break;
				case StateTarget.PlayerInCombat:
					wasMet = Player.Instance ? Player.Instance.curStates.Fighting : false;
					break;
				case StateTarget.PlayerIsTurning:
					wasMet = Player.Instance ? Player.Instance.curStates.Turning : false;
					break;
				case StateTarget.PlayerIsFalling:
					wasMet = Player.Instance ? Player.Instance.curStates.Falling : false;
					break;
				case StateTarget.PlayerHasTarget:
					wasMet = Player.Instance ? Player.Instance.TargetedEntitie : false;
					break;
				case StateTarget.PlayerIsStunned:
					wasMet = Player.Instance ? Player.Instance.IsStunned : false;
					break;
				case StateTarget.PlayerIsMovementStopped:
					wasMet = Player.Instance ? Player.Instance.IsMovementStopped : false;
					break;
				case StateTarget.PlayerIsRotationStopped:
					wasMet = Player.Instance ? Player.Instance.IsTurnStopped : false;
					break;
				case StateTarget.PlayerIsInvincible:
					wasMet = Player.Instance ? Player.Instance.IsInvincible : false;
					break;
			}
			return wasMet;
		}
	}
}