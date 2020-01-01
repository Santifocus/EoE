using EoE.Information;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Combatery
{
	[CustomEditor(typeof(Weapon), true), CanEditMultipleObjects]
	public class WeaponEditor : ObjectSettingEditor
	{
		private static bool BaseDataOpen;
		private static bool StandAttackOpen;
		private static bool RunAttackOpen;
		private static bool JumpAttackOpen;
		private static bool RunJumpAttackOpen;

		private AttackStylePart CurrentlyDrawnPart = 0;
		private int CurrentlyDrawnPartCombo = 0;
		protected override void CustomInspector()
		{
			Weapon settings = target as Weapon;
			if (!settings.WeaponPrefab)
			{
				EditorGUILayout.HelpBox("Weapon has no Weapon Prefab selected.", MessageType.Error);
			}
			DrawInFoldoutHeader("Base Data", ref BaseDataOpen, BaseDataArea);
			GUILayout.Space(1);

			if (settings.HasMaskFlag(AttackStylePart.StandAttack))
			{
				CurrentlyDrawnPart = AttackStylePart.StandAttack;
				DrawInFoldoutHeader(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StandAttackSequence))), ref StandAttackOpen, () => DrawAttackSequence(settings.StandAttackSequence, nameof(settings.StandAttackSequence)));
				GUILayout.Space(1);
			}
			if (settings.HasMaskFlag(AttackStylePart.RunAttack))
			{
				CurrentlyDrawnPart = AttackStylePart.RunAttack;
				DrawInFoldoutHeader(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.RunAttackSequence))), ref RunAttackOpen, () => DrawAttackSequence(settings.RunAttackSequence, nameof(settings.RunAttackSequence)));
				GUILayout.Space(1);
			}
			if (settings.HasMaskFlag(AttackStylePart.JumpAttack))
			{
				CurrentlyDrawnPart = AttackStylePart.JumpAttack;
				DrawInFoldoutHeader(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.JumpAttackSequence))), ref JumpAttackOpen, () => DrawAttackSequence(settings.JumpAttackSequence, nameof(settings.JumpAttackSequence)));
				GUILayout.Space(1);
			}
			if (settings.HasMaskFlag(AttackStylePart.RunJumpAttack))
			{
				CurrentlyDrawnPart = AttackStylePart.RunJumpAttack;
				DrawInFoldoutHeader(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.RunJumpAttackSequence))), ref RunJumpAttackOpen, () => DrawAttackSequence(settings.RunJumpAttackSequence, nameof(settings.RunJumpAttackSequence)));
				GUILayout.Space(1);
			}
			if (settings.HasUltimate)
			{
				SerializedProperty ultimateSettingsProperty = serializedObject.FindProperty(nameof(settings.UltimateSettings));
				FoldoutFromSerializedProperty(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.UltimateSettings))), ultimateSettingsProperty, 0, true);
				if (ultimateSettingsProperty.isExpanded)
				{
					DrawUltimateSettings();
				}
				GUILayout.Space(1);
			}

			ObjectField<ComboSet>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ComboEffects))), ref settings.ComboEffects);
		}

		private void BaseDataArea()
		{
			Weapon settings = target as Weapon;

			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseDamage))), ref settings.BaseDamage, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseManaCost))), ref settings.BaseManaCost, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseEnduranceCost))), ref settings.BaseEnduranceCost, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseKnockback))), ref settings.BaseKnockback, 1);
			SliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseCritChance))), ref settings.BaseCritChance, 0, 1, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			ObjectField<WeaponController>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WeaponPrefab))), ref settings.WeaponPrefab, 1);
			EnumField<ElementType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WeaponElement))), ref settings.WeaponElement, 1);
			EnumFlagField<CauseType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WeaponCauseType))), ref settings.WeaponCauseType, 1);
			EnumFlagField<AttackStylePart>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ContainedParts))), ref settings.ContainedParts, 1);
			EnumFlagField<AttackStylePartFallback>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.FallBackPart))), ref settings.FallBackPart, 1);
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.HasUltimate))), ref settings.HasUltimate, 1);


			if (!settings.HasMaskFlag((AttackStylePart)settings.FallBackPart))
			{
				settings.FallBackPart = AttackStylePartFallback.None;
				isDirty = true;
			}

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			Vector3Field(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WeaponPositionOffset))), ref settings.WeaponPositionOffset, 1);
			Vector3Field(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WeaponRotationOffset))), ref settings.WeaponRotationOffset, 1);
		}
		private void DrawAttackSequence(AttackSequence sequence, string sequenceName)
		{
			SerializedProperty sequenceArray = serializedObject.FindProperty(sequenceName).FindPropertyRelative(nameof(sequence.AttackSequenceParts));
			for (int i = 0; i < sequence.AttackSequenceParts.Length; i++)
			{
				CurrentlyDrawnPartCombo = i;
				DrawAttackStyle(sequence.AttackSequenceParts[i], sequenceArray.GetArrayElementAtIndex(i), i, 1);
				if (i < sequence.AttackSequenceParts.Length - 1)
				{
					LineBreak(new Color(0.8f, 0.5f, 0f, 1));
					FloatField(new GUIContent("Maximum Delay"), ref sequence.PartsMaxDelays[i], 1);
					LineBreak(new Color(0.8f, 0.5f, 0f, 1));
				}
			}

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			//Array resizer buttons
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("+"))
			{
				sequenceArray.InsertArrayElementAtIndex(sequence.AttackSequenceParts.Length);
				List<AttackStyle> newContent = new List<AttackStyle>(sequence.AttackSequenceParts);
				newContent.Add(new AttackStyle());
				sequence.AttackSequenceParts = newContent.ToArray();

				if (sequence.AttackSequenceParts.Length > 1)
				{
					serializedObject.FindProperty(sequenceName).FindPropertyRelative(nameof(sequence.PartsMaxDelays)).InsertArrayElementAtIndex(sequence.PartsMaxDelays.Length);
					List<float> newDelayContent = new List<float>(sequence.PartsMaxDelays);
					newDelayContent.Add(0);
					sequence.PartsMaxDelays = newDelayContent.ToArray();
				}

				isDirty = true;
			}
			EditorGUI.BeginDisabledGroup(sequence.AttackSequenceParts.Length == 0);
			if (GUILayout.Button("-"))
			{
				List<AttackStyle> newContent = new List<AttackStyle>(sequence.AttackSequenceParts);
				newContent.RemoveAt(newContent.Count - 1);
				sequence.AttackSequenceParts = newContent.ToArray();
				sequenceArray.arraySize = sequence.AttackSequenceParts.Length;

				if (sequence.AttackSequenceParts.Length > 1)
				{
					List<float> newDelayContent = new List<float>(sequence.PartsMaxDelays);
					newDelayContent.RemoveAt(newDelayContent.Count - 1);
					sequence.PartsMaxDelays = newDelayContent.ToArray();
					serializedObject.FindProperty(sequenceName).FindPropertyRelative(nameof(sequence.PartsMaxDelays)).arraySize = sequence.PartsMaxDelays.Length;
				}
				isDirty = true;
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
		}
		private void DrawAttackStyle(AttackStyle style, SerializedProperty styleProperty, int index, int offSet)
		{
			FoldoutFromSerializedProperty(new GUIContent(IndexToName()), styleProperty, offSet);
			if (styleProperty.isExpanded)
			{
				//Animation
				Header("Animation Settings", offSet);
				EnumField<AttackAnimation>(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.AnimationTarget))), ref style.AnimationTarget, offSet + 1);
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.StopMovement))), ref style.StopMovement, offSet + 1);

				LineBreak(new Color(0.25f, 0.25f, 0.65f, 0.25f));
				EnumField<MultiplicationType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.AnimationMultiplicationType))), ref style.AnimationMultiplicationType, offSet + 1);
				if (style.AnimationMultiplicationType == MultiplicationType.FlatValue)
				{
					if (FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.AnimationSpeedFlatValue))), ref style.AnimationSpeedFlatValue, offSet + 1))
					{
						style.AnimationSpeedFlatValue = Mathf.Max(style.AnimationSpeedFlatValue, 0);
					}
				}
				else
				{
					AnimationCurveField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.AnimationSpeedCurve))), ref style.AnimationSpeedCurve, offSet + 1);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.AnimationSpeedCurveMultiplier))), ref style.AnimationSpeedCurveMultiplier, offSet + 1);
					if (FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.AnimationSpeedCurveTimeframe))), ref style.AnimationSpeedCurveTimeframe, offSet + 1))
					{
						style.AnimationSpeedCurveTimeframe = Mathf.Max(style.AnimationSpeedCurveTimeframe, 0);
					}
				}

				//Charging
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				Header("Charge Settings", offSet);
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.NeedsCharging))), ref style.NeedsCharging, offSet + 1);
				if (style.NeedsCharging)
				{
					SerializedProperty chargeProperty = styleProperty.FindPropertyRelative(nameof(style.ChargeSettings));
					DrawChargeSettings(style.ChargeSettings, chargeProperty, offSet + 1);
				}

				//Multipliers
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				Header("Multiplier Settings", offSet);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.DamageMultiplier))), ref style.DamageMultiplier, offSet + 1);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.ManaCostMultiplier))), ref style.ManaCostMultiplier, offSet + 1);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.EnduranceCostMultiplier))), ref style.EnduranceCostMultiplier, offSet + 1);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.KnockbackMultiplier))), ref style.KnockbackMultiplier, offSet + 1);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.CritChanceMultiplier))), ref style.CritChanceMultiplier, offSet + 1);

				//Collision
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				Header("Collision Settings", offSet);
				EnumFlagField<ColliderMask>(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.CollisionMask))), ref style.CollisionMask, offSet + 1);
				EnumFlagField<ColliderMask>(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.StopOnCollisionMask))), ref style.StopOnCollisionMask, offSet + 1);
				ObjectField<EffectSingle>(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.DirectHit))), ref style.DirectHit, offSet + 1);

				//Overrides
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				Header("Override Settings", offSet);
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.OverrideElement))), ref style.OverrideElement, offSet + 1);
				if (style.OverrideElement)
					EnumField<ElementType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.OverridenElement))), ref style.OverridenElement, offSet + 2);
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.OverrideCauseType))), ref style.OverrideCauseType, offSet + 1);
				if (style.OverrideCauseType)
					EnumField<CauseType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.OverridenCauseType))), ref style.OverridenCauseType, offSet + 2);

				//Combo
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				Header("Combo Settings", offSet);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.ComboIncreaseMaxDelay))), ref style.ComboIncreaseMaxDelay, offSet + 1);
				IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.OnHitComboWorth))), ref style.OnHitComboWorth, offSet + 1);

				//Attack effects
				GUILayout.Space(6);
				LineBreak(new Color(0.5f, 0.25f, 0.65f, 1), false);
				LineBreak(new Color(0.5f, 0.25f, 0.65f, 1), false);
				Header("Attack Effects", offSet);

				SerializedProperty attackEffectsProperty = styleProperty.FindPropertyRelative(nameof(style.AttackEffects));
				FoldoutFromSerializedProperty(new GUIContent(ObjectNames.NicifyVariableName(nameof(style.AttackEffects))), attackEffectsProperty, offSet + 1);
				if (attackEffectsProperty.isExpanded)
				{
					int newSize = style.AttackEffects.Length;
					DelayedIntField("Size", ref newSize, offSet + 2);

					for (int i = 0; i < style.AttackEffects.Length; i++)
					{
						DrawAttackEffect(style.AttackEffects[i], attackEffectsProperty.GetArrayElementAtIndex(i), i, offSet + 2);
					}

					if (style.AttackEffects.Length != newSize)
					{
						isDirty = true;
						AttackEffect[] newArray = new AttackEffect[newSize];
						for (int i = 0; i < newSize; i++)
						{
							if (i < style.AttackEffects.Length)
								newArray[i] = style.AttackEffects[i];
							else
								newArray[i] = new AttackEffect();
						}
						style.AttackEffects = newArray;
						attackEffectsProperty.arraySize = newSize;
					}
				}
			}
			string IndexToName()
			{
				if (index == 0)
				{
					return "Start Attack";
				}
				else
				{
					return index + ". Combo";
				}
			}
		}
		private void DrawChargeSettings(AttackChargeSettings settings, SerializedProperty settingsProperty, int offSet)
		{
			FoldoutFromSerializedProperty(new GUIContent("Charge Settings"), settingsProperty, offSet);
			if (settingsProperty.isExpanded)
			{
				EnumFlagField<AttackChargeEffectMask>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectMask))), ref settings.EffectMask, offSet + 1);

				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				SliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AnimationChargeStartpoint))), ref settings.AnimationChargeStartpoint, 0, 1, offSet + 1);
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ApplyMoveStunWhileCharging))), ref settings.ApplyMoveStunWhileCharging, offSet + 1);
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WaitAtFullChargeForRelease))), ref settings.WaitAtFullChargeForRelease, offSet + 1);

				//Charge values
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ChargeTime))), ref settings.ChargeTime, offSet + 1);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartCharge))), ref settings.StartCharge, offSet + 1);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MaximumCharge))), ref settings.MaximumCharge, offSet + 1);

				SliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MinRequiredCharge))), ref settings.MinRequiredCharge, settings.StartCharge, settings.MaximumCharge, offSet + 1);

				//DirectHit overrides
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				SerializedProperty hitOverridesProperty = settingsProperty.FindPropertyRelative(nameof(settings.ChargeBasedDirectHits));
				DrawArray<ChargeBasedDirectHit>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ChargeBasedDirectHits))), ref settings.ChargeBasedDirectHits, hitOverridesProperty, DrawDirectHitOverride, offSet + 1, new GUIContent(". Charge Based Direct Hit"));

				//FX
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				SerializedProperty fxProperty = settingsProperty.FindPropertyRelative(nameof(settings.FXObjects));
				DrawCustomFXObjectArray(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.FXObjects))), ref settings.FXObjects, fxProperty, offSet + 1);

				SerializedProperty fxMultipliedProperty = settingsProperty.FindPropertyRelative(nameof(settings.FXObjectsWithMutliplier));
				DrawCustomFXObjectArray(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.FXObjectsWithMutliplier))), ref settings.FXObjectsWithMutliplier, fxMultipliedProperty, offSet + 1);

				//Buffs
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
				SerializedProperty buffProperty = settingsProperty.FindPropertyRelative(nameof(settings.BuffOnUserWhileCharging));
				ObjectArrayField<Buff>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BuffOnUserWhileCharging))), ref settings.BuffOnUserWhileCharging, buffProperty, new GUIContent(". Buff"), offSet + 1);
			}
		}
		private void DrawAttackEffect(AttackEffect effect, SerializedProperty effectProperty, int index, int offSet)
		{
			FoldoutFromSerializedProperty(new GUIContent(IndexToName()), effectProperty, offSet);
			if (effectProperty.isExpanded)
			{
				//Activation Info
				SliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(effect.AtAnimationPoint))), ref effect.AtAnimationPoint, 0, 1, offSet + 1);
				SliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(effect.ChanceToActivate))), ref effect.ChanceToActivate, 0, 1, offSet + 1);
				EnumFlagField<AttackEffectType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(effect.ContainedEffectType))), ref effect.ContainedEffectType, offSet + 1);
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);

				//Impulse Velocity
				if (effect.HasMaskFlag(AttackEffectType.ImpulseVelocity))
				{
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(effect.ImpulseVelocity))), ref effect.ImpulseVelocity, offSet + 1);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(effect.ImpulseVelocityFallOffTime))), ref effect.ImpulseVelocityFallOffTime, offSet + 1);

					EnumField<InherritDirection>(new GUIContent(ObjectNames.NicifyVariableName(nameof(effect.ImpulseVelocityDirection))), ref effect.ImpulseVelocityDirection, offSet + 1);
					if (effect.ImpulseVelocityDirection == InherritDirection.Target)
						EnumField<InherritDirection>(new GUIContent(ObjectNames.NicifyVariableName(nameof(effect.ImpulseVelocityFallbackDirection))), ref effect.ImpulseVelocityFallbackDirection, offSet + 1);

					EnumField<DirectionBase>(new GUIContent(ObjectNames.NicifyVariableName(nameof(effect.ImpulseDirectionBase))), ref effect.ImpulseDirectionBase, offSet + 2);
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);
				}
				//FX
				if (effect.HasMaskFlag(AttackEffectType.FX))
				{
					SerializedProperty fxProperty = effectProperty.FindPropertyRelative(nameof(effect.FXObjects));
					DrawCustomFXObjectArray(new GUIContent(ObjectNames.NicifyVariableName(nameof(effect.FXObjects))), ref effect.FXObjects, fxProperty, offSet + 1);
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);
				}
				//AOE
				if (effect.HasMaskFlag(AttackEffectType.AOE))
				{
					SerializedProperty aoeProperty = effectProperty.FindPropertyRelative(nameof(effect.AOEEffects));
					bool aoeArrayOpen = aoeProperty.isExpanded;
					ObjectArrayField<EffectAOE>(new GUIContent(ObjectNames.NicifyVariableName(nameof(effect.AOEEffects))), ref effect.AOEEffects, ref aoeArrayOpen, new GUIContent(". AOE"), offSet + 1);
					if (aoeProperty.isExpanded != aoeArrayOpen)
						aoeProperty.isExpanded = aoeArrayOpen;

					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);
				}
				//Projectile
				if (effect.HasMaskFlag(AttackEffectType.CreateProjectile))
				{
					SerializedProperty projectileArrayProperty = effectProperty.FindPropertyRelative(nameof(effect.ProjectileInfos));
					FoldoutFromSerializedProperty(new GUIContent("Projectile Settings"), projectileArrayProperty, offSet + 1);
					if (projectileArrayProperty.isExpanded)
					{
						int newSize = effect.ProjectileInfos.Length;
						DelayedIntField("Size", ref newSize, offSet + 2);

						for (int i = 0; i < effect.ProjectileInfos.Length; i++)
						{
							DrawProjectileInfo(projectileArrayProperty.GetArrayElementAtIndex(i), effect.ProjectileInfos[i], i, offSet + 2);
						}

						if (effect.ProjectileInfos.Length != newSize)
						{
							isDirty = true;
							ProjectileInfo[] newArray = new ProjectileInfo[newSize];
							for (int i = 0; i < newSize; i++)
							{
								if (i < effect.ProjectileInfos.Length)
									newArray[i] = effect.ProjectileInfos[i];
								else
									newArray[i] = new ProjectileInfo();
							}
							effect.ProjectileInfos = newArray;
							projectileArrayProperty.arraySize = newSize;
						}
					}

					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);
				}
			}

			string IndexToName()
			{
				return (index + 1) + ". Effect";
			}
		}
		private void DrawDirectHitOverride(GUIContent content, ChargeBasedDirectHit selfSettings, SerializedProperty selfProperty, int offSet)
		{
			Weapon settings = target as Weapon;

			FoldoutFromSerializedProperty(content, selfProperty, offSet);
			if (selfProperty.isExpanded)
			{
				float min = settings[CurrentlyDrawnPart].AttackSequenceParts[CurrentlyDrawnPartCombo].ChargeSettings.MinRequiredCharge;
				float max = settings[CurrentlyDrawnPart].AttackSequenceParts[CurrentlyDrawnPartCombo].ChargeSettings.MaximumCharge;

				SliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(selfSettings.MinRequiredCharge))), ref selfSettings.MinRequiredCharge, min, max, offSet + 1);
				SliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(selfSettings.MaxRequiredCharge))), ref selfSettings.MaxRequiredCharge, selfSettings.MinRequiredCharge, max, offSet + 1);

				GUILayout.Space(4);
				ObjectField<EffectSingle>(new GUIContent(ObjectNames.NicifyVariableName(nameof(selfSettings.DirectHitOverride))), ref selfSettings.DirectHitOverride, offSet + 1);
			}
		}
		private void DrawProjectileInfo(SerializedProperty projectileProperty, ProjectileInfo info, int index, int offSet)
		{
			FoldoutFromSerializedProperty(new GUIContent((index + 1) + ". Projectile Info"), projectileProperty, offSet);
			if (projectileProperty.isExpanded)
			{
				ObjectField<ProjectileData>(new GUIContent("ProjectileData"), ref info.Projectile, offSet + 1);

				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(info.ExecutionDelay))), ref info.ExecutionDelay, offSet + 1);

				IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(info.ExecutionCount))), ref info.ExecutionCount, offSet + 1);
				if (info.ExecutionCount > 1)
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(info.ExecutionRepeatDelay))), ref info.ExecutionRepeatDelay, offSet + 1);
			}
		}
		private void DrawUltimateSettings()
		{
			WeaponUltimate settings = (target as Weapon).UltimateSettings;
			ObjectField<Ultimate>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Ultimate))), ref settings.Ultimate, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.TotalRequiredCharge))), ref settings.TotalRequiredCharge, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OnUseChargeRemove))), ref settings.OnUseChargeRemove, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OnHitCharge))), ref settings.OnHitCharge, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OnCritHitCharge))), ref settings.OnCritHitCharge, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OnKillCharge))), ref settings.OnKillCharge, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.PerComboPointCharge))), ref settings.PerComboPointCharge, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ChargeOverTimeOnCombat))), ref settings.ChargeOverTimeOnCombat, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OutOfCombatDecrease))), ref settings.OutOfCombatDecrease, 1);
		}
	}
}