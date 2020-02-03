using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(LevelingSettings), true), CanEditMultipleObjects]
	public class LevelingSettingsEditor : ObjectEditor
	{
		private static bool LevelingCurveOpen;
		private static bool SkillPointIncrementOpen;
		private static bool IncrementValuesOpen;

		private int testInput;
		private string testOutput;
		private bool needsToCalculateTestValue = true;
		protected override void CustomInspector()
		{
			DrawInFoldoutHeader(new GUIContent("Curve Settings"), ref LevelingCurveOpen, LevelingCurveSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Rotation Increase Settings"), ref SkillPointIncrementOpen, RotationIncrementsArea);
			DrawInFoldoutHeader(new GUIContent("Skill Point Settings"), ref IncrementValuesOpen, IncrementValuesArea);
		}

		private void LevelingCurveSettingsArea()
		{
			LevelingSettings settings = target as LevelingSettings;

			Header("Curve: (x^2 * a) + (x * b) + (c)", 1, false);
			if (IntField(new GUIContent("Test Level", "Input a level to find out what amount of Experience that is required to reach it."), ref testInput, 1) || needsToCalculateTestValue)
			{
				needsToCalculateTestValue = false;
				testOutput = settings.curve.GetRequiredExperience(testInput).ToString();
			}
			Header("Required Experience: " + testOutput, 1, false, false);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			bool changed = false;
			changed |= DoubleField(new GUIContent("A", "Value of a in (x^2) * a."), ref settings.curve.a, 1);
			changed |= DoubleField(new GUIContent("B", "Value of b in x * b."), ref settings.curve.b, 1);
			changed |= DoubleField(new GUIContent("C", "Value of c."), ref settings.curve.c, 1);
			Header("Experience(x) = (x^2 * " + settings.curve.a + ") + (x * " + settings.curve.b + ") + (" + settings.curve.c + ")", 1, false);
			if (changed)
				needsToCalculateTestValue = true;

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			if (GUILayout.Button("Visualize"))
			{
				string baseURL = "https://www.wolframalpha.com/input/?i=plot+";
				string aPart = "x%5E2" + "*" + settings.curve.a + "%2B";
				string bPart = "x" + "*" + settings.curve.b + "%2B";
				string cPart = settings.curve.c.ToString();
				Application.OpenURL(baseURL + aPart + bPart + cPart);
			}
		}

		private void RotationIncrementsArea()
		{
			LevelingSettings settings = target as LevelingSettings;

			FloatField(new GUIContent("Per Ten Levels Base Points", "This is the base amount of for the stat increases multiplied for every 10 levels. (0-9 = x1, 10 - 19 = x2 ...)"), ref settings.PerTenLevelsBasePoints, 1);
			FloatField(new GUIContent("Rotation Extra Points"), ref settings.RotationExtraPoints, 1);
		}

		private void IncrementValuesArea()
		{
			LevelingSettings settings = target as LevelingSettings;

			IntField(new GUIContent("Attribute Points Per Level", "When leveling up how many skillpoints does the player gain that he can use to improve Health/Mana/Endurance?"), ref settings.AttributePointsPerLevel, 1);

			FloatField(new GUIContent("Health Per Skill Point"), ref settings.HealthPerSkillPoint, 1);
			FloatField(new GUIContent("Mana Per Skill Point"), ref settings.ManaPerSkillPoint, 1);
			FloatField(new GUIContent("Endurance Per Skill Point"), ref settings.EndurancePerSkillPoint, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			IntField(new GUIContent("Skill Points Per Level", "When leveling up how many skillpoints does the player gain that he can use to improve his PhysicalDamage/MagicDamage/Defense?"), ref settings.SkillPointsPerLevel, 1);

			FloatField(new GUIContent("Physical Damage Per Skill Point"), ref settings.PhysicalDamagePerSkillPoint, 1);
			FloatField(new GUIContent("Magic Damage Per Skill Point"), ref settings.MagicDamagePerSkillPoint, 1);
			FloatField(new GUIContent("Defense Per Skill Point"), ref settings.DefensePerSkillPoint, 1);
		}
	}
}