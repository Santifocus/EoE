using EoE.Information;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Entities
{
	public class Caster : Enemy
	{
		public override EnemySettings enemySettings => settings;
		public CasterSettings settings;
		protected override void InRangeBehaivior()
		{
			LookAtTarget();
			DecideOnbehaiviorPattern();
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
	}
}