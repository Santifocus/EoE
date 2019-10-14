using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EoE.Information
{
	public class GameSettings : ScriptableObject
	{
		[HideInInspector] public float SecondsPerEntititeRegen;
		[HideInInspector] public AnimationCurve TurnSpeedCurve = new AnimationCurve();

		[HideInInspector] public float WhenFallingExtraVelocity = 0.5f;
		[HideInInspector] public AnimationCurve FallDamageCurve = new AnimationCurve();

		//Damage Numbers
		[HideInInspector] public float DamageNumberLifeTime = 1;
		[HideInInspector] public bool ShowRegenNumbers = true;
		[HideInInspector] public float DamageNumberFlySpeed = 1;
		[HideInInspector] public float DamageNumberRandomMovementPower = 1;
		[HideInInspector] public Gradient PhysicalDamageColors = new Gradient();
		[HideInInspector] public Gradient MagicalDamageColors = new Gradient();
		[HideInInspector] public Gradient HealColors = new Gradient();

		[HideInInspector] public ElementEffectivenessRow[] EffectivenessMatrix;

		public float GetEffectiveness(ElementType inflicter, ElementType receiver)
		{
			return EffectivenessMatrix[(int)inflicter][(int)receiver];
		}
		[System.Serializable]
		public class ElementEffectivenessRow
		{
			public float this[int index] { get => Row[index]; set => Row[index] = value; }
			public float[] Row;
			public ElementEffectivenessRow()
			{
				int elementCount = System.Enum.GetNames(typeof(ElementType)).Length;
				Row = new float[elementCount];
				for(int i = 0; i < elementCount; i++)
				{
					Row[i] = 1;
				}
			}
		}


#if UNITY_EDITOR
		[CustomEditor(typeof(GameSettings)), CanEditMultipleObjects]
		protected class GameSettingsEditor : Editor
		{
			private bool isDirty = false;
			public override void OnInspectorGUI()
			{
				DrawDefaultInspector();
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

				Header("Basic Settings");
				FloatField(new GUIContent("Seconds Per Entitie Regen", "How many seconds for each regeneration cyle? This will not change the amount of healing only the frequency. (In Seconds)"), ref settings.SecondsPerEntititeRegen);
				AnimationCurveField(new GUIContent("Turn Speed Multiplaction", "How much speed does a Entitie have when it is fully turning vs. walking straight. 0 In the curve means the Entitie walks straight, 1 means when it is currently facing the opposite direction of where it wants to go."), ref settings.TurnSpeedCurve);
				GUILayout.Space(4);
				FloatField(new GUIContent("WhenFallingExtraVelocity", "When Entities fall then how much velocity (multiplicative) should be added to the normal gravity?"), ref settings.WhenFallingExtraVelocity);
				AnimationCurveField(new GUIContent("Fall Damage curve", "When en Entitie hits the ground how much damage should it receive based on velocity. X-Axis == FallVelocity, Y-Axis == Damage"), ref settings.FallDamageCurve);

				GUILayout.Space(8);
				Header("Damage Number Settings");
				GUILayout.Space(4);
				FloatField(new GUIContent("Damage Number Lifetime", "After a damage number spawned, how long until it disapears? (In Seconds)"), ref settings.DamageNumberLifeTime);
				BoolField(new GUIContent("Show Regen Numbers"), ref settings.ShowRegenNumbers);
				FloatField(new GUIContent("Damage Number Fly Speed"), ref settings.DamageNumberFlySpeed);
				FloatField(new GUIContent("Damage Number Random Movement Power", "Damage numbers will get a pseudo random velocity added and multiplied by this number."), ref settings.DamageNumberRandomMovementPower);

				GUILayout.Space(4);
				GradientField(new GUIContent("Physical Damage Colors"), ref settings.PhysicalDamageColors);
				GradientField(new GUIContent("Magical Damage Color"), ref settings.MagicalDamageColors);
				GradientField(new GUIContent("Heal Colors"), ref settings.HealColors);

				GUILayout.Space(8);
				Header("Element Effectiveness Settings");
				//Effectiveness matrix
				int elementCount = System.Enum.GetNames(typeof(ElementType)).Length;

				if (settings.EffectivenessMatrix == null)
				{
					settings.EffectivenessMatrix = new ElementEffectivenessRow[elementCount];
					for(int y = 0; y < elementCount; y++)
					{
						settings.EffectivenessMatrix[y] = new ElementEffectivenessRow();
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
						if(newVal != settings.EffectivenessMatrix[x][y])
						{
							settings.EffectivenessMatrix[x][y] = newVal;
							isDirty = true;
						}
					}
					GUILayout.EndHorizontal();
				}
			}

			protected void Header(string content) => Header(new GUIContent(content));
			protected void Header(GUIContent content)
			{
				GUILayout.Space(8);
				GUILayout.Label(content, EditorStyles.boldLabel);
				GUILayout.Space(4);
			}
			protected bool FloatField(string content, ref float curValue) => FloatField(new GUIContent(content), ref curValue);
			protected bool FloatField(GUIContent content, ref float curValue)
			{
				float newValue = EditorGUILayout.FloatField(content, curValue);
				if (newValue != curValue)
				{
					isDirty = true;
					curValue = newValue;
					return true;
				}
				return false;
			}
			protected bool IntField(string content, ref int curValue) => IntField(new GUIContent(content), ref curValue);
			protected bool IntField(GUIContent content, ref int curValue)
			{
				int newValue = EditorGUILayout.IntField(content, curValue);
				if (newValue != curValue)
				{
					isDirty = true;
					curValue = newValue;
					return true;
				}
				return false;
			}
			protected bool BoolField(string content, ref bool curValue) => BoolField(new GUIContent(content), ref curValue);
			protected bool BoolField(GUIContent content, ref bool curValue)
			{
				bool newValue = EditorGUILayout.Toggle(content, curValue);
				if (newValue != curValue)
				{
					isDirty = true;
					curValue = newValue;
					return true;
				}
				return false;
			}
			protected bool Vector3Field(string content, ref Vector3 curValue) => Vector3Field(new GUIContent(content), ref curValue);
			protected bool Vector3Field(GUIContent content, ref Vector3 curValue)
			{
				Vector3 newValue = EditorGUILayout.Vector3Field(content, curValue);
				if (newValue != curValue)
				{
					isDirty = true;
					curValue = newValue;
					return true;
				}
				return false;
			}
			protected bool GradientField(string content, ref Gradient curValue) => GradientField(new GUIContent(content), ref curValue);
			protected bool GradientField(GUIContent content, ref Gradient curValue)
			{
				//Create a copy so we can compare them afterwards
				Gradient newValue = new Gradient();
				newValue.colorKeys = curValue.colorKeys;
				newValue.alphaKeys = curValue.alphaKeys;

				newValue = EditorGUILayout.GradientField(content, newValue);

				if (!newValue.Equals(curValue))
				{
					isDirty = true;
					curValue = newValue;
					return true;
				}
				return false;
			}
			protected bool AnimationCurveField(string content, ref AnimationCurve curValue) => AnimationCurveField(new GUIContent(content), ref curValue);
			protected bool AnimationCurveField(GUIContent content, ref AnimationCurve curValue)
			{
				//Create a copy so we can compare them afterwards
				AnimationCurve newValue = new AnimationCurve(curValue.keys);
				newValue = EditorGUILayout.CurveField(content, newValue);

				if (!newValue.Equals(curValue))
				{
					isDirty = true;
					curValue = newValue;
					return true;
				}
				return false;
			}
		}
#endif
	}
}