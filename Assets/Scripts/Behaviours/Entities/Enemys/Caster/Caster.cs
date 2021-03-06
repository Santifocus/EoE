﻿using EoE.Information;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Behaviour.Entities
{
	public class Caster : Enemy
	{
		private const float LAZY_ARRIVAL_DISTANCE = 8;
		private const float LAZY_NEXT_TO_ALLY_MUL = 5;
		public override EnemySettings enemySettings => settings;
		[SerializeField] private CasterSettings settings = default;
		[SerializeField] private Animator animator = default;

		private bool panicModeActive;
		private FXInstance[] panicModeBoundFX;
		private Enemy foundAlly;
		private bool completedPanicMode;
		private bool disallowNormalMode;

		protected override void InRangeBehaivior()
		{
			if(!panicModeActive)
				DecideOnbehaiviorPattern();
		}
		protected override void EntitieUpdate()
		{
			Behaivior();
			AnimationControl();
		}
		private void Behaivior()
		{
			if (foundAlly && !foundAlly.Alive)
				foundAlly = null;
			if (panicModeActive)
			{
				if (foundAlly)
				{
					RefreshDataOnPlayer(false);
					overrideTargetPosition = foundAlly.ActuallWorldPosition;
					if (NextToAlly())
						DeactivatePanicMode();
				}
				else
				{
					FindNearestAlly(settings.PanicModeAlliedSearchRange);
					if (!foundAlly || NextToAlly())
						DeactivatePanicMode();
				}
			}
			else if ((disallowNormalMode || CastingCooldown > 0) && !InActivationCompound)
			{
				switch (settings.CooldownBehaivior)
				{
					case CastCooldownBehaivior.WaitHere:
						{
							overrideTargetPosition = ActuallWorldPosition;
							PointOfInterestIsTarget(true);
						}
						break;

					case CastCooldownBehaivior.StayAtDistance:
						{
							if (!targetPosition.HasValue)
								break;

							Vector3 intendedPosition = targetPosition.Value + (ActuallWorldPosition - targetPosition.Value).normalized * settings.TargetDistance;
							overrideTargetPosition = intendedPosition;
							bool atIntendedPos = (intendedPosition - ActuallWorldPosition).sqrMagnitude < (pointOfInterest.HasValue ? LAZY_ARRIVAL_DISTANCE : 0.5f);
							PointOfInterestIsTarget(atIntendedPos);
						}
						break;

					case CastCooldownBehaivior.GotoTarget:
						{
							if (overrideTargetPosition.HasValue)
								overrideTargetPosition = null;
						}
						break;

					case CastCooldownBehaivior.FleeToAlly:
						{
							if (foundAlly)
							{
								overrideTargetPosition = foundAlly.ActuallWorldPosition;
								bool reachedAlly = NextToAlly(pointOfInterest.HasValue);
								disallowNormalMode = !reachedAlly;
								PointOfInterestIsTarget(reachedAlly);
							}
							else
							{
								disallowNormalMode = false;
								FindNearestAlly(settings.TargetDistance);
								if (!foundAlly)
									goto case CastCooldownBehaivior.WaitHere;
							}
						}
						break;
				}
			}
			else
			{
				PointOfInterestIsTarget(false);
				overrideTargetPosition = null;
			}

			if (!completedPanicMode)
			{
				bool panicMode = settings.PanicModeThreshold >= (CurHealth / CurMaxHealth);
				if (panicModeActive != panicMode)
				{
					if (panicMode)
						ActivatePanicMode();
					else
						DeactivatePanicMode(true);
				}
			}
		}
		private void AnimationControl()
		{
			float sqrVelocity = agent.velocity.sqrMagnitude;
			if(sqrVelocity > 0.1f)
			{
				animator.SetFloat("MoveSpeed", Mathf.Sqrt(sqrVelocity) / settings.AnimationWalkSpeedDivider);
				animator.SetBool("Moving", true);
			}
			else
			{
				animator.SetBool("Moving", false);
			}

			animator.SetBool("Casting", InActivationCompound);
		}
		protected override void Death()
		{
			animator.SetTrigger("Death");
			base.Death();
		}
		private void PointOfInterestIsTarget(bool state)
		{
			if (state)
			{
				if (!pointOfInterest.HasValue)
				{
					MovementStops++;
				}
				pointOfInterest = targetPosition;
			}
			else
			{
				if (pointOfInterest.HasValue)
				{
					pointOfInterest = null;
					MovementStops--;
				}
			}
		}
		private bool NextToAlly(bool lazy = false)
		{
			float dif = (ActuallWorldPosition - foundAlly.ActuallWorldPosition).sqrMagnitude;
			float agentSizes = agent.radius + foundAlly.agent.radius;
			float allowedDif = (agentSizes * agentSizes) * 10f * (lazy ? LAZY_NEXT_TO_ALLY_MUL : 1);

			bool nextToAlly = dif < allowedDif;
			if (nextToAlly)
			{
				for(int i = 0; i < AllEntities.Count; i++)
				{
					if(AllEntities[i] is Enemy && (AllEntities[i].ActuallWorldPosition - ActuallWorldPosition).sqrMagnitude < allowedDif)
					{
						AllEntities[i].StartCombat();
					}
				}
			}
			return nextToAlly;
		}
		private void ActivatePanicMode()
		{
			PointOfInterestIsTarget(false);
			overrideTargetPosition = null;
			panicModeActive = true;
			panicModeBoundFX = ActivateActivationEffects(settings.PanicModeEffects);
			FindNearestAlly(settings.PanicModeAlliedSearchRange);
		}
		private void DeactivatePanicMode(bool becauseOfHeal = false)
		{
			panicModeActive = false;
			FXManager.FinishFX(ref panicModeBoundFX);
			overrideTargetPosition = null;

			if (!becauseOfHeal)
				completedPanicMode = true;
		}
		private void FindNearestAlly(float maxSearchRange)
		{
			float shortestDistance = (maxSearchRange * maxSearchRange) * 1.01f;
			for (int i = 0; i < AllEntities.Count; i++)
			{
				if (!(AllEntities[i] is Enemy) || !AllEntities[i].Alive || AllEntities[i] == this)
					continue;

				float dist = (AllEntities[i].ActuallWorldPosition - ActuallWorldPosition).sqrMagnitude;
				if (dist < shortestDistance)
				{
					shortestDistance = dist;
					foundAlly = AllEntities[i] as Enemy;
				}
			}
		}
		private void DecideOnbehaiviorPattern()
		{
			float distanceToPlayer = (Player.Instance.ActuallWorldPosition - ActuallWorldPosition).sqrMagnitude;

			List<CasterSettings.CasterBehaiviorPattern> possiblePatterns = new List<CasterSettings.CasterBehaiviorPattern>(settings.BehaiviorPatterns.Length);

			for(int i = 0; i < settings.BehaiviorPatterns.Length; i++)
			{
				if(	distanceToPlayer > (settings.BehaiviorPatterns[i].MinRange * settings.BehaiviorPatterns[i].MinRange) &&
					distanceToPlayer < (settings.BehaiviorPatterns[i].MaxRange * settings.BehaiviorPatterns[i].MaxRange) &&
					settings.BehaiviorPatterns[i].TargetCompound.CanActivate(this, 1, 1, 1))
				{
					possiblePatterns.Add(settings.BehaiviorPatterns[i]);
				}
			}
			if(possiblePatterns.Count > 0)
			{
				float totalChoiceAmount = 0;
				for(int i = 0; i < possiblePatterns.Count; i++)
				{
					totalChoiceAmount += possiblePatterns[i].ChoiceRelativeChance;
				}

				float choiceNormalized = Random.value;
				CasterSettings.CasterBehaiviorPattern choosenPattern = null;
				if (totalChoiceAmount <= 0)
				{
					choosenPattern = possiblePatterns[0];
				}
				else
				{
					for (int i = 0; i < possiblePatterns.Count; i++)
					{
						choiceNormalized -= (possiblePatterns[i].ChoiceRelativeChance / totalChoiceAmount);
						if(choiceNormalized <= 0)
						{
							choosenPattern = possiblePatterns[i];
							break;
						}
					}
				}

				ActivateCompound(choosenPattern.TargetCompound);
			}
		}
	}
}