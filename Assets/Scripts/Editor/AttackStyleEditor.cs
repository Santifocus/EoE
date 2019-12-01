using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EoE.Weapons
{
	[CustomEditor(typeof(AttackStyle), true), CanEditMultipleObjects]
	public class AttackStyleEditor : Editor
	{
		private const string ATTACK_COMBO_DELAY_DISCR = "What is the max delay (In Seconds) before the attack combo will be canceled? (Time begins after animation of previous attack ended)";
		private bool atLeastOneIncompleteCombo;

		private int drawnAttacksCombos;
		private static bool[] foldOutStates;

		private int innerDrawnAttacks;
		private List<bool> innerFoldoutStates;

		private List<string> failedCombos;

		private bool illegalAndOrAnimationConditions;
		private bool illegalAnimationHaltCondition;

		public override void OnInspectorGUI()
		{
			AttackStyle t = target as AttackStyle;
			CustomInspector(t);
			if (EoEEditor.isDirty)
			{
				EoEEditor.isDirty = false;
				EditorUtility.SetDirty(t);
			}
		}

		private void CustomInspector(AttackStyle target)
		{
			if (foldOutStates == null)
				foldOutStates = new bool[8];
			if (innerFoldoutStates == null)
			{
				int? innerFoldouts = target.standAttack?.attacks?.Length +
										target.sprintAttack.attacks?.Length +
										target.jumpAttack?.attacks?.Length +
										target.sprintJumpAttack.attacks?.Length +
										target.standHeavyAttack.attacks?.Length +
										target.sprintHeavyAttack.attacks?.Length +
										target.jumpHeavyAttack.attacks?.Length +
										target.sprintJumpHeavyAttack.attacks?.Length;

				innerFoldoutStates = new List<bool>(innerFoldouts.HasValue ? innerFoldouts.Value : 8);
				for (int i = 0; i < (innerFoldouts.HasValue ? innerFoldouts.Value : 0); i++)
				{
					innerFoldoutStates.Add(false);
				}
			}

			atLeastOneIncompleteCombo = false;
			drawnAttacksCombos = 0;
			innerDrawnAttacks = 0;
			failedCombos = new List<string>();
			illegalAnimationHaltCondition = false;
			illegalAndOrAnimationConditions = false;

			EditorGUILayout.HelpBox("It is recommended to read the tooltips of settings. (Hover over Setting name)", MessageType.None);

			EoEEditor.Header("Normal Attacks");
			CreateAttackInput(new GUIContent("Stand Attack", "Start this attack when: Standing and attacking."), ref target.standAttack);
			CreateAttackInput(new GUIContent("Sprint Attack", "Start this attack when: Running and attacking."), ref target.sprintAttack);
			CreateAttackInput(new GUIContent("Jump Attack", "Start this attack when: In Air and attacking."), ref target.jumpAttack);
			CreateAttackInput(new GUIContent("Sprint Jump Attack", "Start this attack when: In Air, Running and attacking."), ref target.sprintJumpAttack);

			EoEEditor.Header("Heavy Attacks");
			CreateAttackInput(new GUIContent("Heavy Stand Attack", "Start this attack when: Standing and Heavy-attacking."), ref target.standHeavyAttack);
			CreateAttackInput(new GUIContent("Heavy Sprint Attack", "Start this attack when: Running and Heavy-attacking."), ref target.sprintHeavyAttack);
			CreateAttackInput(new GUIContent("Heavy Jump Attack", "Start this attack when: In Air and Heavy-attacking."), ref target.jumpHeavyAttack);
			CreateAttackInput(new GUIContent("Heavy Sprint Jump Attack", "Start this attack when: In Air, Running and Heavy-attacking."), ref target.sprintJumpHeavyAttack);

			if (atLeastOneIncompleteCombo)
			{
				GUILayout.Space(10);
				if (failedCombos.Count == 1)
				{
					EditorGUILayout.HelpBox(failedCombos[0] + " is a Attack-combo, but has a disabled attack, this is not prohibited.", MessageType.Error);
				}
				else
				{
					string sources = "";
					for (int i = 0; i < failedCombos.Count; i++)
					{
						if (i < failedCombos.Count - 1)
						{
							sources += failedCombos[i] + ((i < failedCombos.Count - 2) ? ", " : " ");
						}
						else //i == failedCombos.Count - 1
						{
							sources += "and " + failedCombos[i];
						}
					}
					EditorGUILayout.HelpBox(sources + " are Attack-combos, but have a disabled attack, this is not prohibited.", MessageType.Error);
				}
			}
			if (illegalAnimationHaltCondition)
			{
				EditorGUILayout.HelpBox("If 'Halt Animation Till Cancel' is enabled then there MUST be a condition under which the animation can be canceled!", MessageType.Error);
			}
			if (illegalAndOrAnimationConditions)
			{
				EditorGUILayout.HelpBox("Cannot have a AND condition if one or both required state(s) = IGNORE", MessageType.Error);
			}
		}

		private void CreateAttackInput(GUIContent content, ref AttackCombo curVal)
		{
			if (curVal == null)
				curVal = new AttackCombo();
			if (curVal.attacks == null)
			{
				curVal.attacks = new Attack[1];
				curVal.delays = new float[0];

				if (innerFoldoutStates.Count < innerDrawnAttacks)
					innerFoldoutStates.Add(false);
				else
					innerFoldoutStates.Insert(innerDrawnAttacks, false);
			}

			foldOutStates[drawnAttacksCombos] = EditorGUILayout.BeginFoldoutHeaderGroup(foldOutStates[drawnAttacksCombos], content);
			if (foldOutStates[drawnAttacksCombos])
			{
				int curArraySize = curVal.attacks.Length;
				for (int i = 0; i < curArraySize; i++)
				{
					AttackField(curArraySize == 1 ? "Attack" : (i == 0 ? "Start Attack" : i + ". Combo"), ref curVal.attacks[i], 1);

					EoEEditor.LineBreak();
					if (i < curVal.attacks.Length - 1)
					{
						GUILayout.Space(8);
						EoEEditor.FloatField(new GUIContent("Max Delay to " + (i + 1) + ". combo.", ATTACK_COMBO_DELAY_DISCR), ref curVal.delays[i]);
						GUILayout.Space(8);
					}
				}
				GUILayout.Space(5);
				EditorGUILayout.BeginHorizontal();
				if (curArraySize < 1 || GUILayout.Button("+"))
				{
					int newArraySize = curArraySize + 1;
					Attack[] newAttackArray = new Attack[newArraySize];
					float[] newDelayArray = new float[newArraySize - 1];

					if (innerFoldoutStates.Count < innerDrawnAttacks + newArraySize)
						innerFoldoutStates.Add(false);
					else
						innerFoldoutStates.Insert(innerDrawnAttacks + curArraySize, false);

					for (int i = 0; i < curArraySize; i++)
					{
						newAttackArray[i] = curVal.attacks[i];
					}
					curVal.attacks = newAttackArray;

					for (int i = 0; i < curArraySize - 1; i++)
					{
						newDelayArray[i] = curVal.delays[i];
					}
					curVal.delays = newDelayArray;
				}

				EditorGUI.BeginDisabledGroup(curArraySize <= 1);
				if (GUILayout.Button("-"))
				{
					int newArraySize = curArraySize - 1;
					Attack[] newAttackArray = new Attack[newArraySize];
					float[] newDelayArray = new float[newArraySize - 1];

					innerFoldoutStates.RemoveAt(innerDrawnAttacks + newArraySize);

					for (int i = 0; i < newArraySize; i++)
					{
						newAttackArray[i] = curVal.attacks[i];
					}
					curVal.attacks = newAttackArray;

					for (int i = 0; i < newArraySize - 1; i++)
					{
						newDelayArray[i] = curVal.delays[i];
					}
					curVal.delays = newDelayArray;
				}
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(15);
			}
			else
				innerDrawnAttacks += curVal.attacks.Length;

			EditorGUILayout.EndFoldoutHeaderGroup();
			drawnAttacksCombos++;

			//Check if attack combo is breaking any rules
			if (curVal.attacks.Length > 1)
			{
				for (int i = 0; i < curVal.attacks.Length; i++)
				{
					if (curVal.attacks[i] != null && !curVal.attacks[i].enabled)
					{
						failedCombos.Add(content.text);
						atLeastOneIncompleteCombo = true;
						break;
					}
				}
			}
		}
		private bool AttackField(string content, ref Attack curValue, int offSet = 0) => AttackField(new GUIContent(content), ref curValue, offSet);
		private bool AttackField(GUIContent content, ref Attack curValue, int offSet = 0)
		{
			if (curValue == null)
				curValue = new Attack();

			bool changed = false;
			bool state = innerFoldoutStates[innerDrawnAttacks];
			EoEEditor.Foldout(content, ref state, offSet);
			innerFoldoutStates[innerDrawnAttacks] = state;

			if (innerFoldoutStates[innerDrawnAttacks])
			{
				EoEEditor.BoolField(new GUIContent("Enabled", "If disabled this Attack will NEVER be executed."), ref curValue.enabled, offSet);
				if (curValue.enabled)
				{

					changed |= AttackInfoField(new GUIContent("Attack Stats", "Settings for this attack / combo-part."), ref curValue.info, offSet + 1);
					changed |= AttackAnimationField(new GUIContent("Animation Conditions", "Under which conditions should this attack be canceled / Continued."), ref curValue.animationInfo, offSet + 1);
					changed |= AttackVelocityEffectField(new GUIContent("Attack Velocity Effect", "When this attack is used what forces should be applied to the user?"), ref curValue.velocityEffect, offSet + 1);
				}
			}
			innerDrawnAttacks++;
			return changed;
		}

		protected bool AttackInfoField(string content, ref AttackInfo curValue, int offSet = 0) => AttackInfoField(new GUIContent(content), ref curValue, offSet);
		protected bool AttackInfoField(GUIContent content, ref AttackInfo curValue, int offSet = 0)
		{
			bool changed = false;
			EoEEditor.Header(content, offSet);

			changed |= EoEEditor.FloatField(new GUIContent("Damage Mutliplier", "Multiplies the base damage of the weapon by this amount."), ref curValue.damageMutliplier, offSet);
			changed |= EoEEditor.FloatField(new GUIContent("Endurance Multiplier", "Multiplies the base endurance usage of the weapon by this amount."), ref curValue.enduranceMultiplier, offSet);
			changed |= EoEEditor.FloatField(new GUIContent("Knockback Mutliplier", "Multiplies the base knockback of the weapon by this amount."), ref curValue.knockbackMutliplier, offSet);
			return changed;
		}
		private enum AndOr { AND, OR }
		protected bool AttackAnimationField(string content, ref AttackAnimationInfo curValue, int offSet = 0) => AttackAnimationField(new GUIContent(content), ref curValue, offSet);
		protected bool AttackAnimationField(GUIContent content, ref AttackAnimationInfo curValue, int offSet = 0)
		{
			bool changed = false;
			EoEEditor.Header(content, offSet);

			System.Enum anim = curValue.animation;
			bool enumChange = EoEEditor.EnumField(new GUIContent("Animation", "The animation that should be started when this attack is executed."), ref anim, offSet);
			if (enumChange)
			{
				curValue.animation = (AttackAnimation)anim;
				changed = true;
			}

			changed |= EoEEditor.BoolField(new GUIContent("Halt Animation Till Cancel", "If enabled the animation will not be stopped untill a canceling condition is met. Use with caution!"), ref curValue.haltAnimationTillCancel, offSet);

			System.Enum cancelOnSprint = curValue.cancelWhenSprinting;
			enumChange = EoEEditor.EnumField(new GUIContent("Cancel When Sprinting = ", "If the players Sprinting state equals to this, the animation will be canceled. (Ignore = either state wont cancel)"), ref cancelOnSprint, offSet);
			if (enumChange)
			{
				curValue.cancelWhenSprinting = (AnimationCancelCondition)cancelOnSprint;
				changed = true;
			}

			System.Enum andOrState = curValue.bothStates ? AndOr.AND : AndOr.OR;
			enumChange = EoEEditor.EnumField(new GUIContent(" ", "Combine Sprint and Jump conditions logically to only cancel if both are met or either. AND = Sprinting AND Jumping condition must be met. OR = If either is met."), ref andOrState, offSet);
			if (enumChange)
			{
				curValue.bothStates = (AndOr)andOrState == AndOr.AND;
				changed = true;
			}

			System.Enum cancelOnGround = curValue.cancelWhenOnGround;
			enumChange = EoEEditor.EnumField(new GUIContent("Cancel When On ground = ", "If the players OnGround state equals to this, the animation will be canceled. (Ignore = either state wont cancel)"), ref cancelOnGround, offSet);
			if (enumChange)
			{
				curValue.cancelWhenOnGround = (AnimationCancelCondition)cancelOnGround;
				changed = true;
			}

			changed |= EoEEditor.BoolField(new GUIContent("Penetrate Entities", "When enabled the weapon can go througth Entities, if not it will stop the animation after hitting one Entitie."), ref curValue.penetrateEntities, offSet);
			changed |= EoEEditor.BoolField(new GUIContent("Penetrate Terrain", "When enabled the weapon can go througth Walls/Terrain, if not it will stop the animation after hitting one Wall/Terrain."), ref curValue.penetrateTerrain, offSet);

			illegalAnimationHaltCondition |= (curValue.haltAnimationTillCancel && curValue.cancelWhenSprinting == AnimationCancelCondition.Ignore && curValue.cancelWhenOnGround == AnimationCancelCondition.Ignore && curValue.penetrateEntities && curValue.penetrateTerrain);

			illegalAndOrAnimationConditions |= curValue.bothStates && (curValue.cancelWhenSprinting == AnimationCancelCondition.Ignore || curValue.cancelWhenOnGround == AnimationCancelCondition.Ignore);

			return changed;
		}

		protected bool AttackVelocityEffectField(string content, ref AttackVelocityEffect curValue, int offSet = 0) => AttackVelocityEffectField(new GUIContent(content), ref curValue, offSet);
		protected bool AttackVelocityEffectField(GUIContent content, ref AttackVelocityEffect curValue, int offSet = 0)
		{
			bool changed = false;

			EoEEditor.Header(content, offSet);

			System.Enum intent = curValue.velocityIntent;
			bool enumChanged = EoEEditor.EnumField(new GUIContent("Velocity Effect", "When this attack is executed: Off == No effect;  Add == Velocity of user + the given values; Set == Velocity of user will be set to the given values."), ref intent, offSet);
			if (enumChanged)
			{
				changed = true;
				curValue.velocityIntent = (AttackVelocityIntent)intent;
			}

			bool fullyOff = curValue.velocityIntent == AttackVelocityIntent.Off;
			EditorGUI.BeginDisabledGroup(fullyOff);
			changed |= EoEEditor.BoolField(new GUIContent("Ignore Vertical Velocity", "When enabled then this Velocity-Effect wont affect the vertical velocity."), ref curValue.ignoreVerticalVelocity, offSet);
			changed |= EoEEditor.BoolField(new GUIContent("Apply Force on Animation Charge End", "When enabled the delay to when the force will be applied is set by the animation itself. (For example Stab has a set delay of 0.15 seconds)"), ref curValue.applyForceAfterAnimationCharge, offSet);
			if (!fullyOff)
				EditorGUI.BeginDisabledGroup(curValue.applyForceAfterAnimationCharge);
			changed |= EoEEditor.FloatField(new GUIContent("Apply Force Delay", "Custom time of when to apply the calculated forces. (Wont work if the delay is longer then the animation itself!)"), ref curValue.applyForceDelay, offSet);
			if (!fullyOff)
				EditorGUI.EndDisabledGroup();

			changed |= VelocityDirectionField(new GUIContent("Right Effect", "When used: with which force should the user be pushed to the right? (Negative value == Left)"), ref curValue.useRightValue, ref curValue.rightValue, fullyOff, offSet);
			changed |= VelocityDirectionField(new GUIContent("Up Effect", "When used: with which force should the user be pushed upwards? (Negative value == downwards)"), ref curValue.useUpValue, ref curValue.upValue, fullyOff, offSet);
			changed |= VelocityDirectionField(new GUIContent("Forward Effect", "When used: with which force should the user be pushed forward? (Negative value == back)"), ref curValue.useForwardValue, ref curValue.forwardValue, fullyOff, offSet);

			EditorGUI.EndDisabledGroup();

			return changed;
		}
		private bool VelocityDirectionField(GUIContent content, ref bool curState, ref float curValue, bool fullyOff, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			bool changed = EoEEditor.BoolField(new GUIContent("", content.tooltip), ref curState, offSet);

			GUILayout.FlexibleSpace();
			if (!fullyOff)
				EditorGUI.BeginDisabledGroup(!curState);
			changed |= EoEEditor.FloatField(content, ref curValue);
			if (!fullyOff)
				EditorGUI.EndDisabledGroup();

			EditorGUILayout.EndHorizontal();
			return changed;
		}
	}
}