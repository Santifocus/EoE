using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(EntitySettings), true), CanEditMultipleObjects]
	public class EntitySettingsEditor : ScriptableObjectEditor
	{
		protected static bool BaseSettingsOpen;
		protected static bool CombatSettingsOpen;
		protected static bool StatSettingsOpen;
		protected static bool MovementSettingsOpen;
		protected static bool EffectSettingsOpen;
		protected override void CustomInspector()
		{
			DrawInFoldoutHeader("Base Settings", ref BaseSettingsOpen, BaseSettingsArea);
			DrawInFoldoutHeader("Combat Settings", ref CombatSettingsOpen, CombatSettings);
			DrawInFoldoutHeader("Stat Settings", ref StatSettingsOpen, StatSettingsArea);
			DrawInFoldoutHeader("Movement Settings", ref MovementSettingsOpen, MovementSettingsArea);
		}

		protected virtual void BaseSettingsArea()
		{
			EntitySettings settings = target as EntitySettings;

			if (FloatField(new GUIContent("Entitie Mass", "What is the mass of this Entitie? This will be used for Knockback,- and Acceleration - calculations."), ref settings.EntitieMass, 1) && settings.EntitieMass < EntitySettings.MIN_ENTITIE_MASS)
			{
				settings.EntitieMass = EntitySettings.MIN_ENTITIE_MASS;
			}
			Vector3Field(new GUIContent("Mass Center", "Where is the Mass center of this Entitie? Based on this the Knockback direction will be calculated."), ref settings.MassCenter, 1);
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ShowEntitieLevel))), ref settings.ShowEntitieLevel, 1);
			EnumField(new GUIContent("Entitie Element", "Which element is this Entitie? (Will be used for damage calculations)"), ref settings.EntitieElement, 1);

			if (!(target is PlayerSettings))
			{
				ObjectField(new GUIContent("Possible Drops", "When the Entitie dies, this DropTable will be used for calculation of what to drop. If there is no DropTable nothing will be dropped."), ref settings.PossibleDropsTable, 1);
				IntField(new GUIContent("Experience Worth", "How many Experience should this Entitie dsrop on death?"), ref settings.ExperienceWorth, 1);

				LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
				DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DeathEffects))), ref settings.DeathEffects, serializedObject.FindProperty(nameof(settings.DeathEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			}
		}

		protected virtual void CombatSettings()
		{
			EntitySettings settings = target as EntitySettings;

			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BasePhysicalDamage))), ref settings.BasePhysicalDamage, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseMagicalDamage))), ref settings.BaseMagicalDamage, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseDefense))), ref settings.BaseDefense, 1);
		}

		protected virtual void StatSettingsArea()
		{
			EntitySettings settings = target as EntitySettings;
			ObjectField(new GUIContent("Level Settings", "The Level settings that should be applied to the Entitie."), ref settings.LevelSettings, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			//Health
			FloatField(new GUIContent("Health", "What base Health does the Entitie have?"), ref settings.Health, 1);
			BoolField(new GUIContent("Do Health Regen", "Should this Entitie Regen over time?"), ref settings.DoHealthRegen, 1);
			if (settings.DoHealthRegen)
			{
				FloatField(new GUIContent("Health Regen Amount", "How much Health should this Entitie regenerate? (Per Second)"), ref settings.HealthRegen, 2);
				FloatField(new GUIContent("Health Regen Cooldown After Taking Damage", "After the Entitie took damage how long should health regen be stopped? (Per Second)"), ref settings.HealthRegenCooldownAfterTakingDamage, 2);
				FloatField(new GUIContent("Health Regen In Combat Factor", "In combat regeneration will be multiplied by this amount."), ref settings.HealthRegenInCombatMultiplier, 2);

				GUILayout.Space(4);
				ObjectField(new GUIContent("Health Regen Particles", "The particles that will be played while this Entitie regens health."), ref settings.HealthRegenParticles, 2);
				Vector3Field(new GUIContent("Health Regen Particles Offset", "The regen particles will be displayed with this offset to the Entitie."), ref settings.HealthRegenParticlesOffset, 2);
			}

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			//Mana
			FloatField(new GUIContent("Mana", "What base Mana does the Entitie have."), ref settings.Mana, 1);
			BoolField(new GUIContent("Do Mana Regen", "Should this Entitie Regen over time?"), ref settings.DoManaRegen, 1);
			if (settings.DoManaRegen)
			{
				FloatField(new GUIContent("Mana Regen Amount", "How much Mana should this Entitie regenerate? (Per Second)"), ref settings.ManaRegen, 2);
				FloatField(new GUIContent("Mana Regen In Combat Factor", "In combat regeneration will be multiplied by this amount."), ref settings.ManaRegenInCombatMultiplier, 2);
			}
		}

		protected virtual void MovementSettingsArea()
		{
			EntitySettings settings = target as EntitySettings;

			FloatField(new GUIContent("Move Speed", "What is the maximum speed at which this Entitie walks?"), ref settings.WalkSpeed, 1);
			FloatField(new GUIContent("Run Speed Multiplicator", "Multilpies the walkspeed when running."), ref settings.RunSpeedMultiplicator, 1);
			FloatField(new GUIContent("Turn Speed", "How fast does the Entitie to the target direction in degree? More == Faster"), ref settings.TurnSpeed, 1);
			FloatField(new GUIContent("In Air Turn Speed Multiplier", "When in air, by what amount should the Turnspeed be multiplied by?"), ref settings.InAirTurnSpeedMultiplier, 1);

			GUILayout.Space(4);
			FloatField(new GUIContent("Move Acceleration", "How fast can this Entitie get to its maximum speed? (Approximatly in Seconds)"), ref settings.MoveAcceleration, 1);
			FloatField(new GUIContent("No Move Deceleration", "How fast does this Entitie Decelerate? (In Seconds)"), ref settings.NoMoveDeceleration, 1);
			FloatField(new GUIContent("In Air Acceleration Multiplier", "When in air, by what amount should (A/De)-cceleration be multiplied by?"), ref settings.InAirAccelerationMultiplier, 1);
		}
	}
}