using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(LevelingSettings), true), CanEditMultipleObjects]
	public class LevelingSettingsEditor : ObjectSettingEditor
	{
		[MenuItem("EoE/LevelingSettings")]
		public static void CreateLevelingSettings()
		{
			LevelingSettings asset = CreateInstance<LevelingSettings>();

			AssetDatabase.CreateAsset(asset, "Assets/Settings/New LevelingSettings.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
			Debug.Log("Created: 'New LevelingSettings' at: Assets/Settings/...");
		}

		private static bool LevelingCurveOpen;
		private static bool SkillPointIncrementOpen;
		private static bool RotationIncreaseSettings;
		private int testInput;
		private string testOutput;
		private bool needsToCalculateTestValue = true;
		protected override void CustomInspector() 
		{
			LevelingCurveSettingsArea();
			RotationIncrements();
			IncrementValues();
		}

		private void LevelingCurveSettingsArea()
		{
			LevelingSettings settings = target as LevelingSettings;

			FoldoutHeader("Curve Settings", ref LevelingCurveOpen);
			if (LevelingCurveOpen)
			{
				Header("Curve: (x^2 * a) + (x * b) + (c)", 1, false);
				if (IntField(new GUIContent("Test Level", "Input a level to find out what amount of souls is required to reach it."), ref testInput, 1) || needsToCalculateTestValue)
				{
					needsToCalculateTestValue = false;
					testOutput = settings.curve.GetRequiredSouls(testInput).ToString();
				}
				Header("Required Souls: " + testOutput, 1, false, false);

				LineBreak();
				bool changed = false;
				changed |= DoubleField(new GUIContent("A", "Value of a in (x^2) * a."), ref settings.curve.a, 1);
				changed |= DoubleField(new GUIContent("B", "Value of b in x * b."), ref settings.curve.b, 1);
				changed |= DoubleField(new GUIContent("C", "Value of c."), ref settings.curve.c, 1);
				Header("Souls(x) = (x^2 * " + settings.curve.a + ") + (x * " + settings.curve.b + ") + (" + settings.curve.c + ")", 1, false);
				if (changed)
					needsToCalculateTestValue = true;

				LineBreak();
				if (GUILayout.Button("Visualize"))
				{
					string baseURL = "https://www.wolframalpha.com/input/?i=plot+";
					string aPart = "x%5E2" + "*" + settings.curve.a + "%2B";
					string bPart = "x" + "*" + settings.curve.b + "%2B";
					string cPart = settings.curve.c.ToString();
					Application.OpenURL(baseURL + aPart + bPart + cPart);
				}
			}
			EndFoldoutHeader();
		}

		private void RotationIncrements()
		{
			LevelingSettings settings = target as LevelingSettings;

			FoldoutHeader(new GUIContent("Rotation Increase Settings"), ref RotationIncreaseSettings);
			if (RotationIncreaseSettings)
			{
				FloatField(new GUIContent("Per Ten Levels Base Points", "This is the base amount of for the stat increases multiplied for every 10 levels. (0-9 = x1, 10 - 19 = x2 ...)"), ref settings.PerTenLevelsBasePoints);
				FloatField(new GUIContent("Rotation Extra Points"), ref settings.RotationExtraPoints);
			}
			EndFoldoutHeader();
		}

		private void IncrementValues()
		{
			LevelingSettings settings = target as LevelingSettings;

			FoldoutHeader(new GUIContent("Skill Point Settings"), ref SkillPointIncrementOpen);
			if (SkillPointIncrementOpen)
			{
				IntField(new GUIContent("Base Skill Points Per Level", "When leveling up how many skillpoints does the player gain that he can use to improve Health/Mana/Endurance?"), ref settings.BaseSkillPointsPerLevel);
				FloatField(new GUIContent("Health Per Skill Point"), ref settings.HealthPerSkillPoint);
				FloatField(new GUIContent("Mana Per Skill Point"), ref settings.ManaPerSkillPoint);
				FloatField(new GUIContent("Endurance Per Skill Point"), ref settings.EndurancePerSkillPoint);

				LineBreak();
				IntField(new GUIContent("Extra Skill Points Per Level", "When leveling up how many skillpoints does the player gain that he can use to improve his PhysicalDamage/MagicDamage/Defense?"), ref settings.ExtraSkillPointsPerLevel);

				FloatField(new GUIContent("Physical Damage Per Skill Point"), ref settings.PhysicalDamagePerSkillPoint);
				FloatField(new GUIContent("Magic Damage Per Skill Point"), ref settings.MagicDamagePerSkillPoint);
				FloatField(new GUIContent("Defense Per Skill Point"), ref settings.DefensePerSkillPoint);
			}
			EndFoldoutHeader();
		}
	}
}