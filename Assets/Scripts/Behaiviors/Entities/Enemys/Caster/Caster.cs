using EoE.Information;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Entities
{
	public class Caster : Enemy
	{
		private const float LAZY_ARRIVAL_DISTANCE = 8;
		private const float LAZY_NEXT_TO_ALLY_MUL = 5;
		public override EnemySettings enemySettings => settings;
		public CasterSettings settings;

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
			if (foundAlly && !foundAlly.Alive)
				foundAlly = null;
			if (panicModeActive)
			{
				if (foundAlly)
				{
					RefreshDataOnPlayer(false);
					overrideTargetPosition = foundAlly.actuallWorldPosition;
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
			else if((disallowNormalMode || CastingCooldown > 0) && !InActivationCompound)
			{
				switch (settings.CooldownBehaivior)
				{
					case CastCooldownBehaivior.WaitHere:
						{
							overrideTargetPosition = actuallWorldPosition;
							PointOfInterestIsTarget(true);
						}
						break;

					case CastCooldownBehaivior.StayAtDistance:
						{
							if (!targetPosition.HasValue)
								break;

							Vector3 intendedPosition = targetPosition.Value + (actuallWorldPosition - targetPosition.Value).normalized * settings.TargetDistance;
							overrideTargetPosition = intendedPosition;
							bool atIntendedPos = (intendedPosition - actuallWorldPosition).sqrMagnitude < (pointOfInterest.HasValue ? LAZY_ARRIVAL_DISTANCE : 0.5f);
							PointOfInterestIsTarget(atIntendedPos);
						}
						break;

					case CastCooldownBehaivior.GotoTarget:
						{
							if(overrideTargetPosition.HasValue)
								overrideTargetPosition = null;
						}
						break;

					case CastCooldownBehaivior.FleeToAlly:
						{
							if (foundAlly)
							{
								overrideTargetPosition = foundAlly.actuallWorldPosition;
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
				bool panicMode = settings.PanicModeThreshold >= (curHealth / curMaxHealth);
				if (panicModeActive != panicMode)
				{
					if (panicMode)
						ActivatePanicMode();
					else
						DeactivatePanicMode(true);
				}
			}
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
			float dif = (actuallWorldPosition - foundAlly.actuallWorldPosition).sqrMagnitude;
			float agentSizes = agent.radius + foundAlly.agent.radius;
			float allowedDif = (agentSizes * agentSizes) * 10f * (lazy ? LAZY_NEXT_TO_ALLY_MUL : 1);

			bool nextToAlly = dif < allowedDif;
			if (nextToAlly)
			{
				for(int i = 0; i < AllEntities.Count; i++)
				{
					if(AllEntities[i] is Enemy && (AllEntities[i].actuallWorldPosition - actuallWorldPosition).sqrMagnitude < allowedDif)
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

				float dist = (AllEntities[i].actuallWorldPosition - actuallWorldPosition).sqrMagnitude;
				if (dist < shortestDistance)
				{
					shortestDistance = dist;
					foundAlly = AllEntities[i] as Enemy;
				}
			}
		}
		private void DecideOnbehaiviorPattern()
		{
			float distanceToPlayer = (Player.Instance.actuallWorldPosition - actuallWorldPosition).sqrMagnitude;

			List<int> possiblePatterns = new List<int>(settings.BehaiviorPatterns.Length);

			for(int i = 0; i < settings.BehaiviorPatterns.Length; i++)
			{
				if(	distanceToPlayer > (settings.BehaiviorPatterns[i].MinRange * settings.BehaiviorPatterns[i].MinRange) &&
					distanceToPlayer < (settings.BehaiviorPatterns[i].MaxRange * settings.BehaiviorPatterns[i].MaxRange) &&
					settings.BehaiviorPatterns[i].TargetCompound.CanActivate(this, 1, 1, 1))
				{
					possiblePatterns.Add(i);
				}
			}
			if(possiblePatterns.Count > 0)
			{
				int targetPatternIndex = possiblePatterns[Random.Range(0, possiblePatterns.Count)];
				ActivateCompound(settings.BehaiviorPatterns[targetPatternIndex].TargetCompound);
			}
		}
	}
}