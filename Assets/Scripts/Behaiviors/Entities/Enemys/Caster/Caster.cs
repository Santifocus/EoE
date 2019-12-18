using EoE.Information;

namespace EoE.Entities
{
	public class Caster : Enemy
	{
		public override EnemySettings enemySettings => settings;
		public CasterSettings settings;
		protected override void InRangeBehaivior()
		{
			LookAtPlayer();
			CastSpell(settings.CasterAttack);
		}
	}
}