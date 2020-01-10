namespace EoE.Combatery
{
	[System.Flags] public enum SpellPart { Cast = (1 << 0), Start = (1 << 1), Projectile = (1 << 2) }
	[System.Flags] public enum SpellMovementRestrictionsMask { WhileCasting = (1 << 0), WhileShooting = (1 << 1) }
	public class Spell : CombatObject
	{
		public float SpellCooldown = 3;

		public SpellPart ContainedParts = (SpellPart)(-1);
		public SpellMovementRestrictionsMask MovementRestrictions = (SpellMovementRestrictionsMask)(-1);
		public SpellMovementRestrictionsMask RotationRestrictions = (SpellMovementRestrictionsMask)(-1);
		public SpellPart SubtractManaAtPart = SpellPart.Start;

		public SpellCastPart CastInfo = new SpellCastPart();
		public SpellBeginningPart StartInfo = new SpellBeginningPart();

		public ProjectileInfo[] ProjectileInfos = new ProjectileInfo[0];

		public bool HasSpellPart(SpellPart part)
		{
			return ((int)ContainedParts | (int)part) == ((int)ContainedParts);
		}
	}

	[System.Serializable]
	public class SpellCastPart
	{
		public float Duration = 3;
		public CustomFXObject[] VisualEffects = new CustomFXObject[0];
		public EffectAOE[] StartEffects = new EffectAOE[0];
		public EffectAOE[] WhileEffects = new EffectAOE[0];
		public float WhileTickTime = 0.1f;
	}
	[System.Serializable]
	public class SpellBeginningPart
	{
		public CustomFXObject[] VisualEffects = new CustomFXObject[0];
		public EffectAOE[] Effects = new EffectAOE[0];
	}
}