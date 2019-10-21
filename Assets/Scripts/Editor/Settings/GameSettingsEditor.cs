using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace EoE.Information
{
	[CustomEditor(typeof(GameSettings)), CanEditMultipleObjects]
	public class GameSettingsEditor : Editor
	{
		private bool isDirty = false;
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
			GameSettings settings = target as GameSettings;

			EoEEditor.Header("Basic Settings");
			EoEEditor.FloatField(new GUIContent("Seconds Per Entitie Regen", "How many seconds for each regeneration cyle? This will not change the amount of healing only the frequency. (In Seconds)"), ref settings.SecondsPerEntititeRegen);
			EoEEditor.AnimationCurveField(new GUIContent("Turn Speed Multiplaction", "How much speed does a Entitie have when it is fully turning vs. walking straight. 0 In the curve means the Entitie walks straight, 1 means when it is currently facing the opposite direction of where it wants to go."), ref settings.TurnSpeedCurve);
			GUILayout.Space(4);
			EoEEditor.FloatField(new GUIContent("When Falling Extra Velocity", "When Entities fall then how much velocity (multiplicative) should be added to the normal gravity?"), ref settings.WhenFallingExtraVelocity);
			EoEEditor.AnimationCurveField(new GUIContent("Fall Damage curve", "When en Entitie hits the ground how much damage should it receive based on velocity. X-Axis == FallVelocity, Y-Axis == Damage"), ref settings.FallDamageCurve);
			EoEEditor.FloatField(new GUIContent("Ground Hit Velocity Loss", "When an Entitie hits the ground "), ref settings.GroundHitVelocityLoss);
			EoEEditor.FloatField(new GUIContent("Enemy Wandering Urgency", "When a enemy wanders around, at how much of the max speed should the entitie try to reach the wandering point? (0 == None, 0.5 == Half, 1 == Max)"), ref settings.EnemyWanderingUrgency);

			EoEEditor.Header("Damage Number Settings");
			EoEEditor.FloatField(new GUIContent("Damage Number Lifetime", "After a damage number spawned, how long until it disapears? (In Seconds)"), ref settings.DamageNumberLifeTime);
			EoEEditor.BoolField(new GUIContent("Show Regen Numbers"), ref settings.ShowRegenNumbers);
			EoEEditor.FloatField(new GUIContent("Damage Number Fly Speed"), ref settings.DamageNumberFlySpeed);
			EoEEditor.FloatField(new GUIContent("Damage Number Random Movement Power", "Damage numbers will get a pseudo random velocity added and multiplied by this number."), ref settings.DamageNumberRandomMovementPower);

			GUILayout.Space(4);
			EoEEditor.GradientField(new GUIContent("Physical Damage Colors"), ref settings.PhysicalDamageColors);
			EoEEditor.GradientField(new GUIContent("Magical Damage Color"), ref settings.MagicalDamageColors);
			EoEEditor.GradientField(new GUIContent("Heal Colors"), ref settings.HealColors);
			EoEEditor.GradientField(new GUIContent("Standard Text Color"), ref settings.StandardTextColor);

			EoEEditor.Header("Element Effectiveness Settings");
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
				for (int y = 0; y < elementCount; y++)
				{
					float newVal = EditorGUILayout.FloatField(settings.EffectivenessMatrix[x][y], GUILayout.Width(labelWidht));
					if (newVal != settings.EffectivenessMatrix[x][y])
					{
						settings.EffectivenessMatrix[x][y] = newVal;
						isDirty = true;
					}
				}
				GUILayout.EndHorizontal();
			}
		}
	}
}