using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(GameSettings)), CanEditMultipleObjects]
	public class GameSettingsEditor : Editor
	{
		private static bool EntititeVelocitySettingsOpen;
		private static bool BasicBehaiviorOpen;
		private static bool VisualsSettingsOpen;
		private static bool DamageNumberSettingsOpen;
		private static bool OtherSettingsOpen;
		private static bool DamageCalculationsOpen;
		private static bool DamageCalculationValuesOpen;
		private static bool DialogueSettingsOpen;

		public override void OnInspectorGUI()
		{
			CustomInspector();
			if (isDirty)
			{
				isDirty = false;
				EditorUtility.SetDirty(target);
			}
		}
		private void CustomInspector()
		{
			FoldoutHeader("Entitie Velocity Settings", ref EntititeVelocitySettingsOpen);
			if(EntititeVelocitySettingsOpen)
				EntitieVelocityArea();
			EndFoldoutHeader();

			FoldoutHeader("Basic Behaivior Settings", ref BasicBehaiviorOpen);
			if (BasicBehaiviorOpen)
				BasicBehaiviorArea();
			EndFoldoutHeader();

			FoldoutHeader("Visual Settings", ref VisualsSettingsOpen);
			if (VisualsSettingsOpen)
				VisualsSettingsArea();
			EndFoldoutHeader();

			FoldoutHeader("Damage Number Settings", ref DamageNumberSettingsOpen);
			if (DamageNumberSettingsOpen)
				DamageNumberSettingsArea();
			EndFoldoutHeader();

			FoldoutHeader("Dialogue Settings Settings", ref DialogueSettingsOpen);
			if (DialogueSettingsOpen)
				DialogueSettingsArea();
			EndFoldoutHeader();

			FoldoutHeader("Other Settings", ref OtherSettingsOpen);
			if (OtherSettingsOpen)
				OtherSettingsArea();
			EndFoldoutHeader();

			FoldoutHeader("Damage Calculation Settings", ref DamageCalculationsOpen);
			if (DamageCalculationsOpen)
				EffectivenessMatrixArea();
			EndFoldoutHeader();
		}

		private void EntitieVelocityArea()
		{
			GameSettings settings = target as GameSettings;

			FloatField(new GUIContent("When Falling Extra Velocity", "When Entities fall then how much velocity (multiplicative) should be added to the normal gravity?"), ref settings.WhenFallingExtraVelocity);
			AnimationCurveField(new GUIContent("Fall Damage curve", "When en Entitie hits the ground how much damage should it receive based on velocity. X-Axis == FallVelocity, Y-Axis == Damage"), ref settings.FallDamageCurve);
			FloatField(new GUIContent("Ground Hit Velocity Loss", "When an Entitie hits the ground "), ref settings.GroundHitVelocityLoss);
		}

		private void BasicBehaiviorArea()
		{
			GameSettings settings = target as GameSettings;

			FloatField(new GUIContent("Enemy Wandering Urgency", "When a enemy wanders around, at how much of the max speed should the entitie try to reach the wandering point? (0 == None, 0.5 == Half, 1 == Max)"), ref settings.EnemyWanderingUrgency);
			FloatField(new GUIContent("Combat Cooldown", "After a Entitie encounters an Enemy how long does a Entitie have to be out of combat before it will be counted as 'Out of Combat'?"), ref settings.CombatCooldown);
		}

		private void VisualsSettingsArea()
		{
			GameSettings settings = target as GameSettings;

			ObjectField("Enemy Health Bar Prefab", ref settings.EntitieStatDisplayPrefab);
			FloatField(new GUIContent("Enemey Health Bar Lerp Speed", "How fast should the Healthbar value of Enemy healthbars lerp?"), ref settings.EnemeyHealthBarLerpSpeed);
			ObjectField("Hit Entitie Particles", ref settings.HitEntitieParticles);
			ObjectField("Hit Terrain Particles", ref settings.HitTerrainParticles);

		}

		private void DamageNumberSettingsArea()
		{
			GameSettings settings = target as GameSettings;

			FloatField(new GUIContent("Damage Number Lifetime", "After a damage number spawned, how long until it disapears? (In Seconds)"), ref settings.DamageNumberLifeTime);
			BoolField(new GUIContent("Show Regen Numbers"), ref settings.ShowRegenNumbers);
			FloatField(new GUIContent("Damage Number Fly Speed"), ref settings.DamageNumberFlySpeed);
			FloatField(new GUIContent("Damage Number Random Movement Power", "Damage numbers will get a pseudo random velocity added and multiplied by this number."), ref settings.DamageNumberRandomMovementPower);

			GUILayout.Space(4);
			GradientField(new GUIContent("Physical Damage Colors"), ref settings.PhysicalDamageColors);
			GradientField(new GUIContent("Magical Damage Color"), ref settings.MagicalDamageColors);
			GradientField(new GUIContent("Heal Colors"), ref settings.HealColors);
			GradientField(new GUIContent("Standard Text Color"), ref settings.StandardTextColor);
		}

		private void DialogueSettingsArea()
		{
			GameSettings settings = target as GameSettings;

			ObjectField(new GUIContent("Dialogue Box Prefab"), ref settings.DialogueBoxPrefab);
			FloatField(new GUIContent("Show Dialogue Base Delay", "When a dialogue request is sent this is the minumum delay until the DialogBox appears."), ref settings.ShowDialogueBaseDelay);
			FloatField(new GUIContent("Dialogue Delay Per Letter", "When the dialogue box displays a text every letter will take this time to show itself."), ref settings.DialogueDelayPerLetter);
			BoolField(new GUIContent("Skip Delay On Space", "Should the delay of spaces be ignored?."), ref settings.SkipDelayOnSpace);
			FloatField(new GUIContent("Delay To Next Dialogue", "After a dialogue finishes what time should be waited until it will be cleared or the next one starts to show?"), ref settings.DelayToNextDialogue);
		}

		private void OtherSettingsArea()
		{
			GameSettings settings = target as GameSettings;

			FloatField(new GUIContent("Seconds Per Entitie Regen", "How many seconds for each regeneration cyle? This will not change the amount of healing only the frequency. (In Seconds)"), ref settings.SecondsPerEntititeRegen);
			FloatField(new GUIContent("Crit Damage Multiplier", "If a ability / attack was counted as criticall for much should the damage be multiplied?"), ref settings.CritDamageMultiplier);
			ObjectField(new GUIContent("Soul Drop Prefab", "The prefab that will be spawned when an Entite dies and drops souls."), ref settings.SoulDropPrefab);
		}

		private void EffectivenessMatrixArea()
		{
			GameSettings settings = target as GameSettings;

			//Damage Calculation Values
			Foldout(new GUIContent("Damage Calculation Base Values"), ref DamageCalculationValuesOpen);
			if (DamageCalculationValuesOpen)
			{
				Header("((Level + " + settings.DamageLevelAdd + "[A]) * Damage) / " + settings.DamageDivider + "[B]", 1);
				FloatField(new GUIContent("A"), ref settings.DamageLevelAdd, 1);
				FloatField(new GUIContent("B"), ref settings.DamageDivider, 1);

				Header("((Level + " + settings.DamageLevelAdd + "[C]) * Defense) / " + settings.DamageDivider + "[D]", 1);
				FloatField(new GUIContent("C"), ref settings.DefenseLevelAdd, 1);
				FloatField(new GUIContent("D"), ref settings.DefenseLevelDivider, 1);

				GUILayout.Space(5);
			}
			//Effectiveness matrix
			int elementCount = System.Enum.GetNames(typeof(ElementType)).Length;

			if (settings.EffectivenessMatrix == null)
			{
				settings.EffectivenessMatrix = new GameSettings.ElementEffectivenessRow[elementCount];
				for (int y = 0; y < elementCount; y++)
				{
					settings.EffectivenessMatrix[y] = new GameSettings.ElementEffectivenessRow();
				}
				isDirty = true;
			}

			///Draw the effectiveness matrix
			//Element names
			float labelWidht = EditorGUIUtility.currentViewWidth / (elementCount + 2);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("", GUILayout.Width(labelWidht));
			for (int x = 0; x < elementCount; x++)
			{
				GUILayout.FlexibleSpace();
				GUILayout.Label(new GUIContent(((ElementType)x).ToString(), "RECEIVER"), GUILayout.Width(labelWidht));
			}
			GUILayout.EndHorizontal();

			//Matrix
			for (int x = 0; x < elementCount; x++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent(((ElementType)x).ToString(), "ATTACKER"), GUILayout.Width(labelWidht));
				EditorGUI.BeginDisabledGroup(x == 0);
				for (int y = 0; y < elementCount; y++)
				{
					if (y == 0)
						EditorGUI.BeginDisabledGroup(true);

					float newVal = EditorGUILayout.FloatField(settings.EffectivenessMatrix[x][y], GUILayout.Width(labelWidht));
					if (newVal != settings.EffectivenessMatrix[x][y])
					{
						settings.EffectivenessMatrix[x][y] = newVal;
						isDirty = true;
					}

					if (y == 0)
						EditorGUI.EndDisabledGroup();
				}
				GUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
			}
		}
	}
}