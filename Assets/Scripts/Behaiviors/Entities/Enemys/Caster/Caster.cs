using EoE.Information;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Entities
{
	public class Caster : Enemy
	{
		public override EnemySettings enemySettings => settings;
		public CasterSettings settings;

		private bool panicModeActive;
		private FXInstance[] panicModeBoundFX;
		private Enemy foundAlly;
		private bool completedPanicMode;
		
		protected override void InRangeBehaivior()
		{
			if(!panicModeActive)
				DecideOnbehaiviorPattern();
		}
		protected override void EntitieUpdate()
		{
			if (panicModeActive)
			{
				if (foundAlly)
				{
					overrideTargetPosition = foundAlly.actuallWorldPosition;
					if (NextToAlly())
						DeactivatePanicMode();
				}
				else
				{
					FindNearestAlly();
					if (!foundAlly || NextToAlly())
						DeactivatePanicMode();
				}
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
		private bool NextToAlly()
		{
			float dif = (actuallWorldPosition - foundAlly.actuallWorldPosition).sqrMagnitude;
			float maxDif = agent.radius + foundAlly.agent.radius;

			bool nextToAlly = dif < ((maxDif * maxDif) * 3f);
			if (nextToAlly)
			{
				for(int i = 0; i < AllEntities.Count; i++)
				{
					if(AllEntities[i] is Enemy && (AllEntities[i].actuallWorldPosition - actuallWorldPosition).sqrMagnitude < (10 * 10))
					{
						AllEntities[i].StartCombat();
					}
				}
			}
			return nextToAlly;
		}
		private void ActivatePanicMode()
		{
			panicModeActive = true;
			panicModeBoundFX = ActivateActivationEffects(settings.PanicModeEffects);
			FindNearestAlly();
		}
		private void FindNearestAlly()
		{
			float shortestDistance = settings.PanicModeAlliedSearchRange * settings.PanicModeAlliedSearchRange * 1.01f;
			for (int i = 0; i < AllEntities.Count; i++)
			{
				if (!(AllEntities[i] is Enemy) || AllEntities[i] == this)
					continue;

				float dist = (AllEntities[i].actuallWorldPosition - actuallWorldPosition).sqrMagnitude;
				if (dist < shortestDistance)
				{
					shortestDistance = dist;
					foundAlly = AllEntities[i] as Enemy;
				}
			}
		}
		private void DeactivatePanicMode(bool becauseOfHeal = false)
		{
			panicModeActive = false;
			ReleaseBoundEffects();
			overrideTargetPosition = null;

			if (!becauseOfHeal)
				completedPanicMode = true;
		}
		private void DecideOnbehaiviorPattern()
		{
			if (IsCasting)
				return;

			float distanceToPlayer = (Player.Instance.actuallWorldPosition - actuallWorldPosition).sqrMagnitude;

			List<int> possiblePatterns = new List<int>(settings.BehaiviorPatterns.Length);

			for(int i = 0; i < settings.BehaiviorPatterns.Length; i++)
			{
				if(	distanceToPlayer > (settings.BehaiviorPatterns[i].MinRange * settings.BehaiviorPatterns[i].MinRange) &&
					distanceToPlayer < (settings.BehaiviorPatterns[i].MaxRange * settings.BehaiviorPatterns[i].MaxRange) &&
					settings.BehaiviorPatterns[i].TargetSpell.Cost.CanActivate(this, 1, 1, 1))
				{
					possiblePatterns.Add(i);
				}
			}
			if(possiblePatterns.Count > 0)
			{
				int targetPatternIndex = possiblePatterns[Random.Range(0, possiblePatterns.Count)];
				CastSpell(settings.BehaiviorPatterns[targetPatternIndex].TargetSpell);
			}
		}
		private void ReleaseBoundEffects()
		{
			if (panicModeBoundFX == null)
				return;

			for(int i = 0; i < panicModeBoundFX.Length; i++)
			{
				if(panicModeBoundFX[i] != null)
					panicModeBoundFX[i].FinishFX();
			}
			panicModeBoundFX = null;
		}
	}
}