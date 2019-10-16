using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EoE.Weapons
{
	[CustomEditor(typeof(AttackStyle), true), CanEditMultipleObjects]
	public class AttackStyleEditor : Editor
	{
		private const string ENABLED_TOOLTIP = "Disabled == This attack will never be executed";
		private const float HORIZONTAL_SCROLL_PER_DEPTH = 20;

		private static bool[] foldoutStates = new bool[8];
		private int drawnAttacks = 0;
		private bool isDirty;
		private int innerDepth;

		public override void OnInspectorGUI()
		{
			AttackStyle t = target as AttackStyle;
			CustomInspector(t);
			if (isDirty)
			{
				EditorUtility.SetDirty(t);
			}
		}

		private void CustomInspector(AttackStyle target)
		{
			if (foldoutStates == null)
				foldoutStates = new bool[8];
			drawnAttacks = 0;

			Header("Normal Attacks");
			CreateAttackInput(new GUIContent("Stand Attack", "Start this attack when: Standing and attacking."), ref target.standAttack, true);
			CreateAttackInput(new GUIContent("Jump Attack", "Start this attack when: In Air and attacking."), ref target.jumpAttack, true);
			CreateAttackInput(new GUIContent("Sprint Attack", "Start this attack when: Running and attacking."), ref target.sprintAttack, true);
			CreateAttackInput(new GUIContent("Sprint Jump Attack", "Start this attack when: In Air, Running and attacking."), ref target.jumpSprintAttack, true);

			Header("Heavy Attacks");
			CreateAttackInput(new GUIContent("Heavy Stand Attack", "Start this attack when: Standing and Heavy-attacking."), ref target.standHeavyAttack, true);
			CreateAttackInput(new GUIContent("Heavy Jump Attack", "Start this attack when: In Air and Heavy-attacking."), ref target.jumpHeavyAttack, true);
			CreateAttackInput(new GUIContent("Heavy Sprint Attack", "Start this attack when: Running and Heavy-attacking."), ref target.sprintHeavyAttack, true);
			CreateAttackInput(new GUIContent("Heavy Sprint Jump Attack", "Start this attack when: In Air, Running and Heavy-attacking."), ref target.jumpSprintHeavyAttack, true);
		}
		protected void Header(string content) => Header(new GUIContent(content));
		protected void Header(GUIContent content)
		{
			GUILayout.Space(8);
			GUILayout.Label(content, EditorStyles.boldLabel);
			GUILayout.Space(4);
		}

		private void CreateAttackInput(GUIContent content, ref Attack curVal, bool topLevel)
		{
			if (curVal == null)
			{
				curVal = new Attack();
			}

			if (topLevel)
				innerDepth = 1;
			else
				innerDepth++;

			if (topLevel)
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(innerDepth * HORIZONTAL_SCROLL_PER_DEPTH);
					foldoutStates[drawnAttacks] = EditorGUILayout.Foldout(foldoutStates[drawnAttacks], content, true);
				}
				GUILayout.EndHorizontal();
			}

			if (!topLevel || foldoutStates[drawnAttacks])
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(innerDepth * HORIZONTAL_SCROLL_PER_DEPTH);
					BoolField(new GUIContent("Enabled", ENABLED_TOOLTIP), ref curVal.enabled);
				}
				GUILayout.EndHorizontal();

				if (curVal.enabled)
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(innerDepth * HORIZONTAL_SCROLL_PER_DEPTH);
						AttackAnimation newAttackAnimationEnum = (AttackAnimation)EditorGUILayout.EnumPopup("Animation", curVal.animation);

						if (newAttackAnimationEnum != curVal.animation)
						{
							isDirty = true;
							curVal.animation = newAttackAnimationEnum;
						}
					}
					GUILayout.EndHorizontal();


					AttackInfoField("Info", ref curVal.info);

					GUILayout.Space(8);
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(innerDepth * HORIZONTAL_SCROLL_PER_DEPTH);
						GUILayout.Label("Combo: " + innerDepth, EditorStyles.boldLabel);
					}
					GUILayout.EndHorizontal();
					GUILayout.Space(4);

					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(innerDepth * HORIZONTAL_SCROLL_PER_DEPTH);
						BoolField("Has Combo", ref curVal.hasCombo);
					}
					GUILayout.EndHorizontal();

					if (curVal.hasCombo)
					{
						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(innerDepth * HORIZONTAL_SCROLL_PER_DEPTH);
							FloatField(new GUIContent("Combo max Delay", "After animation of the last attack has finished how many Seconds at max can the combo be delayed"), ref curVal.comboMaxDelay);
						}
						GUILayout.EndHorizontal();

						GUIContent newContent = new GUIContent(innerDepth == 1 ? (content.text + " C.1") : content.text.Replace("C." + (innerDepth - 1), "C." + innerDepth), content.tooltip);
						CreateAttackInput(newContent, ref curVal.nextCombo, false);
					}
				}
			}

			if(topLevel)
				drawnAttacks++;
		}
		protected bool AttackInfoField(string content, ref AttackInfo curValue) => AttackInfoField(new GUIContent(content), ref curValue);
		protected bool AttackInfoField(GUIContent content, ref AttackInfo curValue)
		{
			bool changed = false;
			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(innerDepth * HORIZONTAL_SCROLL_PER_DEPTH);
				changed |= FloatField(new GUIContent("Damage Mutliplier", "Multiplies the base damage of the weapon by this amount."), ref curValue.damageMutliplier);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(innerDepth * HORIZONTAL_SCROLL_PER_DEPTH);
				changed |= FloatField(new GUIContent("Endurance Multiplier", "Multiplies the base endurance usage of the weapon by this amount."), ref curValue.enduranceMultiplier);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(innerDepth * HORIZONTAL_SCROLL_PER_DEPTH);
				changed |= FloatField(new GUIContent("Knockback Mutliplier", "Multiplies the base knockback of the weapon by this amount."), ref curValue.knockbackMutliplier);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(innerDepth * HORIZONTAL_SCROLL_PER_DEPTH);
				changed |= BoolField(new GUIContent("Penetrate Entities", "When enabled the weapon can go througth Entities, if not it will stop the animation after hitting one Entitie."), ref curValue.penetrateEntities);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(innerDepth * HORIZONTAL_SCROLL_PER_DEPTH);
				changed |= BoolField(new GUIContent("Penetrate Terrain", "When enabled the weapon can go througth Walls/Terrain, if not it will stop the animation after hitting one Wall/Terrain."), ref curValue.penetrateTerrain);
			}
			GUILayout.EndHorizontal();

			return changed;
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
	}
}