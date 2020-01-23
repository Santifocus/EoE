using UnityEditor;
using UnityEngine;
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
			BoolField(new GUIContent("Enable Debug"), ref (target as GameSettings).debugEnabledInternal);

			DrawInFoldoutHeader("Entitie Velocity Settings", ref EntititeVelocitySettingsOpen, EntitieVelocityArea);
			DrawInFoldoutHeader("Basic Behaivior Settings", ref BasicBehaiviorOpen, BasicBehaiviorArea);
			DrawInFoldoutHeader("Visual Settings", ref VisualsSettingsOpen, VisualsSettingsArea);
			DrawInFoldoutHeader("Damage Number Settings", ref DamageNumberSettingsOpen, DamageNumberSettingsArea);
			DrawInFoldoutHeader("Dialogue Settings Settings", ref DialogueSettingsOpen, DialogueSettingsArea);
			DrawInFoldoutHeader("Other Settings", ref OtherSettingsOpen, OtherSettingsArea);
			DrawInFoldoutHeader("Damage Calculation Settings", ref DamageCalculationsOpen, EffectivenessMatrixArea);
		}

		private void EntitieVelocityArea()
		{
			GameSettings settings = target as GameSettings;

			FloatField(new GUIContent("When Falling Extra Velocity", "When Entities fall then how much velocity (multiplicative) should be added to the normal gravity?"), ref settings.WhenFallingExtraGravity, 1);
			AnimationCurveField(new GUIContent("Fall Damage curve", "When en Entitie hits the ground how much damage should it receive based on velocity. X-Axis == FallVelocity, Y-Axis == Damage"), ref settings.FallDamageCurve, 1);
			FloatField(new GUIContent("Ground Hit Velocity Loss", "When an Entitie hits the ground "), ref settings.GroundHitVelocityLoss, 1);
			FloatField(new GUIContent("GroundHitVelocityLossMinThreshold"), ref settings.GroundHitVelocityLossMinThreshold, 1);
			FloatField(new GUIContent("GroundHitVelocityLossMaxThreshold"), ref settings.GroundHitVelocityLossMaxThreshold, 1);
		}

		private void BasicBehaiviorArea()
		{
			GameSettings settings = target as GameSettings;

			FloatField(new GUIContent("Idle Movement Urgency", "When a Enemy is idle, at how much of his max speed should the Enemy move? (0 == None, 0.5 == Half, 1 == Max)"), ref settings.IdleMovementUrgency, 1);
			FloatField(new GUIContent("Enemy Minimum Investigation Area", "After losing sight of the player, the Enemy first tries to guess where the player is for a set time, after that it will check the close area, the distance the Enemy checks either its 'WanderingFactor' or this value, whichever is bigger."), ref settings.EnemyMinimumInvestigationArea, 1);
			FloatField(new GUIContent("Combat Cooldown", "After a Entitie encounters an Enemy how long does a Entitie have to be out of combat before it will be counted as 'Out of Combat'?"), ref settings.CombatCooldown, 1);
		}

		private void VisualsSettingsArea()
		{
			GameSettings settings = target as GameSettings;

			ObjectField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EntitieStatDisplayPrefab))), ref settings.EntitieStatDisplayPrefab, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EntitieHealthBarLerpSpeed)), "How fast should the Healthbar value of Enemy healthbars lerp?"), ref settings.EntitieHealthBarLerpSpeed, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EntitieHealthBarLerpDelay))), ref settings.EntitieHealthBarLerpDelay, 1);
			ObjectField("Hit Entitie Particles", ref settings.HitEntitieParticles, 1);
			ObjectField("Hit Terrain Particles", ref settings.HitTerrainParticles, 1);
		}

		private void DamageNumberSettingsArea()
		{
			GameSettings settings = target as GameSettings;

			FloatField(new GUIContent("Damage Number Lifetime", "After a damage number spawned, how long until it disapears? (In Seconds)"), ref settings.DamageNumberLifeTime, 1);
			BoolField(new GUIContent("Show Regen Numbers"), ref settings.ShowRegenNumbers, 1);
			FloatField(new GUIContent("Damage Number Fly Speed"), ref settings.DamageNumberFlySpeed, 1);
			FloatField(new GUIContent("Damage Number Random Movement Power", "Damage numbers will get a pseudo random velocity added and multiplied by this number."), ref settings.DamageNumberRandomMovementPower, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			GradientField(new GUIContent("Physical Damage Colors"), ref settings.PhysicalDamageColors, 1);
			GradientField(new GUIContent("Magical Damage Color"), ref settings.MagicalDamageColors, 1);
			GradientField(new GUIContent("Heal Colors"), ref settings.HealColors, 1);
			GradientField(new GUIContent("Standard Text Color"), ref settings.StandardTextColor, 1);
		}

		private void DialogueSettingsArea()
		{
			GameSettings settings = target as GameSettings;

			ObjectField(new GUIContent("Dialogue Box Prefab"), ref settings.DialogueBoxPrefab, 1);
			FloatField(new GUIContent("Dialogue Delay Per Letter", "When the dialogue box displays a text every letter will take this time to show itself."), ref settings.DialogueDelayPerLetter, 1);
			BoolField(new GUIContent("Skip Delay On Space", "Should the delay of spaces be ignored?."), ref settings.SkipDelayOnSpace, 1);
			FloatField(new GUIContent("Delay To Next Dialogue", "After a dialogue finishes what time should be waited until it will be cleared or the next one starts to show?"), ref settings.DelayToNextDialogue, 1);
		}

		private void OtherSettingsArea()
		{
			GameSettings settings = target as GameSettings;

			FloatField(new GUIContent("Seconds Per Entitie Health Regen", "How many seconds for each regeneration cyle? This will not change the amount of healing only the frequency. (In Seconds)"), ref settings.SecondsPerEntitieHealthRegen, 1);
			FloatField(new GUIContent("Crit Damage Multiplier", "If a ability / attack was counted as criticall for much should the damage be multiplied?"), ref settings.CritDamageMultiplier, 1);
			FloatField(new GUIContent("Item Drop Random Velocity Strenght"), ref settings.ItemDropRandomVelocityStrenght, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			FloatSliderField(new GUIContent("Item Sell Multiplier"), ref settings.ItemSellMultiplier, 0, 1, 1);
			IntField(new GUIContent("Base Currency Per Soul"), ref settings.BaseCurrencyPerSoul, 1);
			IntField(new GUIContent("Extra Random Currency Per Soul"), ref settings.ExtraRandomCurrencyPerSoul, 1);
		}

		private void EffectivenessMatrixArea()
		{
			GameSettings settings = target as GameSettings;

			//Damage Calculation Values
			Foldout(new GUIContent("Damage Calculation Base Values"), ref DamageCalculationValuesOpen, 1);
			if (DamageCalculationValuesOpen)
			{
				Header("(({Level} + " + settings.PhysicalDamageLevelAdd + "[A]) * {Damage}) / " + settings.PhysicalDamageDivider + "[B]", 2);
				FloatField(new GUIContent("A"), ref settings.PhysicalDamageLevelAdd, 2);
				FloatField(new GUIContent("B"), ref settings.PhysicalDamageDivider, 2);

				Header("(({Level} + " + settings.PhysicalDefenseLevelAdd + "[C]) * {Defense}) / " + settings.PhysicalDefenseLevelDivider + "[D]", 2);
				FloatField(new GUIContent("C"), ref settings.PhysicalDefenseLevelAdd, 2);
				FloatField(new GUIContent("D"), ref settings.PhysicalDefenseLevelDivider, 2);

				Header("(({Level} + " + settings.MagicDamageLevelAdd + "[E]) * {Damage}) / " + settings.MagicDamageDivider + "[F]", 2);
				FloatField(new GUIContent("E"), ref settings.MagicDamageLevelAdd, 2);
				FloatField(new GUIContent("F"), ref settings.MagicDamageDivider, 2);

				LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
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
				GUILayout.Label(new GUIContent(((ElementType)(1 << x)).ToString(), "RECEIVER"), GUILayout.Width(labelWidht));
			}
			GUILayout.EndHorizontal();

			//Matrix
			for (int x = 0; x < elementCount; x++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent(((ElementType)(1 << x)).ToString(), "ATTACKER"), GUILayout.Width(labelWidht));
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