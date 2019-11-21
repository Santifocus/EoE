using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(EntitieSettings), true), CanEditMultipleObjects]
	public class EntitieSettingsEditor : ObjectSettingEditor
	{
		protected static bool BaseSettingsOpen;
		protected static bool CombatSettingsOpen;
		protected static bool StatSettingsOpen;
		protected static bool MovementSettingsOpen;
		protected static bool VFXSettingsOpen;
		protected override void CustomInspector()
		{
			FoldoutHeader("Base Settings", ref BaseSettingsOpen);
			if (BaseSettingsOpen)
			{
				BaseSettingsArea();
			}
			EndFoldoutHeader();

			FoldoutHeader("Combat Settings", ref CombatSettingsOpen);
			if (CombatSettingsOpen)
			{
				CombatSettings();
			}
			EndFoldoutHeader();

			FoldoutHeader("Stat Settings", ref StatSettingsOpen);
			if (StatSettingsOpen)
			{
				StatSettingsArea();
			}
			EndFoldoutHeader();

			FoldoutHeader("Movement Settings", ref MovementSettingsOpen);
			if (MovementSettingsOpen)
			{
				MovementSettingsArea();
			}
			EndFoldoutHeader();
		}

		protected virtual void BaseSettingsArea()
		{
			EntitieSettings settings = target as EntitieSettings;

			if (FloatField(new GUIContent("Entitie Mass", "What is the mass of this Entitie? This will be used for Knockback,- and Acceleration - calculations."), ref settings.EntitieMass) && settings.EntitieMass < EntitieSettings.MIN_ENTITIE_MASS)
				settings.EntitieMass = EntitieSettings.MIN_ENTITIE_MASS;
			Vector3Field(new GUIContent("Mass Center", "Where is the Mass center of this Entitie? Based on this the Knockback direction will be calculated."), ref settings.MassCenter);
			System.Enum element = settings.EntitieElement;
			bool changed = EnumField(new GUIContent("Entitie Element", "Which element is this Entitie? (Will be used for damage calculations)"), ref element);
			if (changed)
				settings.EntitieElement = (ElementType)element;

			if (!(target is PlayerSettings))
			{
				ObjectField(new GUIContent("Possible Drops", "When the Entitie dies, this DropTable will be used for calculation of what to drop. If there is no DropTable nothing will be dropped."), ref settings.PossibleDropsTable);
				IntField(new GUIContent("Soul Worth", "How many Souls should this Entitie Drop on death?"), ref settings.SoulWorth);
			}
		}

		protected virtual void CombatSettings()
		{
			EntitieSettings settings = target as EntitieSettings;

			FloatField(new GUIContent("Base Attack Damage", "This value will be used for Physical damage calculations. (Against target)"), ref settings.BaseAttackDamage);
			FloatField(new GUIContent("Base Magic Damage", "This value will be used for Magic damage calculations. (Against target)"), ref settings.BaseMagicDamage);
			FloatField(new GUIContent("Base Defense", "This value will be used for Physical damage calculations. (On self)"), ref settings.BaseDefense);
		}

		protected virtual void StatSettingsArea()
		{
			EntitieSettings settings = target as EntitieSettings;

			//Health
			FloatField(new GUIContent("Start Health", "With what health does this Entitie Spawn?"), ref settings.Health);
			BoolField(new GUIContent("Do Health Regen", "Should this Entitie Regen over time?"), ref settings.DoHealthRegen);
			if (settings.DoHealthRegen)
			{
				FloatField(new GUIContent("Health Regen Amount", "How much Health should this Entitie regenerate? (Per Second)"), ref settings.HealthRegen, 1);
				FloatField(new GUIContent("Health Regen In Combat Factor", "In combat regeneration will be multiplied by this amount."), ref settings.HealthRegenInCombatMultiplier, 1);

				ObjectField(new GUIContent("Health Regen Particles", "The particles that will be played while this Entitie regens health."), ref settings.HealthRegenParticles);
				Vector3Field(new GUIContent("Health Regen Particles Offset", "The regen particles will be displayed with this offset to the Entitie."), ref settings.HealthRegenParticlesOffset);
			}

			//Mana
			GUILayout.Space(5);
			FloatField(new GUIContent("Mana", "Magic uses Mana. When a Entitie is out of Mana he cant use Magic anymore."), ref settings.Mana);
			BoolField(new GUIContent("Do Mana Regen", "Should this Entitie Regen over time?"), ref settings.DoManaRegen);
			if (settings.DoManaRegen)
			{
				FloatField(new GUIContent("Mana Regen Amount", "How much Mana should this Entitie regenerate? (Per Second)"), ref settings.ManaRegen, 1);
				FloatField(new GUIContent("Mana Regen In Combat Factor", "In combat regeneration will be multiplied by this amount."), ref settings.ManaRegenInCombatMultiplier, 1);
			}
		}

		protected virtual void MovementSettingsArea()
		{
			EntitieSettings settings = target as EntitieSettings;

			FloatField(new GUIContent("Move Speed", "What is the maximum speed at which this Entitie walks?"), ref settings.WalkSpeed);
			FloatField(new GUIContent("Run Speed Multiplicator", "Multilpies the walkspeed when running."), ref settings.RunSpeedMultiplicator);
			FloatField(new GUIContent("Turn Speed", "How fast does the Entitie to the target direction in degree? More == Faster"), ref settings.TurnSpeed);
			FloatField(new GUIContent("In Air Turn Speed Multiplier", "When in air, by what amount should the Turnspeed be multiplied by?"), ref settings.InAirTurnSpeedMultiplier);

			GUILayout.Space(4);
			FloatField(new GUIContent("Move Acceleration", "How fast can this Entitie get to its maximum speed? (Approximatly in Seconds)"), ref settings.MoveAcceleration);
			FloatField(new GUIContent("No Move Deceleration", "How fast does this Entitie Decelerate? (In Seconds)"), ref settings.NoMoveDeceleration);
			FloatField(new GUIContent("In Air Acceleration Multiplier", "When in air, by what amount should (A/De)-cceleration be multiplied by?"), ref settings.InAirAccelerationMultiplier);
		}
	}
}