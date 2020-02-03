using System.Collections;
using System.Collections.Generic;
using EoE.Entities;
using UnityEngine;

namespace EoE.Behaivior
{
	public class ElevatorPlane : ContactActivationObject
	{
		private enum ReturnWhen { Never, ReachedTarget, SecondContact }
		private enum MoveIntent { None, ToTarget, Back }
		[SerializeField] private Vector3 positionChange = default;
		[SerializeField] private float changeTime = 1;
		[SerializeField] private ReturnWhen returnWhen = ReturnWhen.Never;

		private Vector3 worldBasePos;
		private MoveIntent curMoveIntent;
		private float moveTime;
		private bool moving;
		private bool reachedTarget;

		private void Start()
		{
			worldBasePos = transform.position;
		}
		protected override void OnContact(Entity targetEntity)
		{
			base.OnContact(targetEntity);
			if (moving)
				return;

			moving = true;
			curMoveIntent = (reachedTarget && (returnWhen == ReturnWhen.SecondContact)) ? MoveIntent.Back : MoveIntent.ToTarget;
		}

		private void Update()
		{
			if (moving)
			{
				moveTime += Time.deltaTime;
				if(moveTime >= changeTime)
				{
					moveTime = 0;
					UpdatePosition(1);
					if(curMoveIntent == MoveIntent.ToTarget && returnWhen == ReturnWhen.ReachedTarget)
					{
						curMoveIntent = MoveIntent.Back;
					}
					else
					{
						moving = false;
						reachedTarget = curMoveIntent == MoveIntent.ToTarget;
						curMoveIntent = MoveIntent.None;
					}
				}
				else
				{
					UpdatePosition(moveTime / changeTime);
				}
			}
		}
		private void UpdatePosition(float normalized)
		{
			if (curMoveIntent == MoveIntent.ToTarget)
				transform.position = Vector3.LerpUnclamped(worldBasePos, worldBasePos + positionChange, normalized);
			else
				transform.position = Vector3.LerpUnclamped(worldBasePos + positionChange, worldBasePos, normalized);
		}
	}
}