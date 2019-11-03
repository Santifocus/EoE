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
		private static bool BaseIncrementsOpen;
		private static bool SkillPointIncrementOpen;
		private int TestInput;
		private string TestOutput;
		private bool needsToCalculateTestValue = true;
		protected override void CustomInspector() 
		{
			LevelingCurveSettingsArea();
			IncrementValues();
		}

		private void LevelingCurveSettingsArea()
		{
			LevelingSettings settings = target as LevelingSettings;

			FoldoutHeader("Curve Settings", ref LevelingCurveOpen);
			if (LevelingCurveOpen)
			{
				Header("Curve: (x^2 * a) + (x * b) + (c)", 1, false);
				if (IntField(new GUIContent("Test Level", "Input a level to find out what amount of souls is required to reach it."), ref TestInput, 1) || needsToCalculateTestValue)
				{
					needsToCalculateTestValue = false;
					TestOutput = settings.curve.GetRequiredSouls(TestInput).ToString();
				}
				Header("Required Souls: " + TestOutput, 1, false, false);

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

		private void IncrementValues()
		{
			LevelingSettings settings = target as LevelingSettings;

			int incrementingStat = 5;
			Header("Leveling Increments");
			if (settings.baseIncrementPerLevel == null || settings.baseIncrementPerLevel.Length != incrementingStat)
			{
				settings.baseIncrementPerLevel = new float[incrementingStat];
			}
			if (settings.incrementsForSkillpoint == null || settings.incrementsForSkillpoint.Length != incrementingStat)
			{
				settings.incrementsForSkillpoint = new float[incrementingStat];
			}

			FoldoutHeader(new GUIContent("Base Increments", "For every level the player increases his Stats by these amounts."), ref BaseIncrementsOpen);
			if (BaseIncrementsOpen)
			{
				for (int i = 0; i < incrementingStat; i++)
				{
					FloatField(((TargetStat)i).ToString(), ref settings.baseIncrementPerLevel[i], 1);
				}
			}
			EndFoldoutHeader();

			FoldoutHeader(new GUIContent("SkillPoint Increments", "When the player applies a Skillpoint to a stat he gains the listed amount."), ref SkillPointIncrementOpen);
			if (SkillPointIncrementOpen)
			{
				for (int i = 0; i < incrementingStat; i++)
				{
					FloatField(((TargetStat)i).ToString(), ref settings.incrementsForSkillpoint[i], 1);
				}
			}
			EndFoldoutHeader();
		}
	}
}